#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Ship.Game.Play.Beans.Tiles
{
    public class TextureProperties
    {
        private bool _hasFeature = true;
        private bool _isMountain;

        public string TextureName { get; private set; }
        public int[] Vals { get; private set; }

        public bool HasFeature { get { return _hasFeature; } set { _hasFeature = value; } }

        public bool IsMountain { get { return _isMountain; } set { _isMountain = value; } }

        public void SetLocations(string texture, int[] nums)
        {
            TextureName = texture;
            Vals = nums;
        }

        public void SetLocations(string texture, bool isMountain)
        {
            TextureName = texture;
            IsMountain = isMountain;
        }
    }
}