#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Utils;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class MessageManager
    {
        private const int MaxNumberOfMessages = 4;


        private readonly SpriteFont _messageFont;
        private readonly SpriteBatch _spriteBatch;
        private Message[] _messages = new Message[MaxNumberOfMessages];
        private List<Message> _messagesToShow = new List<Message>();
        private int _nextFreeLocation;
        private int _previousFreeLocation = MaxNumberOfMessages - 1;

        public MessageManager()
        {
            if (Self != null)
                return;

            Self = this;

            _spriteBatch = PlayScreen.Spritebatch;

            _messageFont = MainGame.FpsFont;
            for (var i = 0; i < MaxNumberOfMessages; i++)
                _messages[i] = new Message();
        }

        public static MessageManager Self { get; private set; }

        public void AddMessage(string message)
        {
            _messages[_nextFreeLocation].SetData(message);
            if (_messages[_previousFreeLocation].IsActive)
                _previousFreeLocation = _nextFreeLocation;
            _nextFreeLocation = _nextFreeLocation + 1 == MaxNumberOfMessages ? 0 : _nextFreeLocation + 1;
        }

        public void Draw()
        {
            for (var i = 0; i < MaxNumberOfMessages; i++)
            {
                if (!_messages[i].IsActive) continue;
                var currMessage = _messages[i];
                currMessage.AddTime(PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds);
                _spriteBatch.DrawString(_messageFont, currMessage.Text, Camera2D.MyCam.Position + currMessage.Position,
                                        currMessage.MessageColor);
            }
        }
    }
}