#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdPartyNinjas.XnaUtility;

#endregion

namespace Ship.Game.Play.Beans.Items.Keys
{
    public class WorldItemKey
    {
        public const byte Wood = 0;
        public const byte TotalLootableObjects = 5;
        public static readonly List<string> AllItems = new List<string> {"wooditem"};
        private static TextureRegion[] _itemRegions;
        public static TextureRegion[] ItemRegions { get { return _itemRegions; } set { if (_itemRegions == null) _itemRegions = value; } }
    }
}