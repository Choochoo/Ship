#region

using System;
using System.Collections.Generic;
using ThirdPartyNinjas.XnaUtility;

#endregion

namespace Ship.Game.Play.Beans.Helpers
{
    public static class DecorHelper
    {
        // public static readonly Dictionary<string, Dictionary<string, byte[]>> DecorCollection = new Dictionary<string, Dictionary<string, byte[]>>();

        //public const String ClimateFrozenString = "frozen";
        //public const String ClimateColdString = "cold";
        public const String ClimateMildString = "mild";
        //public const String ClimateJungleString = "jungle";
        //public const String ClimateHotString = "hot";

        public const String TypeTreeString = "tree";
        public const String TypeRockString = "rock";
        //public const String TypeFlowerString = "flower";
        // public const String TypeExtraString = "extra";
        public const String TypeBushString = "bush";
        //public const String TypeGrassString = "grass";
        public const byte None = 0;
        public const byte Tree = 1;
        public const byte Rock = 2;
        //public const byte Flower = 3;
        //public const byte Extra = 4;
        public const byte Bush = 5;
        // public const byte Grass = 6;
        public const byte Fire = 6;

        public const byte TreeCount = 3;
        public const byte RockCount = 1;
        public const byte BushCount = 2;
        public const byte FireCount = 3;

        public static readonly String[] AllClimates = new[]
            {ClimateMildString};

        public static readonly String[] AllTypes = new[] {TypeTreeString, TypeRockString, TypeBushString};
    }
}