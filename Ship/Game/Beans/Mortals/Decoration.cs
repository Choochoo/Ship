
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Utils;
using Ship.Game.Loaders;
using Ship.Game.WorldGeneration.WorldDrawing;
using System;
namespace Ship.Game.Beans.Mortals
{

    public class Decoration : IComparable
    {
        private int _value = 0;
        private bool _visible = false;
        private Vector2 _renderPosition;
        private readonly TextureRegion[] _tileRegions;
        private Rectangle _myBounds;

        public Decoration(ref TextureRegion[] tr) { _tileRegions = tr; }

        internal int MySectorSpotX { get; set; }

        internal int MySectorSpotY { get; set; }

        internal int SectorX { get; set; }

        internal int SectorY { get; set; }

        internal int MyVectorSpotX { get; set; }

        internal int MyVectorSpotY { get; set; }

        internal Vector2 Position { get; set; }

        public int Value { get { return _value; } 
            set { 
                _value = value;
                _visible = _value != 0;
                if (!Visible) return;

                _myBounds = _tileRegions[Value].Bounds;

                var rand = new Random(MySectorSpotX + MySectorSpotY + SectorX + SectorY);
                var valueX = (Position.X + rand.Next(16,48)) - (_myBounds.Width/2.0f);
                var valueY = Position.Y - _tileRegions[Value].Bounds.Height + rand.Next(16, 48);
                rand = null;
                 _renderPosition = new Vector2(valueX,valueY);
            } 
        }

        public bool Visible { get { return _visible; } }


        private bool _leftFine;
        private bool _rightFine;
        private bool _upFine;
        private bool _downFine;

        internal int Draw(ref SpriteBatch spriteBatch,ref Texture2D mainTextures,ref Camera2D cam)
        {
            if (!Visible)
                return 0;

            _leftFine = cam.Position.X < _renderPosition.X + _myBounds.Width;
            _rightFine = cam.Position.X + cam.ViewportWidth > _renderPosition.X;
            _upFine = cam.Position.Y < _renderPosition.Y + _myBounds.Height;
            _downFine = cam.Position.Y + cam.ViewportHeight > _renderPosition.Y;

            //var drawColor = MySectorSpotY == 0 || MySectorSpotX == 0 ? Color.Red : Color.White;
            if(_leftFine && _rightFine && _upFine && _downFine)
            {
                spriteBatch.Draw(mainTextures, _renderPosition, _myBounds, Color.White);
                return 1;
            }
            return 0;
        }

        public int CompareTo(object obj)
        {
            if (!this.Visible)
                return 1;

            var that = (Decoration) (obj);

            if (!that.Visible)
                return -1;

            if(that.Position.Y==this.Position.Y)
            {
                if (this.Position.X < that.Position.X)
                    return -1;
                else return 1;
            }
            else
            {
                if (this.Position.Y < that.Position.Y)
                    return -1;
                else return 1;
            }
        }
    }
}
