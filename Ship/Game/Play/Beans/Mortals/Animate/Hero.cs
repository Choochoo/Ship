#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Utils;
using Ship.Game.Play.Beans.Constants;
using Lidgren.Network;
using Ship.Game.Play.Beans.Tiles;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Beans.Sounds;
using Ship.Game.Play.Beans.Animations;
using Ship.Game.Play.Beans.Helpers;
using Ship.Game.Play.Beans.Items;
using Ship.Game.Play.Beans.Mortals.Animate.Extras;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Mortals.Animate
{
    public class Hero
    {
        public const byte Stopped = 0;
        public const byte MoveUp = 1;
        public const byte MoveDown = 2;
        public const byte MoveLeft = 3;
        public const byte MoveRight = 4;

        public const byte ActionNone = 0;
        public const byte ActionLeftMouseButton = 1;
        public const byte ActionRightMouseButton = 2;

        public const byte EquippedNone = 0;
        public const byte EquippedAxe = 1;
        public const byte EquippedHammer = 2;
        private readonly CharacterAnim _axeDown;
        private readonly CharacterAnim _axeLeft;
        private readonly CharacterAnim _axeRight;
        private readonly CharacterAnim _axeUp;

        private readonly Vector2 _centerPosition = new Vector2(MainGame.WindowWidthCenter - 32.0f,
                                                               MainGame.WindowHeightCenter - 32.0f);

        private readonly CharacterAnim _dead;
        private readonly CharacterAnim _hammerDown;
        private readonly CharacterAnim _hammerLeft;
        private readonly CharacterAnim _hammerRight;
        private readonly CharacterAnim _hammerUp;

        private readonly Texture2D _mainTexture;
        private readonly Inventory _myInventory;

        //tweens


        private readonly CharacterAnim _runDown;
        private readonly CharacterAnim _runLeft;
        private readonly CharacterAnim _runRight;
        private readonly CharacterAnim _runUp;

        private readonly CharacterAnim _sickleDown;
        private readonly CharacterAnim _sickleLeft;
        private readonly CharacterAnim _sickleRight;
        private readonly CharacterAnim _sickleUp;
        private readonly SpriteBatch _spriteBatch;
        private readonly CharacterAnim _walkDown;
        private readonly CharacterAnim _walkLeft;
        private readonly CharacterAnim _walkRight;
        private readonly CharacterAnim _walkUp;

        private readonly CharacterAnim _waterDown;
        private readonly CharacterAnim _waterLeft;
        private readonly CharacterAnim _waterRight;
        private readonly CharacterAnim _waterUp;
        private readonly DecorationSound _woodChop;
        private readonly List<WorldItem> removeWorldItem = new List<WorldItem>();


        // end tweens

        private Rectangle _attackRect;
        private Color _drawColor = Color.White;
        private Rectangle _hitRect;

        private bool _isMoving;
        private CharacterAnim _lastAnimation;
        //private CharacterAnim _lastLowerAnimation;
        private int _lastChange;
        private Rectangle _lootRect;


        private byte _myAction;
        private byte _myEquipped;


        private byte _myKeyboardDirection;
        private byte _myLastAction;

        private byte _myLastKeyBoardDirection;
        private byte _myMouseDirection;
        // private byte _myLastMouseDirection;
        private TileCollection _myTile;
        private Vector2 _position;

        public Hero()
        {
            if (MyHero == null)
                MyHero = this;

            _myInventory = new Inventory(this);
            _mainTexture = PlayScreen.CharacterTexture;
            _hitRect = new Rectangle(0, 0, 28, 25);
            _lootRect = new Rectangle(0, 0, 100, 100);
            _attackRect = new Rectangle(0, 0, 25, 25);
            _spriteBatch = PlayScreen.Spritebatch;
            _woodChop = new DecorationSound("wood", 3);
            _walkDown = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "down-walk", 1,
                                          new[] {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)});
            _walkUp = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "up-walk", 1,
                                        new[] {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)});
            _walkLeft = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "left-walk", 1,
                                          new[] {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)});
            _walkRight = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-walk", 1,
                                           new[] {new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0)});


            //_runDown = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "down-run", new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_runLeft = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "left-run", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_runRight = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-run", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_runUp = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-run", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });

            //_sickleDown = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "down-sickle", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_sickleUp = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "up-sickle", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_sickleLeft = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "left-sickle", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_sickleRight = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-sickle", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });

            //_hammerDown = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "down-hammer", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_hammerUp = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "up-hammer", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_hammerLeft = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "left-hammer", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_hammerRight = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-hammer", 4, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });

            _axeDown = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "down-axe", 0,
                                         new[]
                                             {
                                                 new Vector2(0, -18), new Vector2(0, -12), new Vector2(-4, 6),
                                                 new Vector2(0, 6)
                                             });
            _axeUp = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "up-axe", 0,
                                       new[]
                                           {
                                               new Vector2(-2, -16), new Vector2(-2, -34), new Vector2(-2, 2),
                                               new Vector2(-2, 2)
                                           });
            _axeLeft = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "left-axe", 0,
                                         new[]
                                             {
                                                 new Vector2(-8, -18), new Vector2(-40, -18), new Vector2(-38, 2),
                                                 new Vector2(-38, 2)
                                             });
            _axeRight = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-axe", 0,
                                          new[]
                                              {
                                                  new Vector2(-7, -19), new Vector2(0, -19), new Vector2(0, 1),
                                                  new Vector2(0, 1)
                                              });

            //_waterDown = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-water", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_waterUp = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-water", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_waterLeft = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-water", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            //_waterRight = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "right-water", 2, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });


            // _dead = new CharacterAnim(PlayScreen.CharacterAtlas, "hero", "dead", 5, new[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) });
            _myEquipped = EquippedAxe;

            _lastAnimation = _walkDown;

            _walkDown.IsWalk = _walkLeft.IsWalk = _walkRight.IsWalk = _walkUp.IsWalk = true;
        }

        public static Hero MyHero { get; private set; }

        public Rectangle HitRect { get { return _hitRect; } }
        public Rectangle AttackRect { get { return _attackRect; } }

        public Inventory MyInventory { get { return _myInventory; } }

        public TileCollection MyTile { get { return _myTile; } }

        public void LoadData()
        {
            _position = Camera2D.MyCam.Position + _centerPosition;

            _hitRect.X = (int) _position.X + 0;
            _hitRect.Y = (int) _position.Y + 25;

            for (var x = 0; x < TileManager.TileColls.GetLength(0); x++)
            {
                for (var y = 0; y < TileManager.TileColls.GetLength(1); y++)
                {
                    if (TileManager.TileColls[x, y].HitRect.Intersects(HitRect))
                    {
                        _myTile = TileManager.TileColls[x, y];
                        break;
                    }
                }
                if (MyTile != null)
                    break;
            }

#if DEBUG
            MyTile.ShowHitBox = true;
#endif
        }

        public void Draw()
        {
            if (_myAction != _myLastAction)
                _lastAnimation.Restart();


            switch (_myAction)
            {
                case ActionLeftMouseButton:
                    LeftMouse();
                    break;
                case ActionRightMouseButton:
                    //Bow();
                    break;
                default:
                    Walk();
                    break;
            }


            if (_myKeyboardDirection != Stopped)
                _myLastKeyBoardDirection = _myKeyboardDirection;

            _myLastAction = _myAction;


#if DEBUG
            if (PlayScreen.HitBox)
            {
                // _spriteBatch.Draw(PlayScreen.ErrorBox, _lootRect, PlayScreen.HitBoxDebugColor);
                // _spriteBatch.Draw(PlayScreen.ErrorBox, AttackRect, MainGame.AttackDebugColor);
            }

#endif
        }

        private void Walk()
        {
            switch (_myLastKeyBoardDirection)
            {
                case Stopped:
                    _spriteBatch.Draw(_mainTexture, _position, _lastAnimation.GetFrame(_isMoving).Bounds, Color.White);
                    break;
                case MoveUp:
                    _lastAnimation = _walkUp;
                    break;
                case MoveDown:
                    _lastAnimation = _walkDown;
                    break;
                case MoveLeft:
                    _lastAnimation = _walkLeft;
                    break;
                case MoveRight:
                    _lastAnimation = _walkRight;
                    break;
            }


            var lowerBounds = _lastAnimation.GetFrame(_isMoving).Bounds;
            var lowerOffset = _lastAnimation.GetOffset();
            _spriteBatch.Draw(_mainTexture, _position + lowerOffset, lowerBounds, Color.White);
        }

        //private void Bow()
        //{
        //    if (_lastChange > 10)
        //    {
        //        _lastPosition = _lastPosition + 1 == NumBows ? NumBows - 1 : _lastPosition + 1;
        //        _lastChange = 0;
        //    }


        //    //System.Diagnostics.Debug.WriteLine("lp:{0}", _lastPosition);

        //    switch (_myLastDirection)
        //    {
        //        case MoveUp:
        //            _spriteBatch.Draw(_mainTexture, _position, _bowUp[_lastPosition].Bounds, MainGame.TimeColor);
        //            break;
        //        case MoveDown:
        //            _spriteBatch.Draw(_mainTexture, _position, _bowDown[_lastPosition].Bounds, MainGame.TimeColor);
        //            break;
        //        case MoveLeft:
        //            _spriteBatch.Draw(_mainTexture, _position, _bowLeft[_lastPosition].Bounds, MainGame.TimeColor);
        //            break;
        //        case MoveRight:
        //            _spriteBatch.Draw(_mainTexture, _position, _bowRight[_lastPosition].Bounds, MainGame.TimeColor);
        //            break;
        //    }
        //}

        private void LeftMouse()
        {
            CharacterAnim leftActionTween;

            switch (_myLastKeyBoardDirection)
            {
                case MoveUp:
                    switch (_myEquipped)
                    {
                        case EquippedAxe:
                            leftActionTween = _axeUp;
                            break;
                        case EquippedHammer:
                            leftActionTween = _hammerUp;
                            break;
                        default:
                            leftActionTween = _waterUp;
                            break;
                    }

                    _attackRect.Y = _hitRect.Y - 50;
                    break;
                case MoveDown:
                    switch (_myEquipped)
                    {
                        case EquippedAxe:
                            leftActionTween = _axeDown;
                            break;
                        case EquippedHammer:
                            leftActionTween = _hammerDown;
                            break;
                        default:
                            leftActionTween = _waterDown;
                            break;
                    }
                    _attackRect.Y = _hitRect.Y + 50;
                    break;
                case MoveLeft:
                    short damage = 2;
                    switch (_myEquipped)
                    {
                        case EquippedAxe:
                            leftActionTween = _axeLeft;
                            damage *= 5;
                            break;
                        case EquippedHammer:
                            leftActionTween = _hammerLeft;
                            break;
                        default:
                            damage = 0;
                            leftActionTween = _waterLeft;
                            break;
                    }
                    _attackRect.X = _hitRect.X - 50;

                    var value = DecorManager.DecorSprites[MyTile.LeftVectorX, MyTile.MyVectorSpotY];
                    if (value != null && value.Visible && value.MyType == DecorHelper.Tree &&
                        _attackRect.Intersects(value.HitRect))
                    {
                        value.IsHit(damage);
                        _woodChop.Play();
                    }

                    break;
                default:
                    switch (_myEquipped)
                    {
                        case EquippedAxe:
                            leftActionTween = _axeRight;
                            break;
                        case EquippedHammer:
                            leftActionTween = _hammerRight;
                            break;
                        default:
                            leftActionTween = _waterRight;
                            break;
                    }
                    _attackRect.X = _hitRect.X + 50;
                    _attackRect.X = _hitRect.X + 50;
                    break;
            }
            var bounds = leftActionTween.GetFrame(_isMoving).Bounds;
            var offset = leftActionTween.GetOffset();
            _spriteBatch.Draw(_mainTexture, _position + offset, bounds, _drawColor);

            if (leftActionTween.CurrentFrame == leftActionTween.DefaultFrame)
                _spriteBatch.Draw(_mainTexture, _position + offset, bounds, _drawColor);
        }


        public void Update(bool isMoving)
        {
            _myAction = Inputs.MyInputs.Action;
            _position = Camera2D.MyCam.Position + _centerPosition;

            _hitRect.X = (int) _position.X + 0;
            _hitRect.Y = (int) _position.Y + 25;
            _lootRect.X = _hitRect.X - 40;
            _lootRect.Y = _hitRect.Y - 40;
            _attackRect.X = _hitRect.X;
            _attackRect.Y = _hitRect.Y;
            _isMoving = isMoving;
            if (_isMoving)
                CheckMyTile();

            CheckForLoot();
            //_lastPosition = _isMoving ? _lastPosition : 0;
            _myMouseDirection = Inputs.MyInputs.MouseDirection;
            _myKeyboardDirection = Inputs.MyInputs.KeyboardDirection;
        }

        private void CheckForLoot()
        {
            CheckTileForLoot(ref TileManager.TileColls[MyTile.LeftVectorX, MyTile.MyVectorSpotY]);
            CheckTileForLoot(ref TileManager.TileColls[MyTile.RightVectorX, MyTile.MyVectorSpotY]);
            CheckTileForLoot(ref TileManager.TileColls[MyTile.MyVectorSpotX, MyTile.BottomVectorY]);
            CheckTileForLoot(ref TileManager.TileColls[MyTile.MyVectorSpotX, MyTile.TopVectorY]);

            CheckTileForLoot(ref TileManager.TileColls[MyTile.LeftVectorX, MyTile.TopVectorY]);
            CheckTileForLoot(ref TileManager.TileColls[MyTile.RightVectorX, MyTile.TopVectorY]);
            CheckTileForLoot(ref TileManager.TileColls[MyTile.LeftVectorX, MyTile.BottomVectorY]);
            CheckTileForLoot(ref TileManager.TileColls[MyTile.RightVectorX, MyTile.BottomVectorY]);
        }

        private void CheckTileForLoot(ref TileCollection tileCollection)
        {
            var len = tileCollection.ItemsOnTile.Count;
            for (var i = 0; i < len; i++)
            {
                var wi = tileCollection.ItemsOnTile[i];
                if (!wi.HitRect.Intersects(_lootRect)) continue;


                wi.Loot(this);
            }

            while (removeWorldItem.Count != 0)
            {
                var wi = removeWorldItem.ElementAt(0);
                tileCollection.RemoveWorldItem(ref wi);
                removeWorldItem.RemoveAt(0);
            }
        }

        public void CheckMyTile()
        {
            var closestX = MyTile.HitRect.Center.X - HitRect.Center.X;
            closestX = closestX < 0 ? -closestX : closestX;

            var closestY = MyTile.HitRect.Center.Y - HitRect.Center.Y;
            closestY = closestY < 0 ? -closestY : closestY;

            var smallestDifference = closestX + closestY;

            IsClosestRect(ref TileManager.TileColls[MyTile.LeftVectorX, MyTile.MyVectorSpotY], ref smallestDifference);
            IsClosestRect(ref TileManager.TileColls[MyTile.RightVectorX, MyTile.MyVectorSpotY], ref smallestDifference);
            IsClosestRect(ref TileManager.TileColls[MyTile.MyVectorSpotX, MyTile.BottomVectorY],
                          ref smallestDifference);
            IsClosestRect(ref TileManager.TileColls[MyTile.MyVectorSpotX, MyTile.TopVectorY], ref smallestDifference);

            IsClosestRect(ref TileManager.TileColls[MyTile.LeftVectorX, MyTile.TopVectorY], ref smallestDifference);
            IsClosestRect(ref TileManager.TileColls[MyTile.RightVectorX, MyTile.TopVectorY], ref smallestDifference);
            IsClosestRect(ref TileManager.TileColls[MyTile.LeftVectorX, MyTile.BottomVectorY], ref smallestDifference);
            IsClosestRect(ref TileManager.TileColls[MyTile.RightVectorX, MyTile.BottomVectorY], ref smallestDifference);
        }

        private void IsClosestRect(ref TileCollection testColl, ref int smallestDifference)
        {
            if (testColl.HitRect.Intersects(HitRect))
            {
                var newestX = testColl.HitRect.Center.X - HitRect.Center.X;
                newestX = newestX < 0 ? -newestX : newestX;

                var newestY = testColl.HitRect.Center.Y - HitRect.Center.Y;
                newestY = newestY < 0 ? -newestY : newestY;

                if (smallestDifference > newestX + newestY)
                {
                    smallestDifference = newestX + newestY;
#if DEBUG
                    MyTile.ShowHitBox = false;
#endif
                    _myTile = testColl;
#if DEBUG
                    MyTile.ShowHitBox = true;
#endif
                }
            }
        }
    }
}