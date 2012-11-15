using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship.Game.WorldGeneration.WorldDrawing;
using Ship.Game.Loaders;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Ship.Game.Beans.Mortals.Keys;
using Microsoft.Xna.Framework;
using Ship.Game.WorldGeneration.Noise;
using System.ComponentModel;

namespace Ship.Game.WorldGeneration
{

    public static class CreateWorldTerrain
    {
        
        #region Consts
        
        private const double TreeHigh = .80;
        private const double TreeMedium = .65;
        private const double TreeLow = .45;
        private const double TreeNone = .2;

        private const double MountainHigh = .5;
        private const double MountainMedium = .4;
        private const double MountainLow = .2;
        #endregion

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

        private static string GetTempString(byte tempValue)
        {
            switch (tempValue)
            {
                case TemperatureGenerator.Frozen:
                    return DecorKeys.ClimateFrozenString;
                case TemperatureGenerator.Cold:
                    return DecorKeys.ClimateColdString;
                case TemperatureGenerator.Mild:
                    return DecorKeys.ClimateMildString;
                case TemperatureGenerator.Warm:
                    return DecorKeys.ClimateJungleString;
                case TemperatureGenerator.Hot:
                    return DecorKeys.ClimateHotString;
            }
            return "";
        }


        private static double GetExtraProbability(byte tempValue)
        {
            return .2;
            var output = 0.0;
            switch (tempValue)
            {
                case TemperatureGenerator.Frozen:
                    output = .3;
                    break;
                case TemperatureGenerator.Cold:
                    output = .2;
                    break;
                case TemperatureGenerator.Mild:
                    output = .1;
                    break;
                case TemperatureGenerator.Warm:
                    output = .14;
                    break;
                case TemperatureGenerator.Hot:
                    output = .3;
                    break;
            }
            return output;
        }

        private static double GetFlowerProbability(byte tempValue)
        {
            return .1;
            switch (tempValue)
            {
                case TemperatureGenerator.Frozen:
                    return .3f;
                case TemperatureGenerator.Cold:
                    return .2f;
                case TemperatureGenerator.Mild:
                    return .1f;
                case TemperatureGenerator.Warm:
                    return .14f;
                case TemperatureGenerator.Hot:
                    return .3f;
            }
        }

        private static double GetBushProbability(byte tempValue)
        {
            return .1;
            var output = 0.0;
            switch (tempValue)
            {
                case TemperatureGenerator.Frozen:
                    output = .3;
                    break;
                case TemperatureGenerator.Cold:
                    output= .2;
                    break;
                case TemperatureGenerator.Mild:
                    output = .1;
                    break;
                case TemperatureGenerator.Warm:
                    output = .14;
                    break;
                case TemperatureGenerator.Hot:
                    output = .3;
                    break;
            }
            return output;
        }

