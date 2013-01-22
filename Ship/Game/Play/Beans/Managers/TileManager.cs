#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Utils;
using Ship.Game.Play.Loaders;
using System.Collections.Generic;
using Ship.Game.Play.Beans.Tiles;
using System.Threading.Tasks;
using System.Diagnostics;
using Ship.Game.Play.Beans.Helpers;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class TileManager : IDisposable
    {
        public const float SpriteWidth = 32;
        public const int Sprite2XWidth = (int) (SpriteWidth)*2;
        public const int TileBuffer = Sprite2XWidth*2;
        public const short ScreenBuffer = 2048;

        //public const byte MaxLight = 1;
        //public const byte MaxLightRadius = (MaxLight*2);
        private readonly int[] _currentSurroundings = new int[9];
        private readonly Color[] _getIntensity;
        private readonly Vector2 _moveRasterDown = new Vector2(0, -Sprite2XWidth);
        private readonly Vector2 _moveRasterLeft = new Vector2(Sprite2XWidth, 0);
        private readonly Vector2 _moveRasterRight = new Vector2(-Sprite2XWidth, 0);
        private readonly Vector2 _moveRasterUp = new Vector2(0, Sprite2XWidth);
        private readonly SpriteBatch _spriteBatch;
        private readonly int[] _surroundings = new int[4];
        private readonly TextureRegion[] _tileRegions;
        //private int CharSpawnX = 0;
        // private int CharSpawnY = 0;
        private GraphicsDevice _graphicsDevice;
        private Manager _manager;
        private TileCollection _mostLeftUpSprite;
        private TileCollection _mostRightDownSprite;
        private bool _renderToggle = true;
        private bool _terrainLoaded;
        private Vector2 _terrainPosition;

        private RenderTarget2D _terrainTextureOne;
        private RenderTarget2D _terrainTextureTwo;
        private int _terrainTileCollectionHeight;
        private int _terrainTileCollectionWidth;
        private TileCollection[,] _tileColls;
        private LoaderBase[][] _tileLoaders;

        public TileManager(Manager manager)
        {
            _manager = manager;
            _spriteBatch = PlayScreen.Spritebatch;
            var mainAtlas = PlayScreen.DecorationAtlas;

            _tileRegions = new TextureRegion[1 + (TileHelper.TypesOfTerrain.Count*24)];

            //for (var i = 0; i < _getIntensity.Length; i++)
            //{
            //    var minusLength = _getIntensity.Length - (float) i;
            //    minusLength /= _getIntensity.Length;
            //    var val = 128 + (int) (minusLength*128);
            //    _getIntensity[i] = new Color(val, val, val);
            //}

            var terrainCounter = 0;
            foreach (KeyValuePair<string, byte> t in TileHelper.TypesOfTerrain)
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

        public TileCollection MostLeftUpSprite { get { return _mostLeftUpSprite; } }
        public TileCollection MostRightDownSprite { get { return _mostRightDownSprite; } }

        public static TileCollection[,] TileColls { get; private set; }

        public void Dispose()
        {
            // TODO Auto-generated method stub
        }


        public void Update()
        {
            if (_terrainLoaded)
                CheckTerrain();
        }


        private void CheckTerrain()
        {
            if (_mostLeftUpSprite.HitRect.X + TileBuffer > Camera2D.MyCam.Position.X)
                _manager.MoveDirection(MoveConstants.Left);
            if (_mostRightDownSprite.HitRect.X - TileBuffer <
                Camera2D.MyCam.ViewportWidth + Camera2D.MyCam.Position.X)
                _manager.MoveDirection(MoveConstants.Right);
            if (_mostLeftUpSprite.HitRect.Y + TileBuffer > Camera2D.MyCam.Position.Y)
                _manager.MoveDirection(MoveConstants.Up);
            if (_mostRightDownSprite.HitRect.Y - TileBuffer <
                Camera2D.MyCam.ViewportHeight + Camera2D.MyCam.Position.Y)
                _manager.MoveDirection(MoveConstants.Down);
        }

        internal void LoadData(ref LoaderBase[][] cl)
        {
            _tileLoaders = cl;
            _terrainTileCollectionWidth = Convert.ToInt16(ScreenBuffer/64);
            _terrainTileCollectionHeight = _terrainTileCollectionWidth;
            _tileColls = new TileCollection[_terrainTileCollectionWidth,_terrainTileCollectionHeight];
            _terrainTextureOne = new RenderTarget2D(MainGame.GraphicD, ScreenBuffer, ScreenBuffer);
            _terrainTextureTwo = new RenderTarget2D(MainGame.GraphicD, ScreenBuffer, ScreenBuffer);

            _terrainPosition = Vector2.Zero;

            _graphicsDevice = MainGame.GraphicD;

            for (var i = 0; i < _terrainTileCollectionWidth; i ++)
            {
                for (var j = 0; j < _terrainTileCollectionHeight; j ++)
                {
                    var tc = new TileCollection(_tileRegions, i, j, _terrainTileCollectionWidth,
                                                _terrainTileCollectionHeight);
                    _tileColls[i, j] = tc;

                    if (i == 0 && j == 0)
                        _mostLeftUpSprite = tc;
                    else if (_terrainTileCollectionWidth - 1 == i && _terrainTileCollectionHeight - 1 == j)
                        _mostRightDownSprite = tc;

                    tc.SectorX = PlayScreen.SpawnX;
                    tc.SectorY = PlayScreen.SpawnY;
                    tc.MySectorSpotX = i;
                    tc.MySectorSpotY = j;
                    tc.SetPosition(i*Sprite2XWidth, j*Sprite2XWidth);
                    var ar = GetSurroundings(ref tc.MyLoaderBase, tc.SectorX, tc.SectorY, tc.MySectorSpotX,
                                             tc.MySectorSpotY);
                    tc.SetSurroundings(FindSurroundings(ar));
                }
            }
            InitialDraw();
            TileColls = _tileColls;
            _terrainLoaded = true;
        }

        private int[] FindSurroundings(int[] vals)
        {
            var tileVal = vals[4];
            _surroundings[0] = _surroundings[1] = _surroundings[2] = _surroundings[3] = 0;
            if (tileVal == 0)
                return _surroundings;
            var topLeft = vals[0];
            var topMiddle = vals[1];
            var topRight = vals[2];
            var midLeft = vals[3];

            var midRight = vals[5];
            var bottomLeft = vals[6];
            var bottomMiddle = vals[7];
            var bottomRight = vals[8];
            bool[] ba = null;
            if (tileVal == TileHelper.RiverWater || tileVal == TileHelper.OceanWater)
            {
                ba = new[]
                    {
                        topLeft == TileHelper.OceanWater || topLeft == TileHelper.RiverWater,
                        topMiddle == TileHelper.OceanWater || topMiddle == TileHelper.RiverWater,
                        topRight == TileHelper.OceanWater || topRight == TileHelper.RiverWater,
                        midLeft == TileHelper.OceanWater || midLeft == TileHelper.RiverWater,
                        midRight == TileHelper.OceanWater || midRight == TileHelper.RiverWater,
                        bottomLeft == TileHelper.OceanWater || bottomLeft == TileHelper.RiverWater,
                        bottomMiddle == TileHelper.OceanWater || bottomMiddle == TileHelper.RiverWater,
                        bottomRight == TileHelper.OceanWater || bottomRight == TileHelper.RiverWater
                    };
            }
            else if (tileVal == TileHelper.Sand)
            {
                ba = new[]
                    {
                        topLeft == TileHelper.OceanWater || topLeft == TileHelper.Sand,
                        topMiddle == TileHelper.OceanWater || topMiddle == TileHelper.Sand,
                        topRight == TileHelper.OceanWater || topRight == TileHelper.Sand,
                        midLeft == TileHelper.OceanWater || midLeft == TileHelper.Sand,
                        midRight == TileHelper.OceanWater || midRight == TileHelper.Sand,
                        bottomLeft == TileHelper.OceanWater || bottomLeft == TileHelper.Sand,
                        bottomMiddle == TileHelper.OceanWater || bottomMiddle == TileHelper.Sand,
                        bottomRight == TileHelper.OceanWater || bottomRight == TileHelper.Sand
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
            var offset = tileVal*24;
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

            _surroundings[0] = topLeft;
            _surroundings[1] = topRight;
            _surroundings[2] = bottomLeft;
            _surroundings[3] = bottomRight;

            return _surroundings;
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
            return null;
        }

        private int[] GetSurroundings(ref LoaderBase myLoaderBase, int sectX, int sectY, int sectSpotX, int sectSpotY)
        {
            myLoaderBase = GetSpriteTile(sectX, sectY);

            var holder = 0;
            const byte tileLength = PlayScreen.SectorTileSize;
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
                    _currentSurroundings[holder++] = val;
                }
            }

            return _currentSurroundings;
        }

        public void MoveTerrainDown()
        {
            //System.Diagnostics.Debug.WriteLine("hit tiles Move down");
            var wasTheLastOfLoader = _mostLeftUpSprite.MySectorSpotY + 1 >= PlayScreen.SectorTileSize;
            //old left values
            var oldVectorY = _mostLeftUpSprite.MyVectorSpotY;

            //new right values
            var newSectorY = _mostRightDownSprite.MySectorSpotY + 1 >= PlayScreen.SectorTileSize
                                 ? _mostRightDownSprite.SectorY + 1
                                 : _mostRightDownSprite.SectorY;
            var newSectorSpotY = _mostRightDownSprite.MySectorSpotY + 1 >= PlayScreen.SectorTileSize
                                     ? 0
                                     : _mostRightDownSprite.MySectorSpotY + 1;

            var newPositionY = _mostRightDownSprite.HitRect.Y + Sprite2XWidth;

            //switch leftpos/rightpos sprites
            _mostRightDownSprite =
                TileColls[
                    _mostRightDownSprite.MyVectorSpotX,
                    _mostRightDownSprite.MyVectorSpotY + 1 >= _terrainTileCollectionHeight
                        ? 0
                        : _mostRightDownSprite.MyVectorSpotY + 1];
            _mostLeftUpSprite =
                TileColls[
                    _mostLeftUpSprite.MyVectorSpotX,
                    _mostLeftUpSprite.MyVectorSpotY + 1 >= _terrainTileCollectionHeight
                        ? 0
                        : _mostLeftUpSprite.MyVectorSpotY + 1];

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionWidth; innerlooper++)
            {
                var oldSprite = TileColls[innerlooper, oldVectorY];
                oldSprite.SetPosition(oldSprite.HitRect.X, newPositionY);
                oldSprite.SectorY = newSectorY;
                oldSprite.MySectorSpotY = newSectorSpotY;
                var ar = GetSurroundings(ref oldSprite.MyLoaderBase, oldSprite.SectorX, oldSprite.SectorY,
                                         oldSprite.MySectorSpotX, oldSprite.MySectorSpotY);
                oldSprite.SetSurroundings(FindSurroundings(ar));
            }

            if (_mostLeftUpSprite.SectorY != _tileLoaders[1][1].SectorY && wasTheLastOfLoader)
            {
                if (_tileLoaders[1][0].SectorY == _mostLeftUpSprite.SectorY)
                    _manager.MoveLoaderBase(MoveConstants.Up);
                else
                    _manager.MoveLoaderBase(MoveConstants.Down);
            }

            PreDraw(MoveConstants.Down);
        }

        public void MoveTerrainLeft()
        {
            //System.Diagnostics.Debug.WriteLine("hit tiles Move left");
            var wasTheLastOfLoader = _mostRightDownSprite.MySectorSpotX == 0;
            //old left values
            var oldVectorX = _mostRightDownSprite.MyVectorSpotX;

            //new right values
            var newSectorX = _mostLeftUpSprite.MySectorSpotX - 1 < 0
                                 ? _mostLeftUpSprite.SectorX - 1
                                 : _mostLeftUpSprite.SectorX;
            var newSectorSpotX = _mostLeftUpSprite.MySectorSpotX - 1 < 0
                                     ? PlayScreen.SectorTileSize - 1
                                     : _mostLeftUpSprite.MySectorSpotX - 1;

            var newPositionX = _mostLeftUpSprite.HitRect.X - Sprite2XWidth;

            //switch leftpos/rightpos sprites
            _mostLeftUpSprite =
                TileColls[
                    _mostLeftUpSprite.MyVectorSpotX - 1 < 0
                        ? _terrainTileCollectionWidth - 1
                        : _mostLeftUpSprite.MyVectorSpotX - 1, _mostLeftUpSprite.MyVectorSpotY];
            _mostRightDownSprite =
                TileColls[
                    _mostRightDownSprite.MyVectorSpotX - 1 < 0
                        ? _terrainTileCollectionWidth - 1
                        : _mostRightDownSprite.MyVectorSpotX - 1, _mostRightDownSprite.MyVectorSpotY];

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionHeight; innerlooper++)
            {
                var oldSprite = TileColls[oldVectorX, innerlooper];
                oldSprite.SetPosition(newPositionX, oldSprite.HitRect.Y);
                oldSprite.SectorX = newSectorX;
                oldSprite.MySectorSpotX = newSectorSpotX;
                var ar = GetSurroundings(ref oldSprite.MyLoaderBase, oldSprite.SectorX, oldSprite.SectorY,
                                         oldSprite.MySectorSpotX, oldSprite.MySectorSpotY);
                oldSprite.SetSurroundings(FindSurroundings(ar));
            }

            if (_mostLeftUpSprite.SectorX != _tileLoaders[1][1].SectorX && wasTheLastOfLoader)
            {
                if (_tileLoaders[2][1].SectorX == _mostLeftUpSprite.SectorX)
                    _manager.MoveLoaderBase(MoveConstants.Right);
                else
                    _manager.MoveLoaderBase(MoveConstants.Left);
            }

            PreDraw(MoveConstants.Left);
        }

        public void MoveTerrainRight()
        {
            //System.Diagnostics.Debug.WriteLine("hit tiles Move right");
            var wasTheLastOfLoader = _mostLeftUpSprite.MySectorSpotX + 1 >= PlayScreen.SectorTileSize;

            //old left values
            var oldVectorX = _mostLeftUpSprite.MyVectorSpotX;

            //new right values
            var newSectorX = _mostRightDownSprite.MySectorSpotX + 1 >= PlayScreen.SectorTileSize
                                 ? _mostRightDownSprite.SectorX + 1
                                 : _mostRightDownSprite.SectorX;

            var newSectorSpotX = _mostRightDownSprite.MySectorSpotX + 1 >= PlayScreen.SectorTileSize
                                     ? 0
                                     : _mostRightDownSprite.MySectorSpotX + 1;

            var newPositionX = _mostRightDownSprite.HitRect.X + Sprite2XWidth;

            //switch leftpos/rightpos sprites
            _mostLeftUpSprite =
                TileColls[
                    _mostLeftUpSprite.MyVectorSpotX + 1 >= _terrainTileCollectionWidth
                        ? 0
                        : _mostLeftUpSprite.MyVectorSpotX + 1, _mostLeftUpSprite.MyVectorSpotY];
            _mostRightDownSprite =
                TileColls[
                    _mostRightDownSprite.MyVectorSpotX + 1 >= _terrainTileCollectionWidth
                        ? 0
                        : _mostRightDownSprite.MyVectorSpotX + 1, _mostRightDownSprite.MyVectorSpotY];

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionHeight; innerlooper++)
            {
                var oldSprite = TileColls[oldVectorX, innerlooper];
                oldSprite.SetPosition(newPositionX, oldSprite.HitRect.Y);
                oldSprite.SectorX = newSectorX;
                oldSprite.MySectorSpotX = newSectorSpotX;
                var ar = GetSurroundings(ref oldSprite.MyLoaderBase, oldSprite.SectorX, oldSprite.SectorY,
                                         oldSprite.MySectorSpotX, oldSprite.MySectorSpotY);
                oldSprite.SetSurroundings(FindSurroundings(ar));
            }


            if (_mostRightDownSprite.SectorX != _tileLoaders[1][1].SectorX && wasTheLastOfLoader)
            {
                if (_tileLoaders[2][1].SectorX == _mostRightDownSprite.SectorX)
                    _manager.MoveLoaderBase(MoveConstants.Right);
                else
                    _manager.MoveLoaderBase(MoveConstants.Left);
            }

            PreDraw(MoveConstants.Right);
        }

        public void MoveTerrainUp()
        {
            //System.Diagnostics.Debug.WriteLine("hit tiles Move up");
            // System.out.println("hit up");
            var wasTheLastOfLoader = _mostRightDownSprite.MySectorSpotY == 0;

            var oldVectorY = _mostRightDownSprite.MyVectorSpotY;

            //new right values
            var newSectorY = _mostLeftUpSprite.MySectorSpotY - 1 < 0
                                 ? _mostLeftUpSprite.SectorY - 1
                                 : _mostLeftUpSprite.SectorY;
            var newSectorSpotY = _mostLeftUpSprite.MySectorSpotY - 1 < 0
                                     ? PlayScreen.SectorTileSize - 1
                                     : _mostLeftUpSprite.MySectorSpotY - 1;

            var newPositionY = _mostLeftUpSprite.HitRect.Y - Sprite2XWidth;

            //switch leftpos/rightpos sprites

            _mostRightDownSprite =
                TileColls[
                    _mostRightDownSprite.MyVectorSpotX,
                    _mostRightDownSprite.MyVectorSpotY - 1 < 0
                        ? _terrainTileCollectionHeight - 1
                        : _mostRightDownSprite.MyVectorSpotY - 1];
            _mostLeftUpSprite =
                TileColls[
                    _mostLeftUpSprite.MyVectorSpotX,
                    _mostLeftUpSprite.MyVectorSpotY - 1 < 0
                        ? _terrainTileCollectionHeight - 1
                        : _mostLeftUpSprite.MyVectorSpotY - 1];

            for (var innerlooper = 0; innerlooper < _terrainTileCollectionWidth; innerlooper++)
            {
                var oldSprite = TileColls[innerlooper, oldVectorY];
                oldSprite.SetPosition(oldSprite.HitRect.X, newPositionY);
                oldSprite.SectorY = newSectorY;
                oldSprite.MySectorSpotY = newSectorSpotY;
                var ar = GetSurroundings(ref oldSprite.MyLoaderBase, oldSprite.SectorX, oldSprite.SectorY,
                                         oldSprite.MySectorSpotX, oldSprite.MySectorSpotY);
                oldSprite.SetSurroundings(FindSurroundings(ar));
            }


            if (_mostLeftUpSprite.SectorY != _tileLoaders[1][1].SectorY && wasTheLastOfLoader)
            {
                if (_tileLoaders[1][0].SectorY == _mostLeftUpSprite.SectorY)
                    _manager.MoveLoaderBase(MoveConstants.Up);
                else
                    _manager.MoveLoaderBase(MoveConstants.Down);
            }


            PreDraw(MoveConstants.Up);
        }

        private void InitialDraw()
        {
            _graphicsDevice.SetRenderTarget(_terrainTextureOne);
            _graphicsDevice.Clear(Color.Blue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);


            for (short i = 0; i < _terrainTileCollectionWidth; i++)
            {
                for (short j = 0; j < _terrainTileCollectionHeight; j++)
                    _tileColls[i, j].Draw();
            }

            _spriteBatch.End();
            _graphicsDevice.SetRenderTarget(null);
        }


        private void PreDraw(byte direction)
        {
            _renderToggle = !_renderToggle;
            var terrainTexture = _renderToggle ? _terrainTextureOne : _terrainTextureTwo;
            var terrainTextureOther = _renderToggle ? _terrainTextureTwo : _terrainTextureOne;

            _terrainPosition.X = _mostLeftUpSprite.HitRect.X;
            _terrainPosition.Y = _mostLeftUpSprite.HitRect.Y;

            _graphicsDevice.SetRenderTarget(terrainTexture);
            _graphicsDevice.Clear(Color.Green);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            var count = 0;
            var i = 0;
            switch (direction)
            {
                case MoveConstants.Down:
                    _spriteBatch.Draw(terrainTextureOther, _moveRasterDown, Color.White);

                    i = _mostLeftUpSprite.MyVectorSpotX;
                    while (count < _terrainTileCollectionWidth)
                    {
                        TileColls[i, _mostRightDownSprite.MyVectorSpotY].Draw(direction, count);
                        i = i + 1 == _terrainTileCollectionWidth ? 0 : i + 1;
                        count++;
                    }

                    break;
                case MoveConstants.Up:
                    _spriteBatch.Draw(terrainTextureOther, _moveRasterUp, Color.White);

                    i = _mostLeftUpSprite.MyVectorSpotX;
                    while (count < _terrainTileCollectionWidth)
                    {
                        TileColls[i, _mostLeftUpSprite.MyVectorSpotY].Draw(direction, count);
                        i = i + 1 == _terrainTileCollectionWidth ? 0 : i + 1;
                        count++;
                    }

                    break;
                case MoveConstants.Right:
                    _spriteBatch.Draw(terrainTextureOther, _moveRasterRight, Color.White);
                    i = _mostLeftUpSprite.MyVectorSpotY;
                    while (count < _terrainTileCollectionHeight)
                    {
                        TileColls[_mostRightDownSprite.MyVectorSpotX, i].Draw(direction, count);
                        i = i + 1 == _terrainTileCollectionHeight ? 0 : i + 1;
                        count++;
                    }
                    break;
                case MoveConstants.Left:
                    _spriteBatch.Draw(terrainTextureOther, _moveRasterLeft, Color.White);
                    i = _mostLeftUpSprite.MyVectorSpotY;

                    while (count < _terrainTileCollectionHeight)
                    {
                        TileColls[_mostLeftUpSprite.MyVectorSpotX, i].Draw(direction, count);
                        i = i + 1 == _terrainTileCollectionHeight ? 0 : i + 1;
                        count++;
                    }

                    break;
            }
            _spriteBatch.End();
            _graphicsDevice.SetRenderTarget(null);
        }

        public void Draw()
        {
            _spriteBatch.Draw(_renderToggle ? _terrainTextureOne : _terrainTextureTwo, _terrainPosition, Color.White);
            //test collections
#if DEBUG
            for (var x = 0; x < _terrainTileCollectionWidth; x++)
            {
                for (var y = 0; y < _terrainTileCollectionHeight; y++)
                    _tileColls[x, y].DrawHitBox();
            }
#endif
        }
    }
}