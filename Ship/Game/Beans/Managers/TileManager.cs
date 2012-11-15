#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Utils;
using Ship.Game.Loaders;
using System.Collections.Generic;
using Ship.Game.Beans.Tiles;
using Ship.Game.Beans.Events;
using Ship.Game.Beans.Mortals.Keys;
using System.Threading.Tasks;
using System.Diagnostics;

#endregion

namespace Ship.Game.Beans.Managers
{
    public class TileManager : IDisposable
    {
        public delegate void MoveUpdateHandler(object sender, MoveArgs e);
        public event MoveUpdateHandler OnMoveTile;
        public event MoveUpdateHandler OnMoveLoader;

        private const float SpriteWidth = 32.0f;
        private const float TileBuffer = SpriteWidth*4;

        public const byte Addonwidth = 16;
        public const byte Addonheight = 16;
        
        private Texture2D _mainTextures;

        private LoaderBase[][] _tileLoaders;
        private TextureRegion[] _tileRegions;

        private TileCollection _mostDownSprite;
        private TileCollection _mostLeftSprite;
        private TileCollection _mostRightSprite;
        private TileCollection _mostUpSprite;
        private bool _terrainLoaded;
        private int _terrainTileCollectionHeight;
        private int _terrainTileCollectionWidth;
        private TileCollection[,] _tileColls;

        public TileManager(ref Texture2D mainTextures, ref TextureAtlas mainAtlas)
        {
            _mainTextures = mainTextures;

            _tileRegions = new TextureRegion[1 + (TileKeys.TypesOfTerrain.Count * 24)];


            var terrainCounter = 0;
            foreach (KeyValuePair<string, byte> t in TileKeys.TypesOfTerrain)
            {
                for (var i = 0; i < 24; i++)
                {
                    if (t.Value == 0)
                        _tileRegions[terrainCounter++] = mainAtlas.GetRegion(t.Key);
                    else
                    {
                        var newName = String.Format("{0}{1}", t.Key, i);
                        _tileRegions[terrainCounter++] = mainAtlas.GetRegion(newName);
                    }
                }
            }
        }

        public TileCollection[,] TileColls { get { return _tileColls; } }

        public TileCollection MostDownSprite { get { return _mostDownSprite; } }

        public TileCollection MostLeftSprite { get { return _mostLeftSprite; } }

        public TileCollection MostRightSprite { get { return _mostRightSprite; } }

        public TileCollection MostUpSprite { get { return _mostUpSprite; } }

        public void Update(GameTime gameTime, Camera2D cam)
        {
            if (_terrainLoaded)
                CheckTerrain(cam);
        }

      

        private void CheckTerrain(Camera2D c)
        {
            var cameraX = c.Position.X;
            var cameraY = c.Position.Y;
            if (MostLeftSprite.Position.X + TileBuffer > cameraX)
                OnMoveTile(this, new MoveArgs(MoveArgs.Left));
            if (MostRightSprite.Position.X - TileBuffer < c.ViewportWidth + cameraX)
                OnMoveTile(this, new MoveArgs(MoveArgs.Right));
            if (MostUpSprite.Position.Y + TileBuffer > cameraY)
                OnMoveTile(this, new MoveArgs(MoveArgs.Up));
            if ((MostDownSprite.Position.Y + (SpriteWidth * 2)) - TileBuffer < c.ViewportHeight + cameraY)
                OnMoveTile(this, new MoveArgs(MoveArgs.Down));
        }


