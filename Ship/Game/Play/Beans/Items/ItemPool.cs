#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdPartyNinjas.XnaUtility;
using Ship.Game.Play.Beans.Items.Keys;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Beans.Mortals.Inanimate;
using Ship.Game.Play.Beans.Tiles;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Beans.Mortals.Animate;

#endregion

namespace Ship.Game.Play.Beans.Items
{
    public class ItemPool
    {
        private readonly int _capacity;
        private readonly List<WorldItem> _itemCollectionsActive;
        private readonly Queue<WorldItem> _itemCollectionsInActive;
        private readonly Random _random;

        public ItemPool(int capacity)
        {
            if (MyItemPool == null)
                MyItemPool = this;
            else return;

            _capacity = capacity;
            _itemCollectionsActive = new List<WorldItem>(_capacity);
            _itemCollectionsInActive = new Queue<WorldItem>(_capacity);
            _random = new Random();
            for (var i = 0; i < capacity; i++)
                _itemCollectionsInActive.Enqueue(new WorldItem(this));
        }

        public static ItemPool MyItemPool { get; private set; }


        internal void AddToInactive(WorldItem wic)
        {
            _itemCollectionsActive.Remove(wic);
            _itemCollectionsInActive.Enqueue(wic);
        }

        public void AddToActive(ref byte[] items, int numberOfItemsInArray, ref TileCollection tile)
        {
            if (_itemCollectionsInActive.Count < numberOfItemsInArray)
            {
                _itemCollectionsActive.Sort();
                var numberToFreeUp = numberOfItemsInArray - _itemCollectionsInActive.Count;
                for (var i = 0; i < numberToFreeUp; i++)
                {
                    var item = _itemCollectionsActive[i];
                    _itemCollectionsInActive.Enqueue(item);
                }
                for (var i = 0; i < numberToFreeUp; i++)
                    _itemCollectionsActive.RemoveAt(i);
            }

            for (var i = 0; i < numberOfItemsInArray; i++)
            {
                var newWic = _itemCollectionsInActive.Dequeue();
                _itemCollectionsActive.Add(newWic);
                SetItems(ref items[i], ref tile, ref newWic);
            }
        }

        internal void AddToActiveDropped(ref WorldItem item)
        {
            _itemCollectionsActive.Add(item);
            //find where item is at and add it to the tile.
            var startTile = Hero.MyHero.MyTile;
            while (
                !(item.HitRect.X >= startTile.HitRect.X &&
                  item.HitRect.X <= startTile.HitRect.X + TileManager.Sprite2XWidth))
            {
                if (item.HitRect.X > startTile.HitRect.X)
                    startTile = TileManager.TileColls[startTile.RightVectorX, startTile.MyVectorSpotY];
                else
                    startTile = TileManager.TileColls[startTile.LeftVectorX, startTile.MyVectorSpotY];
            }
            while (
                !(item.HitRect.Y >= startTile.HitRect.Y &&
                  item.HitRect.Y <= startTile.HitRect.Y + TileManager.Sprite2XWidth))
            {
                if (item.HitRect.Y > startTile.HitRect.Y)
                    startTile = TileManager.TileColls[startTile.MyVectorSpotX, startTile.BottomVectorY];
                else
                    startTile = TileManager.TileColls[startTile.MyVectorSpotX, startTile.TopVectorY];
            }
            item.SetTile(ref startTile);
        }

        private void SetItems(ref byte item, ref TileCollection tile, ref WorldItem worldItem)
        {
            var tileXPos = _random.Next(-1, 1);
            var tileYPos = _random.Next(-1, 1);
            var offsetx = _random.Next(TileManager.Sprite2XWidth);
            var offsety = _random.Next(TileManager.Sprite2XWidth);
            var newTileX = tileXPos < 0 ? tile.LeftVectorX : tileXPos > 0 ? tile.RightVectorX : tile.MyVectorSpotX;
            var newTileY = tileYPos < 0 ? tile.TopVectorY : tileYPos > 0 ? tile.BottomVectorY : tile.MyVectorSpotY;
            var newTile = TileManager.TileColls[newTileX, newTileY];
            newTile.AddWorldItem(ref worldItem);
            worldItem.SetType(ref newTile, item, newTile.HitRect.X + offsetx, newTile.HitRect.Y + offsety);
        }

        public void Update()
        {
            for (var i = 0; i < _itemCollectionsActive.Count; i++)
                _itemCollectionsActive[i].Update();
        }

        public void Draw()
        {
            for (var i = 0; i < _itemCollectionsActive.Count; i++)
                _itemCollectionsActive[i].Draw();
        }
    }
}