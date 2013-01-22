#region

using Microsoft.Xna.Framework;
using ThirdPartyNinjas.XnaUtility;
using Microsoft.Xna.Framework.Graphics;
using Ship.Game.Play.Utils;
using Microsoft.Xna.Framework.Input;
using System;
using Ship.Game.Play.WorldGeneration;
using Ship.Game.Play.WorldGeneration.WorldDrawing;
using Ship.Game.Play.Beans.Items;
using Ship.Game.Play.Beans.Constants;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class Manager
    {
        private readonly CharacterManager _charManager;
        private readonly DecorManager _decorManager;
        private readonly InterfaceManager _interfaceManager;
        private readonly ItemPool _itemPool;
        private readonly LoaderBaseManager _loaderBaseManager;
        private readonly MessageManager _messageManager;
        private readonly Vector2 _miniMapLoc = new Vector2(10, 10);
        private readonly MiniTileManager _miniTileManager;
        private readonly TileManager _tileManager;
        private bool _refreshMiniTileManager = true;
        private bool _refreshTileManager = false;

        public Manager()
        {
            if (MyManager == null)
                MyManager = this;
            else return;

            //!!watch order!!
            _tileManager = new TileManager(this);
            _charManager = new CharacterManager();
            _decorManager = new DecorManager();
            _miniTileManager = new MiniTileManager();
            _interfaceManager = new InterfaceManager();
            _messageManager = new MessageManager();
            _loaderBaseManager = new LoaderBaseManager();
            _itemPool = new ItemPool(20);

            //load data
            _tileManager.LoadData(ref _loaderBaseManager.Loaders);
            _decorManager.LoadData();
            _charManager.LoadData();
        }

        public static Manager MyManager { get; private set; }

        public void MoveLoaderBase(byte direction)
        {
            switch (direction)
            {
                case MoveConstants.Up:
                    _loaderBaseManager.RotateTilesUp();
                    break;
                case MoveConstants.Down:
                    _loaderBaseManager.RotateTilesDown();
                    break;
                case MoveConstants.Left:
                    _loaderBaseManager.RotateTilesLeft();
                    break;
                case MoveConstants.Right:
                    _loaderBaseManager.RotateTilesRight();
                    break;
            }
            PlayScreen.SpawnX = _loaderBaseManager.Loaders[1][1].SectorX;
            PlayScreen.SpawnY = _loaderBaseManager.Loaders[1][1].SectorY;
            WorldData.MyWorldData.FogData[PlayScreen.SpawnX][PlayScreen.SpawnY] = FogWar.None;
            _miniTileManager.Shift(direction);
            //CreateWorldTerrain.UpdateSpace();
            _refreshMiniTileManager = true;
        }

        public void MoveDirection(byte direction)
        {
            switch (direction)
            {
                case MoveConstants.Up:
                    //System.Diagnostics.Debug.WriteLine("Hit up");
                    _tileManager.MoveTerrainUp();
                    _decorManager.MoveUp(_tileManager.MostLeftUpSprite, _tileManager.MostRightDownSprite);
                    break;
                case MoveConstants.Down:
                    //System.Diagnostics.Debug.WriteLine("Hit down");
                    _tileManager.MoveTerrainDown();
                    _decorManager.MoveDown(_tileManager.MostLeftUpSprite, _tileManager.MostRightDownSprite);
                    break;
                case MoveConstants.Left:
                    //System.Diagnostics.Debug.WriteLine("Hit left");
                    _tileManager.MoveTerrainLeft();
                    _decorManager.MoveLeft(_tileManager.MostLeftUpSprite, _tileManager.MostRightDownSprite);
                    break;
                case MoveConstants.Right:
                    //System.Diagnostics.Debug.WriteLine("Hit right");
                    _tileManager.MoveTerrainRight();
                    _decorManager.MoveRight(_tileManager.MostLeftUpSprite, _tileManager.MostRightDownSprite);
                    break;
            }
            _decorManager.SortPool();
        }


        public void Draw()
        {
            _tileManager.Draw();

            _decorManager.StartDraw(Hero.MyHero);
            _charManager.Draw();
            _itemPool.Draw();
            _decorManager.FinishDraw();
            _miniTileManager.Draw();
            _interfaceManager.Draw();
            _charManager.DrawInventory();
            _messageManager.Draw();
        }

        internal void Update()
        {
            _tileManager.Update();
            _itemPool.Update();
            _charManager.Update(_decorManager.DecorPool);
            _interfaceManager.Update();
        }

        internal void CheckForBaked()
        {
            //_miniTileManager.PreDraw();
            //_tileManager.PreDraw(ref spriteBatch);
        }

        internal bool OnInterfaceRects()
        {
            var index = _interfaceManager.CheckRects();

            if (index != -1)
                _charManager.MyHero.MyInventory.FollowMouse(index);

            return index != -1;
        }

        internal void ReleaseItemIntoWorld()
        {
            var item = _charManager.MyHero.MyInventory.RemoveFromInventory();
            _itemPool.AddToActiveDropped(ref item);
        }
    }
}