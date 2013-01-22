#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.Beans.Sounds;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Animations
{
    public class DestroyTreeAnim
    {
        private const float FirstAnimationTime = 00.0f;
        private Rectangle _lootBounds;
        private BasicSound _mySound = new BasicSound("treeexplode");
        private Vector2 _position;
        private float _scale = 1f;
        private float _timeSinceAnimationStarted;

        public DestroyTreeAnim(string treefallingsound) { _position = Vector2.Zero; }
        public bool IsPlaying { get; set; }

        internal void UpdatePositions(Rectangle bounds, float positionX, float positionY)
        {
            IsPlaying = true;
            _lootBounds = bounds;
            _timeSinceAnimationStarted = 0;
            _position.X = positionX;
            _position.Y = positionY;
        }

        internal void Play()
        {
            _mySound.Play();
            _timeSinceAnimationStarted += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
            IsPlaying = _timeSinceAnimationStarted < FirstAnimationTime;
        }

        internal void Draw()
        {
            PlayScreen.Spritebatch.Draw(PlayScreen.DecorationTexture, _position, _lootBounds, Color.White, 0f,
                                        Vector2.Zero,
                                        _scale, SpriteEffects.None, 0f);
        }
    }
}