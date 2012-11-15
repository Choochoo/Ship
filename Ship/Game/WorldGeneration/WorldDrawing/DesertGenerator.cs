#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;

#endregion

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class DesertGenerator : MapBase
    {
        public const byte Exist = 1;
        public const byte None = 0;

        public DesertGenerator()
        {
            var output = Noise.NoiseGen.GetWorldSimplex(Utils.Seeds.DesertSeed,
                                                  MainGame.MapWidth, MainGame.MapHeight, new Vector2(0, 0), 1, 1.0f,10.0f);
                   
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var c = output[x, y];
                    if (c > 200)
                        CheckColor(x, y, Exist);
                }
            }
        }

        private void CheckColor(int x, int y, byte color)
        {
            if (MainGame.WorldData.TempData[x][ y] != TemperatureGenerator.Hot) return;
            if (MainGame.WorldData.HeightData[x][ y] >= HeightGenerator.Plains)
                Data[x][ y]=color;
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
                    Utility.IntToColor(TerrainColors.DesertExist)
                };
        }

        internal void Destroy() { Data = null; }
    }
}