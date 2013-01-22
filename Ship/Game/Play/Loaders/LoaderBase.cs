#region

using Ship.Game.Play.WorldGeneration;
using Ship.Game.Play.Utils;
using System.IO;
using System;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.ScreenComponents.Screens;
using Ship.Game.Play.Beans.Helpers;
using Ship.Game.Play.WorldGeneration.Noise;
using Ship.Game.Play.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework;
using System.Diagnostics;

#endregion

namespace Ship.Game.Play.Loaders
{
    public class LoaderBase
    {
        #region GetSets

        public int SectorX { get; private set; }

        public int SectorY { get; private set; }

        //public byte Temperature { get { return _temperature; } }

        #endregion

        private static int _startx;
        private static int _starty;
        public SectorData SectorData;

        private Stopwatch stopwatch = new Stopwatch();


        public LoaderBase() { SectorData = new SectorData(); }

        public TimeSpan LoadSector(int sectorX, int sectorY)
        {
            // Utility.TimeStart();
            SectorX = sectorX;
            SectorY = sectorY;
            var file = String.Format("gamedata/{0}-{1}", sectorX, sectorY);
            //_temperature = WorldData.MyWorldData.TempData[sectorX][sectorY];
            stopwatch.Start();
            
            if (File.Exists(file))
                SectorData.Read(file);
            else
            {
                SectorData = StartProcess(PlayScreen.SpawnX == sectorX && PlayScreen.SpawnY == sectorY,sectorX, sectorY);
            }
            stopwatch.Start();
            return stopwatch.Elapsed;
        }

        #region Probabilities
        private static double GetHeightProbability(WorldData wd, int sectorX, int sectorY)
        {
            switch (wd.HeightData[sectorX][sectorY])
            {
                case HeightGenerator.Mountains:
                    return MountainHigh;
                case HeightGenerator.Hills:
                    return MountainMedium;
                default:
                    return MountainLow;
            }
        }


        private static double GetTreeProbability(byte val)
        {
            switch (val)
            {
                case ForestGenerator.Dense:
                    return TreeHigh;
                case ForestGenerator.Medium:
                    return TreeMedium;
                case ForestGenerator.Light:
                    return TreeLow;
            }
            return TreeNone;
        }
        #endregion

        public SectorData StartProcess(bool isFirst, int sectorX, int sectorY)
        {
            //var time = Environment.TickCount;
            if (!Directory.Exists("gamedata"))
                Directory.CreateDirectory("gamedata");

            var filename = String.Format("gamedata/{0}-{1}", sectorX, sectorY);
            var sd = Run(sectorX, sectorY);
            //sd.DecorData[PlayScreen.SectorTileSize/2,PlayScreen.SectorTileSize/2] = 
            sd.Write(filename);
            return sd;
        }

        private SectorData Run(int sectorX, int sectorY)
        {
            var rand = new Random((sectorX*MainGame.MapWidth) + (sectorY*MainGame.MapHeight));
            //

            var heightProbability = GetHeightProbability(WorldData.MyWorldData, sectorX, sectorY);
            var heightValue = (byte) (byte.MaxValue*(1.0 - heightProbability));

            //
            var forestB = WorldData.MyWorldData.ForestData[sectorX][sectorY];
            var desertB = WorldData.MyWorldData.DesertData[sectorX][sectorY];
            var grassB = WorldData.MyWorldData.GrassData[sectorX][sectorY];
           // var tempValue = WorldData.MyWorldData.TempData[sectorX][sectorY];
            //var tempString = GetTempString(tempValue);
            var tempT = 1.0f - GetTreeProbability(forestB);
            var treeProbability = GetTreeProbability(forestB);
            var treeProbValue = (byte) ((1.0 - treeProbability)*byte.MaxValue);
            const float rockProbability = .01f;
            //var rockProbability = GetRockProbability(heightValue, tempValue);
            //var bushProbability = GetBushProbability(tempValue);
            //var flowerProbability = GetFlowerProbability(tempValue);
            //var extraProbability = GetExtraProbability(tempValue);

            //_desertProbability = desertB == DesertGenerator.Exist ? .4f : 0f;
            var grassProbability = grassB == GrassGenerator.Exist ? .25f : .05f;

            var tileData = GameTiles(sectorX, sectorY, WorldData.MyWorldData, rand);
            var decorData = GameDecor(tileData, sectorX, sectorY, WorldData.MyWorldData, new[] {treeProbValue},new[] {treeProbability, rockProbability},rand);


            return new SectorData(tileData, decorData);
        }


