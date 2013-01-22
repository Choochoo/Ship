
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Utils;
using Ship.Game.Loaders;
using Ship.Game.WorldGeneration.WorldDrawing;
using System;
namespace Ship.Game.Beans.Mortals
{

    public class DecorationFire :Decoration
    {
        private Rectangle _firePit;
        private Rectangle _fire;
        private Rectangle _smoke;
        private Texture2D _myTexture;

        public DecorationFire(ref Texture2D myTexture, Rectangle firePit, Rectangle fire, Rectangle smoke)
        {
            _myTexture = myTexture;
            _fire = fire;
            _firePit = firePit;
            _smoke = smoke;
        }

        public override int Draw(ref SpriteBatch spriteBatch,ref Texture2D mainTextures,ref Camera2D cam)
        {
            if (!Visible)
                return 0;

            LeftFine = cam.Position.X < RenderPosition.X + MyBounds.Width;
            RightFine = cam.Position.X + cam.ViewportWidth > RenderPosition.X;
            UpFine = cam.Position.Y < RenderPosition.Y + MyBounds.Height;
            DownFine = cam.Position.Y + cam.ViewportHeight > RenderPosition.Y;

            //var drawColor = MySectorSpotY == 0 || MySectorSpotX == 0 ? Color.Red : Color.White;
            if(LeftFine && RightFine && UpFine && DownFine)
            {
                spriteBatch.Draw(mainTextures, RenderPosition, MyBounds, Color.White);
                return 1;
            }
            return 0;
        }

       
    }
}
