
using Microsoft.Xna.Framework;
using ThirdPartyNinjas.XnaUtility;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Utils;
using Microsoft.Xna.Framework.Input;
using System;
using Ship.Game.Beans.Events;
using Ship.Game.WorldGeneration;

namespace Ship.Game.Beans.Managers
{
    public class Managers
    {
        private static int _spawnX = 215;
        private static int _spawnY = 450;

        private readonly Vector2 _miniMapLoc = new Vector2(10, 10);

        private readonly TileSpanManager _tileSpanManager;
        private readonly TileManager _tileManager;
        private readonly DecorManager _decorManager;
        private readonly LoaderBaseManager _loaderBaseManager;
        private readonly MiniTileManager _miniTileManager;
        
        public Managers(ref Texture2D mainTexture,ref  TextureAtlas mainAtlas,Texture2D grassText)
        {
            //!!watch order!!
            //_tileSpanManager = new TileSpanManager(mainTexture, mainAtlas);
            _tileSpanManager = new TileSpanManager(grassText);
            _tileManager = new TileManager(ref mainTexture, ref mainAtlas);
            _decorManager = new DecorManager(ref mainTexture, ref mainAtlas);
            _miniTileManager = new MiniTileManager(ref mainTexture, ref mainAtlas, _spawnX, _spawnY, 3);
            _loaderBaseManager = new LoaderBaseManager();

            //load data
            _tileManager.LoadData(ref _loaderBaseManager.Loaders);
            _decorManager.LoadData(_tileManager.TileColls);

            //events
            _tileManager.OnMoveTile += new TileManager.MoveUpdateHandler(MoveDirection);
            _tileManager.OnMoveLoader += new TileManager.MoveUpdateHandler(MoveLoaderBase);
        }

        public static int SpawnX { get { return _spawnX; } }

        public static int SpawnY { get { return _spawnY; } }

        private void MoveLoaderBase(object sender, MoveArgs e)
        {
            switch (e.Direction)
            {
                case MoveArgs.Up:
                    _loaderBaseManager.RotateTilesUp();
                    break;
                case MoveArgs.Down:
                    _loaderBaseManager.RotateTilesDown();
                    break;
                case MoveArgs.Left:
                    _loaderBaseManager.RotateTilesLeft();
                    break;
                case MoveArgs.Right:
                    _loaderBaseManager.RotateTilesRight();
                    break;
            }
            _spawnX = _loaderBaseManager.Loaders[1][1].SectorX;
            _spawnY = _loaderBaseManager.Loaders[1][1].SectorY;
            switch (e.Direction)
            {
                case MoveArgs.Up:
                    _miniTileManager.Shift(MiniTileManager.RotateUp, SpawnX, SpawnY);
                    break;
                case MoveArgs.Down:
                    _miniTileManager.Shift(MiniTileManager.RotateDown, SpawnX, SpawnY);
                    break;
                case MoveArgs.Left:
                    _miniTileManager.Shift(MiniTileManager.RotateLeft, SpawnX, SpawnY);
                    break;
                case MoveArgs.Right:
                    _miniTileManager.Shift(MiniTileManager.RotateRight, SpawnX, SpawnY);
                    break;
            }
            CreateWorldTerrain.UpdateSpace(_spawnX,_spawnY);
        }

        private void MoveDirection(object sender, MoveArgs moveTileArgs)
        {
            switch (moveTileArgs.Direction)
            {
                case MoveArgs.Up:
                    _tileManager.MoveTerrainUp();
                    _decorManager.MoveUp(_tileManager.MostUpSprite, _tileManager.MostDownSprite);
                    break;
                case MoveArgs.Down:
                    _tileManager.MoveTerrainDown();
                    _decorManager.MoveDown(_tileManager.MostUpSprite,_tileManager.MostDownSprite);
                    break;
                case MoveArgs.Left:
                    _tileManager.MoveTerrainLeft();
                    _decorManager.MoveLeft(_tileManager.MostLeftSprite,_tileManager.MostRightSprite);
                    break;
                case MoveArgs.Right:
                    _tileManager.MoveTerrainRight();
                    _decorManager.MoveRight(_tileManager.MostLeftSprite, _tileManager.MostRightSprite);
                    break;
            }
            _decorManager.SortVisibles();
        }


        public int Draw(ref SpriteBatch spriteBatch, ref Camera2D cam) 
        {

            _tileSpanManager.Draw(ref spriteBatch, ref cam);
            var numOfDraws = _tileManager.Draw(ref spriteBatch, ref cam);
            numOfDraws += _decorManager.Draw(ref spriteBatch, ref cam);
             _miniTileManager.Draw(ref spriteBatch, cam.Position + _miniMapLoc);
            return numOfDraws;
        }

        internal void Update(GameTime gameTime, Camera2D cam)
        {
            _tileManager.Update(gameTime,cam);
        }
    }
}
