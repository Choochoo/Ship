#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship.Game.Play.Beans.Items;
using ThirdPartyNinjas.XnaUtility;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Beans.Managers;
using Ship.Game.Play.Utils;

#endregion

namespace Ship.Game.Play.Beans.Mortals.Animate.Extras
{
    public class Inventory
    {
        public const int SizeOfHeroInventory = 42;
        private readonly List<WorldItem> _quickItems = new List<WorldItem>(10);
        private readonly List<WorldItem> _worldItems = new List<WorldItem>(SizeOfHeroInventory);
        private Hero _hero;
        private WorldItem itemFollowMouse;
        private const byte maxCollectionSize = byte.MaxValue;
        public Inventory(Hero hero) { _hero = hero; }
        public List<WorldItem> WorldItems { get { return _worldItems; } }

        public static bool MovingItem { get; private set; }

        public void AddToInventory(WorldItem item)
        {
            var itemCount = item.Count;
            for (var i = 0; i < _worldItems.Count; i++)
            {
                WorldItem currentItem = _worldItems[i];
                if (currentItem.MyType == item.MyType && currentItem.Count != maxCollectionSize)
                {
                    if (currentItem.Count + itemCount > maxCollectionSize)
                    {
                        var addon = (byte)(maxCollectionSize - currentItem.Count);
                        currentItem.Count += addon;
                    }
                    else
                    {
                        currentItem.Count += itemCount;
                        item.Count = 0;
                    }
                    break;
                }
                
                
            }

            if (item.Count == 0)
            {
                item.InInventory = false;
            }
            else 
            {
                item.InventoryPosition = _worldItems.Count;
                System.Diagnostics.Debug.WriteLine("pos:"+item.InventoryPosition);
                item.InInventory = true;
                if (item.InventoryPosition < 10)
                    _quickItems.Add(item);
                _worldItems.Add(item);
            }
        }

        public void FollowMouse(int index)
        {
            if(_quickItems.Count == 0)
            return;

            MovingItem = true;
            itemFollowMouse = _quickItems[index];
        }

        public WorldItem RemoveFromInventory()
        {
            MovingItem = false;
            const int limit = 120;
            var gotoX = Inputs.MousePointerRect.X - _hero.HitRect.X;
            var gotoY = Inputs.MousePointerRect.Y - _hero.HitRect.Y;
            var xneg = gotoX < 0;
            var yneg = gotoY < 0;
            gotoX = xneg ? -gotoX : gotoX;
            gotoY = yneg ? -gotoY : gotoY;

            var percX = gotoX/((float) gotoX + gotoY);
            var percY = gotoY/((float) gotoX + gotoY);
            gotoX = (int) (percX*(xneg ? -limit : limit));
            gotoY = (int) (percY*(yneg ? -limit : limit));

            var outputX = (float) (_hero.HitRect.X + gotoX);
            var outputY = (float) (_hero.HitRect.Y + gotoY);
            itemFollowMouse.PutOnGround(outputX, outputY);
            itemFollowMouse.InInventory = false;
            _quickItems.Remove(itemFollowMouse);
            _worldItems.Remove(itemFollowMouse);
            var output = itemFollowMouse;
            itemFollowMouse = null;
            return output;
        }

        public void Draw()
        {
            for (var i = 0; i < _quickItems.Count; i++)
            {
                var worldItem = _quickItems[i];
                Vector2 rectPos;
                if (itemFollowMouse != null && itemFollowMouse == worldItem)
                    rectPos = new Vector2(Inputs.MousePointerRect.X, Inputs.MousePointerRect.Y);
                else if (worldItem.InventoryPosition < 10)
                    rectPos = InterfaceManager.MyInterfaceManager.QuickItems[worldItem.InventoryPosition];
                worldItem.DrawForMenu(ref rectPos);
            }
        }
    }
}