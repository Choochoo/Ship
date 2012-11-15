using System;
using Ship.Game.WorldGeneration.Noise;
namespace Ship.Game.Utils
{
    public static class Seeds
    {
        //public const int FRACTAL = 2;
        // public const int NOISE_X = 70;
        private static int _mapSeed = int.MinValue;
        private static int _forestSeed = int.MinValue;
        private static int _grassSeed = int.MinValue;
        private static int _desertSeed = int.MinValue;
        private static int _waterSeed = int.MinValue;

        public static int MapSeed { get { return _mapSeed; } set { _mapSeed = _mapSeed == int.MinValue ? value : _mapSeed; } }

        public static int ForestSeed { get { return _forestSeed; } set { _forestSeed = _forestSeed == int.MinValue ? value : _forestSeed; } }

        public static int GrassSeed { get { return _grassSeed; } set { _grassSeed = _grassSeed == int.MinValue ? value : _grassSeed; } }

        public static int DesertSeed { get { return _desertSeed; } set { _desertSeed = _desertSeed == int.MinValue ? value : _desertSeed; } }

        public static int WaterSeed { get { return _waterSeed; } set { _waterSeed = _waterSeed == int.MinValue ? value : _waterSeed; } }
       // public static readonly Random Randomizer = new Random(MapSeed);

        public static void CreateNewSeeds()
        {
            Seeds.MapSeed = 400; // Utility.XORandom()*int.MAX_VALUE;
            Seeds.ForestSeed = 500; // Utility.XORandom()*int.MAX_VALUE;
            Seeds.GrassSeed = 600; // Utility.XORandom()*int.MAX_VALUE;
            Seeds.DesertSeed = 700; // Utility.XORandom()*int.MAX_VALUE;
            Seeds.WaterSeed = 800;
            SimplexNoise.GenGrad(MapSeed);
        }
    }
}