        internal void LoadData(ref LoaderBase[][] cl)
        {
            _tileLoaders = cl;
            var ww = MainGame.WindowWidth;
            var hh = MainGame.WindowHeight;
            var terrainTileWidth = (ww/SpriteWidth) + Addonwidth;
            var terrainTileHeight = (hh/SpriteWidth) + Addonheight;
            terrainTileWidth -= terrainTileWidth%4;
            terrainTileHeight -= terrainTileHeight%4;
            _terrainTileCollectionWidth = (int)(terrainTileWidth/2);
            _terrainTileCollectionHeight = (int)(terrainTileHeight/2);
            _tileColls = new TileCollection[_terrainTileCollectionWidth,_terrainTileCollectionHeight];


            for (var i = 0; i < _terrainTileCollectionWidth; i++)
            {
                for (var j = 0; j < _terrainTileCollectionHeight; j++)
                    TileColls[i, j] = new TileCollection();
            }
            for (var i = 0; i < terrainTileWidth; i += 2)
            {
                for (var j = 0; j < terrainTileHeight; j += 2)
                {
                    var jScore = (int) (j*.5);
                    var iScore = (int) (i*.5);
                    var tc = TileColls[iScore, jScore];
                    if (i == 0 && j == 0)
                        _mostLeftSprite = _mostUpSprite = tc;
                    else if (i == terrainTileWidth - 2 && j == terrainTileHeight - 2)
                        _mostRightSprite = _mostDownSprite = tc;
                    tc.SectorX = Managers.SpawnX;
                    tc.SectorY = Managers.SpawnY;
                    tc.MyVectorSpotX = tc.MySectorSpotX = iScore;
                    tc.MyVectorSpotY = tc.MySectorSpotY = jScore;
                    tc.Position = new Vector2((i*SpriteWidth), (j*SpriteWidth));
                    var ar = GetSurroundings(ref tc.MyLoaderBase,tc.SectorX, tc.SectorY, tc.MySectorSpotX, tc.MySectorSpotY);
                    tc.SetSurroundings(FindSurroundings(ar));
                }
            }
            _terrainLoaded = true;
        }

        private int[] FindSurroundings(int[] vals)
        {
            var tileVal = vals[4];

            if (tileVal == 0)
            {
                return new []{0,0,0,0};
            }
            var topLeft = vals[0];
            var topMiddle = vals[1];
            var topRight = vals[2];
            var midLeft = vals[3];

            var midRight = vals[5];
            var bottomLeft = vals[6];
            var bottomMiddle = vals[7];
            var bottomRight = vals[8];
            bool[] ba = null;
            if (tileVal == TileKeys.RiverWater || tileVal == TileKeys.OceanWater)
            {
                ba = new[]
                    {
                        topLeft == TileKeys.OceanWater || topLeft == TileKeys.RiverWater,
                        topMiddle == TileKeys.OceanWater || topMiddle == TileKeys.RiverWater,
                        topRight == TileKeys.OceanWater || topRight == TileKeys.RiverWater,
                        midLeft == TileKeys.OceanWater || midLeft == TileKeys.RiverWater,
                        midRight == TileKeys.OceanWater || midRight == TileKeys.RiverWater,
                        bottomLeft == TileKeys.OceanWater || bottomLeft == TileKeys.RiverWater,
                        bottomMiddle == TileKeys.OceanWater || bottomMiddle == TileKeys.RiverWater,
                        bottomRight == TileKeys.OceanWater || bottomRight == TileKeys.RiverWater
                    };
            }
            else if (tileVal == TileKeys.Sand)
            {
                ba = new[]
                    {
                        topLeft == TileKeys.OceanWater || topLeft == TileKeys.Sand,
                        topMiddle == TileKeys.OceanWater || topMiddle == TileKeys.Sand,
                        topRight == TileKeys.OceanWater || topRight == TileKeys.Sand,
                        midLeft == TileKeys.OceanWater || midLeft == TileKeys.Sand,
                        midRight == TileKeys.OceanWater || midRight == TileKeys.Sand,
                        bottomLeft == TileKeys.OceanWater || bottomLeft == TileKeys.Sand,
                        bottomMiddle == TileKeys.OceanWater || bottomMiddle == TileKeys.Sand,
                        bottomRight == TileKeys.OceanWater || bottomRight == TileKeys.Sand
                    };
            }
            else
            {
                ba = new[]
                    {
                        topLeft == tileVal, topMiddle == tileVal, topRight == tileVal,
                        midLeft == tileVal, midRight == tileVal, bottomLeft == tileVal,
                        bottomMiddle == tileVal, bottomRight == tileVal
                    };
            }


            return FillArea(ba, tileVal);
        }

