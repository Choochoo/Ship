#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Microsoft.Xna.Framework;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Animations
{
    public class CharacterAnim
    {
        public const byte SpeedSuperFast = 1;
        public const byte SpeedFast = 2;
        public const byte SpeedNormal = 2;
        public const byte SpeedSlow = 2;
        public const byte SpeedSuperSlow = 2;
        private readonly int _defaultFrame;

        // private Texture2D _spritesheet;

        //  private TextureAtlas _textureAtlas;

        private readonly TextureRegion[] _myFrames;

        private readonly Vector2[] _myFramesOffsets;
        private int _currentFrame;
        private int _lastUpdate;
        private TextureAtlas _myAtlas;

        public CharacterAnim(TextureAtlas atlas, string prefix, string action, int defaultFrame,
                             Vector2[] myFramesOffsets, byte speed = SpeedNormal)
        {
            _myAtlas = atlas;
            //_spriteBatch = PlayScreen.Spritebatch;
            // var mainAtlas = PlayScreen.DecorationAtlas;
            AnimationPrefix = prefix;
            AnimationAction = action;
            AnimationLength = myFramesOffsets.Length;
            AnimationSpeed = speed;
            _defaultFrame = defaultFrame;
            _currentFrame = _defaultFrame;
            _myFramesOffsets = myFramesOffsets;

            _myFrames = new TextureRegion[AnimationLength];

            for (var i = 0; i < AnimationLength; i++)
                _myFrames[i] = atlas.GetRegion(string.Format("{0}-{1}{2}", prefix, action, i));
        }

        public byte AnimationSpeed { get; set; }

        public string AnimationPrefix { get; private set; }

        public string AnimationAction { get; private set; }

        public int AnimationLength { get; private set; }

        public int CurrentFrame { get { return _currentFrame; } }

        public int DefaultFrame { get { return _currentFrame; } }

        public bool IsWalk { get; set; }


        public TextureRegion GetFrame(bool isMoving)
        {
            _lastUpdate += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
            isMoving = !IsWalk || isMoving;

            if (isMoving && _lastUpdate > 100)
            {
                _currentFrame = _currentFrame + 1 >= AnimationLength ? 0 : _currentFrame + 1;
                _lastUpdate = 0;
            }
            return _myFrames[_currentFrame];
        }

        internal void Restart()
        {
            _currentFrame = 0;
            _lastUpdate = 0;
        }

        internal Vector2 GetOffset() { return _myFramesOffsets[_currentFrame]; }

        internal TextureRegion GetReverseFrame(bool isMoving, ref GameTime gameTime)
        {
            _lastUpdate += gameTime.ElapsedGameTime.Milliseconds;
            isMoving = !IsWalk || isMoving;

            if (isMoving && _lastUpdate > 111)
            {
                _currentFrame = _currentFrame - 1 > 0 ? _currentFrame - 1 : AnimationLength - 1;
                _lastUpdate = 0;
            }
            return _myFrames[_currentFrame];
        }
    }
}