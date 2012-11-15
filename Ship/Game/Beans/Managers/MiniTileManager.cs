#region

using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.WorldGeneration;
using Ship.Game.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ship.Game.Utils;
using Ship.Game.Beans.Tiles;
using System.Threading;
using System.ComponentModel;

#endregion

namespace Ship.Game.Beans.Managers
{
    public class MiniTileManager
    {
        public const byte RotateUp = 0;
        public const byte RotateDown = 1;
        public const byte RotateLeft = 2;
        public const byte RotateRight = 3;
        // //terrain
        private readonly WorldData _wd;
        private int _xOffset;
        private int _yOffset;
        private readonly MiniTileCollection[][] _land;
        private readonly MiniTileCollection[][] _landFeature;
        private readonly Texture2D _mainTextures;
        private readonly TextureAtlas _mainAtlas;
        private readonly int _amount ;

        public MiniTileManager(ref Texture2D mainTextures, ref TextureAtlas mainAtlas, int xOffset, int yOffset, int amount)
        {
            _wd = MainGame.WorldData;
            _mainAtlas = mainAtlas;
            _mainTextures = mainTextures;
            _amount = amount;
            var minusPos = (int)(System.Math.Floor(amount / 2.0));
            _xOffset = xOffset - minusPos;
            _yOffset = yOffset - minusPos;
            _land = new MiniTileCollection[amount][];
            _landFeature = new MiniTileCollection[amount][];

            //testing
            

            for (var i = 0; i < amount; i++)
            {
                _land[i] = new MiniTileCollection[amount];
                _landFeature[i] = new MiniTileCollection[amount];
                
                for(var j = 0; j < amount; j++)
                {
                    _land[i][j] = new MiniTileCollection(i, j);
                    _landFeature[i][j] = new MiniTileCollection(i, j);
                }
            }

            MakeHeight();
            MakeFeatures();
            _bw.DoWork += Run;


        }

        

        public void Draw(ref SpriteBatch spriteBatch, Vector2 mapLocation)
        {
            for (var i = 0; i < _land.Length; i++ )
            {
                for (var j = 0; j < _land.Length; j++)
                {
                    
                    _land[i][j].Draw(spriteBatch, _mainTextures, mapLocation,GameLayer.MiniMapLand);
                    _landFeature[i][j].Draw(spriteBatch, _mainTextures, mapLocation, GameLayer.MiniMapFeature);
                }
            }
        }

        private void MakeFeatures()
        {
            for (var x = 0; x < _amount; x++)
            {
                for (var y = 0; y < _amount; y++)
                {
                    if (!_landFeature[x][y].NeedsNewTexture) continue;

                    _landFeature[x][y].Visible = FillTrees(x, y);
                    if(!_landFeature[x][y].Visible)
                        _landFeature[x][y].Visible = FillGrass(x, y);
                    if(!_landFeature[x][y].Visible)
                        _landFeature[x][y].Visible = FillMountains(x, y);
                }
            }
        }

        private void MakeHeight()
        {
            for (var x = 0; x < _amount; x++)
                {
                    for (var y = 0; y < _amount; y++)
                    {
                        if (!_land[x][y].NeedsNewTexture) continue;
                        FillHeight( x, y);
                        _land[x][y].Visible = true;
                    }
                }
        }

        private int[] GetData(byte[][] mapData, int x, int y , byte type)
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
            var sur = new [] {topLeft, top, topRight, left, center, right, bottomLeft, bottom, bottomRight};
            return SetSurroundings(sur, (center >= HeightGenerator.Plains), center == HeightGenerator.Beach || center == HeightGenerator.Desert);
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
            var sur = new []{topLeft, top, topRight, left, center, right, bottomLeft, bottom,bottomRight};
            return SetSurroundingsOther(sur);
        }

        private bool FillMountains(int x, int y)
        {
            var center = _wd.HeightShowData[x + _xOffset][y + _yOffset];
            if (center != HeightGenerator.Hills && center != HeightGenerator.Mountains) return false;

            var temperature = _wd.TempData[x + _xOffset][y + _yOffset];
            var forest = _wd.ForestData[x + _xOffset][y + _yOffset];
            var textureName = center == HeightGenerator.Hills ? "hill_" : "mountain_";

            switch (temperature)
            {
                case TemperatureGenerator.Cold:
                    textureName += "snow";
                    break;
                case TemperatureGenerator.Mild:
                    textureName += "plain";
                    break;
                case TemperatureGenerator.Warm:
                    textureName += "jungle";
                    break;
                case TemperatureGenerator.Hot:
                    textureName += "desert";
                    break;
            }

            if(center != HeightGenerator.Mountains)
            textureName = forest > ForestGenerator.None ? textureName+"_trees" : textureName;


            if (textureName.Length == 0)
                System.Diagnostics.Debug.WriteLine("Bad news, check mini tile manager");
            else
            {
                var bounds1 = _mainAtlas.GetRegion(textureName).Bounds;
                _landFeature[x][y].UpdateTexture(bounds1, bounds1, bounds1, bounds1, true);
                return true;
            }
            return false;
        }

