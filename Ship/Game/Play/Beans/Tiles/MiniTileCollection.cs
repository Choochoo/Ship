#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Tiles
{
    public class MiniTileCollection
    {
        private readonly Color _drawColor = Color.White;
        private Rectangle _bottomLeft;
        private Rectangle _bottomRight;
        private Vector2 _moveVector;
        private Rectangle _topLeft;
        private Rectangle _topRight;

        public MiniTileCollection(int mapLocX, int mapLocY, int currVectorX, int currVectorY, int topVectorY,
                                  int bottomVectorY, int leftVectorX, int rightVectorX)
        {
            MapLocX = mapLocX;
            MapLocY = mapLocY;
            CurrVectorX = currVectorX;
            CurrVectorY = currVectorY;
            TopVectorY = topVectorY;
            BottomVectorY = bottomVectorY;
            RightVectorX = rightVectorX;
            LeftVectorX = leftVectorX;
        }


        public int MapLocX { get; set; }
        public int MapLocY { get; set; }

        public int CurrVectorX { get; private set; }
        public int CurrVectorY { get; private set; }

        public int TopVectorY { get; private set; }
        public int BottomVectorY { get; private set; }
        public int LeftVectorX { get; private set; }
        public int RightVectorX { get; private set; }

        public bool Visible { get; set; }

        internal bool IsElevated { get; private set; }

        public void UpdateTexture(Rectangle topLeft, Rectangle topRight, Rectangle bottomLeft, Rectangle bottomRight,
                                  bool isElevated)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
            IsElevated = isElevated;
            Visible = true;
        }

        internal void Draw(byte direction = 0, int location = 0)
        {
            if (!Visible) return;

            _moveVector.X = _moveVector.Y = 0;
            switch (direction)
            {
                case MoveConstants.Up:
                    _moveVector.X = location*MiniTileManager.TileSizeX2;
                    break;
                case MoveConstants.Down:
                    _moveVector.X = location*MiniTileManager.TileSizeX2;
                    _moveVector.Y = (MiniTileManager.Amount - 1)*MiniTileManager.TileSizeX2;
                    break;
                case MoveConstants.Left:
                    _moveVector.Y = location*MiniTileManager.TileSizeX2;
                    break;
                case MoveConstants.Right:
                    _moveVector.X = (MiniTileManager.Amount - 1)*MiniTileManager.TileSizeX2;
                    _moveVector.Y = location*MiniTileManager.TileSizeX2;
                    break;
                default:
                    _moveVector.X = CurrVectorX*MiniTileManager.TileSizeX2;
                    _moveVector.Y = CurrVectorY*MiniTileManager.TileSizeX2;
                    break;
            }

            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector, _topLeft, _drawColor);
            if (IsElevated) return;
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + MiniTileManager.MiniCollectionRight,
                                        _topRight, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + MiniTileManager.MiniCollectionBottom,
                                        _bottomLeft, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture,
                                        _moveVector + MiniTileManager.MiniCollectionRight +
                                        MiniTileManager.MiniCollectionBottom, _bottomRight, _drawColor);
        }
    }
}