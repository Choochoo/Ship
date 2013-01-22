#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Utils;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Beans.Mortals;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Beans.Mortals.Inanimate;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class CharacterManager
    {
        private const byte HeroSpeed = 4;
        private readonly Hero _myHero;

        public CharacterManager() { _myHero = new Hero(); }

        public Hero MyHero { get { return _myHero; } }

        public void Update(Decoration[] decorPool)
        {
            Decoration hitX = null;
            Decoration hitY = null;
            //var speed = isReversed ? HeroSpeed/2 : HeroSpeed;
            var movX = Inputs.MyInputs.MovX;
            var movY = Inputs.MyInputs.MovY;
            movX *= HeroSpeed;
            movY *= HeroSpeed;

            for (var i = 0; i < decorPool.Length; i++)
            {
                if (!decorPool[i].Visible)
                    break;

                if (movX != 0 && hitX == null)
                {
                    var rect = MyHero.HitRect;
                    rect.X += movX;
                    if (decorPool[i].HitRect.Intersects(rect))
                        hitX = decorPool[i];
                }
                if (movY != 0 && hitY == null)
                {
                    var rect = MyHero.HitRect;
                    rect.Y += movY;
                    if (decorPool[i].HitRect.Intersects(rect))
                        hitY = decorPool[i];
                }
                if (hitX != null && hitY != null)
                    break;
            }

            var campos = Camera2D.MyCam.Position;
            if (movX != 0)
            {
                if (hitX == null)
                    campos.X += movX;
                else
                {
                    campos.X += movX > 0
                                    ? (MyHero.HitRect.X - (hitX.HitRect.X - MyHero.HitRect.Width))
                                    : ((hitX.HitRect.X + hitX.HitRect.Width) - MyHero.HitRect.X);
                }
            }

            if (movY != 0)
            {
                if (hitY == null)
                    campos.Y += movY;
                else
                {
                    campos.Y += movY > 0
                                    ? (MyHero.HitRect.Y - (hitY.HitRect.Y - MyHero.HitRect.Height))
                                    : ((hitY.HitRect.Y + hitY.HitRect.Height) - MyHero.HitRect.Y);
                }
            }

            var isMoving = campos != Camera2D.MyCam.Position;
            Camera2D.MyCam.Position = campos;
            MyHero.Update(isMoving);
        }

        public void Draw() { MyHero.Draw(); }

        internal void LoadData() { _myHero.LoadData(); }

        internal void DrawInventory() { MyHero.MyInventory.Draw(); }
    }
}