#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;
using System;
using Ship.Game.WorldGeneration.Noise;

#endregion

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class TemperatureGenerator : MapBase
    {
        public const byte Hot = 4;
        public const byte Warm = 3;
        public const byte Mild = 2;
        public const byte Cold = 1;
        public const byte Frozen = 0;

        private readonly Random _rand = new Random(Seeds.ForestSeed);

        public TemperatureGenerator()
        {

            // desert
            SetupZone(.95, .05, Cold);
            // forest
            SetupZone(.8, .05, Mild);
            // // ice
            SetupZone(.65, .04, Warm);
            // // frozen
            SetupZone(.5, .03, Hot);
            _rand = null;
        }

        private void SetupZone(double percentageOfScreen, double percentHeightVariation, byte color)
        {

            var output = NoiseGen.GetWorldSimplex(_rand.Next(),
                                                  MainGame.MapWidth, 1, new Vector2(0, 0), 2, 1.0f, 1.0f);
            var halfHeight = (int) (MainGame.MapHeight*.5);
            //var midYval = (int) (MainGame.MapHeight*midPerc);
            // var addon = (int)(MainGame.MapHeight * variation);
            var midYval = (int) (MainGame.MapHeight*(1 - percentageOfScreen));
            for (var i = 0; i < output.GetLength(0); i++)
            {
                var diff = ((halfHeight - midYval) + (output[i, 0]/255.0)*(percentHeightVariation*MainGame.MapHeight));
                // diff += addon;
                for (var j = 0; j < diff; j++)
                {
                    Data[i][ (halfHeight + j)] = color;
                    Data[i][ (halfHeight - j)] = color;
                    //Data[i, midYval2 + j] = color;
                    //Data[i, midYval2 - j] = color;
                }
            }
        }

        public byte[] ByteColor()
        {
            return new byte[5]
                {
                    Cold,
                    Frozen,
                    Hot,
                    Mild,
                    Warm
                };
        }

        public Color[] MapColors()
        {
            return new Color[5]
                {
                    Utility.IntToColor(TerrainColors.TemperatureCold),
                    Utility.IntToColor(TerrainColors.TemperatureFrozen),
                    Utility.IntToColor(TerrainColors.TemperatureHot),
                    Utility.IntToColor(TerrainColors.TemperatureMild),
                    Utility.IntToColor(TerrainColors.TemperatureWarm)
                };
        }

        internal void Destroy() { Data = null; }
    }
}