using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using Ship.Game.Mutiplayer.Constants;
using Ship.Game.Beans.Managers;

namespace Ship.Game.Mutiplayer
{
    public class Client
    {

        private NetClient _client;
        private List<HeroEntity> _listOfOtherPlayers; 

        public void SetupClient(string gameName, string ip = "127.0.0.1", int port = 14242)
        {
            // Create new client, with previously created configs
            var config = new NetPeerConfiguration(gameName);
            _listOfOtherPlayers = new List<HeroEntity>();
            _client = new NetClient(config);
            _client.Start();

            // Connect client, to ip previously requested from user
            var outmsg = _client.CreateMessage();
            outmsg.Write(PacketType.Login);
            outmsg.WriteAllProperties(MainGame.MyHero.MyLoginInfo);
            _client.Connect(ip, port, outmsg);
        }


        public void SendData(string message)
        {
            // Create new message
            NetOutgoingMessage outmsg = _client.CreateMessage();

            // Write byte = Set "MOVE" as packet type
            outmsg.Write(message);


            // Send it to server
            _client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);

        }

        public void Update()
        {
            // Create new incoming message holder
            var inc = _client.ReadMessage();
            if (inc == null)
                return;

            if (inc.MessageType == NetIncomingMessageType.Data)
            {
                var state = inc.ReadByte();
                switch (state)
                {
                    case PacketType.LoginAccepted:
                        MainGame.MyHero.MyLoginInfo.Connection = inc.SenderConnection;
                        
                        MessageManager.Self.AddMessage("You are successfully logged in");
                        break;
                    case PacketType.NewHero:
                        break;
                }
            }
            
        }

        private sbyte _lastSentMovX = 0;
        private sbyte _lastSentMovY = 0;
        internal void SendMovement(sbyte movX, sbyte movY)
        {
            if (movX == _lastSentMovX && movY == _lastSentMovY)
                return;

            var msg = _client.CreateMessage();

            if (movX == 0 && movY == 0)
            {
                msg.Write(PacketType.MoveStopped);
            }
            else
            {
                msg.Write(PacketType.MoveChanged);
                msg.Write(movX);
                msg.Write(movY);
            }

            _lastSentMovX = movX;
            _lastSentMovY = movY;

            _client.SendMessage(msg, NetDeliveryMethod.UnreliableSequenced);
        }
    }
}
