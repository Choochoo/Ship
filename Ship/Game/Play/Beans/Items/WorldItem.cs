#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Beans.Items.Keys;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Beans.Animations;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Beans.Sounds;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.Beans.Tiles;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Items
{
    public class WorldItem : IComparable
    {
        private const float SpeedToCharacter = 7f;
        private readonly BasicSound _pickupSound;
        private readonly ItemPool _pool;
        private readonly LootDropAnim _tween = new LootDropAnim();
        private bool _active;
        private bool _forceQuit;
        private Hero _gotoHero;
        private Vector2 _gotoVector = Vector2.One;
        private Rectangle _hitRect;
        private int _lifetime;
        private TextureRegion _myRegion;
        private TileCollection _myTile;
        private int _speedSet;

        public WorldItem(ItemPool pool)
        {
            _hitRect = Rectangle.Empty;
            _pool = pool;
            _pickupSound = new BasicSound("itempickup");
        }

        public int InventoryPosition { get; set; }
        public byte MyType { get; set; }

        public TextureRegion MyRegion { get { return _myRegion; } }

        public Rectangle HitRect { get { return _hitRect; } }

        public byte Count { get; set; }

        public bool InInventory { get; set; }

        public int Lifetime { get { return _lifetime; } }

        public int CompareTo(object obj)
        {
            if (InInventory)
                return 1;

            var that = (WorldItem) (obj);

            if (that.InInventory)
                return -1;

            if (that.Lifetime <= Lifetime)
                return -1;

            return 1;
        }

        public void SetType(ref TileCollection myTile, byte type, float positionX, float positionY)
        {
            MyType = type;
            Count = 1;
            _myRegion = WorldItemKey.ItemRegions[type];
            _hitRect.Width = _myRegion.Bounds.Width;
            _hitRect.Height = _myRegion.Bounds.Height;
            PutOnGround(positionX, positionY);
            SetTile(ref myTile);
        }

        public void PutOnGround(float positionX, float positionY)
        {
            _active = true;
            _hitRect.X = (int) positionX;
            _hitRect.Y = (int) positionY;
            _tween.UpdatePositions(positionX, positionY);
            InventoryPosition = 0;
            InInventory = false;
            _forceQuit = false;
            _gotoHero = null;
            _speedSet = 0;
            _lifetime = 0;
        }


        internal bool Update()
        {
            _lifetime = Lifetime + PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
            if (_forceQuit || InInventory)
            {
                _active = false;
                return false;
            }

            if (_active)
            {
                if (_gotoHero != null)
                {
                    if (HitRect.Intersects(_gotoHero.HitRect))
                    {
                        Pickup();
                        _active = false;
                        _gotoHero = null;
                        return false;
                    }

                    if (_speedSet < 10)
                    {
                        var diffx = _gotoHero.HitRect.X - HitRect.X;
                        var diffy = _gotoHero.HitRect.Y - HitRect.Y;
                        _gotoVector.X = diffx/SpeedToCharacter;
                        _gotoVector.Y = diffy/SpeedToCharacter;
                        _gotoVector.X += _gotoVector.X < 0 ? -1.4f : 1.4f;
                        _gotoVector.Y += _gotoVector.Y < 0 ? -1.4f : 1.4f;
                        _speedSet = 0;
                    }
                    _speedSet++;
                }
                else
                    _gotoVector.X = _gotoVector.Y = 0;


                _tween.Update(ref _gotoVector, ref _hitRect);
                return true;
            }

            return false;
        }

        internal void Draw()
        {
            if (_active && !_forceQuit)
            {
                PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _tween.RenderPosition, _myRegion.Bounds, Color.White,
                                            0f, Vector2.Zero, _tween.Scale, SpriteEffects.None, 0f);
                if(Count > 1)
                PlayScreen.Spritebatch.DrawString(MainGame.FpsFont, Count.ToString(), _tween.Position, Color.White);
            }
        }

        public void DrawForMenu(ref Vector2 pos)
        {
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, pos, _myRegion.Bounds, Color.White);
            PlayScreen.Spritebatch.DrawString(MainGame.FpsFont, Count.ToString(), pos, Color.White);
        }

        private void Pickup()
        {
            _pickupSound.Play();
            _myTile.RemoveWorldItem(this);
            _gotoHero.MyInventory.AddToInventory(this);
            //!!it does remove this from tile in hero!!//
            if (!InInventory)
                Retire();
            else
            {
                _forceQuit = true;
                _active = false;
            }
            
        }

        internal void Retire()
        {
            Count = 0;

            _forceQuit = true;
            _active = false;
            _pool.AddToInactive(this);
        }

        internal void Loot(Hero hero)
        {
            if (_gotoHero == null)
                _gotoHero = hero;
        }

        internal void SetTile(ref TileCollection startTile)
        {
            _myTile = startTile;
            _myTile.AddWorldItem(this, true);
        }
    }
}