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

namespace Ship.Game.Welcome
{
    public class MainInput
    {
        private static Rectangle _mousePointerRect = new Rectangle(0, 0, 10, 10);

        public MainInput()
        {
            if (MyInputs == null)
                MyInputs = this;
            else return;
        }

        public static Rectangle MousePointerRect { get { return _mousePointerRect; } private set { _mousePointerRect = value; } }
        public static MainInput MyInputs { get; private set; }

        public void Update()
        {
            var mouseState = Mouse.GetState();
            _mousePointerRect.X = mouseState.X - 5;
            _mousePointerRect.Y = mouseState.Y - 5;
        }
    }
}