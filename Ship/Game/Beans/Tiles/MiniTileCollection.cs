#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Ship.Game.Beans.Tiles
{
    public class MiniTileCollection
    {
        private const byte TileWh = 16;

        private Rectangle _topLeft;
        private Rectangle _topRight;
        private Rectangle _bottomLeft;
        private Rectangle _bottomRight;
        
        private Vector2 _savedMapLocation;
        private Vector2 _savedMapLocationLeft;
        private Vector2 _savedMapLocationRight;
        private Vector2 _savedMapLocationBottom;
        private Vector2 _savedMapLocationBottomRight;

        internal int SpotX { get; set; }
        internal int SpotY { get; set; }

        internal bool NeedsRefreshed { get; private set; }

        internal bool Visible { get; set; }

        internal bool NeedsNewTexture { get; set; }

        internal bool IsElevated { get; private set; }

        public MiniTileCollection(int spotX, int spotY)
        {
            SpotX = spotX;
            SpotY = spotY;
            NeedsNewTexture = true;
        }

        internal void UpdateTexture(Rectangle topLeft, Rectangle topRight, Rectangle bottomLeft, Rectangle bottomRight, bool isElevated) 
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomLeft = bottomLeft;
            _bottomRight = bottomRight;
            IsElevated = isElevated;
            NeedsNewTexture = false;
            Visible = true;
        }

        internal void Draw(SpriteBatch spriteBatch, Texture2D mainTexture, Vector2 mapLocation,float layer)
        {
            if (!Visible) return;
            if(!mapLocation.Equals(_savedMapLocation) || NeedsRefreshed)
            {
                _savedMapLocation = mapLocation;
                NeedsRefreshed = false;

                var startLocationX = (TileWh * 2) * SpotX;
                var startLocationY = (TileWh * 2) * SpotY;

                _savedMapLocationLeft = new Vector2(startLocationX+mapLocation.X,startLocationY+mapLocation.Y);
                _savedMapLocationRight = new Vector2(startLocationX+(mapLocation.X + TileWh),startLocationY+mapLocation.Y);
                _savedMapLocationBottom = new Vector2(startLocationX + mapLocation.X, startLocationY + (mapLocation.Y + TileWh));
                _savedMapLocationBottomRight = new Vector2(startLocationX + (mapLocation.X + TileWh), startLocationY + (mapLocation.Y + TileWh));
            }

            spriteBatch.Draw(mainTexture, _savedMapLocationLeft, _topLeft, Color.White,0.0f,Vector2.Zero,1.0f,SpriteEffects.None,layer);
            if (IsElevated) return;
            spriteBatch.Draw(mainTexture, _savedMapLocationRight, _topRight, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            spriteBatch.Draw(mainTexture, _savedMapLocationBottom, _bottomLeft, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            spriteBatch.Draw(mainTexture, _savedMapLocationBottomRight, _bottomRight, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
        }

        internal void SetSpots(int i, int j)
        {
            NeedsRefreshed = true;
            SpotX = i;
            SpotY = j;
        }
    }
}