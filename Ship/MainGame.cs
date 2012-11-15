using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ship.Game.Beans.Tiles;
using Ship.Game.Loaders;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.WorldGeneration;
using Ship.Game.Utils;
using Ship.Game.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System;
using Ship.Game;
using Ship.Game.WorldGeneration.Noise;
using Ship.Game.Beans.Managers;

namespace Ship
{
    /// <summary>
    ///   This is the main type for your game
    /// </summary>

    public class MainGame : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        public const byte SectorTileSize = 64;
        public const byte MiniTileSize = 30;
        private Vector2 _movement = Vector2.Zero;
        private Camera2D _cam;
        private SpriteBatch _spriteBatch;

        private TextureAtlas _mainAtlas;
        private Texture2D _mainTexture;
        private CreateWorld _cw;
        private static WorldData _worldData;
        private Managers _managers;
        

        //DEBUG SECTION!!
        #if DEBUG
        private SpriteFont _fpsFont;
        private TestBox tb;
        private const bool showFPS = true;
        private const bool showLocation = true;
        private int FPS = 0;
        private int frameCounter = 0;
        private float elapseTime = 0;
        #endif

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Components.Add(new GamerServicesComponent(this));
            this.IsFixedTimeStep = false;
            //_graphics.SynchronizeWithVerticalRetrace = false;
        }

        /// <summary>
        ///   Allows the game to perform any initialization it needs to before starting to run. This is where it can query for any required services and load any non-graphic related content. Calling base.Initialize will enumerate through any components and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            SetupSize();

            Seeds.CreateNewSeeds();
            //GamerServicesDispatcher.WindowHandle = Window.Handle;
          //  System.Windows.Forms.Form gameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
           // gameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            base.Initialize();
        }

        private void GetStorageDeviceResult(IAsyncResult result)
        {
            if (!Guide.IsVisible)
            {
                StorageD = StorageDevice.EndShowSelector(result);
            }
        }

        private void SetupSize()
        {
            WindowWidth = 800;
            WindowHeight = 600;
            MapWidth = 800;
            MapHeight = 600;
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            _cam = new Camera2D(GraphicsDevice.Viewport);
        }

        /// <summary>
        ///   LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            if (!GamerServicesDispatcher.IsInitialized)
                GamerServicesDispatcher.Initialize(Services);

            if (!Guide.IsVisible)
                StorageDevice.BeginShowSelector(GetStorageDeviceResult, (object)"getResult");


            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mainTexture = Content.Load<Texture2D>("decorations");
            _mainAtlas = Content.Load<TextureAtlas>("decorationsJSON");

#if DEBUG
            if (false)
            {
                _worldData = new WorldData();
                _cw = new CreateWorld(GraphicsDevice, ref _worldData, false, false);
                tb = new TestBox(_mainTexture, _mainAtlas, GraphicsDevice, Content.Load<Texture2D>("grass"));
            }
            else
            {

                _fpsFont = Content.Load<SpriteFont>("FPSFont");
            #endif
                // Create a new SpriteBatch, which can be used to draw textures.
                
                // showMap = new CreateWorld(GraphicsDevice);
                if (WorldData == null)
                {
                    //look for file
                    //Utility.RemoveSave();
                    //_worldData = Utility.LoadGameData();
                    if (MainGame.WorldData == null)
                    {
                        _worldData = new WorldData();
                        _cw = new CreateWorld(GraphicsDevice, ref _worldData, false, false);
                    }
                }
                _managers = new Managers(ref _mainTexture, ref _mainAtlas, Content.Load<Texture2D>("grass"));
#if DEBUG
            }
#endif

            GC.Collect();
        }

        /// <summary>
        ///   UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        ///   Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Update(GameTime gameTime)
        {
#if DEBUG
            if (tb == null)
            {
                // Allows the game to exit
#endif
                _movement = Vector2.Zero;
                var keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Left))
                    _movement.X--;
                if (keyboardState.IsKeyDown(Keys.Right))
                    _movement.X++;
                if (keyboardState.IsKeyDown(Keys.Up))
                    _movement.Y--;
                if (keyboardState.IsKeyDown(Keys.Down))
                    _movement.Y++;
                _cam.Position += _movement * 10;

                _managers.Update(gameTime,_cam);
            #if DEBUG
            }

           
            
            if(showFPS)
                UpdateFps(gameTime);
#endif

                GamerServicesDispatcher.Update();
            base.Update(gameTime);
        }

#if DEBUG
        private void UpdateFps(GameTime gameTime)
        {
            elapseTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            frameCounter++;

            if (elapseTime > 1)
            {
                FPS = frameCounter;
                frameCounter = 0;
                elapseTime = 0;
            }
        }
#endif

        /// <summary>
        ///   This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, _cam.TransformMatrix);

#if DEBUG
            if(tb != null)
            {
                tb.Draw(_spriteBatch);
            }
          // else if (_cw != null)
            //    _cw.Draw(_spriteBatch, gameTime);
            else
            {
                #endif
                var drawCalls = _managers.Draw(ref _spriteBatch, ref _cam);

#if DEBUG
                if (showFPS)
                    _spriteBatch.DrawString(_fpsFont, "FPS " + (1000/gameTime.ElapsedGameTime.TotalMilliseconds).ToString(), new Vector2(_cam.Position.X, _cam.Position.Y + 100), Color.Red);

                if(showLocation)
                    _spriteBatch.DrawString(_fpsFont, String.Format("X:{0} Y:{1}",Managers.SpawnX,Managers.SpawnY), new Vector2(_cam.Position.X, _cam.Position.Y+130), Color.Red);

                _spriteBatch.DrawString(_fpsFont, String.Format("Draws:{0}", drawCalls), new Vector2(_cam.Position.X, _cam.Position.Y + 160), Color.Red);
                
            }
            #endif

            _spriteBatch.End();

            base.Draw(gameTime);
        
        }

        public static WorldData WorldData { get { return _worldData; } }

        public static int MapWidth { get; private set; }
        public static int MapHeight { get; private set; }

        public static int WindowWidth { get; private set; }
        public static int WindowHeight { get; private set; }

        public static StorageDevice StorageD { get; private set; }
    }
}
