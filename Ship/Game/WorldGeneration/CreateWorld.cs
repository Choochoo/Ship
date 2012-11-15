


#region

using Ship.Game.Utils;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using System.ComponentModel;

#endregion

namespace Ship.Game.WorldGeneration
{
    public sealed class CreateWorld : MapBase
    {
        private readonly bool _showWorld;
        private readonly bool _saveWorld;
        private Texture2D _renderTexture;

        public CreateWorld(GraphicsDevice graphics,ref  WorldData wd, bool showWorld, bool saveWorld)
        {
            _showWorld = showWorld;
            _saveWorld = false;// saveWorld;

            long time = System.Environment.TickCount;

            System.Diagnostics.Debug.WriteLine("--Generator Map");

            var temp = new TemperatureGenerator();
            wd.TempData = temp.Data;
            temp.Destroy();
            
            _renderTexture = WriteToTexture(graphics,wd.TempData, temp.ByteColor(), temp.MapColors());
               
            var height = new HeightGenerator();
            wd.HeightData = height.Data;
            height.Destroy();
            
            //_renderTexture = WriteToTexture(graphics, height.Data, HeightGenerator.ByteValueArray, height.MapColors());

            var water = new WaterGenerator();
            wd.WaterSquareData = water.WaterSquares;
            water.Destroy();
            
            //_renderTexture = WriteToTexture(graphics, water.Data, null, water.MapColors());

            var desert = new DesertGenerator();
            wd.DesertData = desert.Data;
            desert.Destroy();
            
            //_renderTexture = WriteToTexture(graphics, desert.Data, desert.ByteColor(), desert.MapColors());

            var forest = new ForestGenerator();
            wd.ForestData = forest.Data;
            forest.Destroy();
            
            //_renderTexture = WriteToTexture(graphics, forest.Data, forest.ByteColor(), forest.MapColors());

            var grass = new GrassGenerator();
            wd.GrassData = grass.Data;
            grass.Destroy();
            //_renderTexture = WriteToTexture(graphics, grass.Data, grass.ByteColor(), grass.MapColors());

            wd.HeightShowData = height.ToShowableByteArray(wd);
            wd.GrassShowData = grass.ToShowableByteArray(wd);
            wd.ForestShowData = forest.ToShowableByteArray(wd);

            forest = null;
            desert = null;
            water = null;
            height = null;
            temp = null;
            grass = null;

            if (_showWorld)
            {
                var map = new MapGenerator();
                _renderTexture = WriteToTexture(graphics, map.Data, map.ByteColor(), map.MapColors());
                WaterTexture(wd.WaterSquareData);
            }
            if (_saveWorld)
            {
                Utility.SaveGameData(wd);
            }
            System.Diagnostics.Debug.WriteLine("--Generator Map COMPLETE");
            System.Diagnostics.Debug.WriteLine("took: " + (System.Environment.TickCount - time));
            System.Diagnostics.Debug.WriteLine("--Saving Entire Map");
            time = System.Environment.TickCount;
            System.Diagnostics.Debug.WriteLine("--Saving Entire COMPLETE");
            System.Diagnostics.Debug.WriteLine("took: " + (System.Environment.TickCount - time));
        }
        //private BackgroundWorker bw = new BackgroundWorker();
        private void WaterTexture(WaterSquare[][] waterSquare)
        {
            for (var i = 0; i < MainGame.MapWidth; i++)
            {
                for (var j = 0; j < MainGame.MapHeight; j++)
                {
                    if(waterSquare[i][j]==null)continue;
                    var r = new Rectangle(i, j, 1, 1);
                    var color = new Color[1];
                    color[0] = new Color(0, 0, waterSquare[i][j].HighestSize);

                    _renderTexture.SetData(0, r, color, 0, 1);


                }
            }
        }

        public Texture2D WriteToTexture(GraphicsDevice gd, byte[][] data,byte[] byteData, Color[] colorData)
        {
            var outputTexture = _renderTexture ?? new Texture2D(gd, MainGame.MapWidth, MainGame.MapHeight);
            for (var i = 0; i < MainGame.MapWidth; i++)
            {
                for (var j = 0; j < MainGame.MapHeight; j++)
                {
                    if(byteData==null)
                    {
                        if (data[i][j] == 0) continue;

                        var r = new Rectangle(i, j, 1, 1);
                        var color = new Color[1];
                        var colorBlue = new Color(0, 0, (int)(byte.MaxValue - (byte.MaxValue * ((float)data[i][j] / byte.MaxValue))));
                        color[0] = colorBlue;

                        outputTexture.SetData(0, r, color, 0, 1);
                    }
                    else
                    {
                        for (var k = 0; k < byteData.Length; k++)
                        {
                            if (data[i][j] != byteData[k]) continue;

                            var r = new Rectangle(i, j, 1, 1);
                            var color = new Color[1];
                            color[0] = colorData[k];

                            outputTexture.SetData(0, r, color, 0, 1);
                        }
                    }
                    
                }
            }
            return outputTexture;
        }


        public void Dispose() 
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(_renderTexture != null)
            {
                spriteBatch.Draw(_renderTexture, new Vector2(0,0), new Rectangle(0,0,_renderTexture.Width,_renderTexture.Height), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GameLayer.TileMap);
            }
        }
    }
}
