#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Utils;
using Ship.Game.Beans.Mortals.Keys;
using Ship.Game.Loaders;

#endregion

namespace Ship.Game.Beans.Tiles
{
    public class TileCollection
    {
        private const byte TileWh = 32;
        private int _bottomLeft;
        private int _bottomRight;
        private int _topLeft;
        private int _topRight;
        private bool _downFine;
        private bool _leftFine;
        private Vector2 _position;
        private bool _rightFine;

        private Vector2 _savedMapLocationBottom;
        private Vector2 _savedMapLocationBottomRight;
        private Vector2 _savedMapLocationLeft;
        private Vector2 _savedMapLocationRight;
        
        private bool _upFine;

        internal int MySectorSpotX { get; set; }

        internal int MySectorSpotY { get; set; }

        internal int SectorX { get; set; }

        internal int SectorY { get; set; }

        internal int MyVectorSpotX { get; set; }

        internal int MyVectorSpotY { get; set; }

        internal LoaderBase MyLoaderBase;

        internal Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                _savedMapLocationLeft = new Vector2(value.X, value.Y);
                _savedMapLocationRight = new Vector2(value.X + TileWh, Position.Y);
                _savedMapLocationBottom = new Vector2(value.X, value.Y + TileWh);
                _savedMapLocationBottomRight = new Vector2(value.X + TileWh, value.Y + TileWh);
            }
        }




        internal int Draw(ref SpriteBatch spriteBatch, ref  Texture2D mainTextures, ref  TextureRegion[] tileRegions, ref  Camera2D cam)
        {
            //var drawColor = MySectorSpotY == 0 || MySectorSpotX == 0 ? Color.Red : Color.White;
            var drawColor = Color.White;
            var numOfDraws = 0;
            _leftFine = cam.Position.X < _savedMapLocationLeft.X + tileRegions[_topLeft].Bounds.Width;
            _rightFine = cam.Position.X + cam.ViewportWidth > _savedMapLocationRight.X;
            _upFine = cam.Position.Y < _savedMapLocationLeft.Y + tileRegions[_topLeft].Bounds.Height;
            _downFine = cam.Position.Y + cam.ViewportHeight > _savedMapLocationRight.Y;

            if (_leftFine && _upFine && _topLeft != 0)
            {
                spriteBatch.Draw(mainTextures, _savedMapLocationLeft, tileRegions[_topLeft].Bounds, drawColor, 0.0f,
                                 Vector2.Zero, 1.0f, SpriteEffects.None, GameLayer.TileMap);
                numOfDraws++;
            }
            if (_rightFine && _upFine && _topRight != 0)
            {
                spriteBatch.Draw(mainTextures, _savedMapLocationRight, tileRegions[_topRight].Bounds, drawColor, 0.0f,
                                 Vector2.Zero, 1.0f, SpriteEffects.None, GameLayer.TileMap);
                numOfDraws++;
            }
            if (_leftFine && _downFine && _bottomLeft != 0)
            {
                spriteBatch.Draw(mainTextures, _savedMapLocationBottom, tileRegions[_bottomLeft].Bounds, drawColor, 0.0f,
                                 Vector2.Zero, 1.0f, SpriteEffects.None, GameLayer.TileMap);
                numOfDraws++;
            }
            if (_rightFine && _downFine && _bottomRight != 0)
            {
                spriteBatch.Draw(mainTextures, _savedMapLocationBottomRight, tileRegions[_bottomRight].Bounds, drawColor,
                                 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GameLayer.TileMap);
                numOfDraws++;
            }

            return numOfDraws;
        }

        public void SetSurroundings(int[] p)
        {
            _topLeft = p[0];
            _topRight = p[1];
            _bottomLeft = p[2];
            _bottomRight = p[3];
        }
    }
}