        private static double GetRockProbability(byte heightValue, byte tempValue)
        {
            var output = 0.0;
            switch (tempValue)
            {
                case TemperatureGenerator.Frozen:
                    output = .03;
                    break;
                case TemperatureGenerator.Cold:
                    output = .03;
                    break;
                case TemperatureGenerator.Mild:
                    output = .03;
                    break;
                case TemperatureGenerator.Warm:
                    output = .03;
                    break;
                case TemperatureGenerator.Hot:
                    output = .03;
                    break;
            }
            output = heightValue == HeightGenerator.Hills ? output*1.1 : output;
            output = heightValue == HeightGenerator.Mountains ? output*1.2 : output;
            return output;
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

        public static SectorData StartProcess(bool isFirst, int sectorX, int sectorY)
        {
            var time = System.Environment.TickCount;
            if (!Directory.Exists("gamedata"))
                Directory.CreateDirectory("gamedata");

            var filename = String.Format("gamedata/{0}-{1}", sectorX, sectorY);
            var sd = Run(sectorX, sectorY);
            //sd.DecorData[MainGame.SectorTileSize/2,MainGame.SectorTileSize/2] = 
            sd.Write(filename);
            return sd;
        }

        private static SectorData Run(int sectorX, int sectorY)
        {
            
            var rand = new Random((sectorX * MainGame.MapWidth) + (sectorY * MainGame.MapHeight));
            //

            var heightProbability = GetHeightProbability(MainGame.WorldData, sectorX, sectorY);
            var heightValue = (byte)(byte.MaxValue * (1.0 - heightProbability));

            //
            var forestB = MainGame.WorldData.ForestData[sectorX][sectorY];
            var desertB = MainGame.WorldData.DesertData[sectorX][sectorY];
            var grassB = MainGame.WorldData.GrassData[sectorX][sectorY];
            var tempValue = MainGame.WorldData.TempData[sectorX][sectorY];
            var tempString = GetTempString(tempValue);
            var tempT = 1.0f - GetTreeProbability(forestB);
            var treeProbability = GetTreeProbability(forestB);
            var treeProbValue = (byte)((1.0 - treeProbability) * byte.MaxValue);
            var rockProbability = GetRockProbability(heightValue, tempValue);
            var bushProbability = GetBushProbability(tempValue);
            var flowerProbability = GetFlowerProbability(tempValue);
            var extraProbability = GetExtraProbability(tempValue);

            //_desertProbability = desertB == DesertGenerator.Exist ? .4f : 0f;
            var grassProbability = grassB == GrassGenerator.Exist ? .25f : .05f;

            var tileData = GameTiles(sectorX, sectorY, MainGame.WorldData, rand);
            var decorData = GameDecor(tileData, sectorX, sectorY, MainGame.WorldData, new[] { treeProbValue }, new[] { treeProbability, rockProbability },
                      rand, tempString);


            return new SectorData(tileData, decorData);
        }


        private static byte[,] GameDecor(byte[,] tiledata, int sectorX, int sectorY, WorldData wd, byte[] probabilities, double[] probPercentages, Random rand, String tempString)
        {
            var decorData = new byte[MainGame.SectorTileSize, MainGame.SectorTileSize];


            var decorSimplex = new byte[MainGame.SectorTileSize,MainGame.SectorTileSize];
            var heightValue = wd.HeightData[sectorX][sectorY];

            var treeProbValue = probabilities[0];

            var treeProbability = probPercentages[0];
            var rockProbability = probPercentages[1];


            if (heightValue < HeightGenerator.ShallowOcean)
                return decorData;



            NoiseGen.GetDecorSimplex(ref decorSimplex, new Vector2(sectorX, sectorY));

            
            
                for (var y = 0; y < MainGame.SectorTileSize; y++)
                {
                    var incY = false;
                    for (var x = 0; x < MainGame.SectorTileSize; x++)
                    {
                    if (tiledata[x, y] != TileKeys.Grass)
                        continue;
                    
                    if (decorSimplex[x,y] > treeProbValue)
                    {
                        
                        if (DecorKeys.DecorCollection != null && DecorKeys.DecorCollection.ContainsKey(tempString) && DecorKeys.DecorCollection[tempString].ContainsKey(DecorKeys.TypeTreeString))
                        {
                            var array = DecorKeys.DecorCollection[tempString][DecorKeys.TypeTreeString];
                            var value = array[rand.Next(0, array.Length)];
                            decorData[x, y] = value;
                            if (treeProbability == TreeLow || treeProbability == TreeNone)
                            {
                                x++;
                               // incY = true;
                            }
                                
                        }
                    }
                    else if (decorSimplex[x, y] > treeProbValue - 10)
                    {
                        if (DecorKeys.DecorCollection != null && DecorKeys.DecorCollection.ContainsKey(tempString) && DecorKeys.DecorCollection[tempString].ContainsKey(DecorKeys.TypeBushString))
                        {
                            var array = DecorKeys.DecorCollection[tempString][DecorKeys.TypeBushString];
                            var value = array[rand.Next(0, array.Length)];
                            decorData[x, y] = value;
                        }
                    }
                    else if (rand.NextDouble() < rockProbability)
                    {
                        if (DecorKeys.DecorCollection != null && DecorKeys.DecorCollection.ContainsKey(tempString) && DecorKeys.DecorCollection[tempString].ContainsKey(DecorKeys.TypeRockString))
                        {
                            var array = DecorKeys.DecorCollection[tempString][DecorKeys.TypeRockString];
                            var value = array[rand.Next(0, array.Length)];
                            decorData[x, y] = value;
                        }
                    }
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


        private static byte[,] GameTiles( int sectorX, int sectorY, WorldData wd, Random rand)
        {
            var heightValue = wd.HeightData[sectorX][sectorY];
            var waterArea = MainGame.WorldData.WaterSquareData[sectorX][sectorY];

            var tileData = new byte[MainGame.SectorTileSize, MainGame.SectorTileSize];
            var waterTiles = new byte[MainGame.SectorTileSize, MainGame.SectorTileSize];
            var tileSimplex = new byte[MainGame.SectorTileSize, MainGame.SectorTileSize];


                if (heightValue >= HeightGenerator.Beach)
                {

                    if(waterArea!=null)
                         NoiseGen.GetRiverSquare(ref waterTiles, waterArea, TileKeys.RiverWater,rand);
                    

                    NoiseGen.GetTileSimplex(ref tileSimplex,new Vector2(sectorX, sectorY));
                    NoiseGen.MergeData(ref tileData, waterTiles);

                    //for (var x = 0; x < MainGame.SectorTileSize; x++ )
                    //{
                    //    for (var y = 0; y < MainGame.SectorTileSize; y++ )
                    //    {
                    //        if (tileData[x][ y] != 0) continue;
                    //        if (tileSimplex[x][ y] > heightValue)
                    //            tileData[x][ y] = TileKeys.Wall;
                    //    }
                    //}
                }
                else if (heightValue <= HeightGenerator.ShallowOcean)
                {
                    NoiseGen.OceanWithLand(ref tileData, MainGame.WorldData.HeightData, sectorX, sectorY, TileKeys.OceanWater);
                    

                    if (heightValue == HeightGenerator.ShallowOcean)
                    {

                        NoiseGen.AddSand(ref tileData, TileKeys.OceanWater, TileKeys.Grass, TileKeys.Sand, 2, 4,rand);
                        if (waterArea != null)
                            NoiseGen.GetRiverSquare(ref waterTiles, waterArea, TileKeys.RiverWater,rand);

                        NoiseGen.MergeData(ref tileData, waterTiles, TileKeys.Sand);
                    }
                    
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Something bad happened in Loaderbase");
                }
            return tileData;
        }

        private static int _startx = 0;
        private static int _starty = 0;
        public static void UpdateSpace(int startX, int startY)
        {
            if (!Directory.Exists("gamedata"))
                Directory.CreateDirectory("gamedata");

            _startx = startX;
            _starty = startY;

            var bw = new BackgroundWorker();
            bw.DoWork += Working;
            bw.RunWorkerAsync();
        }

        private static void Working(object sender, DoWorkEventArgs e)
        {
            var left = _startx-1 >= 0 ? _startx-1 : _startx;
            left = _startx - 2 >= 0 ? _startx - 2 : left;

            var right = _startx +1 < MainGame.MapWidth ? _startx + 1 : _startx;
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

    }
}
