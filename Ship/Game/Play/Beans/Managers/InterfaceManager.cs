#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship.Game.Play.Beans.Constants;
using Microsoft.Xna.Framework;
using Ship.Game.Play.Beans.Helpers;
using Ship.Game.Play.Beans.Mortals.Animate;
using Ship.Game.Play.Utils;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Managers
{
    public class InterfaceManager
    {
        private bool _visibleHudDisplay = true;
        private bool _visibleExperienceMeter = true;
        private bool _visibleExperienceBar = true;
        private bool _visibleInventory;


        private readonly Rectangle _hudDisplay;
        private readonly Rectangle _experienceBar;
        private readonly Rectangle _experienceMeter;
        private readonly Rectangle _inventory;


        private readonly Vector2 _hudDisplayPosition = new Vector2(10.5f, 10.5f);
        private readonly Vector2 _experienceMeterPosition = new Vector2(117.25f, 581f);
        private readonly Vector2 _experienceBarPosition = new Vector2(119.25f, 583f);
        private readonly Vector2 _inventoryPosition = new Vector2(33.5f, 506.5f);


        public InterfaceManager()
        {
            MyInterfaceManager = this;
            _hudDisplay = PlayScreen.InterfaceAtlas.GetRegion(InterfaceHelper.HudDisplay).Bounds;

            _experienceMeter = PlayScreen.InterfaceAtlas.GetRegion(InterfaceHelper.ExperienceMeter).Bounds;
            _experienceBar = PlayScreen.InterfaceAtlas.GetRegion(InterfaceHelper.ExperienceBar).Bounds;

            _inventory = PlayScreen.InterfaceAtlas.GetRegion(InterfaceHelper.Inventory).Bounds;

            for (var i = 0; i < _quickitems.Length; i++)
            {
                _quickItemsRelativePositions[i].X = 135 + (i*55.8f);
                _quickItemsRelativePositions[i].Y = 532f;
                _quickitems[i].Width = _quickitems[i].Height = 34;
            }
        }

        private Vector2 _xpBarScale = Vector2.One;

        public static InterfaceManager MyInterfaceManager { get; private set; }

        public Vector2[] QuickItems { get { return _quickItemsAbsolutePositions; } }

        internal void Update()
        {
            _xpBarScale.X = 100.0f/500.0f;

            for (var i = 0; i < _quickitems.Length; i++)
            {
                _quickItemsAbsolutePositions[i].X =
                    (int) (_quickItemsRelativePositions[i].X + Camera2D.MyCam.Position.X);
                _quickItemsAbsolutePositions[i].Y =
                    (int) (_quickItemsRelativePositions[i].Y + Camera2D.MyCam.Position.Y);
                _quickitems[i].X = (int) (_quickItemsAbsolutePositions[i].X);
                _quickitems[i].Y = (int) (_quickItemsAbsolutePositions[i].Y);
            }

            if (Inputs.MyInputs.GameAction == 0) return;

            switch (Inputs.MyInputs.GameAction)
            {
                case ActionConstants.ToggleInventory:
                    _visibleInventory = !_visibleInventory;
                    break;
            }
        }

        private readonly Rectangle[] _quickitems = new Rectangle[10];
        private readonly Vector2[] _quickItemsRelativePositions = new Vector2[10];
        private readonly Vector2[] _quickItemsAbsolutePositions = new Vector2[10];
#if DEBUG
        private bool _showItems = true;
#endif

        internal void Draw()
        {
            if (_visibleHudDisplay)
            {
                PlayScreen.Spritebatch.Draw(PlayScreen.InterfaceTexture, Camera2D.MyCam.Position + _hudDisplayPosition,
                                            _hudDisplay, Color.White);
            }

            if (_visibleExperienceMeter)
            {
                PlayScreen.Spritebatch.Draw(PlayScreen.InterfaceTexture,
                                            Camera2D.MyCam.Position + _experienceMeterPosition, _experienceMeter,
                                            Color.White);
            }

            if (_visibleExperienceBar)
            {
                PlayScreen.Spritebatch.Draw(PlayScreen.InterfaceTexture,
                                            Camera2D.MyCam.Position + _experienceBarPosition, _experienceBar,
                                            Color.White, 0f, Vector2.Zero, _xpBarScale,
                                            Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0.0f);
            }

            if (_visibleInventory)
            {
                PlayScreen.Spritebatch.Draw(PlayScreen.InterfaceTexture, Camera2D.MyCam.Position + _inventoryPosition,
                                            _inventory, Color.White);
            }

#if DEBUG
            if (_showItems)
            {
                for (var i = 0; i < _quickitems.Length; i++)
                    PlayScreen.Spritebatch.Draw(PlayScreen.ErrorBox, _quickitems[i], Color.White);
            }
#endif
        }

        internal int CheckRects()
        {
            for (var i = 0; i < _quickitems.Length; i++)
            {
                if (_quickitems[i].Intersects(Inputs.MousePointerRect))
                    return i;
            }
            return -1;
        }
    }
}