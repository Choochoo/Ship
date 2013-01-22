
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Utils;
using Ship.Game.Loaders;
using Ship.Game.WorldGeneration.WorldDrawing;
using System;
namespace Ship.Game.Beans.Mortals
{

    public class DecorationFoliage :Decoration,IDecoration
    {

        protected Random Rand;

        public DecorationFoliage(ref TextureRegion[] tr) { TileRegions = tr; }

        public override int Value { get { return _value; } 
            set
            {
                base.Value = value;

                Rand = new Random(MySectorSpotX + MySectorSpotY + SectorX + SectorY);

                var valueX = (Position.X + Rand.Next(16, 48)) - (MyBounds.Width / 2.0f);
                var valueY = Position.Y - TileRegions[Value].Bounds.Height + Rand.Next(16, 48);
                RenderPosition = new Vector2(valueX,valueY);
            } 
        }

        public override int Draw(ref SpriteBatch spriteBatch,ref Texture2D mainTextures,ref Camera2D cam)
        {
            base.Draw(ref spriteBatch, ref mainTextures, ref cam);
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
