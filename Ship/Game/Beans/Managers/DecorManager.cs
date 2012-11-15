using System;
using Ship.Game.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Utils;
using Microsoft.Xna.Framework;
using Ship.Game.Beans.Mortals;
using System.Collections.Generic;
using Ship.Game.Beans.Mortals.Keys;
using Ship.Game.Beans.Tiles;

namespace Ship.Game.Beans.Managers
{
    public class DecorManager
    {
        private Texture2D _mainTextures;
        private TextureRegion[] _tileRegions;
        private Decoration[] _decorPool;
        private TileCollection[,] _tileColls;
        private int _tileCollWidth;
        private int _tileCollHeight;

        public DecorManager(ref Texture2D mainTextures,ref TextureAtlas mainAtlas)
        {
            _mainTextures = mainTextures;

            var regionContainer = new List<TextureRegion>();
            regionContainer.Add(null);//if it == 0, don't display anything
            foreach (string climate in DecorKeys.AllClimates)
            {
                var insideDict = new Dictionary<string, byte[]>();
                DecorKeys.DecorCollection.Add(climate, insideDict);
                foreach (string type in DecorKeys.AllTypes)
                {
                    
                    var inc = 1;
                    var itemString = String.Format("decor_{0}_{1}{2}", climate,type , inc);
                    var newTextureRegion = mainAtlas.GetRegion(itemString);
                    List<byte> nums = null;

                    if(newTextureRegion!= null)
                        nums = new List<byte>();

                    while(newTextureRegion != null)
                    {
                        nums.Add((byte)regionContainer.Count);
                        regionContainer.Add(newTextureRegion);
                        inc++;
                        itemString = String.Format("decor_{0}_{1}{2}", climate, type, inc);
                        newTextureRegion = mainAtlas.GetRegion(itemString);
                    }

                    if(nums!= null)
                        insideDict.Add(type, nums.ToArray());
                }
            }

            _tileRegions = regionContainer.ToArray();
        }

        internal void LoadData(TileCollection[,] tileColls)
        {
            _tileCollWidth = tileColls.GetLength(0);
            _tileCollHeight = tileColls.GetLength(1);
            _decorPool = new Decoration[_tileCollWidth * _tileCollHeight];
            _tileColls = tileColls;
            var count = 0;

            for (var i = 0; i < _tileCollWidth; i++)
            {
                for (var j = 0; j < _tileCollHeight; j++)
                {
                    var s = new Decoration(ref _tileRegions);
                    _decorPool[count++] = s;
                    s.SectorX = tileColls[i, j].SectorX;
                    s.SectorY = tileColls[i, j].SectorY;
                    s.MySectorSpotX = tileColls[i, j].MySectorSpotX;
                    s.MySectorSpotY = tileColls[i, j].MySectorSpotY;
                    s.Position = tileColls[i, j].Position;
                    var ld = tileColls[i,j].MyLoaderBase;
                    s.Value = ld.SectorData.DecorData[s.MySectorSpotX, s.MySectorSpotY];
                }
            }
        }

        public void SortVisibles()
        {
            Array.Sort(_decorPool);
        }

        public void MoveDown(TileCollection mostUpColl, TileCollection mostDownColl)
        {
            //System.Diagnostics.Debug.WriteLine("hit Decor Move down");
            var poolSize = _decorPool.Length;
            var lastNonVisibleLocation = poolSize - 1;
            //destroy old up
            for (byte innerlooper = 0; innerlooper < poolSize; innerlooper++)
            {
                if (!_decorPool[innerlooper].Visible)
                {
                    lastNonVisibleLocation = innerlooper;
                    break;
                }

                var currDecor = _decorPool[innerlooper];
                if (currDecor.MySectorSpotY == mostUpColl.MySectorSpotY - 1 && currDecor.SectorX == mostUpColl.SectorX && currDecor.SectorY == mostUpColl.SectorY)
                {
                    currDecor.Value = 0;
                }
                    
            }

            //add most down
            for (byte innerlooper = 0; innerlooper < _tileCollWidth; innerlooper++)
            {
                var currColl = _tileColls[innerlooper, mostDownColl.MyVectorSpotY];
                var currData = currColl.MyLoaderBase.SectorData.DecorData[innerlooper, mostDownColl.MySectorSpotY];
                if (currData == 0) continue;
                var currPoolDecor = _decorPool[lastNonVisibleLocation++];
                currPoolDecor.MySectorSpotX = innerlooper;
                currPoolDecor.MySectorSpotY = currColl.MySectorSpotY;
                currPoolDecor.SectorX = currColl.SectorX;
                currPoolDecor.SectorY = currColl.SectorY;
                currPoolDecor.Position = currColl.Position;
                currPoolDecor.Value = currData;
            }
        }

