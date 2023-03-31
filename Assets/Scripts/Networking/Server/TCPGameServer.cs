﻿using System;
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
        private Dictionary<Guid, TcpMessageChannel> clients = new Dictionary<Guid, TcpMessageChannel>();
        private List<TcpMessageChannel> brokenClients = new List<TcpMessageChannel>();

        private void Awake() {
            Log.LogInfo("Starting server on port " + serverPort, this, ConsoleColor.Gray);

            //start listening for incoming connections (with max 50 in the queue)
            //we allow for a lot of incoming connections, so we can handle them
            //and tell them whether we will accept them or not instead of bluntly declining them
            listener = new TcpListener(IPAddress.Any, serverPort);
            listener.Start(50);
        }

        private TCPGameServer() { }

        private void Update() {
            //check for new members	
            processNewClients();
            processExistingClients();
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

                clients.Add(newClientGuid, channel);


                ConnectEvent connectEvent = new ConnectEvent();
                connectEvent.guid = newClientGuid;


                channel.SendMessage(connectEvent);

                EventBus<JoinQuitEvent>.Raise(new JoinQuitEvent(clients.Count));
            }
        }

        /// <summary>
        /// Method to process existing clients
        /// </summary>
        private void processExistingClients() {
            foreach (TcpMessageChannel client in clients.Values) {
                if (client.HasMessage()) {
                    var messageObject = client.ReceiveMessage();

                    switch (messageObject) {
                        case DisconnectEvent disconnectEvent:
                            handleDisconnectEvent(disconnectEvent);
                            break;

                    }
                }
            }
        }

        private void handleDisconnectEvent(DisconnectEvent disconnectEvent) {
            Debug.Log("Added client with guid " + disconnectEvent.guid + " to broken clients list");
            brokenClients.Add(clients.FirstOrDefault(x => x.Key == disconnectEvent.guid).Value);
        }



        /// <summary>
        /// Method to get rid of faulty clients
        /// </summary>
        private void cleanupFaultyClients() {
            if (clients.Count < 1 || brokenClients.Count < 1) return;

            for (int i = brokenClients.Count; i > 0; i--) {
                Debug.Log("removed client with guid " + clients.FirstOrDefault(x => x.Value == brokenClients[i - 1]).Key + " from list HEHE dont look linq method in debug loG XDXDXDXXDXD");
                clients.Remove(clients.FirstOrDefault(x => x.Value == brokenClients[i - 1]).Key);
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

    }

}


