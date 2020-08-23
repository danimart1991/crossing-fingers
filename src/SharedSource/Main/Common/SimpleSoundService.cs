using System.Collections.Generic;
using WaveEngine.Common;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Sound;

namespace CrossingFingers_Wave.Common
{
    /// <summary>
    /// Simple Sound Service
    /// </summary>
    public class SimpleSoundService : Service
    {
        #region Fields

        /// <summary>
        /// GameStorage used to check if sound must be played.
        /// </summary>
        private GameStorage _gameStorage;

        /// <summary>
        /// The sound player
        /// </summary>
        private SoundPlayer _soundPlayer;

        /// <summary>
        /// The bank
        /// </summary>
        private SoundBank _bank;

        /// <summary>
        /// The sounds
        /// </summary>
        private Dictionary<SoundType, SoundInfo> _sounds;

        /// <summary>
        /// The muted
        /// </summary>
        private bool _muted;

        #endregion

        #region Properties

        /// <summary>
        /// List sounds
        /// </summary>
        public enum SoundType
        {
            Button,
            Back,
            Rotating,
            Timer,
            Pick,
            Win,
            Lose
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SimpleSoundService" /> is mute.
        /// </summary>
        public bool Mute
        {
            get { return _muted; }
            set
            {
                _muted = value;
                WaveServices.MusicPlayer.IsMuted = value;

                if (_muted)
                {
                    WaveServices.SoundPlayer.StopAllSounds();
                }
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSoundService" /> class.
        /// </summary>
        protected override void Initialize()
        {
            _gameStorage = Catalog.GetItem<GameStorage>();

            _soundPlayer = WaveServices.SoundPlayer;

            // fill sound info
            _sounds = new Dictionary<SoundType, SoundInfo>();
            _sounds.Add(SoundType.Button, new SoundInfo(WaveContent.Assets.Sounds.Button_wav));
            _sounds.Add(SoundType.Back, new SoundInfo(WaveContent.Assets.Sounds.Clap_wav));
            _sounds.Add(SoundType.Rotating, new SoundInfo(WaveContent.Assets.Sounds.Rotating_wav));
            _sounds.Add(SoundType.Timer, new SoundInfo(WaveContent.Assets.Sounds.Timer_wav));
            _sounds.Add(SoundType.Pick, new SoundInfo(WaveContent.Assets.Sounds.Pick_wav));
            _sounds.Add(SoundType.Win, new SoundInfo(WaveContent.Assets.Sounds.Win_wav));
            _sounds.Add(SoundType.Lose, new SoundInfo(WaveContent.Assets.Sounds.Lose_wav));

            _bank = new SoundBank();
            _soundPlayer.RegisterSoundBank(_bank);

            foreach (var item in _sounds)
            {
                _bank.Add(item.Value);
            }
        }

        /// <summary>
        /// Allow to execute custom logic during the finalization of this instance.
        /// </summary>
        protected override void Terminate()
        {
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="soundType">The sound.</param>
        /// <param name="volume">The volume.</param>
        /// /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <returns>Sound instance</returns>
        public SoundInstance PlaySound(SoundType soundType, float volume = 1f, bool loop = false)
        {
            if (_muted || _gameStorage == null || !_gameStorage.AreSoundsEnabled)
            {
                return null;
            }

            return _soundPlayer.Play(_sounds[soundType], volume, loop);
        }

        public void StopAllSounds()
        {
            WaveServices.SoundPlayer.StopAllSounds();
        }

        #endregion
    }
}