        private bool FillGrass(int x, int y)
        {
            var center = _wd.GrassShowData[x + _xOffset][y + _yOffset];
            if (center != GrassGenerator.Exist) return false;

            var vals = GetDataOther(_wd.GrassShowData, x + _xOffset, y + _yOffset);
            var temperature = _wd.TempData[x + _xOffset][y + _yOffset];

            var textureName = "";
            switch (temperature)
            {
                case TemperatureGenerator.Cold:
                    textureName = "grass_snow";
                    break;
                case TemperatureGenerator.Mild:
                    textureName = "grass_plain";
                    break;
                case TemperatureGenerator.Warm:
                    textureName = "grass_jungle";
                    break;
                case TemperatureGenerator.Hot:
                    textureName = "grass_desert";
                    break;
            }
            return SetBounds(textureName, _landFeature, vals, x, y);
        }

        private bool FillTrees(int x, int y)
        {
            var center = _wd.ForestShowData[x + _xOffset][y + _yOffset];
            if (center == ForestGenerator.None)
                return false;
            var temperature = _wd.TempData[x + _xOffset][y + _yOffset];
            var vals = GetDataOther(_wd.ForestShowData, x + _xOffset, y + _yOffset);

            var textureName = "";

            switch (temperature)
            {
                case TemperatureGenerator.Cold:
                    textureName = "trees_snow";
                    break;
                case TemperatureGenerator.Mild:
                    textureName = "trees_plain";
                    break;
                case TemperatureGenerator.Warm:
                    textureName = "trees_jungle";
                    break;
                case TemperatureGenerator.Hot:
                    textureName = "trees_desert";
                    break;
            }

            return SetBounds(textureName, _landFeature, vals, x, y);

        }

        private bool SetBounds(string textureName,MiniTileCollection[][] updateMe, int[] vals, int x, int y)
        {
            if (textureName.Length == 0)
                System.Diagnostics.Debug.WriteLine("Bad news, check mini tile manager");
            else
            {
                var bounds1 = _mainAtlas.GetRegion(string.Format("{0}{1}", textureName, vals[0])).Bounds;
                var bounds2 = _mainAtlas.GetRegion(string.Format("{0}{1}", textureName, vals[1])).Bounds;
                var bounds3 = _mainAtlas.GetRegion(string.Format("{0}{1}", textureName, vals[2])).Bounds;
                var bounds4 = _mainAtlas.GetRegion(string.Format("{0}{1}", textureName, vals[3])).Bounds;
                updateMe[x][y].UpdateTexture(bounds1, bounds2, bounds3, bounds4, false);
                return true;
            }
            return false;
        }

        private void FillHeight( int x, int y)
        {
            var center = _wd.HeightShowData[x + _xOffset][y + _yOffset];

            var temperature = _wd.TempData[x + _xOffset][y + _yOffset];
            var vals = GetData(_wd.HeightShowData, x + _xOffset, y + _yOffset, center);

            var textureName = "";
            if (temperature == TemperatureGenerator.Frozen || temperature == TemperatureGenerator.Cold)
            {
                switch (center)
                {
                    case HeightGenerator.DeepOcean:
                    case HeightGenerator.Ocean:
                    case HeightGenerator.ShallowOcean:
                        textureName = "water_snow";
                        break;
                    case HeightGenerator.Beach:
                        textureName = "beach";
                        break;
                    case HeightGenerator.Plains:
                    case HeightGenerator.Hills:
                    case HeightGenerator.Mountains:
                        textureName = "snow";
                        break;
                }
            }
            else
            {
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
                
            }
            SetBounds(textureName, _land, vals, x, y);


        }

        public int[] SetSurroundings(byte[] vals, bool isGrass, bool isBeach)
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

