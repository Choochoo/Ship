#region

using System;
using Ship.Game.Play.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Utils;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Beans.Mortals;
using System.Collections.Generic;
using Ship.Game.Play.Beans.Tiles;
using System.Diagnostics;
using Ship.Game.Play.Beans.Helpers;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Beans.Mortals.Inanimate;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class DecorManager
    {
        private readonly int _fireStart;
        private Decoration[] _decorPool;
        private Decoration[,] _decorSprites;
        private Texture2D _mainTextures;
        private int _stoppingPoint;
        private int _tileCollHeight;
        private int _tileCollWidth;
        private TileCollection[,] _tileColls;
        private TextureRegion[] _tileRegions;


        public DecorManager()
        {
            _mainTextures = PlayScreen.DecorationTexture;
            var mainAtlas = PlayScreen.DecorationAtlas;

            var regionContainer = new List<TextureRegion>();
            //regionContainer.Add(null);//if it == 0, don't display anything
            for (var climate = 0; climate < DecorHelper.AllClimates.Length; climate++)
            {
                // var insideDict = new Dictionary<string, byte[]>();
                //DecorKeys.DecorCollection.Add(climate, insideDict);
                for (var i = 1; i <= DecorHelper.TreeCount; i++)
                {
                    var itemString = String.Format("decor_{0}_{1}{2}", DecorHelper.AllClimates[climate],
                                                   DecorHelper.TypeTreeString, i);
                    var region = mainAtlas.GetRegion(itemString);
                    regionContainer.Add(region);
                }

                for (var i = 1; i <= DecorHelper.RockCount; i++)
                {
                    var itemString = String.Format("decor_{0}_{1}{2}", DecorHelper.AllClimates[climate],
                                                   DecorHelper.TypeRockString, i);
                    var region = mainAtlas.GetRegion(itemString);

                    regionContainer.Add(region);
                }

                for (var i = 1; i <= DecorHelper.BushCount; i++)
                {
                    var itemString = String.Format("decor_{0}_{1}{2}", DecorHelper.AllClimates[climate],
                                                   DecorHelper.TypeBushString, i);
                    var region = mainAtlas.GetRegion(itemString);

                    //Debug.WriteLine("rock value is {0}", regionContainer.Count);
                    regionContainer.Add(region);
                }
            }

            _fireStart = regionContainer.Count;
            for (var i = 1; i <= DecorHelper.FireCount; i++)
            {
                var itemString = String.Format("decor_fire{0}", i);
                var region = mainAtlas.GetRegion(itemString);
                regionContainer.Add(region);
            }

            _tileRegions = regionContainer.ToArray();
        }

        public int FireStart { get { return _fireStart; } }

        public Decoration[] DecorPool { get { return _decorPool; } }

        public static Decoration[,] DecorSprites { get; private set; }

        internal void LoadData()
        {
            _tileCollWidth = TileManager.TileColls.GetLength(0);
            _tileCollHeight = TileManager.TileColls.GetLength(1);
            _decorPool = new Decoration[_tileCollWidth*_tileCollHeight];
            _decorSprites = new Decoration[_tileCollWidth,_tileCollHeight];
            DecorSprites = _decorSprites;
            _tileColls = TileManager.TileColls;
            var count = 0;

            for (var i = 0; i < _tileCollWidth; i++)
            {
                for (var j = 0; j < _tileCollHeight; j++)
                {
                    var s = new Decoration(this, ref _tileRegions);
                    DecorPool[count++] = s;
                    DecorSprites[i, j] = s;
                    s.SectorX = _tileColls[i, j].SectorX;
                    s.SectorY = _tileColls[i, j].SectorY;
                    s.MySectorSpotX = _tileColls[i, j].MySectorSpotX;
                    s.MySectorSpotY = _tileColls[i, j].MySectorSpotY;
                    s.MyVectorSpotX = _tileColls[i, j].MyVectorSpotX;
                    s.MyVectorSpotY = _tileColls[i, j].MyVectorSpotY;
                    s.UpdatePosition(_tileColls[i, j].HitRect.X, _tileColls[i, j].HitRect.Y);
                    var ld = _tileColls[i, j].MyLoaderBase;
                    CreateLightForTile(s, ld);
                }
            }
            SortPool();
        }

        private void CreateLightForTile(Decoration s, LoaderBase ld)
        {
            if (!s.SetInfo(this, ld.SectorData.DecorData[s.MySectorSpotX, s.MySectorSpotY])) return;
            //_tileColls[s.MyVectorSpotX, s.MyVectorSpotY].StarterLight(ref _tileColls, _tileCollWidth, _tileCollHeight);
        }

        public void SortPool() { Array.Sort(DecorPool); }

        public void MoveDown(TileCollection mostUpColl, TileCollection mostDownColl)
        {
            for (byte innerlooper = 0; innerlooper < _tileCollWidth; innerlooper++)
            {
                var mostUpYVal = mostUpColl.MyVectorSpotY - 1 >= 0
                                     ? mostUpColl.MyVectorSpotY - 1
                                     : _tileCollHeight - 1;


                var s = DecorSprites[innerlooper, mostDownColl.MyVectorSpotY];
                var currColl = _tileColls[innerlooper, mostDownColl.MyVectorSpotY];
                s.SectorX = currColl.SectorX;
                s.SectorY = currColl.SectorY;
                s.MySectorSpotX = currColl.MySectorSpotX;
                s.MySectorSpotY = currColl.MySectorSpotY;
                s.UpdatePosition(currColl.HitRect.X, currColl.HitRect.Y);
                var ld = currColl.MyLoaderBase;
                DecorSprites[innerlooper, mostUpYVal].SetInfo(null, DecorHelper.None);

                //currColl.CheckLightOfNeighbor(ref _tileColls, _tileCollWidth, _tileCollHeight);
                CreateLightForTile(s, ld);
            }
        }

        public void MoveUp(TileCollection mostUpColl, TileCollection mostDownColl)
        {
            //System.Diagnostics.Debug.WriteLine("hit Decor Move up");

            for (byte innerlooper = 0; innerlooper < _tileCollWidth; innerlooper++)
            {
                var mostDownYVal = mostDownColl.MyVectorSpotY + 1 >= _tileCollHeight
                                       ? 0
                                       : mostDownColl.MyVectorSpotY + 1;

                var s = DecorSprites[innerlooper, mostUpColl.MyVectorSpotY];
                var currColl = _tileColls[innerlooper, mostUpColl.MyVectorSpotY];
                s.SectorX = currColl.SectorX;
                s.SectorY = currColl.SectorY;
                s.MySectorSpotX = currColl.MySectorSpotX;
                s.MySectorSpotY = currColl.MySectorSpotY;
                s.UpdatePosition(currColl.HitRect.X, currColl.HitRect.Y);
                var ld = currColl.MyLoaderBase;
                DecorSprites[innerlooper, mostDownYVal].SetInfo(null, DecorHelper.None);

                //currColl.CheckLightOfNeighbor(ref _tileColls, _tileCollWidth, _tileCollHeight);
                CreateLightForTile(s, ld);
            }
        }

        public void MoveLeft(TileCollection mostLeftColl, TileCollection mostRightColl)
        {
            for (byte innerlooper = 0; innerlooper < _tileCollHeight; innerlooper++)
            {
                var mostUpYVal = mostRightColl.MyVectorSpotX + 1 >= _tileCollWidth
                                     ? 0
                                     : mostRightColl.MyVectorSpotX + 1;

                var s = DecorSprites[mostLeftColl.MyVectorSpotX, innerlooper];
                var currColl = _tileColls[mostLeftColl.MyVectorSpotX, innerlooper];
                s.SectorX = currColl.SectorX;
                s.SectorY = currColl.SectorY;
                s.MySectorSpotX = currColl.MySectorSpotX;
                s.MySectorSpotY = currColl.MySectorSpotY;
                s.UpdatePosition(currColl.HitRect.X, currColl.HitRect.Y);
                var ld = currColl.MyLoaderBase;
                DecorSprites[mostUpYVal, innerlooper].SetInfo(null, DecorHelper.None );

                //currColl.CheckLightOfNeighbor(ref _tileColls, _tileCollWidth, _tileCollHeight);
                CreateLightForTile(s, ld);
            }
        }

        public void MoveRight(TileCollection mostLeftColl, TileCollection mostRightColl)
        {
            for (byte innerlooper = 0; innerlooper < _tileCollHeight; innerlooper++)
            {
                var mostUpYVal = mostLeftColl.MyVectorSpotX - 1 >= 0
                                     ? mostLeftColl.MyVectorSpotX - 1
                                     : _tileCollWidth - 1;

                var s = DecorSprites[mostRightColl.MyVectorSpotX, innerlooper];
                var currColl = _tileColls[mostRightColl.MyVectorSpotX, innerlooper];
                s.SectorX = currColl.SectorX;
                s.SectorY = currColl.SectorY;
                s.MySectorSpotX = currColl.MySectorSpotX;
                s.MySectorSpotY = currColl.MySectorSpotY;
                s.UpdatePosition(currColl.HitRect.X, currColl.HitRect.Y);
                var ld = currColl.MyLoaderBase;
                DecorSprites[mostUpYVal, innerlooper].SetInfo(null, DecorHelper.None);

                //currColl.CheckLightOfNeighbor(ref _tileColls, _tileCollWidth, _tileCollHeight);
                CreateLightForTile(s, ld);
            }
        }

        public void StartDraw(Hero hero)
        {
            for (int i = 0, len = DecorPool.Length; i < len; i++)
            {
                if (!DecorPool[i].Visible || DecorPool[i].HitRect.Y > hero.HitRect.Y)
                {
                    _stoppingPoint = i;
                    break;
                }
                DecorPool[i].Draw();
            }
        }

        public void FinishDraw()
        {
            for (int i = _stoppingPoint, len = DecorPool.Length; i < len; i++)
            {
                if (!DecorPool[i].Visible)
                    return;
                DecorPool[i].Draw();
            }
        }

        public void Dispose()
        {
            // TODO Auto-generated method stub
        }
    }
}