        private int[] FillArea(bool[] boolVals, int tileVal)
        {
            var upLeftFine = boolVals[0]; // FindSquare( _x, _y, num, -1, -1 );
            var upFine = boolVals[1]; // FindSquare( _x, _y, num, 0, -1 );
            var upRightFine = boolVals[2]; // FindSquare( _x, _y, num, 1, -1 );
            var leftFine = boolVals[3]; // FindSquare( _x, _y, num, -1, 0 );
            var rightFine = boolVals[4]; // FindSquare( _x, _y, num, 1, 0 );
            var bottomLeftFine = boolVals[5]; // FindSquare( _x, _y, num, -1, 1 );
            var bottomFine = boolVals[6]; // FindSquare( _x, _y, num, 0, 1 );
            var bottomRightFine = boolVals[7]; // FindSquare( _x, _y, num, 1, 1
            // );
            // upLeftFine = upFine = upRightFine = leftFine = rightFine =
            // bottomLeftFine = bottomFine = bottomRightFine = true;
            // topleft,top,topright
            // midLeft,mid,midright
            // bottomleft,bottom,bottomright
            // 0,1,2
            // 3,4,5
            // 6,7,8
            // 9,10,11,12
            // 13,14,15,16
            int topLeftNum, topRightNum, bottomLeftNum, bottomRightNum;
            var offset = tileVal * 24;
            if (!leftFine && rightFine && upFine && bottomFine)
            {
                topLeftNum = 12;
                topRightNum = 13;
                bottomLeftNum = 16;
                bottomRightNum = 13;
                if (!upRightFine)
                    topRightNum = 3;
                if (!bottomRightFine)
                    bottomRightNum = 7;
            }
            else if (leftFine && !rightFine && upFine && bottomFine)
            {
                topLeftNum = 13;
                topRightNum = 15;
                bottomLeftNum = 13;
                bottomRightNum = 19;
                if (!bottomLeftFine)
                    bottomLeftNum = 6;
                if (!upLeftFine)
                    topLeftNum = 2;
            }
            else if (leftFine && rightFine && !upFine && bottomFine)
            {
                topLeftNum = 9;
                topRightNum = 10;
                bottomLeftNum = 13;
                bottomRightNum = 13;
                if (!bottomLeftFine)
                    bottomLeftNum = 6;
                if (!bottomRightFine)
                    bottomRightNum = 7;
            }
            else if (leftFine && rightFine && upFine && !bottomFine)
            {
                topLeftNum = 13;
                topRightNum = 13;
                bottomLeftNum = 21;
                bottomRightNum = 22;
                if (!upRightFine)
                    topRightNum = 3;
                if (!upLeftFine)
                    topLeftNum = 2;
            }
            else if (!leftFine && rightFine && !upFine && bottomFine)
            {
                if (bottomRightFine)
                {
                    topLeftNum = 8;
                    topRightNum = 9;
                    bottomLeftNum = 12;
                    bottomRightNum = 13;
                }
                else
                {
                    topLeftNum = 8;
                    topRightNum = 9;
                    bottomLeftNum = 12;
                    bottomRightNum = 7;
                }
            }
            else if (!leftFine && rightFine && upFine)
            {
                if (upRightFine)
                {
                    topLeftNum = 16;
                    topRightNum = 13;
                    bottomLeftNum = 20;
                    bottomRightNum = 21;
                }
                else
                {
                    topLeftNum = 16;
                    topRightNum = 3;
                    bottomLeftNum = 20;
                    bottomRightNum = 21;
                }
            }
            else if (leftFine && !rightFine && !upFine && bottomFine)
            {
                if (bottomLeftFine)
                {
                    topLeftNum = 9;
                    topRightNum = 11;
                    bottomLeftNum = 13;
                    bottomRightNum = 15;
                }
                else
                {
                    topLeftNum = 9;
                    topRightNum = 11;
                    bottomLeftNum = 6;
                    bottomRightNum = 15;
                }
            }
            else if (leftFine && !rightFine && upFine)
            {
                if (upLeftFine)
                {
                    topLeftNum = 13;
                    topRightNum = 15;
                    bottomLeftNum = 21;
                    bottomRightNum = 23;
                }
                else
                {
                    topLeftNum = 2;
                    topRightNum = 15;
                    bottomLeftNum = 21;
                    bottomRightNum = 23;
                }
            }
            else if (!leftFine && !rightFine && !upFine && bottomFine)
            {
                topLeftNum = 0;
                topRightNum = 1;
                bottomLeftNum = 12;
                bottomRightNum = 15;
            }
            else if (leftFine && !rightFine)
            {
                topLeftNum = 9;
                topRightNum = 11;
                bottomLeftNum = 21;
                bottomRightNum = 5;
            }
            else if (!leftFine && rightFine)
            {
                topLeftNum = 0;
                topRightNum = 9;
                bottomLeftNum = 4;
                bottomRightNum = 21;
            }
            else if (!leftFine && upFine && !bottomFine)
            {
                topLeftNum = 12;
                topRightNum = 15;
                bottomLeftNum = 4;
                bottomRightNum = 5;
            }
            else if (leftFine && !upFine)
            {
                topLeftNum = 9;
                topRightNum = 10;
                bottomLeftNum = 21;
                bottomRightNum = 22;
            }
            else if (!leftFine && upFine)
            {
                topLeftNum = 12;
                topRightNum = 15;
                bottomLeftNum = 16;
                bottomRightNum = 19;
            }
            else if (leftFine)
            {
                var topLefts = 13;
                var topRights = 13;
                var bottomLefts = 13;
                var bottomRights = 13;
                if (!upLeftFine)
                    topLefts = 2;
                if (!upRightFine)
                    topRights = 3;
                if (!bottomLeftFine)
                    bottomLefts = 6;
                if (!bottomRightFine)
                    bottomRights = 7;
                topLeftNum = topLefts;
                topRightNum = topRights;
                bottomLeftNum = bottomLefts;
                bottomRightNum = bottomRights;
            }
            else
            {
                topLeftNum = 0;
                topRightNum = 1;
                bottomLeftNum = 4;
                bottomRightNum = 5;
            }


            var topLeft = topLeftNum + offset;

            var topRight = topRightNum + offset;

            var bottomLeft = bottomLeftNum + offset;

            var bottomRight = bottomRightNum + offset;

            return new[] { topLeft, topRight, bottomLeft, bottomRight };
        }

