#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

#endregion

namespace Ship.Game.ScreenComponents
{
    public abstract class GameScreen
    {
        //remove string in construc/add spritebatch per
        protected GameScreenManager GameScreenManager;
        protected GameScreen(GameScreenManager gameScreenManager) { GameScreenManager = gameScreenManager; }

        public abstract void Update(ref GameTime gameTime);
        public abstract void Draw(ref GameTime gameTime);
        public abstract void UnloadContent();
    }
}