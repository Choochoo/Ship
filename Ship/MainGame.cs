using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System;
using Microsoft.Xna.Framework.Content;
using Ship.Game.ScreenComponents;
using Ship.Game.ScreenComponents.Screens;
using ThirdPartyNinjas.XnaUtility;

namespace Ship
{
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        #region Fields
        private readonly GraphicsDeviceManager _graphics;
        private GameScreenManager _screenManager ;
        #endregion

        #region Initialization
        public MainGame()
        {
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            _graphics = new GraphicsDeviceManager(this);
            
            //_graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            WindowWidth = 800;
            WindowHeight = 600;
            WindowWidthCenter = WindowWidth / 2;
            WindowHeightCenter = WindowHeight / 2;
            MapWidth = 300;
            MapHeight = 300;
            GraphicD = GraphicsDevice;
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            ContentLoader = Content;
            base.Initialize();
        }
#endregion

        #region Load/Unload
        protected override void LoadContent()
        {
            var interfaceTexture = MainGame.ContentLoader.Load<Texture2D>("Screens/Shared/interface");
            var interfaceAtlas = MainGame.ContentLoader.Load<TextureAtlas>("Screens/Shared/interfaceJSON");

           
            _screenManager = new GameScreenManager();
            //_screenManager.Push(new WelcomeScreen(_screenManager, interfaceTexture, interfaceAtlas));
            _screenManager.Push(new PlayScreen(_screenManager, interfaceTexture, interfaceAtlas));

            if (!GamerServicesDispatcher.IsInitialized)
                GamerServicesDispatcher.Initialize(Services);

            if (!Guide.IsVisible)
                StorageDevice.BeginShowSelector(GetStorageDeviceResult, "getResult");

            FpsFont = ContentLoader.Load<SpriteFont>("Fonts/FPSFont");

            GC.Collect();
        }

        private void GetStorageDeviceResult(IAsyncResult result)
        {
            if (!Guide.IsVisible)
                StorageD = StorageDevice.EndShowSelector(result);
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Update and Draw
        protected override void Update(GameTime gameTime)
        {
            _screenManager.Update(ref gameTime);
            GamerServicesDispatcher.Update();
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            _screenManager.Draw(ref gameTime);
            base.Draw(gameTime);
        }

        #endregion

        #region Static Fields

        public static int MapWidth { get; private set; }
        public static int MapHeight { get; private set; }

        public static int WindowWidth { get; private set; }
        public static int WindowHeight { get; private set; }

        public static int WindowWidthCenter { get; private set; }
        public static int WindowHeightCenter { get; private set; }

        public static StorageDevice StorageD { get; private set; }
        public static SpriteFont FpsFont { get; private set; }
        public static GraphicsDevice GraphicD { get; private set; }
        public static ContentManager ContentLoader { get; private set; }

#endregion
    }
}