        public int[] SetSurroundingsOther(byte[] vals)
        {
            var topLeft = vals[0];
            var topMiddle = vals[1];
            var topRight = vals[2];
            var midLeft = vals[3];
            var midRight = vals[5];
            var bottomLeft = vals[6];
            var bottomMiddle = vals[7];
            var bottomRight = vals[8];

            var ba = new []
                {
                    topLeft != 0, topMiddle != 0, topRight != 0,
                    midLeft != 0, midRight != 0, bottomLeft != 0,
                    bottomMiddle != 0, bottomRight != 0
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
                {
                    topRightNum = 3;
                }
                if (!bottomRightFine)
                {
                    bottomRightNum = 7;
                }
            }
            else if (leftFine && !rightFine && upFine && bottomFine)
            {
                topLeftNum = 13;
                topRightNum = 15;
                bottomLeftNum = 17;
                bottomRightNum = 19;
                if (!bottomLeftFine)
                {
                    bottomLeftNum = 6;
                }
                if (!upLeftFine)
                {
                    topLeftNum = 2;
                }
            }
            else if (leftFine && rightFine && !upFine && bottomFine)
            {
                topLeftNum = 9;
                topRightNum = 10;
                bottomLeftNum = 13;
                bottomRightNum = 14;
                if (!bottomLeftFine)
                {
                    bottomLeftNum = 6;
                }
                if (!bottomRightFine)
                {
                    bottomRightNum = 7;
                }
            }
            else if (leftFine && rightFine && upFine && !bottomFine)
            {
                topLeftNum = 13;
                topRightNum = 14;
                bottomLeftNum = 21;
                bottomRightNum = 22;
                if (!upRightFine)
                {
                    topRightNum = 3;
                }
                if (!upLeftFine)
                {
                    topLeftNum = 2;
                }
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
                int topLefts = 13;
                int topRights = 14;
                int bottomLefts = 17;
                int bottomRights = 18;
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
            int[] ar = { topLeftNum, topRightNum, bottomLeftNum, bottomRightNum };
            return ar;
        }

        private BackgroundWorker _bw = new BackgroundWorker();
        private byte _direction;
        private int _newOffsetX;
        private int _newOffsetY;

        public void Shift(byte direction,int newOffsetX, int newOffsetY) 
        { 
            _direction = direction;
            _newOffsetX = newOffsetX;
            _newOffsetY = newOffsetY;
            _bw.RunWorkerAsync();
        }

        private void Run(object sender, DoWorkEventArgs e)
        {
            var minusPos = (int)(System.Math.Floor(_amount / 2.0));
            _xOffset = _newOffsetX - minusPos;
            _yOffset = _newOffsetY - minusPos;
            switch (_direction)
            {
                case RotateUp:
                    ShiftUp(_land, true);
                    ShiftUp(_landFeature, false);
                    break;
                case RotateDown:
                    ShiftDown(_land, true);
                    ShiftDown(_landFeature, false);
                    break;
                case RotateLeft:
                    ShiftLeft(_land, true);
                    ShiftLeft(_landFeature, false);
                    break;
                case RotateRight:
                    ShiftRight(_land, true);
                    ShiftRight(_landFeature, false);
                    break;
            }
        }

        private void ShiftUp(MiniTileCollection[][] data, bool isLand)
        {
            for (var i = 0; i < _amount; i++)
            {
                var temptl = data[i][_amount-1];
                temptl.NeedsNewTexture = true;
                for (var j = _amount-1; j >= 0; j--)
                {
                    if(j==0)
                        data[i][j] = temptl;
                    else
                        data[i][j] = data[i][j-1];
                }
            }

            OrganizeTileCollection(data, isLand);
        }

        private void ShiftDown(MiniTileCollection[][] data, bool isLand)
        {
            for (var i = 0; i < _amount; i++)
            {
                var firstArray = data[i][0];
                firstArray.NeedsNewTexture = true;
                for (var j = 1; j < _amount; j++)
                {
                    
                        data[i][j-1] = data[i][j];
                        if (j == _amount - 1)
                            data[i][j] = firstArray;
                }
            }

            OrganizeTileCollection(data, isLand);
        }

        private void ShiftLeft(MiniTileCollection[][] data, bool isLand)
        {
            var firstArray = data[_amount - 1];
            SetNeedNewTexture(firstArray);
            for (var j = _amount-1; j > 0; j--)
                data[j] = data[(j-1)];
            data[0] = firstArray;

            OrganizeTileCollection(data, isLand);
        }

        private void ShiftRight(MiniTileCollection[][] data, bool isLand)
        {
            var firstArray = data[0];
            SetNeedNewTexture(firstArray);
            for(var j = 1; j < _amount; j++)
                data[(j-1)] = data[j];

            data[data.Length-1] = firstArray;

            OrganizeTileCollection(data, isLand);
        }

        private void SetNeedNewTexture(IEnumerable<MiniTileCollection> data)
        {
            foreach (var t in data)
                t.NeedsNewTexture = true;
        }

        private void OrganizeTileCollection(MiniTileCollection[][] data, bool isLand)
        {
            for (var i = 0; i < _amount; i++)
            {
                for (var j = 0; j < _amount; j++)
                {
                    data[i][j].SetSpots(i, j);
                    if (!data[i][j].NeedsNewTexture) continue;

                    if (isLand)
                       MakeHeight();
                    else
                       MakeFeatures();
                }
            }
        }
    }
}