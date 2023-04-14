using System;
using System.Net.Sockets;
using System.Net;
using shared;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace server {

    /**
     * Basic TCPGameServer that runs our game.
     * 
     * Server is made up out of different rooms that can hold different members.
     * Each member is identified by a TcpMessageChannel, which can also be used for communication.
     * In this setup each client is only member of ONE room, but you could change that of course.
     * 
     * Each room is responsible for cleaning up faulty clients (since it might involve gameplay, status changes etc).
     * 
     * As you can see this setup is limited/lacking:
     * - only 1 game can be played at a time
     */
    class TCPGameServer : MonoBehaviour {

        private Dictionary<UnityEngine.Color, UnityEngine.Vector3> spawnInformation = new Dictionary<UnityEngine.Color, UnityEngine.Vector3> {
            { UnityEngine.Color.red, new UnityEngine.Vector3(18, 4, 5) },
            { UnityEngine.Color.blue, new UnityEngine.Vector3(-2, 4, 49) },
            { UnityEngine.Color.green, new UnityEngine.Vector3(-2.6f, 4, -2.6f) },
            { UnityEngine.Color.yellow, new UnityEngine.Vector3(36, 4, 37) },
            { UnityEngine.Color.magenta, new UnityEngine.Vector3(19, 4, 44) },
            { UnityEngine.Color.cyan, new UnityEngine.Vector3(36, 4, 10) }
        };



        public static TCPGameServer Instance { get; private set; }

        [SerializeField] private int serverPort = 55555;    //the port we listen on

        private TcpListener listener;

        private Dictionary<Guid, ClientGameInformation> clients = new Dictionary<Guid, ClientGameInformation>();
        private List<Guid> brokenClients = new List<Guid>();
        [SerializeField] private GameObject playerServerPrefab;
        [SerializeField] private Vector3 spawnPosition;
        public WorldToMinimapHelper worldToMinimapHelper { get; private set; }
        public GameObject playerMinimapPrefab;


        /// <summary>
        /// Events since last sync
        /// </summary>
        private Queue<NetworkEvent> syncEvents = new Queue<NetworkEvent>();

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("There is already an existing GameManager present. Aborting instantiation.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Log.LogInfo("Starting server on port " + serverPort, this, ConsoleColor.Gray);

            //start listening for incoming connections (with max 50 in the queue)
            //we allow for a lot of incoming connections, so we can handle them
            //and tell them whether we will accept them or not instead of bluntly declining them
            listener = new TcpListener(IPAddress.Any, serverPort);
            listener.Server.NoDelay = true; // Q: What does this do? A: Disable Nagle's algorithm - no this on the other side too

            listener.Start(6);

            NetworkEventBus.SubscribeAll(OnNetworkEvent);

            worldToMinimapHelper = GameObject.FindGameObjectWithTag("Minimap").GetComponent<WorldToMinimapHelper>();

            EventBus<ServerScoreboardUpdateEvent>.Raise(new ServerScoreboardUpdateEvent {
                scores = new Dictionary<UnityEngine.Color, string>()
            });

            // StartCoroutine(sendNetworkEvents());
        }


        private TCPGameServer() { }

        private void Update() {
            brokenClients.Clear();
            //check for new members	
            processNewClients();
            processExistingClients();
            sendEvents();
            sendTransformUpdates();
            cleanupFaultyClients();

            //Debug.Log("Amount of clients on the server; " + clients.Count);
        }


        /// <summary>
        /// Method to process new clients connecting to the server
        /// </summary>
        private void processNewClients() {
            if (listener.Pending()) {
                //get the waiting client
                Log.LogInfo("Accepting new client...", this, ConsoleColor.White);
                TcpClient client = listener.AcceptTcpClient();
                client.Client.NoDelay = true; // Disable Nagle's algorithm - no this on the other side too
                //wrap the client in an easier to use communication channel
                TcpMessageChannel channel = new TcpMessageChannel(client);

                Guid newClientGuid = Guid.NewGuid();
                ClientGameInformation clientGameInformation = new ClientGameInformation(channel);

                clients.Add(newClientGuid, clientGameInformation);


                ConnectEvent connectEvent = new ConnectEvent();
                connectEvent.guid = newClientGuid;



                var instantiated = Instantiate(playerServerPrefab, spawnInformation.ElementAt(clients.Count - 1).Value, Quaternion.identity);
                var nt = instantiated.GetComponent<NetworkTransform>();
                nt.SetKey(newClientGuid);
                nt.Initialize();


                ExistingItemsPacket existingItemsPacket = new ExistingItemsPacket();
                List<NetworkTransform> networkTransforms = new List<NetworkTransform>();

                //add interactables to the list
                foreach (var interactable in AInteractable.interactables) {
                    connectEvent.interactables.Add(new ConnectEvent.InteractablePacket {
                        guid = interactable.GetComponent<NetworkTransform>().key,
                        interactableID = interactable.InteractableID,
                        transformPacket = interactable.GetComponent<NetworkTransform>().GetPacket()
                    });
                }

                // send items before other NetworkTransforms
                foreach (var item in Item.Items.Values) {
                    if (item == null) {
                        continue;
                    }
                    // existingItemsPacket.existingItems.Add(item.GetComponent<NetworkTransform>().GetPacket());
                    networkTransforms.Add(item.GetComponent<NetworkTransform>());
                    existingItemsPacket.existingItemMap.Add(item.GetComponent<NetworkTransform>().key, item.GetComponent<NetworkTransform>().GetPacket());
                }

                //channel.SendMessage(existingItemsPacket);


                foreach (var networkTransform in NetworkTransform.Transforms.Values.ToList()) {
                    if (networkTransforms.Contains(networkTransform)) {
                        continue;
                    }
                    connectEvent.objectTransforms.Add(networkTransform.GetPacket());
                }

                clientGameInformation.movementInputReceiver = instantiated.GetComponent<IMovementInputReceiver>();
                clientGameInformation.player = instantiated.GetComponent<Player>();

                if (clientGameInformation.player == null) {
                    Debug.LogError("Player is null");
                }
                // get the color of the player from the dictionary with the index of clients.count
                clientGameInformation.player.playerColor = spawnInformation.ElementAt(clients.Count - 1).Key;

                connectEvent.colorID = clients.Count - 1;

                worldToMinimapHelper.AddPlayer(clientGameInformation.player);

                if (ScoreboardHandler.Instance == null) {
                    Debug.LogError("ScoreboardHandler is null");
                }

                ScoreboardHandler.Instance.AddPlayer(clientGameInformation.player);

                // Debug.Log("Player color: " + clientGameInformation.player.playerColor);

                NetworkEventBus.Raise(new ScoreUpdatedEvent {
                    source = nt.key,
                    score = 0
                });

                channel.SendMessage(connectEvent);
                EventBus<JoinQuitEvent>.Raise(new JoinQuitEvent(clients.Count));
            }
        }

        /// <summary>
        /// Method to process existing clients
        /// </summary>
        private void processExistingClients() {
            foreach (var client in clients.Values) {
                while (client.tcpMessageChannel.HasMessage()) {
                    var messageObject = client.tcpMessageChannel.ReceiveMessage();

                    switch (messageObject) {
                        case DisconnectEvent disconnectEvent:
                            handleDisconnectEvent(disconnectEvent);
                            break;
                        case TransformPacket transformPacket:
                            handleTransformPacket(transformPacket);
                            break;
                        case InputPacket inputPacket:
                            handleInputPacket(inputPacket, client);
                            break;
                    }
                }
            }

        }

        private void sendTransformUpdates() {
            TransformListPacket transformList = new TransformListPacket();
            foreach (var transformUpdate in NetworkTransform.UpdatedTransforms) {
                var transformPacket = transformUpdate.GetPacket();
                // add packet to listpacket
                transformList.updatedTransforms.Add(transformPacket);
            }

            broadcastMessage(transformList);

            NetworkTransform.UpdatedTransforms.Clear();

        }

        private void sendEvents() {
            while (syncEvents.Count > 0) {
                var networkEvent = syncEvents.Dequeue();
                broadcastMessage(networkEvent);
                // Debug.Log("Sending event: " + networkEvent.GetType());
            }
        }

        private void handleTransformPacket(TransformPacket transformPacket) {
            Debug.LogError("Got a transform packet from a client? How lmfao?");
        }

        private void handleInputPacket(InputPacket inputPacket, ClientGameInformation source) {
            source.movementInputReceiver.DoMove(inputPacket.move);
            source.movementInputReceiver.DoView(inputPacket.view);
            if (inputPacket.powerUpPressed)
                source.player.UsePowerUp();
        }

        private void handleDisconnectEvent(DisconnectEvent disconnectEvent) {
            Debug.Log("OH NO A CLIENT DISCONNECTED WHAT DO WE DO?!");
        }

        private void broadcastMessage(ISerializable message) {
            foreach (var client in clients.Values) {
                client.tcpMessageChannel.SendMessage(message);
            }
        }

        /// <summary>
        /// Method to get rid of faulty clients
        /// </summary>
        private void cleanupFaultyClients() {
            foreach (var client in clients) {
                if (client.Value.tcpMessageChannel.HasErrors()) {
                    brokenClients.Add(client.Key);
                }
            }

            if (brokenClients.Count == 0) return;

            foreach (var brokenClient in brokenClients) {
                try {
                    worldToMinimapHelper.RemovePlayer(clients[brokenClient].player);
                    ScoreboardHandler.Instance.RemovePlayer(clients[brokenClient].player);

                    clients[brokenClient].tcpMessageChannel.Close();
                    clients.Remove(brokenClient);
                    broadcastMessage(new PlayerDisconnectEvent() { guid = brokenClient });

                    Destroy(NetworkTransform.Transforms[brokenClient].gameObject);
                    NetworkTransform.Transforms.Remove(brokenClient);
                } catch (Exception e) { Debug.LogError(e); }
            }

            EventBus<JoinQuitEvent>.Raise(new JoinQuitEvent(clients.Count));
        }

        /// <summary>
        /// Method to get the IP address of the server
        /// </summary>
        /// <returns>The IP of the server</returns>
        public string GetServerAddress() {
            //get the local IP address
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        /// <summary>
        /// Method to get the port the server is running on
        /// </summary>
        /// <returns>The port of the server</returns>
        public int GetServerPort() {
            return serverPort;
        }

        public int GetAmountOfClients() {
            return this.clients.Count;
        }

        private void OnNetworkEvent(NetworkEvent newEvent) {
            // Debug.Log("Got event: " + newEvent.GetType());
            syncEvents.Enqueue(newEvent);
        }

    }

}


