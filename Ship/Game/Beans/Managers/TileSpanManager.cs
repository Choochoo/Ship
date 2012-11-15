using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdPartyNinjas.XnaUtility;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;
using Microsoft.Xna.Framework;

namespace Ship.Game.Beans.Managers
{
    public class TileSpanManager
    {
        private readonly Texture2D _grassTexture;
        //private Texture2D _waterTexture;
        private readonly int _maxWidth;
        private readonly int _maxHeight;
        private readonly Rectangle _rect;
        public TileSpanManager(Texture2D grass) 
        { 
            _grassTexture = grass;
            _maxWidth = MainGame.WindowWidth + 64;
            _maxHeight = MainGame.WindowHeight + 64;
            _rect = new Rectangle(0,0,_maxWidth,_maxHeight);
        }

        public void Draw(ref SpriteBatch batch, ref Camera2D cam)
        {
            var xVal = cam.Position.X - (cam.Position.X%32) - 32;
            var yVal = cam.Position.Y - (cam.Position.Y % 32) - 32;

            batch.Draw(_grassTexture,new Vector2(xVal,yVal),_rect,Color.White );
        }
    }
}
