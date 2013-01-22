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
    public class DecorationSound
    {
        private readonly int _numberOfSounds;
        private readonly Random _rand;
        private readonly SoundEffect[] _sounds;
        private int _currentDuration;
        private int _currentSound;
        private int _currentTotalTime;
        private bool _isPlaying;

        public DecorationSound(string soundName, int numberOfSounds)
        {
            _numberOfSounds = numberOfSounds;
            _sounds = new SoundEffect[numberOfSounds];
            _rand = new Random();
            for (var i = 0; i < numberOfSounds; i++)
                _sounds[i] =
                    MainGame.ContentLoader.Load<SoundEffect>(string.Format("Screens/Play/Sounds/{0}{1}", soundName, i));
        }

        public bool IsPlaying { get { return _isPlaying; } }

        public void Play()
        {
            if (!IsPlaying)
            {
                _isPlaying = true;
                _currentDuration = _sounds[_currentSound].Duration.Milliseconds;
                _sounds[_currentSound].Play();
                _currentSound = _rand.Next(0, _numberOfSounds);
                _currentTotalTime = 0;
            }
            else
            {
                _currentTotalTime += PlayScreen.OfficialGametime.ElapsedGameTime.Milliseconds;
                if (_currentTotalTime > _currentDuration)
                    _isPlaying = false;
            }
        }
    }
}