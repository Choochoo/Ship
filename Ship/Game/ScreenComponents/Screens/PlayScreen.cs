#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ship.Game.Play.Beans.Tiles;
using Ship.Game.Play.Loaders;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.WorldGeneration;
using Ship.Game.Play.Utils;
using Ship.Game.Play.WorldGeneration.WorldDrawing;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System;
using Ship.Game;
using Ship.Game.Play.WorldGeneration.Noise;
using Ship.Game.Play.Beans.Managers;
using System.Linq;
using Lidgren.Network;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Ship.Game.Play.Beans.Items.Keys;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Beans.Mortals.Animate.Extras;
using Ship.Game.Play;

#endregion

namespace Ship.Game.ScreenComponents.Screens
{
    public class PlayScreen : GameScreen
    {
        #region Fields

        public const byte SectorTileSize = 64;
        public const byte MiniTileSize = 30;
        private Vector2 _movement = Vector2.Zero;
        private Camera2D _cam;


        //private TextureAtlas _mainAtlas;
        //private Texture2D _mainTexture;
        private Manager _managers;
        private Inputs _myInputs;

        //DEBUG SECTION!!
#if DEBUG
        public static bool HitBox = true;
        private CreateWorld _cw;
        private TestBox _tb;
        private const bool ShowFps = false;
        private const bool ShowLocation = false;
        private const bool ShowMap = false;
        public static readonly Color HitboxDebugColor = new Color(255, 0, 0, 170);
        public static readonly Color AttackDebugColor = new Color(0, 0, 255, 170);
#endif

        #endregion

        #region Initialization

        public PlayScreen(GameScreenManager gameScreenManager, Texture2D interfaceTexture, TextureAtlas interfaceAtlas)
            : base(gameScreenManager)
        {
            SpawnX = 214;
            SpawnY = 250;
            _cam = new Camera2D(MainGame.GraphicD.Viewport);
            Seeds.CreateNewSeeds();
            Spritebatch = new SpriteBatch(MainGame.GraphicD);

            DecorationTexture = MainGame.ContentLoader.Load<Texture2D>("Screens/Play/Sheets/decorations");
            DecorationAtlas = MainGame.ContentLoader.Load<TextureAtlas>("Screens/Play/Sheets/decorationsJSON");

            CharacterTexture = MainGame.ContentLoader.Load<Texture2D>("Screens/Play/Sheets/characters");
            CharacterAtlas = MainGame.ContentLoader.Load<TextureAtlas>("Screens/Play/Sheets/charactersJSON");

            InterfaceTexture = interfaceTexture;
            InterfaceAtlas = interfaceAtlas;

            var worlditems = new TextureRegion[WorldItemKey.AllItems.Count];
            for (var i = 0; i < WorldItemKey.AllItems.Count; i++)
                worlditems[i] = DecorationAtlas.GetRegion(WorldItemKey.AllItems[i]);
            WorldItemKey.ItemRegions = worlditems;

#if DEBUG
            if (ShowMap)
            {
                WorldData.MyWorldData = new WorldData();
                _cw = new CreateWorld(true, false);
                //_tb = new TestBox();
            }
            else
            {
                var az = Enumerable.Range(0, 64*64).Select(i => Color.Red).ToArray();
                ErrorBox = new Texture2D(MainGame.GraphicD, 64, 64, false, SurfaceFormat.Color);
                ErrorBox.SetData(az);
#endif
                // Create a new SpriteBatch, which can be used to draw textures.

                // showMap = new CreateWorld(GraphicsDevice);
                if (WorldData.MyWorldData == null)
                {
                    //look for file
                    //Utility.RemoveSave();
                    WorldData.MyWorldData = Utility.LoadGameData();
                    if (WorldData.MyWorldData == null)
                    {
                        WorldData.MyWorldData = new WorldData();
                        _cw = new CreateWorld(false, true);
                    }
                }
                _managers = new Manager();
                _myInputs = new Inputs();
#if DEBUG
            }
#endif

            GC.Collect();
        }

        #endregion

        #region Unload

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        public override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #endregion

        #region Update and Draw

        public static GameTime OfficialGametime { get; private set; }

        public override void Update(ref GameTime gameTime)
        {
            OfficialGametime = gameTime;
#if DEBUG
            if (!ShowMap && _tb == null)
            {
                // Allows the game to exit
#endif
                _myInputs.Update();
                _managers.Update();
#if DEBUG
            }

#endif
        }


        public override void Draw(ref GameTime gameTime)
        {
            OfficialGametime = gameTime;


#if DEBUG
            if (ShowMap)
            {
                Spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp,null,null);
                _cw.Draw();
            }
            else
            if (_tb != null)
            {
                Spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, null,
                                  null, null, _cam.TransformMatrix);
                _tb.Draw(Spritebatch);
            }
            else
            {
#endif
                _managers.CheckForBaked();

                MainGame.GraphicD.Clear(Color.Green);
                Spritebatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, null,
                                  null, null, _cam.TransformMatrix);

                _managers.Draw();
#if DEBUG

                if (HitBox)
                    Spritebatch.Draw(ErrorBox, Inputs.MousePointerRect, HitboxDebugColor);


                //_spriteBatch.DrawString(_fpsFont, String.Format("Draws:{0}", drawCalls), new Vector2(_cam.Position.X, _cam.Position.Y + 160), Color.Red);
            }
#endif

            Spritebatch.End();
        }

        #endregion

        #region Static Fields

#if DEBUG
        public static Texture2D ErrorBox { get; private set; }
#endif

        public static int SpawnX { get; set; }
        public static int SpawnY { get; set; }


        public static Texture2D DecorationTexture { get; private set; }
        public static TextureAtlas DecorationAtlas { get; private set; }

        public static Texture2D CharacterTexture { get; private set; }
        public static TextureAtlas CharacterAtlas { get; private set; }

        public static Texture2D InterfaceTexture { get; private set; }
        public static TextureAtlas InterfaceAtlas { get; private set; }
        public static SpriteBatch Spritebatch { get; private set; }

        #endregion
    }
}