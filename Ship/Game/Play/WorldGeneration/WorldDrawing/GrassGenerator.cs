#region

using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.WorldGeneration.Noise;
using Ship.Game.Play.Utils;
using Microsoft.Xna.Framework;
using System;

#endregion

namespace Ship.Game.Play.WorldGeneration.WorldDrawing
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
                    if (c > 170) // plains
                        CheckColor(x, y, Exist);
                    else
                        Data[x][y] = None;
                }
            }
        }


        public byte[][] ToShowableByteArray()
        {
            var ba = new byte[MainGame.MapWidth][];
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                ba[x] = new byte[MainGame.MapHeight];
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var forestcolor = WorldData.MyWorldData.ForestData[x][y];
                    if (WorldData.MyWorldData.HeightData[x][y] < HeightGenerator.Hills &&
                        forestcolor < ForestGenerator.Light && WorldData.MyWorldData.WaterSquareData[x][y] == null)
                        ba[x][y] = WorldData.MyWorldData.GrassData[x][y];
                }
            }
            return ba;
        }

        private void CheckColor(int x, int y, byte color)
        {
            if (WorldData.MyWorldData.HeightData[x][y] < HeightGenerator.Plains) return;
            Data[x][y] = color;
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

        internal void Destroy()
        {
            Data = null;
            _rand = null;
        }
    }
}