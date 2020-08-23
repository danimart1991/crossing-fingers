using System;
using CrossingFingers_Wave.Common;
using CrossingFingers_Wave.Scenes;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Media;
using WaveEngine.Components;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework.Services;

namespace CrossingFingers_Wave
{
    public class Game : WaveEngine.Framework.Game
    {
        #region Fields

        private GameStorage _gameStorage;

        #endregion

        #region Public Methods

        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

#if DEBUG
            if (WaveServices.Storage.Exists<GameStorage>())
            {
                WaveServices.Storage.Delete<GameStorage>();
            }
#endif

            // Load storage game data
            if (WaveServices.Storage.Exists<GameStorage>())
            {
                _gameStorage = WaveServices.Storage.Read<GameStorage>();
            }
            else
            {
                _gameStorage = new GameStorage
                {
                    IsMusicEnabled = true,
                    AreSoundsEnabled = true,
                    Record = 0
                };
            }
            Catalog.RegisterItem(_gameStorage);

            WaveServices.RegisterService(new SimpleSoundService());

            StartMusic();

            var screenContext = new ScreenContext(new MainMenuScene(), new PlayersPopupScene(), new RulesScene());
            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(1));
            WaveServices.ScreenContextManager.To(screenContext, transition);
        }

        /// <summary>
        /// Called when [deactivated].
        /// </summary>
        public override void OnDeactivated()
        {
            base.OnDeactivated();

            // Save game storage
            var gameStorage = Catalog.GetItem<GameStorage>();
            WaveServices.Storage.Write(gameStorage);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Starts the music.
        /// </summary>
        private void StartMusic()
        {
            var musicInfo = new MusicInfo(WaveContent.Assets.Music.BackgroundMusic_mp3);
            WaveServices.MusicPlayer.IsRepeat = true;
            WaveServices.MusicPlayer.Volume = 0.6f;
            WaveServices.MusicPlayer.Play(musicInfo);
            WaveServices.MusicPlayer.IsMuted = !_gameStorage.IsMusicEnabled;
        }

        #endregion
    }
}
