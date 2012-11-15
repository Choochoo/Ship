using System;
using System.Collections.Generic;
using ThirdPartyNinjas.XnaUtility;

namespace Ship.Game.Beans.Mortals.Keys
{
    public static class DecorKeys
    {
        public static readonly Dictionary<string, Dictionary<string, byte[]>> DecorCollection = new Dictionary<string, Dictionary<string, byte[]>>();

        public static readonly String[] AllClimates = new[] {ClimateFrozenString,ClimateColdString, ClimateMildString, ClimateJungleString, ClimateHotString };

        public static readonly String[] AllTypes = new[] { TypeTreeString, TypeRockString, TypeFlowerString, TypeExtraString, TypeBushString, TypeGrassString };

        public const String ClimateFrozenString = "frozen";
        public const String ClimateColdString = "cold";
        public const String ClimateMildString = "mild";
        public const String ClimateJungleString = "jungle";
        public const String ClimateHotString = "hot";

        public const String TypeTreeString = "tree";
        public const String TypeRockString = "rock";
        public const String TypeFlowerString = "flower";
        public const String TypeExtraString = "extra";
        public const String TypeBushString = "bush";
        public const String TypeGrassString = "grass";

        public const byte Tree = 0;
        public const byte Rock = 1;
        public const byte Flower = 2;
        public const byte Extra = 3;
        public const byte Bush = 4;
        public const byte Grass = 4;
    }
}
