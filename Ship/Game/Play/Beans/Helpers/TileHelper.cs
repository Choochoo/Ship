#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Ship.Game.Play.Beans.Helpers
{
    public static class TileHelper
    {
        public const byte Grass = 0;
        public const byte Dirt = 1;
        public const byte Sand = 2;
        public const byte Wall = 3;
        public const byte RiverWater = 4;
        public const byte OceanWater = 5;

        //must be named same as file!
        public const String GrassString = "grass";
        public const String DirtString = "dirt";
        public const String SandString = "sand";
        public const String WallString = "wall";
        public const String RiverWaterString = "river";
        public const String OceanWaterString = "water";

        public static readonly Dictionary<string, byte> TypesOfTerrain = new Dictionary<string, byte>
            {
                {GrassString, Grass},
                {DirtString, Dirt},
                {SandString, Sand},
                {WallString, Wall},
                {RiverWaterString, RiverWater},
                {OceanWaterString, OceanWater}
            };
    }
}