        public LoaderBase GetSpriteTile(int sectorX, int sectorY)
        {
            for (var i = 0; i < 3; i = i + 1)
            {
                for (var j = 0; j < 3; j = j + 1)
                {
                    if (_tileLoaders[i][j].SectorX == sectorX
                        && _tileLoaders[i][j].SectorY == sectorY)
                        return _tileLoaders[i][j];
                }
            }
            System.Diagnostics.Debug.WriteLine("could not find sectors: " + sectorX + "," + sectorY);
            System.Diagnostics.Debug.WriteLine("in...");
            return null;
        }

        private int[] GetSurroundings(ref LoaderBase myLoaderBase, int sectX, int sectY, int sectSpotX, int sectSpotY)
        {
            var ar = new int[9];
            myLoaderBase = GetSpriteTile(sectX, sectY);
            
            var holder = 0;
            const byte tileLength = MainGame.SectorTileSize;
            for (var i = -1; i < 2; i = i + 1)
            {
                for (var j = -1; j < 2; j = j + 1)
                {
                    var spotX = sectSpotX + j;
                    var spotY = sectSpotY + i;
                    int val;
                    if (spotY >= 0 && spotX >= 0 && spotY < tileLength && spotX < tileLength)
                        val = myLoaderBase.SectorData.TileData[spotX, spotY];
                    else
                    {
                        var offsetX = 0;
                        offsetX = spotX < 0 ? -1 : offsetX;
                        offsetX = spotX >= tileLength ? 1 : offsetX;
                        //
                        var offsetY = 0;
                        offsetY = spotY < 0 ? -1 : offsetY;
                        offsetY = spotY >= tileLength ? 1 : offsetY;
                        //
                        var tl2 = GetSpriteTile(sectX + offsetX, sectY + offsetY);
                        //
                        var currSpotX = spotX;
                        currSpotX = offsetX == -1 ? tileLength - 1 : currSpotX;
                        currSpotX = offsetX == 1 ? 0 : currSpotX;
                        //
                        var currSpotY = spotY;
                        currSpotY = offsetY == -1 ? tileLength - 1 : currSpotY;
                        currSpotY = offsetY == 1 ? 0 : currSpotY;
                        //
                        val = tl2.SectorData.TileData[currSpotX, currSpotY];
                    }
                    // System.out.println("val:" + val);
                    ar[holder++] = val;
                }
            }

            return ar;
        }