        private byte[,] GameDecor(byte[,] tiledata, int sectorX, int sectorY, WorldData wd, byte[] probabilities,double[] probPercentages, Random rand)
        {
            var decorData = new byte[PlayScreen.SectorTileSize,PlayScreen.SectorTileSize];


            var decorSimplex = new byte[PlayScreen.SectorTileSize,PlayScreen.SectorTileSize];
            var heightValue = wd.HeightData[sectorX][sectorY];

            var treeProbValue = probabilities[0];

            var treeProbability = probPercentages[0];
            var rockProbability = probPercentages[1];


            if (heightValue < HeightGenerator.ShallowOcean)
                return decorData;


            NoiseGen.GetDecorSimplex(ref decorSimplex, new Vector2(sectorX, sectorY));

           // decorData[0, 0] = DecorHelper.Fire;

            for (var y = 0; y < PlayScreen.SectorTileSize; y++)
            {
                var incY = false;
                for (var x = 0; x < PlayScreen.SectorTileSize; x++)
                {
                    // decorData[x, y] = DecorKeys.Fire;
                    if (tiledata[x, y] != TileHelper.Grass && decorData[x, y] == DecorHelper.None)
                        continue;

                    if (decorSimplex[x, y] > treeProbValue && rand.NextDouble() < treeProbability)
                    {
                        decorData[x, y] = DecorHelper.Tree;
                    }
                    else if (decorSimplex[x, y] > treeProbValue - 10 && rand.NextDouble() < treeProbability / 2)
                    {
                        decorData[x, y] = DecorHelper.Bush;
                    }
                    else 
                    if (rand.NextDouble() < treeProbability/6.0f)
                        decorData[x, y] = DecorHelper.Tree;
                    else if (rand.NextDouble() < rockProbability)
                        decorData[x, y] = DecorHelper.Rock;
                    //else if (Seeds.Randomizer.NextDouble() < _bushProbability)
                    //{
                    //    if (DecorKeys.DecorCollection != null && DecorKeys.DecorCollection.ContainsKey(_tempString) && DecorKeys.DecorCollection[_tempString].ContainsKey(DecorKeys.TypeBushString))
                    //    {
                    //        var array = DecorKeys.DecorCollection[_tempString][DecorKeys.TypeBushString];
                    //        var value = array[Seeds.Randomizer.Next(0, array.Length)];
                    //        DecorData[x, y] = value;
                    //    }
                    //}
                    //else if (Seeds.Randomizer.NextDouble() < _flowerProbability)
                    //{
                    //    if (DecorKeys.DecorCollection != null && DecorKeys.DecorCollection.ContainsKey(_tempString) && DecorKeys.DecorCollection[_tempString].ContainsKey(DecorKeys.TypeFlowerString))
                    //    {
                    //        var array = DecorKeys.DecorCollection[_tempString][DecorKeys.TypeFlowerString];
                    //        var value = array[Seeds.Randomizer.Next(0, array.Length)];
                    //        DecorData[x, y] = value;
                    //    }
                    //}
                    //else if (Seeds.Randomizer.NextDouble() < _extraProbability)
                    //{
                    //    if (DecorKeys.DecorCollection != null && DecorKeys.DecorCollection.ContainsKey(_tempString) && DecorKeys.DecorCollection[_tempString].ContainsKey(DecorKeys.TypeExtraString))
                    //    {
                    //        var array = DecorKeys.DecorCollection[_tempString][DecorKeys.TypeExtraString];
                    //        var value = array[Seeds.Randomizer.Next(0, array.Length)];
                    //        DecorData[x, y] = value;
                    //    }
                    //}
                }
                if (incY)
                    y++;
            }
            return decorData;
        }

