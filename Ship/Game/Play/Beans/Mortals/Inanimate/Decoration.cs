#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Utils;
using System;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.Play.Beans.Items.Keys;
using Ship.Game.Play.Beans.Helpers;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Beans.Animations;
using Ship.Game.Play.Beans.Items;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Mortals.Inanimate
{
    public class Decoration : IComparable
    {
        private const byte Foliage = 1;
        private const byte Fire = 2;
        private const short MaxHitpoints = 10;

        private readonly DecorManager _dm;
        private readonly Color _drawColor = Color.White;
        private readonly Texture2D _myTexture;
        private readonly int _numberOfElementsAllTogether;
        private readonly SpriteBatch _spriteBatch;
        private readonly TextureRegion[] _tileRegions;

        protected bool DownFine;
        protected bool LeftFine;
        protected bool RightFine;
        protected bool UpFine;

        private Vector2[] _addons;
        private int _animLastUpdate;
        private Rectangle[] _bounds;
        private Rectangle _hitRect;

        private int _itemTotal;
        private byte[] _itemVals = new byte[WorldItemKey.TotalLootableObjects];
        private int _lastAnimation;
        private int _lasthitPointUpdate;
        private short _myHitpoints = MaxHitpoints;
        private byte _myType;
        private Vector2 _position;

        private DestroyTreeAnim _treeDestroyAnimation;
        private bool _visible;

        public Decoration(DecorManager dm, ref TextureRegion[] tileRegions)
        {
            _dm = dm;
            _hitRect = new Rectangle(0, 0, 50, 50);
            _tileRegions = tileRegions;
            _numberOfElementsAllTogether = DecorHelper.BushCount + DecorHelper.RockCount + DecorHelper.TreeCount;
            _spriteBatch = PlayScreen.Spritebatch;
            _myTexture = PlayScreen.DecorationTexture;
            _treeDestroyAnimation = new DestroyTreeAnim("treefalling");
        }

        internal int MySectorSpotX { get; set; }

        internal int MySectorSpotY { get; set; }

        internal int SectorX { get; set; }

        internal int SectorY { get; set; }

        internal int MyVectorSpotX { get; set; }

        internal int MyVectorSpotY { get; set; }

        internal Vector2 Position { get { return _position; } }

        internal int PoolLocation { get; set; }

        public byte MyType { get; private set; }

        public bool Visible { get { return _visible; } }

        public Rectangle HitRect { get { return _hitRect; } }

        public int CompareTo(object obj)
        {
            if (!Visible)
                return 1;

            var that = (Decoration) (obj);

            if (!that.Visible)
                return -1;

            if (that.HitRect.Y.Equals(HitRect.Y))
            {
                if (that.HitRect.X < HitRect.X)
                    return -1;
            }
            else if (that.HitRect.Y > HitRect.Y)
                return -1;

            return 1;
        }

        public void UpdatePosition(int x, int y)
        {
            _position.X = x;
            _position.Y = y;
        }


        public bool SetInfo(DecorManager decorManager, byte type)
        {
            _visible = type != DecorHelper.None;
            MyType = type;
            if (!_visible) return false;

            _animLastUpdate = 0;
            _lasthitPointUpdate = 500;

            //returns isLightSource;


            if (type >= DecorHelper.Tree && type <= DecorHelper.Bush)
            {
                var rand = new Random(SectorX + MySectorSpotX + SectorY + MySectorSpotY);
                _bounds = new Rectangle[1];
                _addons = new Vector2[1];

                _myType = Foliage;
                var value = 0;
                switch (type)
                {
                    case DecorHelper.Tree:
                        value = rand.Next(1, DecorHelper.TreeCount + 1);

                        break;
                    case DecorHelper.Rock:
                        //as to be more than 1
                        value = (DecorHelper.TreeCount - 1) + 1;
                        break;
                    case DecorHelper.Bush:
                        value = (DecorHelper.TreeCount - 1) + 1 + rand.Next(1, DecorHelper.BushCount + 1);
                        break;
                }

                _bounds[0] = _tileRegions[value].Bounds;
                var valueX = (Position.X + rand.Next(16, 48)) - (_bounds[0].Width/2.0f);
                var valueY = Position.Y - _bounds[0].Height + rand.Next(16, 48);

                _addons[0] = new Vector2(valueX, valueY);

                switch (type)
                {
                    case DecorHelper.Tree:
                        _hitRect.Width = 40;
                        _hitRect.Height = 30;
                        _hitRect.X = (int) (valueX + ((_bounds[0].Width/2.0) - (HitRect.Width/2.0)));
                        _hitRect.Y = (int) (valueY + _bounds[0].Height - _hitRect.Height);
                        break;
                    case DecorHelper.Rock:
                    case DecorHelper.Bush:
                        _hitRect.Width = _bounds[0].Width;
                        _hitRect.Height = _bounds[0].Height - 15;
                        _hitRect.X = (int) valueX;
                        _hitRect.Y = (int) valueY + 15;
                        break;
                }
            }
            else if (type == DecorHelper.Fire)
            {
                _myType = Fire;
                _bounds = new Rectangle[3];
                _addons = new Vector2[4];
                var value = decorManager.FireStart;
                _bounds[0] = _tileRegions[value].Bounds;

                _position.X = 32 - _bounds[0].Width/2;
                _position.Y = 32 - _bounds[0].Height/2;
                _hitRect.X = (int) _position.X;
                _hitRect.Y = (int) _position.Y;
                _bounds[1] = _tileRegions[value + 1].Bounds;
                _bounds[2] = _tileRegions[value + 2].Bounds;
                _addons[0] = new Vector2(6, -4);
                _addons[1] = new Vector2(5, 0);
                _addons[2] = new Vector2(5, -(_bounds[2].Height/2));
                _addons[3] = new Vector2(5, -(_bounds[2].Height));
                return true;
            }

            return false;
        }


        internal void Draw()
        {
            if (!Visible)
                return;

            //LeftFine = cam.Position.X < _renderPosition.X + _mainBounds.Width;
            //RightFine = cam.Position.X + cam.ViewportWidth > _renderPosition.X;
            //UpFine = cam.Position.Y < _renderPosition.Y + _mainBounds.Height;
            //DownFine = cam.Position.Y + cam.ViewportHeight > _renderPosition.Y;

            //if (LeftFine && RightFine && UpFine && DownFine)
            switch (_myType)
            {
                case Foliage:
                    _itemVals[0] = _itemVals[1] = _itemVals[2] = _itemVals[3] = _itemVals[4] = WorldItemKey.Wood;
                    _itemTotal = 5;
                    _spriteBatch.Draw(_myTexture, _addons[0], _bounds[0], _drawColor);
#if DEBUG
                    if (PlayScreen.HitBox)
                        _spriteBatch.Draw(PlayScreen.ErrorBox, _hitRect, PlayScreen.HitboxDebugColor);
#endif
                    return;
                case Fire:
                    _spriteBatch.Draw(_myTexture, Position, _bounds[0], _drawColor);
                    var limit = -(_bounds[2].Height + 5);
                    _lastAnimation += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
                    if (_lastAnimation > 50)
                    {
                        for (var i = 1; i < _addons.Length; i++)
                        {
                            _addons[i].Y--;
                            if ((_addons[i].Y%5).Equals(1))
                                _addons[i].X--;
                            if (_addons[i].Y < limit)
                            {
                                _addons[i].X = 10;
                                _addons[i].Y = 0;
                            }
                        }

                        _lastAnimation = 0;
                    }


                    _spriteBatch.Draw(_myTexture, Position + _addons[0], _bounds[1], _drawColor, 0.0f, Vector2.Zero,
                                      1.0f, SpriteEffects.None, 0.0f);


                    for (var i = 1; i < _addons.Length; i++)
                    {
                        var color = Color.White;
                        var perc = (_addons[i].Y + 1)/(limit + 1);
                        color.A = (byte) ((1 - perc)*byte.MaxValue);
                        _spriteBatch.Draw(_myTexture, Position + _addons[i], _bounds[2], _drawColor, 0.0f, Vector2.Zero,
                                          perc, SpriteEffects.None, 0.0f);
                    }

#if DEBUG

                    if (PlayScreen.HitBox)
                        _spriteBatch.Draw(PlayScreen.ErrorBox, _hitRect, PlayScreen.HitboxDebugColor);

#endif

                    return;
            }

            return;
        }

        internal void IsHit(short damage)
        {
            _animLastUpdate += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
            _lasthitPointUpdate += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;

            if (_animLastUpdate < 500)
            {
                _animLastUpdate = 0;

                if (_lasthitPointUpdate > 500)
                {
                    _lasthitPointUpdate = 0;
                    _myHitpoints -= damage;
                }

                if (_myHitpoints <= 0)
                {
                    ItemPool.MyItemPool.AddToActive(ref _itemVals, _itemTotal,
                                                    ref TileManager.TileColls[MyVectorSpotX, MyVectorSpotY]);
                    _treeDestroyAnimation.Play();
                    _visible = false;
                    _dm.SortPool();
                }
            }
            else
            {
                _animLastUpdate = 0;
                _myHitpoints = MaxHitpoints;
            }
            //_visible = false;
        }
    }
}