        public void MoveTerrainDown()
        {
            //System.Diagnostics.Debug.WriteLine("hit tiles Move down");
            var wasTheLastOfLoader = MostUpSprite.MySectorSpotY + 1 >= MainGame.SectorTileSize;
            for (var innerlooper = 0; innerlooper < _terrainTileCollectionWidth; innerlooper++)
            {
                var switchPos = TileColls[innerlooper, MostUpSprite.MyVectorSpotY].Position;
                TileColls[innerlooper, MostUpSprite.MyVectorSpotY].Position = new Vector2(switchPos.X,
                                                                                            MostDownSprite.Position.Y +
                                                                                            (SpriteWidth*2.0f));
            }

            // swap because everything moved
            var tempSprite = MostDownSprite;
            _mostDownSprite = MostUpSprite;
            // get lowest sectorspotx, then loop it and give it to everybody else
            MostDownSprite.MySectorSpotY = tempSprite.MySectorSpotY;
            MostDownSprite.SectorY = tempSprite.SectorY;
            // ////////////////////////////////

            if (MostDownSprite.MySectorSpotY + 1 >= MainGame.SectorTileSize)
            {
                MostDownSprite.SectorY++;
                MostDownSprite.MySectorSpotY = 0;
            }
            else
                MostDownSprite.MySectorSpotY++;

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionWidth; innerlooper++)
            {
                tempSprite = TileColls[innerlooper, MostUpSprite.MyVectorSpotY];
                tempSprite.SectorY = MostDownSprite.SectorY;
                tempSprite.MySectorSpotY = MostDownSprite.MySectorSpotY;
                var ar = GetSurroundings(ref tempSprite.MyLoaderBase, tempSprite.SectorX, tempSprite.SectorY, tempSprite.MySectorSpotX, tempSprite.MySectorSpotY);
                tempSprite.SetSurroundings(FindSurroundings(ar));
            }


