#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;

#endregion

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class HeightGenerator : MapBase
    {
        public const byte DeepOcean = 2;
        public const byte Ocean = 3;
        public const byte ShallowOcean = 4;
        public const byte Desert = 5;
        public const byte Beach = 6;
        public const byte Plains = 7;
        public const byte Hills = 8;
        public const byte Mountains = 9;


        public HeightGenerator()
        {
            var output = Noise.NoiseGen.GetWorldSimplex(Seeds.MapSeed,MainGame.MapWidth, MainGame.MapHeight, new Vector2(0.0f, 0.0f), 4, 1.0f,1.0f);
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var c = output[x, y];
                    if (c > 230) // plains
                        Data[x][ y] = Mountains;
                    else if (c > 200) // beach
                        Data[x][ y] = Hills;
                    else if (c > 145) // shallow
                        Data[x][ y] = Plains;
                    else if (c > 144) // ocean
                        Data[x][ y] = Beach;
                    else if (c > 120) // ocean
                        Data[x][ y] = ShallowOcean;
                    else if (c > 60) // ocean
                        Data[x][ y] = Ocean;
                    else
                        Data[x][ y] = DeepOcean;
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
                    if (wd.DesertData[x][y] == DesertGenerator.Exist)
                        ba[x][y] = Desert;
                    if (wd.WaterSquareData[x][y]!= null && wd.WaterSquareData[x][y].HighestSize > 5)
                        ba[x][y] = HeightGenerator.ShallowOcean;
                    else if (wd.TempData[x][y] < TemperatureGenerator.Mild && wd.HeightData[x][y] < HeightGenerator.Beach)
                        ba[x][y] = HeightGenerator.ShallowOcean;
                    else
                        ba[x][y] = wd.HeightData[x][y];
                }
            }
            return ba;
        }

        public static readonly byte[] ByteValueArray = new [] { DeepOcean, Ocean, ShallowOcean, Desert, Beach, Plains, Hills, Mountains };

        public Color[] MapColors()
        {
            return new Color[8]
                {
                    Utility.IntToColor(TerrainColors.HeightDeepocean),
                    Utility.IntToColor(TerrainColors.HeightOcean),
                    Utility.IntToColor(TerrainColors.HeightShallowocean),
                    Utility.IntToColor(TerrainColors.HeightBeach),
                    Utility.IntToColor(TerrainColors.HeightBeach),
                    Utility.IntToColor(TerrainColors.HeightPlains),
                    Utility.IntToColor(TerrainColors.HeightHills),
                    Utility.IntToColor(TerrainColors.HeightMountains)
                };
        }

        internal void Destroy() { Data = null; }
    }
}