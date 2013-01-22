#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using System.Timers;
using Ship.Game.ScreenComponents.Screens;

#endregion

namespace Ship.Game.Play.Beans.Sounds
{
    public class BasicSound
    {
        private readonly SoundEffect _sound;
        private int _currentDuration;
        private int _currentTotalTime;
        private bool _isPlaying;

        public BasicSound(string soundName) { _sound = MainGame.ContentLoader.Load<SoundEffect>(string.Format("Screens/Play/Sounds/{0}", soundName)); }

        public bool IsPlaying { get { return _isPlaying; } }

        public void Play()
        {
            _sound.Play();

            //if (!IsPlaying)
            //{
            //    _isPlaying = true;
            //    _currentDuration = _sound.Duration.Milliseconds;
                
            //    _currentTotalTime = 0;
            //}
            //else
            //{
            //    _currentTotalTime += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
            //    if (_currentTotalTime > _currentDuration)
            //        _isPlaying = false;
            //}
        }
    }
}