            if (MostDownSprite.SectorY != _tileLoaders[1][1].SectorY && wasTheLastOfLoader)
            {
                if (_tileLoaders[1][0].SectorY == MostDownSprite.SectorY )
                    OnMoveLoader(this,new MoveArgs(MoveArgs.Up));
                else
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Down));
            }

            var vectorY = MostDownSprite.MyVectorSpotY + 1 >= _terrainTileCollectionHeight ? 0 : MostDownSprite.MyVectorSpotY + 1;
            _mostUpSprite = TileColls[0, vectorY];
        }

        public void MoveTerrainLeft()
        {
            //System.Diagnostics.Debug.WriteLine("hit tiles Move left");
            var wasTheLastOfLoader = MostRightSprite.MySectorSpotX == 0;
            for (var innerlooper = 0; innerlooper < _terrainTileCollectionHeight; innerlooper++)
            {
                var switchPos = TileColls[MostRightSprite.MyVectorSpotX, innerlooper].Position;
                TileColls[MostRightSprite.MyVectorSpotX, innerlooper].Position =
                    new Vector2(MostLeftSprite.Position.X - (SpriteWidth*2.0f), switchPos.Y);
            }

            // swap because everything moved
            var tempSprite = MostLeftSprite;
            _mostLeftSprite = MostRightSprite;
            // get lowest sectorspotx, then loop it and give it to everybody else
            MostLeftSprite.MySectorSpotX = tempSprite.MySectorSpotX;
            MostLeftSprite.SectorX = tempSprite.SectorX;

            if (MostLeftSprite.MySectorSpotX - 1 < 0)
            {
                MostLeftSprite.SectorX--;
                MostLeftSprite.MySectorSpotX = MainGame.SectorTileSize - 1;
            }
            else
                MostLeftSprite.MySectorSpotX = tempSprite.MySectorSpotX - 1;

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionHeight; innerlooper++)
            {
                tempSprite = TileColls[MostRightSprite.MyVectorSpotX, innerlooper];
                tempSprite.SectorX = MostLeftSprite.SectorX;
                tempSprite.MySectorSpotX = MostLeftSprite.MySectorSpotX;
                var ar = (GetSurroundings(ref tempSprite.MyLoaderBase,tempSprite.SectorX,
                                                           tempSprite.SectorY, tempSprite.MySectorSpotX,
                                                           tempSprite.MySectorSpotY));
                tempSprite.SetSurroundings(FindSurroundings(ar));
            }
            if (MostLeftSprite.SectorX != _tileLoaders[1][1].SectorX && wasTheLastOfLoader)
            {
                if (_tileLoaders[2][1].SectorX == MostLeftSprite.SectorX)
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Right));
                else
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Left));
            }

            var vectorX = MostLeftSprite.MyVectorSpotX;
            vectorX = vectorX - 1 < 0 ? _terrainTileCollectionWidth - 1 : vectorX - 1;
            _mostRightSprite = TileColls[vectorX, 0];
        }

        public void MoveTerrainRight()
        {
            // X COORDINATE SHIFT
            //System.Diagnostics.Debug.WriteLine("hit tiles Move right");
            var wasTheLastOfLoader = MostLeftSprite.MySectorSpotX+1 >= MainGame.SectorTileSize;
            for (var innerlooper = 0; innerlooper < _terrainTileCollectionHeight; innerlooper++)
            {
                var switchPos = TileColls[MostLeftSprite.MyVectorSpotX, innerlooper].Position;
                TileColls[MostLeftSprite.MyVectorSpotX, innerlooper].Position =
                    new Vector2(MostRightSprite.Position.X + (SpriteWidth*2.0f), switchPos.Y);
            }
            var tempSprite = MostRightSprite;
            _mostRightSprite = MostLeftSprite;
            // get lowest sectorspotx, then loop it and give it to everybody else
            MostRightSprite.MySectorSpotX = tempSprite.MySectorSpotX;
            MostRightSprite.SectorX = tempSprite.SectorX;
            if (MostRightSprite.MySectorSpotX + 1 >= MainGame.SectorTileSize)
            {
                MostRightSprite.SectorX = MostRightSprite.SectorX + 1;
                MostRightSprite.MySectorSpotX = 0;
            }
            else
                MostRightSprite.MySectorSpotX = tempSprite.MySectorSpotX + 1;

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionHeight; innerlooper++)
            {
                tempSprite = TileColls[MostLeftSprite.MyVectorSpotX, innerlooper];
                tempSprite.SectorX = MostRightSprite.SectorX;
                tempSprite.MySectorSpotX = MostRightSprite.MySectorSpotX;
                var ar = GetSurroundings(ref tempSprite.MyLoaderBase, tempSprite.SectorX, tempSprite.SectorY,
                                         tempSprite.MySectorSpotX, tempSprite.MySectorSpotY);
                    tempSprite.SetSurroundings(FindSurroundings(ar));
            }
            if (MostRightSprite.SectorX != _tileLoaders[1][1].SectorX && wasTheLastOfLoader)
            {
                if (_tileLoaders[2][1].SectorX == MostRightSprite.SectorX)
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Right));
                else
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Left));
            }

            var vectorX = MostRightSprite.MyVectorSpotX + 1 >= _terrainTileCollectionWidth ? 0 : MostRightSprite.MyVectorSpotX + 1;
            _mostLeftSprite = TileColls[vectorX, 0];
        }

        public void MoveTerrainUp()
        {
           // System.Diagnostics.Debug.WriteLine("hit tiles Move up");
            // System.out.println("hit up");
            var wasTheLastOfLoader = MostDownSprite.MySectorSpotY == 0;
            for (var innerlooper = 0; innerlooper < _terrainTileCollectionWidth; innerlooper++)
            {
                var switchPos = TileColls[innerlooper, MostDownSprite.MyVectorSpotY].Position;
                TileColls[innerlooper, MostDownSprite.MyVectorSpotY].Position = new Vector2(switchPos.X,MostUpSprite.Position.Y - (SpriteWidth*2));
            }
            // swap because everything moved
            var tempSprite = MostUpSprite;
            _mostUpSprite = MostDownSprite;

            // get lowest sectorspotx, then loop it and give it to everybody else
            MostUpSprite.MySectorSpotY = tempSprite.MySectorSpotY;
            MostUpSprite.SectorY = tempSprite.SectorY;
            if (MostUpSprite.MySectorSpotY - 1 < 0)
            {
                MostUpSprite.SectorY--;
                MostUpSprite.MySectorSpotY = MainGame.SectorTileSize - 1;
            }
            else
                MostUpSprite.MySectorSpotY = tempSprite.MySectorSpotY - 1;

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionWidth; innerlooper++)
            {
                tempSprite = TileColls[innerlooper, MostDownSprite.MyVectorSpotY];
                tempSprite.SectorY = MostUpSprite.SectorY;
                tempSprite.MySectorSpotY = MostUpSprite.MySectorSpotY;
                var ar = GetSurroundings(ref tempSprite.MyLoaderBase, tempSprite.SectorX, tempSprite.SectorY,
                                         tempSprite.MySectorSpotX, tempSprite.MySectorSpotY);
                tempSprite.SetSurroundings(FindSurroundings(ar));
            }
            if (MostUpSprite.SectorY != _tileLoaders[1][1].SectorY && wasTheLastOfLoader)
            {
                if (_tileLoaders[1][0].SectorY == MostUpSprite.SectorY)
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Up));
                else
                    OnMoveLoader(this, new MoveArgs(MoveArgs.Down));
            }

            var vectorY = MostUpSprite.MyVectorSpotY - 1 < 0 ? _terrainTileCollectionHeight - 1 : MostUpSprite.MyVectorSpotY-1;
            _mostDownSprite = TileColls[0, vectorY];
        }

        


        //private void showTiles()
        //{
        //    System.out.
        //    println("|" + _tileLoaders[0, 0].SectorX + ","
        //            + _tileLoaders[0, 0].SectorY + " - "
        //            + _tileLoaders[1, 0].SectorX + ","
        //            + _tileLoaders[1, 0].SectorY + " - "
        //            + _tileLoaders[2, 0].SectorX + ","
        //            + _tileLoaders[2, 0].SectorY + "|");
        //    System.out.
        //    println("|" + _tileLoaders[0, 1].SectorX + ","
        //            + _tileLoaders[0, 1].SectorY + " - "
        //            + _tileLoaders[1, 1].SectorX + ","
        //            + _tileLoaders[1, 1].SectorY + " - "
        //            + _tileLoaders[2, 1].SectorX + ","
        //            + _tileLoaders[2, 1].SectorY + "|");
        //    System.out.
        //    println("|" + _tileLoaders[0, 2].SectorX + ","
        //            + _tileLoaders[0, 2].SectorY + " - "
        //            + _tileLoaders[1, 2].SectorX + ","
        //            + _tileLoaders[1, 2].SectorY + " - "
        //            + _tileLoaders[2, 2].SectorX + ","
        //            + _tileLoaders[2, 2].SectorY + "|");
        //}

        public int Draw(ref SpriteBatch spriteBatch, ref Camera2D cam)
        {
           // var sw2 = Stopwatch.StartNew();
            var numOfDraws = 0;
            for (short i = 0; i < _terrainTileCollectionWidth; i++)
            {
                for (short j = 0; j < _terrainTileCollectionHeight; j++)
                    numOfDraws += TileColls[i, j].Draw(ref spriteBatch, ref _mainTextures, ref _tileRegions, ref cam);
            }
            //sw2.Stop();
            //System.Diagnostics.Debug.WriteLine("time:{0}", sw2.ElapsedMilliseconds);
            return numOfDraws;
            //var sw3 = Stopwatch.StartNew();
            //var len = _tileColls.GetLength(0);
            //var len2 = _tileColls.GetLength(1);
            //var count = 0;
            //Parallel.For(0,len, x=>
            //    {
            //        for (var y = 0; y < len2; y++ )
            //            count += _tileColls[x,y].Draw(spriteBatch, _mainTextures, _tileRegions, cam);
            //    }
            //    );
            //sw3.Stop();
            //System.Diagnostics.Debug.WriteLine("time:{0}",sw3.ElapsedMilliseconds);
            return 0;
        }

        public void Dispose()
        {
            // TODO Auto-generated method stub
        }
    }

   
}