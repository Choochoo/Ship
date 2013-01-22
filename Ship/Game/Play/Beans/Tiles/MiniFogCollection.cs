#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Tiles
{
    public class MiniFogCollection
    {
        private readonly Color _drawColor = Color.White;
        private Rectangle _bottomLeft;
        private Rectangle _bottomRight;
        private Vector2 _moveVector;
        private Rectangle _topLeft;
        private Rectangle _topRight;

        private int SpotX { get; set; }
        private int SpotY { get; set; }

        internal bool IsAllFogOfWar { get; set; }

        internal bool IsDiscovered { get; set; }

        public void UpdateTexture(Rectangle topLeft, Rectangle topRight, Rectangle bottomLeft, Rectangle bottomRight)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
        }

        internal void Draw()
        {
            if (IsDiscovered) return;

            _moveVector.X = SpotX*MiniTileManager.TileSizeX2;
            _moveVector.Y = SpotY*MiniTileManager.TileSizeX2;

            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector, _topLeft, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + MiniTileManager.MiniCollectionRight,
                                        _topRight, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + MiniTileManager.MiniCollectionBottom,
                                        _bottomLeft, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture,
                                        _moveVector + MiniTileManager.MiniCollectionRight +
                                        MiniTileManager.MiniCollectionBottom, _bottomRight, _drawColor);
        }

        public void SetSpots(int i, int j)
        {
            SpotX = i;
            SpotY = j;
        }
    }
}