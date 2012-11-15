#region

using Microsoft.Xna.Framework.Graphics;
using Ship.Game.WorldGeneration.Noise;
using Ship.Game.Utils;
using Microsoft.Xna.Framework;
using System;

#endregion

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class GrassGenerator : MapBase
    {
        public const byte Exist = 1;
        public const byte None = 0;
        private Random _rand = new Random(Seeds.MapSeed);

        public GrassGenerator()
        {
            var output = NoiseGen.GetWorldSimplex(Seeds.GrassSeed,
                    MainGame.MapWidth, MainGame.MapHeight, new Vector2(0, 0), 1, .5f,
                    35.0f);
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var c = output[x, y];
                    if (c > 170)// plains
                        CheckColor(x, y, Exist);
                    else
                        Data[x][ y]= None;
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
                    var forestcolor = wd.ForestData[x][ y];
                    if (wd.HeightData[x][y] < HeightGenerator.Hills && forestcolor < ForestGenerator.Light && wd.WaterSquareData[x][y] == null)
                    {
                        ba[x][y] = wd.GrassData[x][y];
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
                if (_rand.NextDouble() > .8f)
                    Data[x][ y]= color;
            }
            else
                Data[x][ y] = color;
        }

        public byte[] ByteColor()
        {
            return new byte[1]
                {
                    Exist
                };
        }

        public Color[] MapColors()
        {
            return new Color[1]
                {
                    Utility.IntToColor(TerrainColors.GrassExist)
                };
        }

        internal void Destroy() { Data = null;
            _rand = null;
        }
    }
}
