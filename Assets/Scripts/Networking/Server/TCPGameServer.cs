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
        [SerializeField] private int serverPort = 55555;    //the port we listen on

        private TcpListener listener;
        
        private Dictionary<Guid, ClientGameInformation> clients = new Dictionary<Guid, ClientGameInformation>();
        private List<Guid> brokenClients = new List<Guid>();

        [SerializeField] private GameObject playerServerPrefab;
        
        /// <summary>
        /// Events since last sync
        /// </summary>
        private Queue<NetworkEvent> syncEvents = new Queue<NetworkEvent>();

        private void Awake() {
            Log.LogInfo("Starting server on port " + serverPort, this, ConsoleColor.Gray);

            //start listening for incoming connections (with max 50 in the queue)
            //we allow for a lot of incoming connections, so we can handle them
            //and tell them whether we will accept them or not instead of bluntly declining them
            listener = new TcpListener(IPAddress.Any, serverPort);
            listener.Start(50);

            NetworkEventBus.SubscribeAll(OnNetworkEvent);

            // StartCoroutine(sendNetworkEvents());
        }


        private TCPGameServer() { }

        private void Update() {
            brokenClients.Clear();
            //check for new members	
            processNewClients();
            processExistingClients();
            sendTransformUpdates();
            sendEvents();
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
                //and wrap the client in an easier to use communication channel
                TcpMessageChannel channel = new TcpMessageChannel(client);

                Guid newClientGuid = Guid.NewGuid();
                ClientGameInformation clientGameInformation = new ClientGameInformation(channel);

                clients.Add(newClientGuid, clientGameInformation);


                ConnectEvent connectEvent = new ConnectEvent();
                connectEvent.guid = newClientGuid;



                var instantiated = Instantiate(playerServerPrefab, new Vector3(80, 2, 13), Quaternion.identity);
                var nt = instantiated.GetComponent<NetworkTransform>();
                nt.key = newClientGuid;
                nt.Initialize();

                foreach (var networkTransform in NetworkTransform.Transforms.Values.ToList()) {
                    connectEvent.objectTransforms.Add(networkTransform.GetPacket());
                }

                clientGameInformation.movementInputReceiver = instantiated.GetComponent<IMovementInputReceiver>();

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

            BroadcastMessage(transformList);

            NetworkTransform.UpdatedTransforms.Clear();

        }

        private void sendEvents() {
            while (syncEvents.Count > 0) {
                var networkEvent = syncEvents.Dequeue();
                BroadcastMessage(networkEvent);
            }
        }

        private void handleTransformPacket(TransformPacket transformPacket) {
            Debug.LogError("Got a transform packet from a client? How lmfao?");
        }

        private void handleInputPacket(InputPacket inputPacket, ClientGameInformation source) {
            source.movementInputReceiver.DoMove(inputPacket.move);
            source.movementInputReceiver.DoView(inputPacket.view);
        }

        private void handleDisconnectEvent(DisconnectEvent disconnectEvent) {
            Debug.Log("OH NO A CLIENT DISCONNECTED WHAT DO WE DO?!");
        }

        private void BroadcastMessage(ISerializable message) {
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
                clients[brokenClient].tcpMessageChannel.Close();
                clients.Remove(brokenClient);
                BroadcastMessage(new PlayerDisconnectEvent() { guid = brokenClient });

                Destroy(NetworkTransform.Transforms[brokenClient].gameObject);
                NetworkTransform.Transforms.Remove(brokenClient);
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
            syncEvents.Enqueue(newEvent);
        }

    }

}


