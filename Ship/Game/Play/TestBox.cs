#region

using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework;
using System;
using Ship.Game.Play.Utils;
using System.Collections.Generic;
using Ship.Game.Play.WorldGeneration.Noise;
using Ship.Game.Play.Beans.Tiles;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.WorldGeneration;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play
{
    public class TestBox
    {
        private const float Zoom = 3.0f;
        private readonly TextureAtlas _mainAtlas;
        private readonly Texture2D _mainTextures;
        private readonly Rectangle _rect;
        private readonly Texture2D[,] _texture = new Texture2D[3,3];
        private readonly Vector2[,] _texturePos = new Vector2[3,3];
        private Texture2D _grassTexture;
        private BezierCurve bc = new BezierCurve();
        private Vector2 showPosition;
        private int startTime;
        private TextureRegion textureRegion;

        public TestBox()
        {
            _mainAtlas = PlayScreen.DecorationAtlas;
            _mainTextures = PlayScreen.DecorationTexture;
            _rect = new Rectangle(0, 0, PlayScreen.SectorTileSize, PlayScreen.SectorTileSize);
            TerrainTesting();
        }

        public void TextureTesting(GraphicsDevice graphicsDevice)
        {
            startTime = Environment.TickCount;
            textureRegion = _mainAtlas.GetRegion("grass");
        }

        public void TileTesting(GraphicsDevice graphicsDevice)
        {
            textureRegion = _mainAtlas.GetRegion("decor_hot_tree1");
            var xspot = 1;
            var yspot = 1;
            showPosition = new Vector2(((xspot*PlayScreen.SectorTileSize)*Zoom) - (textureRegion.Bounds.Width/2.0f),
                                       ((yspot*PlayScreen.SectorTileSize)*Zoom) - (textureRegion.Bounds.Height*3) +
                                       ((PlayScreen.SectorTileSize*Zoom)/2));
            //TerrainTesting(graphicsDevice); 
            var black = new[] {Color.Black};
            var red = new[] {Color.Red};
            for (var x = 0; x < _texture.GetLength(0); x++)
            {
                for (var y = 0; y < _texture.GetLength(0); y++)
                {
                    _texture[x, y] = new Texture2D(graphicsDevice, PlayScreen.SectorTileSize, PlayScreen.SectorTileSize);
                    _texturePos[x, y] = new Vector2((x*PlayScreen.SectorTileSize)*Zoom,
                                                    (y*PlayScreen.SectorTileSize)*Zoom);


                    for (var i = 0; i < PlayScreen.SectorTileSize; i++)
                    {
                        for (var j = 0; j < PlayScreen.SectorTileSize; j++)
                        {
                            if (i == 0 || j == 0)
                                _texture[x, y].SetData(0, new Rectangle(i, j, 1, 1), red, 0, 1);
                            else
                                _texture[x, y].SetData(0, new Rectangle(i, j, 1, 1), black, 0, 1);
                        }
                    }
                }
            }
        }


        public void TerrainTesting()
        {
            var red = new[] {Color.Red};
            var black = new[] {Color.Black};


            //var vals = new byte[3][] { new byte[3], new byte[3], new byte[3] };
            //vals[0][0] = 12;  vals[1][0] = 12;  vals[2][0] = 15;
            //vals[0][1] = 0; vals[1][1] = 10;  vals[2][1] = 15;
            //vals[0][2] = 0; vals[1][2] = 0; vals[2][2] = 15;
            var nextRand = new Random().Next();
            //var nextRand = 1575943251;
            var rand = new Random(nextRand);
            var nextX = rand.Next();
            var nextY = rand.Next();
            const byte waterVal = 3;


            var oceanSquares = new[] {new byte[5], new byte[5], new byte[5], new byte[5], new byte[5]};
            if (false)
            {
                const double prob = .8;
                oceanSquares[0][0] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[1][0] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[2][0] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[3][0] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[4][0] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));

                oceanSquares[0][1] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[1][1] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[2][1] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[3][1] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[4][1] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));

                oceanSquares[0][2] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[1][2] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[2][2] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[3][2] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[4][2] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));

                oceanSquares[0][3] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[1][3] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[2][3] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[3][3] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[4][3] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));

                oceanSquares[0][4] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[1][4] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[2][4] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[3][4] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));
                oceanSquares[4][4] = (byte) (waterVal + (rand.NextDouble() > prob ? 1 : 0));

                for (var y = 0; y < oceanSquares[0].Length; y++)
                {
                    for (var x = 0; x < oceanSquares.Length; x++)
                        System.Diagnostics.Debug.Write(oceanSquares[x][y] + ",");
                    System.Diagnostics.Debug.WriteLine("");
                }
            }

            //var seed = 1305465703;

            var xCord = 0;
            var yCord = 0;
            var seed = new Random().Next();
            System.Diagnostics.Debug.WriteLine("seed:" + seed);
            Random newRand;
            while (true)
            {
                seed = new Random().Next();
                System.Diagnostics.Debug.WriteLine("seed:" + seed);
                newRand = new Random(seed);
                xCord = 212; // newRand.Next(0, MainGame.MapWidth - 2);
                yCord = 450; // newRand.Next(0, MainGame.MapHeight - 2);
                var breakout = false;
                for (var x = 0; x < _texture.GetLength(0); x++)
                {
                    for (var y = 0; y < _texture.GetLength(0); y++)
                    {
                        if (WorldData.MyWorldData.WaterSquareData[x + xCord][y + yCord] != null)
                            breakout = true;

                        if (breakout)
                            break;
                    }
                    if (breakout)
                        break;
                }

                if (breakout)
                    break;
            }

            for (var x = 0; x < _texture.GetLength(0); x++)
            {
                for (var y = 0; y < _texture.GetLength(0); y++)
                {
                    //var tiledata = new byte[PlayScreen.SectorTileSize, PlayScreen.SectorTileSize];

                    //NoiseGen.GetDecorSimplex(ref tiledata, new Vector2(x, y));
                    //var tiledata = TestTerrain(nextX + x, nextY + y);
                    //if(masterWaterData[x][y] == null)continue;

                    var tiledata = GetTestWater(WorldData.MyWorldData.WaterSquareData, x + xCord, y + yCord, newRand);
                    //if (oceanSquares[x + 1][y + 1] != waterVal) continue;
                    //var tiledata = NoiseGen.OceanWithLand(PlayScreen.SectorTileSize, oceanSquares, x + 1, y + 1, waterVal);

                    _texture[x, y] = new Texture2D(MainGame.GraphicD, PlayScreen.SectorTileSize,
                                                   PlayScreen.SectorTileSize);
                    _texturePos[x, y] = new Vector2((x*PlayScreen.SectorTileSize)*Zoom,
                                                    (y*PlayScreen.SectorTileSize)*Zoom);

                    for (var i = 0; i < PlayScreen.SectorTileSize; i++)
                    {
                        for (var j = 0; j < PlayScreen.SectorTileSize; j++)
                        {
                            if (false) //noise
                            {
                                var newcolor = new Color(tiledata[i, j], tiledata[i, j], tiledata[i, j]);
                                _texture[x, y].SetData(0, new Rectangle(i, j, 1, 1), new[] {newcolor}, 0, 1);
                            }
                            if (tiledata[i, j] != 0)
                                _texture[x, y].SetData(0, new Rectangle(i, j, 1, 1), red, 0, 1);
                            else
                                _texture[x, y].SetData(0, new Rectangle(i, j, 1, 1), black, 0, 1);
                        }
                    }
                }
            }
        }


        private byte[,] GetTestWater(WaterSquare[][] masterWaterData, int x, int y, Random rand)
        {
            var s = Environment.TickCount;
            var data = new byte[PlayScreen.SectorTileSize,PlayScreen.SectorTileSize];

            if (masterWaterData[x][y] != null)
                NoiseGen.GetRiverSquare(ref data, masterWaterData[x][y], 3, rand);
            System.Diagnostics.Debug.WriteLine("time output:{0}", Environment.TickCount - s);
            return data;
        }

        //private WaterData[] FillArea(byte[][] vals, int SectorX, int SectorY) 
        //{

        //    var top = ValueDecider(vals, SectorX, SectorY, 0, -1);
        //    var left = ValueDecider(vals, SectorX, SectorY, -1, 0);
        //    var right = ValueDecider(vals, SectorX, SectorY, 1, 0);
        //    var bottom = ValueDecider(vals, SectorX, SectorY, 0, 1);

        //    //if there are two that are less than or greater than, handle it here, before its too late!

        //    return new []{top,bottom,left,right};
        //}


        public byte[,] TestTerrain(int sectorX, int sectorY)
        {
            var td = new byte[PlayScreen.SectorTileSize,PlayScreen.SectorTileSize];
            //NoiseGen.GetNoise(ref td, 50, 50, sectorX, sectorY, 1, .7); //wall
            return td;
        }

        //public void Draw(SpriteBatch spriteBatch)
        //{
        //    var textureWidth = _grassTexture.Width*22;
        //    var textureHeight = _grassTexture.Height * 18;
        //    //var difference = System.Environment.TickCount - startTime;
        //    //difference /= 1000;
        //    //difference %= 24;

        //    //int diff = 0;
        //    //if(difference > 12)
        //    //{
        //    //    diff = difference - 12;
        //    //    difference = 12;
        //    //}q
        //    //System.Diagnostics.Debug.WriteLine(difference-diff);
        //    //difference = (int)(((difference - diff) / 12.0) * 128);
        //    //difference += 128;
        //    //var newColor = new Color(difference, difference, difference);
        //    var source = new Rectangle(0, 0, textureWidth, textureHeight);
        //    spriteBatch.Draw(_mainTextures, Vector2.Zero, textureRegion.Bounds, Color.White);
        //    //spriteBatch.Draw(_mainTextures,textureRegion.Bounds, Color.White);
        //}

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var x = 0; x < _texture.GetLength(0); x++)
            {
                for (var y = 0; y < _texture.GetLength(0); y++)
                {
                    if (_texture[x, y] == null) continue;
                    spriteBatch.Draw(_texture[x, y], _texturePos[x, y], _rect, Color.White, 0.0f, Vector2.Zero, Zoom,
                                     SpriteEffects.None, .1f);

                    if (textureRegion != null)
                    {
                        spriteBatch.Draw(_mainTextures, showPosition, textureRegion.Bounds, Color.White, 0.0f,
                                         Vector2.Zero, Zoom, SpriteEffects.None, .08f);
                    }
                }
            }
        }
    }
}