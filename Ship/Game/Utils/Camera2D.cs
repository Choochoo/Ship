using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Ship.Game.Utils
{

    public class Camera2D
    {
        public Camera2D(Viewport vp)
        {
            ViewportWidth = vp.Width;
            ViewportHeight = vp.Height;
            Position = Vector2.Zero;
            Zoom = 1f;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public float ViewportWidth { get; set; }
        public float ViewportHeight { get; set; }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(Zoom) *
                       Matrix.CreateTranslation(-Position.X, -Position.Y, 0);
            }
        }
    }
}
