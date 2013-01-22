using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using Ship.Game.Mutiplayer.Constants;
using Ship.Game.Beans.Mortals.Animate;

namespace Ship.Game.Mutiplayer
{
    public class Host
    {

        // Server object
        private NetServer _gameServer;

        private Thread _serverThread;

        private List<LoginInfo> _heroList;
        private short id = 0;

        public void SetupServer(string gameName, int port = 14242, int maxConnections = 20)
        {
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            var config = new NetPeerConfiguration(gameName) { Port = port, MaximumConnections = maxConnections };
            _heroList = new List<LoginInfo>();
            // Set server port

            // Max client amount
            // Enable New messagetype. Explained later
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            // Create new server based on the configs just defined
            _gameServer = new NetServer(config);

            // Start it
            _gameServer.Start();

            // Eh..
            Console.WriteLine("Server Started");


            // Object that can be used to store and read messages
            NetIncomingMessage inc;
            _serverThread = new Thread(() =>
            {
                while (true)
                {
                    ServerLoop();
                }
            });
            _serverThread.IsBackground = true;
            _serverThread.Start();
        }


        private void ServerLoop()
        {

            var inc = _gameServer.ReadMessage();
            if (inc == null)
                return;

            // Theres few different types of messages. To simplify this process, i left only 2 of em here
            switch (inc.MessageType)
            {
                // If incoming message is Request for connection approval
                // This is the very first packet/message that is sent from client
                // Here you can do new player initialisation stuff
                case NetIncomingMessageType.ConnectionApproval:

                    // Read the first byte of the packet
                    // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                    if (inc.ReadByte() == PacketType.Login)
                    {
                        Console.WriteLine("Incoming LOGIN");

                        // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                        inc.SenderConnection.Approve();
                        var loginInfo = new LoginInfo();
                        inc.ReadAllProperties(loginInfo);
                        _heroList.Add(loginInfo);
                        // Create message, that can be written and sent
                        
                        foreach (LoginInfo hero in _heroList)
                        {
                            if (hero.Connection == inc.SenderConnection)
                            {
                                var outmsg = _gameServer.CreateMessage();
                                outmsg.Write(PacketType.LoginAccepted);
                                outmsg.Write(id);
                                _gameServer.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                            }
                            else
                            {
                                var servermsg = _gameServer.CreateMessage();
                                servermsg.Write(PacketType.NewHero);
                                servermsg.Write(loginInfo.Name);
                                _gameServer.SendMessage(servermsg, hero.Connection, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                            
                        }
                        id++;
                        // Debug
                        Console.WriteLine("Approved new connection and updated the world status");
                    }

                    break;
                // Data type is all messages manually sent from client
                // ( Approval is automated process )
                case NetIncomingMessageType.Data:

                    // Read first byte
                    if (inc.ReadByte() == PacketType.MoveChanged)
                    {
                        var movX = inc.ReadSByte();
                        var movY = inc.ReadSByte();

                        foreach (LoginInfo hero in _heroList)
                        {
                            // If stored connection ( check approved message. We stored ip+port there, to character obj )
                            // Find the correct character
                            if (hero.Connection != inc.SenderConnection)
                                continue;


                            
                        }

                    }
                    break;
                case NetIncomingMessageType.StatusChanged:
                    // In case status changed
                    // It can be one of these
                    // NetConnectionStatus.Connected;
                    // NetConnectionStatus.Connecting;
                    // NetConnectionStatus.Disconnected;
                    // NetConnectionStatus.Disconnecting;
                    // NetConnectionStatus.None;

                    // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                    Console.WriteLine(inc.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)inc.SenderConnection.Status);
                    //if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                    //{
                    //    // Find disconnected character and remove it
                    //    foreach (Character cha in GameWorldState)
                    //    {
                    //        if (cha.Connection == inc.SenderConnection)
                    //        {
                    //            GameWorldState.Remove(cha);
                    //            break;
                    //        }
                    //    }
                    //}
                    break;
                default:
                    // As i statet previously, theres few other kind of messages also, but i dont cover those in this example
                    // Uncommenting next line, informs you, when ever some other kind of message is received
                    Console.WriteLine("Not Important Message");
                    break;

            } // If New messages

            System.Diagnostics.Debug.WriteLine("message:", inc);
        }

    }
}
