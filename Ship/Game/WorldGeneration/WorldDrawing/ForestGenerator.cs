#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;

#endregion

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class ForestGenerator : MapBase
    {
        public const byte Dense = 3;
        public const byte Medium = 2;
        public const byte Light = 1;
        public const byte None = 0;
        private Random _rand = new Random(Seeds.ForestSeed);

        public ForestGenerator() 
        { 
            var output = Noise.NoiseGen.GetWorldSimplex(Seeds.ForestSeed,
                                                        MainGame.MapWidth, MainGame.MapHeight, new Vector2(0, 0), 1, .5f,
                                                        10.0f);
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var c = output[x, y];
                    if (c > 250)
                        CheckColor(x, y, Dense);
                    else if (c > 220)
                        CheckColor(x, y, Medium);
                    else if (c > 170)
                        CheckColor(x, y, Light);
                }
            }
        }

       

        public byte[][] ToShowableByteArray(WorldData wd)
        {
            var ba = new byte[MainGame.MapWidth][];
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                ba[x] = new byte[MainGame.MapHeight];
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    if (wd.ForestData[x][y] == None)
                        continue;
                    
                    if (wd.HeightData[x][y] == HeightGenerator.Plains)
                        if (wd.WaterSquareData[x][ y] == null)
                        {
                            ba[x][y] = wd.ForestData[x][ y];
                        }
                }
            }
            return ba;
        }

        private void CheckColor(int x, int y, byte color)
        {
            if (MainGame.WorldData.HeightData[x][ y] < HeightGenerator.Plains) return;
            if (MainGame.WorldData.TempData[x][ y] == TemperatureGenerator.Frozen) return;

            if (MainGame.WorldData.TempData[x][ y] == TemperatureGenerator.Hot)
            {
                if (_rand.NextDouble() > .8)
                    Data[x][ y] = Light;
            }
            else
            {
                if (MainGame.WorldData.TempData[x][ y] == TemperatureGenerator.Warm)
                {
                    if (color == Medium)
                        color = Dense;
                    if (color == Light)
                        color = Medium;
                }

                Data[x][ y] = color;
            }
        }

        public byte[] ByteColor()
        {
            return new byte[4]
                {
                    Dense,
                    Medium,
                    Light,
                    None
                };
        }

        public Color[] MapColors()
        {
            return new Color[4]
                {
                    Utility.IntToColor(TerrainColors.TreeDense),
                    Utility.IntToColor(TerrainColors.TreeMedium),
                        Utility.IntToColor(TerrainColors.TreeLight),
                            Utility.IntToColor(TerrainColors.TreeNone)
                };
        }

        internal void Destroy() { Data = null;
            _rand = null;
        }
    }
}