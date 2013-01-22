#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Welcome;

#endregion

namespace Ship.Game.ScreenComponents.Screens
{
    public class WelcomeScreen : GameScreen
    {
        private const int XLoc = 270;
        private const int StartY = 150;
        private const int YDiff = 70;
        private readonly Rectangle _buttonExitPosition = new Rectangle(XLoc, StartY + (YDiff * 4),0,0);
        private readonly Rectangle _buttonLoadGamePosition = new Rectangle(XLoc, StartY + (YDiff * 2), 0, 0);
        private readonly Rectangle _buttonNewGamePosition = new Rectangle(XLoc, StartY + YDiff, 0, 0);
        private readonly Rectangle _buttonSettingsPosition = new Rectangle(XLoc, StartY + (YDiff * 3), 0, 0);

        private readonly TextureAtlas _interfaceAtlas;
        private readonly Texture2D _interfaceTexture;

        private MainInput _inputs;
        private SpriteBatch _spriteBatch;


        public WelcomeScreen(GameScreenManager gameScreenManager, Texture2D interfaceTexture,
                             TextureAtlas interfaceAtlas)
            : base(gameScreenManager)
        {
            _interfaceTexture = interfaceTexture;
            _interfaceAtlas = interfaceAtlas;
            _buttonExitPosition.Width =
                _buttonLoadGamePosition.Width =
                _buttonNewGamePosition.Width =
                _buttonSettingsPosition.Width = _interfaceAtlas.GetRegion("NewGame").Bounds.Width;

            _buttonExitPosition.Height =
                _buttonLoadGamePosition.Height =
                _buttonNewGamePosition.Height =
                _buttonSettingsPosition.Height = _interfaceAtlas.GetRegion("NewGame").Bounds.Height;
            _inputs = new MainInput();
            _spriteBatch = new SpriteBatch(MainGame.GraphicD);
        }

        private bool _collideNewGame = false;
        private bool _collideLoadGame = false;
        private bool _collideSettings = false;
        private bool _collideExit = false;

        public override void Update(ref GameTime gameTime)
        {
            _inputs.Update();
            _collideNewGame = MainInput.MousePointerRect.Intersects(_buttonNewGamePosition);
            _collideLoadGame = MainInput.MousePointerRect.Intersects(_buttonLoadGamePosition);
            _collideSettings = MainInput.MousePointerRect.Intersects(_buttonSettingsPosition);
            _collideExit = MainInput.MousePointerRect.Intersects(_buttonExitPosition);
        }

        public override void Draw(ref GameTime gameTime)
        {
            MainGame.GraphicD.Clear(Color.Blue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_interfaceTexture, _buttonNewGamePosition, _interfaceAtlas.GetRegion(string.Format("NewGame{0}",_collideNewGame ? "Over":"")).Bounds,
                              Color.White);
            _spriteBatch.Draw(_interfaceTexture, _buttonLoadGamePosition, _interfaceAtlas.GetRegion(string.Format("LoadGame{0}", _collideLoadGame ? "Over" : "")).Bounds,
                              Color.White);
            _spriteBatch.Draw(_interfaceTexture, _buttonSettingsPosition, _interfaceAtlas.GetRegion(string.Format("Settings{0}", _collideSettings ? "Over" : "")).Bounds,
                              Color.White);
            _spriteBatch.Draw(_interfaceTexture, _buttonExitPosition, _interfaceAtlas.GetRegion(string.Format("Exit{0}", _collideExit ? "Over" : "")).Bounds,
                              Color.White);
            _spriteBatch.End();
        }

        public override void UnloadContent()
        {
            _spriteBatch.Dispose();
            _spriteBatch = null;
        }
    }
}