#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;

#endregion

namespace Ship.Game.WorldGeneration.WorldDrawing
{
    public class MapGenerator : MapBase
    {
        public const byte Border = 6;
        public const byte Plains = 5;
        public const byte Beach = 4;
        public const byte ShallowOcean = 3;
        public const byte Ocean = 2;
        public const byte DeepOcean = 1;

        public MapGenerator()
        {
            var output = Noise.NoiseGen.GetWorldSimplex(Seeds.MapSeed,
                                                        MainGame.MapWidth, MainGame.MapHeight, new Vector2(0, 0), 4, 1.0f,
                                                        1.0f);
            // {145,144,134,60,0},{TerrainColors.Normal_Plains,TerrainColors.Normal_Beach,TerrainColors.Normal_ShallowOcean,TerrainColors.Normal_Ocean,TerrainColors.Normal_DeepOcean};
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var c = output[x, y];
                    if (c > 145) // plains
                        Data[x][ y] = Plains;
                    else if (c > 144) // beach
                        Data[x][ y] = Beach;
                    else if (c > 120) // shallow
                        Data[x][ y] = ShallowOcean;
                    else if (c > 60) // ocean
                        Data[x][ y] = Ocean;
                    else
                        Data[x][ y] = DeepOcean;
                }
            }
            // var sizeVariable:Array = [ 60, 78, 5, 2, 110 ]
            MakeBorder();
            // makeTemperature( MapData.tempMap, MapData.MainGame.MapHeightMap );
            // makeDesert( MapData.desertMap );
        }


        private void MakeBorder()
        {
            for (var x = 0; x < MainGame.MapWidth; x++)
            {
                for (var y = 0; y < MainGame.MapHeight; y++)
                {
                    var pixel = Data[x][ y];
                    if (pixel != Beach && pixel != Plains) continue;
                    var startX = x == 0 ? 0 : -1;
                    var endX = x == MainGame.MapWidth - 1 ? 1 : 2;
                    var startY = y == 0 ? 0 : -1;
                    var endY = y == MainGame.MapHeight - 1 ? 1 : 2;
                    for (var i = startX; i < endX; i++)
                    {
                        for (var j = startY; j < endY; j++)
                        {
                            var currpixel = Data[x + i][ y + j];
                            if (currpixel == ShallowOcean
                                || currpixel == Ocean)
                                Data[x + i][ y + j] = Border;
                        }
                    }
                }
            }
        }

        public byte[] ByteColor()
        {
            return new byte[6]
                {
                    DeepOcean,
                    Ocean,
                    ShallowOcean,
                    Beach,
                    Plains,
                    Border
                };
        }

        public Color[] MapColors()
        {
            return new Color[6]
                {
                    Utility.IntToColor(TerrainColors.NormalDeepocean),
                    Utility.IntToColor(TerrainColors.NormalOcean),
                    Utility.IntToColor(TerrainColors.NormalShallowocean),
                    Utility.IntToColor(TerrainColors.NormalBeach),
                    Utility.IntToColor(TerrainColors.NormalPlains),
                    Utility.IntToColor(TerrainColors.NormalBorder)
                };
        }
    }
}