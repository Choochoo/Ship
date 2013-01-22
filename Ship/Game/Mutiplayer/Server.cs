using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using Ship.Game.Beans.Mortals.Animate;

namespace Ship.Game.Mutiplayer
{
    public class Server
    {
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }

        private readonly Client _myClient;
        private readonly Host _myHost;

        public Server(Hero me)
        {
            _myHost = new Host();
            _myClient = new Client();

            IsServer = IsClient = true;

            _myHost.SetupServer("game");
            _myClient.SetupClient("game");
        }

        public void Update()
        {
            if (IsClient)
                _myClient.Update();
        }
        public void SendMovement(sbyte movX, sbyte movY) { _myClient.SendMovement(movX, movY); }
    }


    
}
