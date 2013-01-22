#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.Utils;

#endregion

namespace Ship.Game.Play.WorldGeneration.WorldDrawing
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

        public static readonly byte[] ByteValueArray = new[]
            {DeepOcean, Ocean, ShallowOcean, Desert, Beach, Plains, Hills, Mountains};


        public HeightGenerator()
        {
            var output = Noise.NoiseGen.GetWorldSimplex(Seeds.MapSeed, MainGame.MapWidth, MainGame.MapHeight,
                                                        new Vector2(0.0f, 0.0f), 6, 1.0f, 1.0f);
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var c = output[x, y];
                    if (c > 230) // plains
                        Data[x][y] = Mountains;
                    else if (c > 200) // beach
                        Data[x][y] = Hills;
                    else if (c > 145) // shallow
                        Data[x][y] = Plains;
                    else if (c > 144) // ocean
                        Data[x][y] = Beach;
                    else if (c > 138) // ocean
                        Data[x][y] = ShallowOcean;
                    else if (c > 60) // ocean
                        Data[x][y] = Ocean;
                    else
                        Data[x][y] = DeepOcean;
                }
            }
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                StartErase(x, 0);
                StartErase(x, MainGame.MapHeight - 1);
            }

            for (var y = 0; y < MainGame.MapHeight; y++)
            {
                StartErase(0, y);
                StartErase(MainGame.MapWidth - 1, y);
            }
        }

        private void StartErase(int x, int y)
        {

            if (Data[x][y] >= ShallowOcean)
            {
                if (Data[x][y] > Plains)
                    Data[x][y] = DeepOcean;
                else if (Data[x][y] >= ShallowOcean)
                    Data[x][y] = Ocean;


                var leftx = x - 1 < 0 ? 0 : x - 1;
                var rightx = x + 1 >= MainGame.MapWidth ? MainGame.MapWidth - 1 : x + 1;
                var upy = y - 1 < 0 ? 0 : y - 1;
                var downy = y + 1 >= MainGame.MapHeight ? MainGame.MapHeight - 1 : y + 1;

                if (leftx != x)
                    StartErase(leftx, y);
                if (rightx != x)
                    StartErase(rightx, y);
                if (upy != y)
                    StartErase(x, upy);
                if (downy != y)
                    StartErase(x, downy);
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
                    if (WorldData.MyWorldData.DesertData[x][y] == DesertGenerator.Exist)
                        ba[x][y] = Desert;
                    if (WorldData.MyWorldData.WaterSquareData[x][y] != null &&
                        WorldData.MyWorldData.WaterSquareData[x][y].HighestSize > 5)
                        ba[x][y] = ShallowOcean;
                    else if (WorldData.MyWorldData.HeightData[x][y] < Beach)
                        ba[x][y] = ShallowOcean;
                    else
                        ba[x][y] = WorldData.MyWorldData.HeightData[x][y];
                }
            }
            return ba;
        }

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