#region

using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.WorldGeneration;
using Ship.Game.Play.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ship.Game.Play.Utils;
using Ship.Game.Play.Beans.Tiles;
using System.Threading;
using System.ComponentModel;
using System;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class MiniTileManager
    {
        public const int TileSize = 16;
        public const int TileSizeX2 = TileSize*2;
        public const int Amount = 5;
        public static readonly Vector2 MiniCollectionBottom = new Vector2(0, TileSize);
        public static readonly Vector2 MiniCollectionRight = new Vector2(TileSize, 0);
        private readonly AlphaTestEffect _a;
        // //terrain
        private readonly GraphicsDevice _graphicsDevice;
        private readonly int _halfAmount;

        private readonly TextureProperties[][] _landFeatureTextureLocations;
        private readonly TextureProperties[][] _landTextureLocations;
        private readonly TextureAtlas _mainAtlas;

        //private readonly Thread _mapMakerThread;
        private readonly Vector2 _mapModifiedLocation = new Vector2(24f, 12f);
        private readonly RenderTarget2D _miniMapFinalDrawTexture;
        private readonly RenderTarget2D _miniMapFogTexture;
        private readonly RenderTarget2D _miniMapTerrainTexture;
        private readonly RenderTarget2D _miniMapTerrainTexture2;

        private readonly DepthStencilState _s1;
        private readonly DepthStencilState _s2;

        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _textureMask;
        private readonly WorldData _wd;
        private MiniTileCollection _bottomRight;
        private bool _currentlyUpdating;
        private MiniFogCollection[,] _fog;
        private MiniTileCollection[,] _land;
        private MiniTileCollection[,] _landFeature;
        private Texture2D _mainTextures;
        private bool _readyForPreDraw;
        private bool _textureToggle;
        private MiniTileCollection _topLeft;
        private bool _updateMiniMap;
        private int _xOffset;
        private int _yOffset;

        public MiniTileManager()
        {
            _wd = WorldData.MyWorldData;
            _mainAtlas = PlayScreen.DecorationAtlas;
            _mainTextures = PlayScreen.DecorationTexture;
            _spriteBatch = PlayScreen.Spritebatch;
            //_mapMakerThread = new Thread(CheckMiniMap);

            _halfAmount = (int) (Math.Floor(Amount/2.0));
            _xOffset = PlayScreen.SpawnX - _halfAmount;
            _yOffset = PlayScreen.SpawnY - _halfAmount;
            _land = new MiniTileCollection[Amount,Amount];
            _landFeature = new MiniTileCollection[Amount,Amount];
            _fog = new MiniFogCollection[Amount,Amount];
            _landTextureLocations = new TextureProperties[MainGame.MapWidth][];
            _landFeatureTextureLocations = new TextureProperties[MainGame.MapWidth][];
            _graphicsDevice = MainGame.GraphicD;

            _miniMapFogTexture = new RenderTarget2D(MainGame.GraphicD, 256, 256, false, SurfaceFormat.Color,
                                                    DepthFormat.Depth24Stencil8,
                                                    0, RenderTargetUsage.DiscardContents);

            _miniMapTerrainTexture = new RenderTarget2D(MainGame.GraphicD, 256, 256, false, SurfaceFormat.Color,
                                                        DepthFormat.Depth24Stencil8,
                                                        0, RenderTargetUsage.DiscardContents);
            _miniMapTerrainTexture2 = new RenderTarget2D(MainGame.GraphicD, 256, 256, false, SurfaceFormat.Color,
                                                         DepthFormat.Depth24Stencil8,
                                                         0, RenderTargetUsage.DiscardContents);

            _miniMapFinalDrawTexture = new RenderTarget2D(MainGame.GraphicD, 256, 256, false, SurfaceFormat.Color,
                                                          DepthFormat.Depth24Stencil8,
                                                          0, RenderTargetUsage.DiscardContents);
            //testing

            _textureMask = CreateCircle(76);

            var m = Matrix.CreateOrthographicOffCenter(0, _miniMapFogTexture.Width, _miniMapFogTexture.Height, 0, 0,1);

            _a = new AlphaTestEffect(_graphicsDevice)
                {
                    Projection = m
                };

            _s1 = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = 1,
                    DepthBufferEnable = false,
                };

            _s2 = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.LessEqual,
                    StencilPass = StencilOperation.Keep,
                    ReferenceStencil = 1,
                    DepthBufferEnable = false,
                };


            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                _landTextureLocations[x] = new TextureProperties[MainGame.MapHeight];
                _landFeatureTextureLocations[x] = new TextureProperties[MainGame.MapHeight];
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var landtext = new TextureProperties();
                    var landfeaturetext = new TextureProperties();
                    FillHeight(ref landtext, x, y);

                    if (!CanFillTrees(ref landfeaturetext, x, y))
                    {
                        if (!CanFillGrass(ref landfeaturetext, x, y))
                        {
                            if (!CanFillMountains(ref landfeaturetext, x, y))
                                landfeaturetext.HasFeature = false;
                        }
                    }


                    _landTextureLocations[x][y] = landtext;
                    _landFeatureTextureLocations[x][y] = landfeaturetext;
                }
            }

            for (var i = 0; i < Amount; i++)
            {
                var left = i - 1 < 0 ? Amount - 1 : i - 1;
                var right = i + 1 >= Amount ? 0 : i + 1;
                for (var j = 0; j < Amount; j++)
                {
                    _fog[i, j] = new MiniFogCollection();

                    var top = j - 1 < 0 ? Amount - 1 : j - 1;
                    var bottom = j + 1 >= Amount ? 0 : j + 1;
                    _land[i, j] = new MiniTileCollection(i + _xOffset, j + _yOffset, i, j, top, bottom, left, right);
                    _landFeature[i, j] = new MiniTileCollection(i + _xOffset, j + _yOffset, i, j, top, bottom, left,
                                                                right);

                    if (i == 0 && j == 0)
                        _topLeft = _land[i, j];
                    else if (i == Amount - 1 && j == Amount - 1)
                        _bottomRight = _land[i, j];

                    var prop = _landTextureLocations[_land[i, j].MapLocX][_land[i, j].MapLocY];
                    SetBounds(prop.TextureName, ref _land[i, j], prop.Vals, false);
                    _land[i, j].Visible = true;

                    var prop2 = _landFeatureTextureLocations[_landFeature[i, j].MapLocX][_landFeature[i, j].MapLocY];
                    if (prop2.HasFeature)
                    {
                        SetBounds(prop2.TextureName, ref _landFeature[i, j], prop2.Vals, prop2.IsMountain);
                        _landFeature[i, j].Visible = true;
                    }
                    else
                        _landFeature[i, j].Visible = false;

                    _fog[i, j].SetSpots(i, j);
                    FillFog(i, j);
                }
            }
            InitialDraw();
        }

        private Texture2D CreateCircle(int radius)
        {
            var outerRadius = radius*2 + 2; // So circle doesn't go out of bounds
            var texture = new Texture2D(_graphicsDevice, outerRadius, outerRadius);

            var data = new Color[outerRadius*outerRadius];

            // Colour the entire texture transparent first.
            for (var i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f/radius;

            for (double angle = 0; angle < Math.PI*2; angle += angleStep)
            {
                var x = (int) Math.Round(radius + radius*Math.Cos(angle));
                var y = (int) Math.Round(radius + radius*Math.Sin(angle));

                data[y*outerRadius + x + 1] = Color.White;
            }

            texture.SetData(data);

            var c = new Color[1];
            bool startChange;
            var halfWidth = texture.Width/2;
            for (var y = texture.Height - 1; y >= 0; y--)
            {
                startChange = false;
                for (var x = texture.Width - 1; x >= halfWidth; x--)
                {
                    if (startChange)
                        texture.SetData(0, new Rectangle(x, y, 1, 1), c, 0, 1);
                    else
                    {
                        texture.GetData(0, new Rectangle(x, y, 1, 1), c, 0, 1);
                        if (c[0] == Color.White)
                            startChange = true;
                    }
                }
                c[0] = Color.Transparent;
            }

            for (var y = texture.Height - 1; y >= 0; y--)
            {
                startChange = false;
                for (var x = 0; x < halfWidth; x++)
                {
                    if (startChange)
                        texture.SetData(0, new Rectangle(x, y, 1, 1), c, 0, 1);
                    else
                    {
                        texture.GetData(0, new Rectangle(x, y, 1, 1), c, 0, 1);
                        if (c[0] == Color.White)
                            startChange = true;
                    }
                }
                c[0] = Color.Transparent;
            }

            return texture;
        }

        private void FillFog(int x, int y)
        {
            var vals = GetData(_wd.FogData, x + _xOffset, y + _yOffset, _wd.FogData[x + _xOffset][y + _yOffset]);
            const string textureName = "fogwar";
            var bstring1 = string.Format("{0}{1}", textureName, vals[0]);
            var bstring2 = string.Format("{0}{1}", textureName, vals[1]);
            var bstring3 = string.Format("{0}{1}", textureName, vals[2]);
            var bstring4 = string.Format("{0}{1}", textureName, vals[3]);
            var bounds1 = _mainAtlas.GetRegion(bstring1).Bounds;
            var bounds2 = _mainAtlas.GetRegion(bstring2).Bounds;
            var bounds3 = _mainAtlas.GetRegion(bstring3).Bounds;
            var bounds4 = _mainAtlas.GetRegion(bstring4).Bounds;
            _fog[x, y].UpdateTexture(bounds1, bounds2, bounds3, bounds4);

            _fog[x, y].IsDiscovered = _wd.FogData[x + _xOffset][y + _yOffset] == FogWar.None;
            _fog[x, y].IsAllFogOfWar = true;
            if (!_fog[x, y].IsDiscovered)
            {
                var added = false;
                for (var xx = -1; xx < 2; xx++)
                {
                    for (var yy = -1; yy < 2; yy++)
                    {
                        if (added)
                            break;

                        if (_wd.FogData[x + xx + _xOffset][y + yy + _yOffset] == FogWar.None)
                        {
                            _fog[x, y].IsAllFogOfWar = false;
                            added = true;
                        }
                    }
                    if (added)
                        break;
                }
            }
            else
                _fog[x, y].IsAllFogOfWar = false;
        }

        private int[] GetData(byte[][] mapData, int x, int y, byte type)
        {
            var center = GetValue(mapData, x, y, type);
            var topLeft = GetValue(mapData, x - 1, y - 1, type);
            var top = GetValue(mapData, x, y - 1, type);
            var topRight = GetValue(mapData, x + 1, y - 1, type);
            var left = GetValue(mapData, x - 1, y, type);
            var right = GetValue(mapData, x + 1, y, type);
            var bottomLeft = GetValue(mapData, x - 1, y + 1, type);
            var bottom = GetValue(mapData, x, y + 1, type);
            var bottomRight = GetValue(mapData, x + 1, y + 1, type);
            var sur = new[] {topLeft, top, topRight, left, center, right, bottomLeft, bottom, bottomRight};
            return SetSurroundings(sur, (center >= HeightGenerator.Plains),
                                   center == HeightGenerator.Beach || center == HeightGenerator.Desert);
        }

        private byte GetValue(byte[][] ba, int x, int y, byte type)
        {
            if (x < 0)
                return type;
            if (y < 0)
                return type;
            if (x >= ba.Length)
                return type;
            return y >= ba[0].Length ? type : ba[x][y];
        }

        private int[] GetDataOther(byte[][] mapData, int x, int y)
        {
            var center = mapData[x][y];
            var topLeft = GetValue(mapData, x - 1, y - 1, center);
            var top = GetValue(mapData, x, y - 1, center);
            var topRight = GetValue(mapData, x + 1, y - 1, center);
            var left = GetValue(mapData, x - 1, y, center);
            var right = GetValue(mapData, x + 1, y, center);
            var bottomLeft = GetValue(mapData, x - 1, y + 1, center);
            var bottom = GetValue(mapData, x, y + 1, center);
            var bottomRight = GetValue(mapData, x + 1, y + 1, center);
            var sur = new[] {topLeft, top, topRight, left, center, right, bottomLeft, bottom, bottomRight};
            return SetSurroundingsOther(sur);
        }

        private bool CanFillMountains(ref TextureProperties prop, int x, int y)
        {
            var center = _wd.HeightShowData[x][y];
            if (center != HeightGenerator.Hills && center != HeightGenerator.Mountains) return false;

           // var temperature = _wd.TempData[x][y];
            var forest = _wd.ForestData[x][y];
            var textureName = center == HeightGenerator.Hills ? "hill_" : "mountain_";

            //switch (temperature)
            //{
            //    case TemperatureGenerator.Cold:
            //        textureName += "snow";
            //        break;
            //    case TemperatureGenerator.Mild:
                    textureName += "plain";
                 //   break;
            //    case TemperatureGenerator.Warm:
            //        textureName += "jungle";
            //        break;
            //    case TemperatureGenerator.Hot:
            //        textureName += "desert";
            //        break;
            //}

            if (center != HeightGenerator.Mountains)
                textureName = forest > ForestGenerator.None ? textureName + "_trees" : textureName;


            if (textureName.Length != 0)
            {
                prop.SetLocations(textureName, true);
                return true;
            }
#if DEBUG
            else
                System.Diagnostics.Debug.WriteLine("Bad news, check mini tile manager");
#endif
            return false;
        }

        private bool CanFillGrass(ref TextureProperties prop, int x, int y)
        {
            var center = _wd.GrassShowData[x][y];
            if (center != GrassGenerator.Exist) return false;

            var vals = GetDataOther(_wd.GrassShowData, x, y);
           // var temperature = _wd.TempData[x][y];

            var textureName = "";
            //switch (temperature)
            //{
            //    case TemperatureGenerator.Cold:
            //        textureName = "grass_snow";
            //        break;
            //    case TemperatureGenerator.Mild:
                    textureName = "grass_plain";
            //        break;
            //    case TemperatureGenerator.Warm:
            //        textureName = "grass_jungle";
            //        break;
            //    case TemperatureGenerator.Hot:
            //        textureName = "grass_desert";
            //        break;
            //}
            //return SetBounds(textureName, ref _landFeature, vals, x, y);
            prop.SetLocations(textureName, vals);
            return true;
        }

        private bool CanFillTrees(ref TextureProperties prop, int x, int y)
        {
            var center = _wd.ForestShowData[x][y];
            if (center == ForestGenerator.None)
                return false;
            //var temperature = _wd.TempData[x][y];
            var vals = GetDataOther(_wd.ForestShowData, x, y);

            var textureName = "";

            //switch (temperature)
            //{
            //    case TemperatureGenerator.Cold:
            //        textureName = "trees_snow";
            //        break;
            //    case TemperatureGenerator.Mild:
                    textureName = "trees_plain";
            //        break;
            //    case TemperatureGenerator.Warm:
            //        textureName = "trees_jungle";
            //        break;
            //    case TemperatureGenerator.Hot:
            //        textureName = "trees_desert";
            //        break;
            //}

            //return SetBounds(textureName, ref _landFeature, vals, x, y);
            prop.SetLocations(textureName, vals);
            return true;
        }

        private void SetBounds(string textureName, ref MiniTileCollection updateMe, int[] vals, bool isElevated)
        {
            if (textureName.Length == 0)
                System.Diagnostics.Debug.WriteLine("Bad news, check mini tile manager");
            else
            {
                if (isElevated)
                {
                    var bounds = _mainAtlas.GetRegion(textureName).Bounds;
                    updateMe.UpdateTexture(bounds, bounds, bounds, bounds, true);
                }
                else
                {
                    var bounds1String = string.Format("{0}{1}", textureName, vals[0]);
                    var bounds2String = string.Format("{0}{1}", textureName, vals[1]);
                    var bounds3String = string.Format("{0}{1}", textureName, vals[2]);
                    var bounds4String = string.Format("{0}{1}", textureName, vals[3]);
                    var bounds1 = _mainAtlas.GetRegion(bounds1String).Bounds;
                    var bounds2 = _mainAtlas.GetRegion(bounds2String).Bounds;
                    var bounds3 = _mainAtlas.GetRegion(bounds3String).Bounds;
                    var bounds4 = _mainAtlas.GetRegion(bounds4String).Bounds;
                    updateMe.UpdateTexture(bounds1, bounds2, bounds3, bounds4, false);
                }
            }
        }

        private void FillHeight(ref TextureProperties prop, int x, int y)
        {
            var center = _wd.HeightShowData[x][y];

            var vals = GetData(_wd.HeightShowData, x, y, center);
            //var temperature = _wd.TempData[x][y];

            var textureName = "";
            //if (temperature == TemperatureGenerator.Frozen || temperature == TemperatureGenerator.Cold)
            //{
            //    switch (center)
            //    {
            //        case HeightGenerator.DeepOcean:
            //        case HeightGenerator.Ocean:
            //        case HeightGenerator.ShallowOcean:
            //            textureName = "water_snow";
            //            break;
            //        case HeightGenerator.Beach:
            //            textureName = "beach";
            //            break;
            //        case HeightGenerator.Plains:
            //        case HeightGenerator.Hills:
            //        case HeightGenerator.Mountains:
            //            textureName = "snow";
            //            break;
            //    }
            //}
            //else
            //{
                switch (center)
                {
                    case HeightGenerator.DeepOcean:
                        textureName = "water_deep";
                        break;
                    case HeightGenerator.Ocean:
                        textureName = "water_ocean";
                        break;
                    case HeightGenerator.ShallowOcean:
                        textureName = "water_shallow";
                        break;
                    case HeightGenerator.Beach:
                        textureName = "beach";
                        break;
                    case HeightGenerator.Plains:
                    case HeightGenerator.Hills:
                    case HeightGenerator.Mountains:
                        textureName = "plains";
                        break;
                }
            //}

            //SetBounds(textureName, ref _land, vals, x, y);

            prop.SetLocations(textureName, vals);
        }

        private int[] SetSurroundings(byte[] vals, bool isGrass, bool isBeach)
        {
            var topLeft = vals[0];
            var topMiddle = vals[1];
            var topRight = vals[2];
            var midLeft = vals[3];
            var tileVal = vals[4];
            if (isGrass)
                tileVal = HeightGenerator.Plains;
            var midRight = vals[5];
            var bottomLeft = vals[6];
            var bottomMiddle = vals[7];
            var bottomRight = vals[8];
            bool[] ba;
            if (isGrass)
            {
                ba = new[]
                    {
                        topLeft >= tileVal, topMiddle >= tileVal, topRight >= tileVal,
                        midLeft >= tileVal, midRight >= tileVal,
                        bottomLeft >= tileVal, bottomMiddle >= tileVal,
                        bottomRight >= tileVal
                    };
            }
            else if (isBeach)
            {
                ba = new[]
                    {
                        topLeft == tileVal, topMiddle == tileVal, topRight == tileVal,
                        midLeft == tileVal, midRight == tileVal,
                        bottomLeft == tileVal, bottomMiddle == tileVal,
                        bottomRight == tileVal
                    };
            }
            else
            {
                ba = new[]
                    {
                        topLeft <= tileVal, topMiddle <= tileVal, topRight <= tileVal,
                        midLeft <= tileVal, midRight <= tileVal,
                        bottomLeft <= tileVal, bottomMiddle <= tileVal,
                        bottomRight <= tileVal
                    };
            }
            return FillArea(ba);
        }

        private int[] SetSurroundingsOther(byte[] vals)
        {
            var topLeft = vals[0];
            var topMiddle = vals[1];
            var topRight = vals[2];
            var midLeft = vals[3];
            var midRight = vals[5];
            var bottomLeft = vals[6];
            var bottomMiddle = vals[7];
            var bottomRight = vals[8];

            var ba = new[]
                {
                    topLeft != 0, topMiddle != 0, topRight != 0,
                    midLeft != 0, midRight != 0,
                    bottomLeft != 0, bottomMiddle != 0, bottomRight != 0
                };


            return FillArea(ba);
        }

        private int[] FillArea(bool[] boolVals)
        {
            var upLeftFine = boolVals[0];
            var upFine = boolVals[1];
            var upRightFine = boolVals[2];
            var leftFine = boolVals[3];
            var rightFine = boolVals[4];
            var bottomLeftFine = boolVals[5];
            var bottomFine = boolVals[6];
            var bottomRightFine = boolVals[7];
            int topLeftNum, topRightNum, bottomLeftNum, bottomRightNum;
            if (!leftFine && rightFine && upFine && bottomFine)
            {
                topLeftNum = 12;
                topRightNum = 13;
                bottomLeftNum = 16;
                bottomRightNum = 14;
                if (!upRightFine)
                    topRightNum = 3;
                if (!bottomRightFine)
                    bottomRightNum = 7;
            }
            else if (leftFine && !rightFine && upFine && bottomFine)
            {
                topLeftNum = 13;
                topRightNum = 15;
                bottomLeftNum = 17;
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
                bottomRightNum = 14;
                if (!bottomLeftFine)
                    bottomLeftNum = 6;
                if (!bottomRightFine)
                    bottomRightNum = 7;
            }
            else if (leftFine && rightFine && upFine && !bottomFine)
            {
                topLeftNum = 13;
                topRightNum = 14;
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
                    bottomRightNum = 18;
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
                    bottomLeftNum = 17;
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
                var topRights = 14;
                var bottomLefts = 17;
                var bottomRights = 18;
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

            int[] ar = {topLeftNum, topRightNum, bottomLeftNum, bottomRightNum};
            return ar;
        }

        public void Shift(byte direction)
        {
            _xOffset = PlayScreen.SpawnX - _halfAmount;
            _yOffset = PlayScreen.SpawnY - _halfAmount;
            switch (direction)
            {
                case MoveConstants.Up:
                    ShiftUp();
                    break;
                case MoveConstants.Down:
                    ShiftDown();
                    break;
                case MoveConstants.Left:
                    ShiftLeft();
                    break;
                case MoveConstants.Right:
                    ShiftRight();
                    break;
            }
            for (var i = 0; i < Amount; i++)
            {
                for (var j = 0; j < Amount; j++)
                {
                    _fog[i, j].SetSpots(i, j);
                    FillFog(i, j);
                }
            }
            AddonDraw(direction);
        }

        private void ShiftUp()
        {
            var oldVectorSpotY = _bottomRight.CurrVectorY;
            var newMapLocY = _topLeft.MapLocY - 1;
            _topLeft = _land[_topLeft.CurrVectorX, oldVectorSpotY];
            _bottomRight = _land[_bottomRight.CurrVectorX, _bottomRight.TopVectorY];
            for (var i = 0; i < Amount; i++)
            {
                var landTemp = _land[i, oldVectorSpotY];
                landTemp.MapLocY = newMapLocY;
                var prop = _landTextureLocations[landTemp.MapLocX][landTemp.MapLocY];
                SetBounds(prop.TextureName, ref landTemp, prop.Vals, false);

                var landFTemp = _landFeature[i, oldVectorSpotY];
                landFTemp.MapLocY = newMapLocY;

                var prop2 = _landFeatureTextureLocations[landFTemp.MapLocX][landFTemp.MapLocY];
                if (prop2.HasFeature)
                {
                    SetBounds(prop2.TextureName, ref landFTemp, prop2.Vals, prop2.IsMountain);
                    landFTemp.Visible = true;
                }
                else
                    landFTemp.Visible = false;
            }
        }

        private void ShiftDown()
        {
            var oldVectorSpotY = _topLeft.CurrVectorY;
            var newMapLocY = _bottomRight.MapLocY + 1;
            _topLeft = _land[_topLeft.CurrVectorX, oldVectorSpotY];
            _bottomRight = _land[_bottomRight.CurrVectorX, _bottomRight.BottomVectorY];
            for (var i = 0; i < Amount; i++)
            {
                // var fogTemp = _fog[i][_amount - 1];
                var landTemp = _land[i, oldVectorSpotY];
                landTemp.MapLocY = newMapLocY;
                var prop = _landTextureLocations[landTemp.MapLocX][landTemp.MapLocY];
                SetBounds(prop.TextureName, ref landTemp, prop.Vals, false);

                var landFTemp = _landFeature[i, oldVectorSpotY];
                landFTemp.MapLocY = newMapLocY;

                var prop2 = _landFeatureTextureLocations[landFTemp.MapLocX][landFTemp.MapLocY];
                if (prop2.HasFeature)
                {
                    SetBounds(prop2.TextureName, ref landFTemp, prop2.Vals, prop2.IsMountain);
                    landFTemp.Visible = true;
                }
                else
                    landFTemp.Visible = false;
            }
        }

        private void ShiftLeft()
        {
            var oldVectorSpotX = _bottomRight.CurrVectorX;
            var newMapLocX = _topLeft.MapLocX - 1;
            _topLeft = _land[oldVectorSpotX, _topLeft.CurrVectorY];
            _bottomRight = _land[_bottomRight.LeftVectorX, _bottomRight.CurrVectorY];
            for (var i = 0; i < Amount; i++)
            {
                // var fogTemp = _fog[i][_amount - 1];
                var landTemp = _land[oldVectorSpotX, i];
                landTemp.MapLocX = newMapLocX;
                var prop = _landTextureLocations[landTemp.MapLocX][landTemp.MapLocY];
                SetBounds(prop.TextureName, ref landTemp, prop.Vals, false);

                var landFTemp = _landFeature[oldVectorSpotX, i];
                landFTemp.MapLocX = newMapLocX;

                var prop2 = _landFeatureTextureLocations[landFTemp.MapLocX][landFTemp.MapLocY];
                if (prop2.HasFeature)
                {
                    SetBounds(prop2.TextureName, ref landFTemp, prop2.Vals, prop2.IsMountain);
                    landFTemp.Visible = true;
                }
                else
                    landFTemp.Visible = false;
            }
        }

        private void ShiftRight()
        {
            var oldVectorSpotX = _topLeft.CurrVectorX;
            var newMapLocX = _bottomRight.MapLocX + 1;
            _topLeft = _land[oldVectorSpotX, _topLeft.CurrVectorY];
            _bottomRight = _land[_bottomRight.RightVectorX, _bottomRight.CurrVectorY];
            for (var i = 0; i < Amount; i++)
            {
                // var fogTemp = _fog[i][_amount - 1];
                var landTemp = _land[oldVectorSpotX, i];
                landTemp.MapLocX = newMapLocX;
                var prop = _landTextureLocations[landTemp.MapLocX][landTemp.MapLocY];
                SetBounds(prop.TextureName, ref landTemp, prop.Vals, false);

                var landFTemp = _landFeature[oldVectorSpotX, i];
                landFTemp.MapLocX = newMapLocX;

                var prop2 = _landFeatureTextureLocations[landFTemp.MapLocX][landFTemp.MapLocY];
                if (prop2.HasFeature)
                {
                    SetBounds(prop2.TextureName, ref landFTemp, prop2.Vals, prop2.IsMountain);
                    landFTemp.Visible = true;
                }
                else
                    landFTemp.Visible = false;
            }
        }

        private void InitialDraw()
        {
            _graphicsDevice.SetRenderTarget(_miniMapFogTexture);
            _graphicsDevice.Clear(Color.Transparent);
            PlayScreen.Spritebatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null);
            for (var i = 0; i < Amount; i++)
            {
                for (var j = 0; j < Amount; j++)
                {
                    if (!_fog[i, j].IsAllFogOfWar)
                        _fog[i, j].Draw();
                    else if (!_fog[i, j].IsDiscovered)
                        _fog[i, j].Draw();
                }
            }
            PlayScreen.Spritebatch.End();

            _graphicsDevice.SetRenderTarget(_miniMapTerrainTexture);
            _graphicsDevice.Clear(Color.Red);
            PlayScreen.Spritebatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null);
            for (var i = 0; i < Amount; i++)
            {
                for (var j = 0; j < Amount; j++)
                {
                    //if (_fog[i][j].IsAllFogOfWar) continue;

                    _land[i, j].Draw();
                    _landFeature[i, j].Draw();
                }
            }
            PlayScreen.Spritebatch.End();

            MaskDraw();
        }

        private void AddonDraw(byte direction)
        {
            _graphicsDevice.SetRenderTarget(_miniMapFogTexture);
            _graphicsDevice.Clear(Color.Transparent);
            PlayScreen.Spritebatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null);
            for (var i = 0; i < Amount; i++)
            {
                for (var j = 0; j < Amount; j++)
                {
                    if (!_fog[i, j].IsAllFogOfWar)
                        _fog[i, j].Draw();
                    else if (!_fog[i, j].IsDiscovered)
                        _fog[i, j].Draw();
                }
            }
            PlayScreen.Spritebatch.End();

            var terrainWriteRt = _textureToggle ? _miniMapTerrainTexture : _miniMapTerrainTexture2;
            var terrainReadRt = _textureToggle ? _miniMapTerrainTexture2 : _miniMapTerrainTexture;

            _graphicsDevice.SetRenderTarget(terrainWriteRt);
            _graphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);

            PlayScreen.Spritebatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null);
            switch (direction)
            {
                case MoveConstants.Down:
                    PlayScreen.Spritebatch.Draw(terrainReadRt, new Vector2(0, -TileSizeX2), Color.White);
                    for (var i = 0; i < Amount; i++)
                    {
                        _land[i, _bottomRight.CurrVectorY].Draw(direction, i);
                        _landFeature[i, _bottomRight.CurrVectorY].Draw(direction, i);
                    }
                    break;
                case MoveConstants.Up:
                    PlayScreen.Spritebatch.Draw(terrainReadRt, new Vector2(0, TileSizeX2), Color.White);
                    for (var i = 0; i < Amount; i++)
                    {
                        _land[i, _topLeft.CurrVectorY].Draw(direction, i);
                        _landFeature[i, _topLeft.CurrVectorY].Draw(direction, i);
                    }
                    break;
                case MoveConstants.Left:
                    PlayScreen.Spritebatch.Draw(terrainReadRt, new Vector2(TileSizeX2, 0), Color.White);
                    for (var i = 0; i < Amount; i++)
                    {
                        _land[_topLeft.CurrVectorX, i].Draw(direction, i);
                        _landFeature[_topLeft.CurrVectorX, i].Draw(direction, i);
                    }
                    break;
                case MoveConstants.Right:
                    PlayScreen.Spritebatch.Draw(terrainReadRt, new Vector2(-TileSizeX2, 0), Color.White);
                    for (var i = 0; i < Amount; i++)
                    {
                        _land[_bottomRight.CurrVectorX, i].Draw(direction, i);
                        _landFeature[_bottomRight.CurrVectorX, i].Draw(direction, i);
                    }
                    break;
            }
            _textureToggle = !_textureToggle;
            PlayScreen.Spritebatch.End();
            MaskDraw();
        }

        private void MaskDraw()
        {
            _graphicsDevice.SetRenderTarget(_miniMapFinalDrawTexture);
            _graphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);

            //The mask  
            PlayScreen.Spritebatch.Begin(SpriteSortMode.Immediate, null, null, _s1, null, _a);
            PlayScreen.Spritebatch.Draw(_textureMask, Vector2.Zero, Color.White);
            PlayScreen.Spritebatch.End();

            PlayScreen.Spritebatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, _s2, null, _a);
            PlayScreen.Spritebatch.Draw(_miniMapTerrainTexture, Vector2.Zero, Color.White);
            PlayScreen.Spritebatch.Draw(_miniMapFogTexture, Vector2.Zero, Color.White);
            PlayScreen.Spritebatch.End();

            _graphicsDevice.SetRenderTarget(null);
        }

        internal void Draw() { _spriteBatch.Draw(_miniMapFinalDrawTexture, Camera2D.MyCam.Position + _mapModifiedLocation, Color.White); }
    }
}