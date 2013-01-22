using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship.Game.Mutiplayer.Constants
{
    public static class PacketType
    {
        public const byte Login = 0;
        public const byte LoginAccepted = 1;
        public const byte NewHero = 2;
        public const byte MoveStopped = 40;
        public const byte MoveChanged = 40;
        public const byte Worldstate = 60;
    }
}