        private byte[,] _tileData = new byte[PlayScreen.SectorTileSize, PlayScreen.SectorTileSize];
        private byte[,] _waterTiles = new byte[PlayScreen.SectorTileSize, PlayScreen.SectorTileSize];
        private byte[,] _tileSimplex = new byte[PlayScreen.SectorTileSize, PlayScreen.SectorTileSize];
        private byte[,] GameTiles(int sectorX, int sectorY, WorldData wd, Random rand)
        {
            var heightValue = wd.HeightData[sectorX][sectorY];
            var waterArea = WorldData.MyWorldData.WaterSquareData[sectorX][sectorY];

            Array.Clear(_tileData,0,_tileData.Length);
            Array.Clear(_waterTiles, 0, _waterTiles.Length);
            Array.Clear(_tileSimplex, 0, _tileSimplex.Length);


            if (heightValue >= HeightGenerator.Beach)
            {
                if (waterArea != null)
                    NoiseGen.GetRiverSquare(ref _waterTiles, waterArea, TileHelper.RiverWater, rand);


                NoiseGen.GetTileSimplex(ref _tileSimplex, new Vector2(sectorX, sectorY));
                NoiseGen.MergeData(ref _tileData, _waterTiles);
            }
            else if (heightValue <= HeightGenerator.ShallowOcean)
            {
                NoiseGen.OceanWithLand(ref _tileData, WorldData.MyWorldData.HeightData, sectorX, sectorY,
                                       TileHelper.OceanWater);


                if (heightValue == HeightGenerator.ShallowOcean)
                {
                    NoiseGen.AddSand(ref _tileData, TileHelper.OceanWater, TileHelper.Grass, TileHelper.Sand, 2, 4, rand);
                    if (waterArea != null)
                        NoiseGen.GetRiverSquare(ref _waterTiles, waterArea, TileHelper.RiverWater, rand);

                    NoiseGen.MergeData(ref _tileData, _waterTiles, TileHelper.Sand);
                }
            }
            else
                System.Diagnostics.Debug.WriteLine("Something bad happened in Loaderbase");
            return _tileData;
        }

        public void UpdateSpace()
        {
            if (!Directory.Exists("gamedata"))
                Directory.CreateDirectory("gamedata");

            _startx = PlayScreen.SpawnX;
            _starty = PlayScreen.SpawnY;
        }

        private void Working()
        {
            var left = _startx - 1 >= 0 ? _startx - 1 : _startx;
            left = _startx - 2 >= 0 ? _startx - 2 : left;

            var right = _startx + 1 < MainGame.MapWidth ? _startx + 1 : _startx;
            right = _startx + 2 < MainGame.MapWidth ? _startx + 2 : right;

            var up = _starty - 1 >= 0 ? _starty - 1 : _starty;
            up = _starty - 2 >= 0 ? _starty - 2 : up;

            var down = _starty + 1 < MainGame.MapHeight ? _starty + 1 : _starty;
            down = _starty + 2 < MainGame.MapHeight ? _starty + 2 : down;


            for (var x = left; x <= right; x++)
            {
                for (var y = up; y <= down; y++)
                {
                    var file = String.Format("gamedata/{0}-{1}", x, y);
                    if (!File.Exists(file))
                        Run(x, y).Write(file);
                }
            }
        }

        #region Consts

        private const double TreeHigh = .80;
        private const double TreeMedium = .65;
        private const double TreeLow = .45;
        private const double TreeNone = .2;

        private const double MountainHigh = .5;
        private const double MountainMedium = .4;
        private const double MountainLow = .2;

        #endregion
    }
}