        public void MoveUp(TileCollection mostUpColl, TileCollection mostDownColl)
        {
            //System.Diagnostics.Debug.WriteLine("hit Decor Move up");
            var poolSize = _decorPool.Length;
            var lastNonVisibleLocation = poolSize - 1;
            //destroy old up
            for (byte innerlooper = 0; innerlooper < poolSize; innerlooper++)
            {
                if (!_decorPool[innerlooper].Visible)
                {
                    lastNonVisibleLocation = innerlooper;
                    break;
                }

                var currDecor = _decorPool[innerlooper];
                if (currDecor.MySectorSpotY == mostDownColl.MySectorSpotY + 1 && currDecor.SectorX == mostDownColl.SectorX && currDecor.SectorY == mostDownColl.SectorY)
                {
                    currDecor.Value = 0;
                }

            }

            //add most down
            for (byte innerlooper = 0; innerlooper < _tileCollWidth; innerlooper++)
            {
                var currColl = _tileColls[innerlooper, mostUpColl.MyVectorSpotY];
                var currData = currColl.MyLoaderBase.SectorData.DecorData[innerlooper, mostUpColl.MySectorSpotY];
                if (currData == 0) continue;
                var currPoolDecor = _decorPool[lastNonVisibleLocation++];
                currPoolDecor.MySectorSpotX = innerlooper;
                currPoolDecor.MySectorSpotY = currColl.MySectorSpotY;
                currPoolDecor.SectorX = currColl.SectorX;
                currPoolDecor.SectorY = currColl.SectorY;
                currPoolDecor.Position = currColl.Position;
                currPoolDecor.Value = currData;
            }
        }

        public void MoveLeft(TileCollection mostLeftColl, TileCollection mostRightColl)
        {
            //System.Diagnostics.Debug.WriteLine("hit Decor Move Left");
            var poolSize = _decorPool.Length;
            var lastNonVisibleLocation = poolSize - 1;
            //destroy old up
            for (byte innerlooper = 0; innerlooper < poolSize; innerlooper++)
            {
                if (!_decorPool[innerlooper].Visible)
                {
                    lastNonVisibleLocation = innerlooper;
                    break;
                }

                var currDecor = _decorPool[innerlooper];
                if (currDecor.MySectorSpotX == mostRightColl.MySectorSpotX + 1 && currDecor.SectorX == mostRightColl.SectorX && currDecor.SectorY == mostRightColl.SectorY)
                {
                    currDecor.Value = 0;
                }

            }

            //add most down
            for (byte innerlooper = 0; innerlooper < _tileCollHeight; innerlooper++)
            {
                var currColl = _tileColls[mostLeftColl.MyVectorSpotX, innerlooper];
                var currData = currColl.MyLoaderBase.SectorData.DecorData[mostLeftColl.MySectorSpotX, innerlooper];
                if (currData == 0) continue;
                var currPoolDecor = _decorPool[lastNonVisibleLocation++];
                currPoolDecor.MySectorSpotX = currColl.MySectorSpotX;
                currPoolDecor.MySectorSpotY = innerlooper;
                currPoolDecor.SectorX = currColl.SectorX;
                currPoolDecor.SectorY = currColl.SectorY;
                currPoolDecor.Position = currColl.Position;
                currPoolDecor.Value = currData;
            }
        }

        public void MoveRight(TileCollection mostLeftColl, TileCollection mostRightColl)
        {
            //System.Diagnostics.Debug.WriteLine("hit Decor Move right");
            var poolSize = _decorPool.Length;
            var lastNonVisibleLocation = poolSize - 1;
            //destroy old up
            for (byte innerlooper = 0; innerlooper < poolSize; innerlooper++)
            {
                if (!_decorPool[innerlooper].Visible)
                {
                    lastNonVisibleLocation = innerlooper;
                    break;
                }

                var currDecor = _decorPool[innerlooper];
                if (currDecor.MySectorSpotX == mostLeftColl.MySectorSpotX - 1 && currDecor.SectorX == mostLeftColl.SectorX && currDecor.SectorY == mostLeftColl.SectorY)
                {
                    currDecor.Value = 0;
                }

            }

            //add most down
            for (byte innerlooper = 0; innerlooper < _tileCollHeight; innerlooper++)
            {
                var currColl = _tileColls[mostRightColl.MyVectorSpotX, innerlooper];
                var currData = currColl.MyLoaderBase.SectorData.DecorData[mostRightColl.MySectorSpotX, innerlooper];
                if (currData == 0) continue;
                var currPoolDecor = _decorPool[lastNonVisibleLocation++];
                currPoolDecor.MySectorSpotX = currColl.MySectorSpotX;
                currPoolDecor.MySectorSpotY = innerlooper;
                currPoolDecor.SectorX = currColl.SectorX;
                currPoolDecor.SectorY = currColl.SectorY;
                currPoolDecor.Position = currColl.Position;
                currPoolDecor.Value = currData;
            }
        }

        
        public int Draw(ref SpriteBatch spriteBatch,ref Camera2D cam)
        {
            var numOfDraws = 0;
            for (int i = 0, len = _decorPool.Length; i < len; i++)
            {
                if (!_decorPool[i].Visible) break;

                numOfDraws += _decorPool[i].Draw(ref spriteBatch, ref _mainTextures,ref  cam );
            }
            return numOfDraws;
        }

        public void dispose()
        {
            // TODO Auto-generated method stub
        }
    }
}