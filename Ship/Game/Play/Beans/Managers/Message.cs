#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Utils;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class Message
    {
        private const int MessageLocX = 250;
        private const float StartFade = 5000.0f;
        private const float FadeTime = 1000.0f;
        private readonly int _messageLocY;
        private bool _isActive;
        private Color _messageColor = Color.Blue;
        private Vector2 _position;
        private string _text;
        private int _timeExisted;
        public Message() { _messageLocY = (int) (Camera2D.MyCam.ViewportHeight - 50); }

        public Vector2 Position { get { return _position; } }

        public string Text { get { return _text; } }

        public bool IsActive { get { return _isActive; } }

        public Color MessageColor { get { return _messageColor; } set { _messageColor = value; } }

        public void SetData(string message)
        {
            _isActive = true;
            _text = message;
            _position.X = MessageLocX;
            _position.Y = _messageLocY;
            _timeExisted = 0;
        }

        internal void AddTime(int addonTime)
        {
            _timeExisted += addonTime;

            if (!(_timeExisted > StartFade)) return;

            var value = _timeExisted - StartFade;
            if (value > FadeTime)
            {
                _messageColor.A = 0;
                _isActive = false;
            }
            else
            {
                value /= FadeTime;
                var alpha = (byte) (byte.MaxValue - (byte.MaxValue*value));
                _messageColor.A = alpha;
            }
        }
    }
}