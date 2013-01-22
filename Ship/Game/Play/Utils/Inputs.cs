#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Beans.Mortals.Animate.Extras;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Beans.Constants;

#endregion

namespace Ship.Game.Play.Utils
{
    public class Inputs
    {
        private static Rectangle _mousePointerRect = new Rectangle(0, 0, 10, 10);
        private bool _inventoryPressed;
        private Vector2 _movement = Vector2.Zero;

        public Inputs()
        {
            if (MyInputs == null)
                MyInputs = this;
            else return;
        }

        public static Rectangle MousePointerRect { get { return _mousePointerRect; } private set { _mousePointerRect = value; } }
        public static Inputs MyInputs { get; private set; }

        public int MovX { get; private set; }
        public int MovY { get; private set; }
        public byte MouseDirection { get; private set; }
        public byte KeyboardDirection { get; private set; }
        public bool IsReversed { get; private set; }
        public int GameAction { get; private set; }
        public byte Action { get; private set; }

        public void Update()
        {
            _movement = Vector2.Zero;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            MouseDirection = Hero.Stopped;
            KeyboardDirection = Hero.Stopped;
            Action = Hero.ActionNone;
            MovX = 0;
            MovY = 0;

            var actionPressed = false;
            IsReversed = false;
            _mousePointerRect.X = mouseState.X - 5;
            _mousePointerRect.Y = mouseState.Y - 5;
            _mousePointerRect.X += (int) Camera2D.MyCam.Position.X;
            _mousePointerRect.Y += (int) Camera2D.MyCam.Position.Y;
            if (!Inventory.MovingItem)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!Manager.MyManager.OnInterfaceRects())
                    {
                        Action = Hero.ActionLeftMouseButton;
                        actionPressed = true;
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    Action = Hero.ActionRightMouseButton;
                    actionPressed = true;
                }
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Released)
                    Manager.MyManager.ReleaseItemIntoWorld();
            }


            if ((mouseState.X > 0 && mouseState.X <= Camera2D.MyCam.ViewportWidth) &&
                (mouseState.Y > 0 && mouseState.Y <= Camera2D.MyCam.ViewportHeight))
            {
                var centerX = mouseState.X - MainGame.WindowWidthCenter;
                var centerY = mouseState.Y - MainGame.WindowHeightCenter;
                var absX = centerX < 0 ? -centerX : centerX;
                var absY = centerY < 0 ? -centerY : centerY;
                if (absX > absY)
                    MouseDirection = centerX < 0 ? Hero.MoveLeft : Hero.MoveRight;
                else
                    MouseDirection = centerY < 0 ? Hero.MoveUp : Hero.MoveDown;
            }


            if (keyboardState.IsKeyDown(Keys.A))
            {
                MovX--;
                KeyboardDirection = Hero.MoveLeft;
                IsReversed = MouseDirection == Hero.MoveRight;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                MovX++;
                KeyboardDirection = Hero.MoveRight;
                IsReversed = MouseDirection == Hero.MoveLeft;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                MovY--;
                KeyboardDirection = Hero.MoveUp;
                IsReversed = MouseDirection == Hero.MoveDown;
            }
            else if (keyboardState.IsKeyDown(Keys.S))
            {
                MovY++;
                KeyboardDirection = Hero.MoveDown;
                IsReversed = MouseDirection == Hero.MoveUp;
            }

            GameAction = 0;
            //GameActions
            if (!_inventoryPressed && keyboardState.IsKeyDown(Keys.I))
            {
                _inventoryPressed = true;
                GameAction = ActionConstants.ToggleInventory;
            }
            else if (_inventoryPressed && keyboardState.IsKeyUp(Keys.I))
                _inventoryPressed = false;


            //_server.SendMovement(movX,movY);
            //_server.Update();

            //_cam.Position += _movement * 4;
            MovX = actionPressed ? 0 : MovX;
            MovY = actionPressed ? 0 : MovY;
        }
    }
}