#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Animations
{
    public class LootDropAnim
    {
        private const float FirstAnimationTime = 200.0f;
        private const float SecondAnimationTime = 300.0f;
        private const float ThirdAnimationTime = 900.0f;
        private const float FourthAnimationTime = 1500.0f;
        private Vector2 _position;
        private float _scale = 1f;
        private float _timeSinceAnimationStarted;

        public LootDropAnim() { _position = Vector2.Zero; }

        public float Scale { get { return _scale; } }

        public Vector2 Position { get { return _position; } }

        internal void UpdatePositions(float positionX, float positionY)
        {
            _timeSinceAnimationStarted = 0;
            _position.X = positionX;
            _position.Y = positionY;
            _scale = 1f;
        }

        internal void Update(ref Vector2 offset, ref Rectangle hitbox)
        {
            _timeSinceAnimationStarted += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
            _position = Position + offset;
            hitbox.X = (int) Position.X;
            hitbox.Y = (int) Position.Y;
            if (_timeSinceAnimationStarted <= FirstAnimationTime)
            {
                var scaleMod = ((_timeSinceAnimationStarted/FirstAnimationTime)*1f);
                _scale = .2f + scaleMod;
            }
            else if (_timeSinceAnimationStarted <= SecondAnimationTime)
                _scale = 1.2f + ((_timeSinceAnimationStarted/SecondAnimationTime)*-.2f);
            else if (_timeSinceAnimationStarted <= ThirdAnimationTime)
                _scale = 1f + ((_timeSinceAnimationStarted/ThirdAnimationTime)*-.15f);
            else if (_timeSinceAnimationStarted <= FourthAnimationTime)
                _scale = .85f + ((_timeSinceAnimationStarted/FourthAnimationTime)*.15f);
            else
                _timeSinceAnimationStarted = SecondAnimationTime;
        }
    }
}