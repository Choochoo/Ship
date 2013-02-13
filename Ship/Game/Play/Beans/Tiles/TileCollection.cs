#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Utils;
using Ship.Game.Play.Loaders;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.Play.Beans.Items;
using System.Collections.Generic;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Tiles
{
    public class TileCollection
    {
        private readonly int _bottomVectorY;
        private readonly int _leftVectorX;
        private readonly int _rightVectorX;
        private readonly int _topVectorY;
        internal LoaderBase MyLoaderBase;
        private int _bottomLeft;
        private int _bottomRight;
        private readonly List<WorldItem> _itemsOnTile = new List<WorldItem>();

        private int _topLeft;
        private int _topRight;

        private readonly TextureRegion[] _tileRegions;

        public TileCollection(TextureRegion[] tileRegions, int vectorx, int vectory, int width, int height)
        {
            _tileRegions = tileRegions;

            MyVectorSpotX = vectorx;
            MyVectorSpotY = vectory;

            _topVectorY = MyVectorSpotY - 1 < 0 ? height - 1 : MyVectorSpotY - 1;
            _bottomVectorY = MyVectorSpotY + 1 < height ? MyVectorSpotY + 1 : 0;
            _leftVectorX = MyVectorSpotX - 1 < 0 ? width - 1 : MyVectorSpotX - 1;
            _rightVectorX = MyVectorSpotX + 1 < width ? MyVectorSpotX + 1 : 0;
            //minus 25 for width of hitbox of hero
            _hitRect = new Rectangle((vectorx*TileManager.Sprite2XWidth), (vectory*TileManager.Sprite2XWidth),
                                     TileManager.Sprite2XWidth - 14, TileManager.Sprite2XWidth - 12);
        }

        public void AddWorldItem(WorldItem wi, bool goBlue = false)
        {
            ItemsOnTile.Add(wi);
#if DEBUG
            if (goBlue)
            {
                ShowHitBox = true;
                _drawHitBoxColor = Color.Blue;
            }
#endif
        }

        internal int MySectorSpotX { get; set; }

        internal int MySectorSpotY { get; set; }

        internal int SectorX { get; set; }

        internal int SectorY { get; set; }

        internal int MyVectorSpotX { get; private set; }

        internal int MyVectorSpotY { get; private set; }

#if DEBUG
        internal bool ShowHitBox { get; set; }
#endif

        public Rectangle HitRect { get { return _hitRect; } }

        public int BottomVectorY { get { return _bottomVectorY; } }

        public int LeftVectorX { get { return _leftVectorX; } }

        public int RightVectorX { get { return _rightVectorX; } }

        public int TopVectorY { get { return _topVectorY; } }

        public List<WorldItem> ItemsOnTile { get { return _itemsOnTile; } }

        private Rectangle _hitRect;

        public void SetPosition(int x, int y)
        {
            _hitRect.X = x;
            _hitRect.Y = y;
            while (ItemsOnTile.Count > 0)
            {
                ItemsOnTile[0].Retire();
                ItemsOnTile.RemoveAt(0);
            }
        }

        private readonly Vector2 _modifierRight = new Vector2(TileManager.SpriteWidth, 0);
        private readonly Vector2 _modifierBottom = new Vector2(0, TileManager.SpriteWidth);
        private Vector2 _moveVector = Vector2.Zero;

#if DEBUG
        private Color _drawHitBoxColor = Color.White;

        internal void DrawHitBox()
        {
            if (_itemsOnTile.Count > 0)
                PlayScreen.Spritebatch.Draw(PlayScreen.ErrorBox, _hitRect, _drawHitBoxColor);
        }
#endif


        private readonly Color _drawColor = Color.White;

        internal void Draw(byte direction = 0, int location = 0)
        {
            _moveVector.X = _moveVector.Y = 0;

            switch (direction)
            {
                case MoveConstants.Up:
                    _moveVector.X = location*TileManager.Sprite2XWidth;
                    break;
                case MoveConstants.Down:
                    _moveVector.X = location*TileManager.Sprite2XWidth;
                    _moveVector.Y = TileManager.ScreenBuffer - TileManager.Sprite2XWidth;
                    break;
                case MoveConstants.Left:
                    _moveVector.Y = location*TileManager.Sprite2XWidth;
                    break;
                case MoveConstants.Right:
                    _moveVector.X = TileManager.ScreenBuffer - TileManager.Sprite2XWidth;
                    _moveVector.Y = location*TileManager.Sprite2XWidth;
                    break;
                default:
                    _moveVector.X = MyVectorSpotX*TileManager.Sprite2XWidth;
                    _moveVector.Y = MyVectorSpotY*TileManager.Sprite2XWidth;
                    break;
            }

            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector, _tileRegions[_topLeft].Bounds,
                                        _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + _modifierRight,
                                        _tileRegions[_topRight].Bounds, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + _modifierBottom,
                                        _tileRegions[_bottomLeft].Bounds, _drawColor);
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _moveVector + _modifierBottom + _modifierRight,
                                        _tileRegions[_bottomRight].Bounds, _drawColor);
        }

        public void SetSurroundings(int[] p)
        {
            _topLeft = p[0];
            _topRight = p[1];
            _bottomLeft = p[2];
            _bottomRight = p[3];
        }

        internal void RemoveWorldItem(WorldItem worldItem) { _itemsOnTile.Remove(worldItem); }
    }
}