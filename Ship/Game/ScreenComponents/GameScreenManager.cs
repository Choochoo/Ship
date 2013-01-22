#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

#endregion

namespace Ship.Game.ScreenComponents
{
    public class GameScreenManager
    {
        private readonly LinkedList<GameScreen> _screens = new LinkedList<GameScreen>();
                                                // Not a 'stack', but we can use it as such

        public void Push(GameScreen screen) { _screens.AddFirst(screen); }

        public void Pop() { _screens.RemoveFirst(); }

        public void GameStart() { }

        public void Update(ref GameTime time) { _screens.First().Update(ref time); }

        public void Draw(ref GameTime time) { _screens.First().Draw(ref time); }
    }
}