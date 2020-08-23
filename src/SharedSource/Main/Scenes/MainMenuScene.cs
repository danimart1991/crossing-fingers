using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Common;
using WaveEngine.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Components;

namespace CrossingFingers_Wave.Scenes
{
    public class MainMenuScene : Scene
    {
        #region Fields

        private SimpleSoundService _soundManager;

        private GameStorage _gameStorage;

        private ButtonUI _playButtonUI, _rouletteButtonUI, _rulesButtonUI;

        private ToggleButtonUI _musicToggleButtonUI, _soundToggleButtonUI;

        #endregion

        #region Scene Definition

        protected override void CreateScene()
        {
            Load(WaveContent.Scenes.MainMenuScene);

            _gameStorage = Catalog.GetItem<GameStorage>();

            _soundManager = WaveServices.GetService<SimpleSoundService>();
            if (_gameStorage.IsMusicEnabled)
            {
                WaveServices.MusicPlayer.MusicEnabled = true;
                WaveServices.MusicPlayer.Volume = 0.6f;
                WaveServices.MusicPlayer.IsMuted = false;
            }

#if DEBUG
            AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }

        protected override void Start()
        {
            base.Start();

            _playButtonUI = EntityManager.Find("playButton").FindComponent<ButtonUI>();
            _playButtonUI.Click += OnPlayButtonClicked;
            _rouletteButtonUI = EntityManager.Find("rouletteButton").FindComponent<ButtonUI>();
            _rouletteButtonUI.Click += OnRouletteButtonClicked;
            _rulesButtonUI = EntityManager.Find("rulesButton").FindComponent<ButtonUI>();
            _rulesButtonUI.Click += OnRulesButtonClicked;

            _musicToggleButtonUI = EntityManager.Find("musicToggleButton").FindComponent<ToggleButtonUI>();
            _musicToggleButtonUI.IsChecked = _gameStorage.IsMusicEnabled;
            _musicToggleButtonUI.Check += OnMusicToggleButtonChecked;
            _musicToggleButtonUI.Uncheck += OnMusicToggleButtonUnchecked;

            _soundToggleButtonUI = EntityManager.Find("soundToggleButton").FindComponent<ToggleButtonUI>();
            _soundToggleButtonUI.IsChecked = _gameStorage.AreSoundsEnabled;
            _soundToggleButtonUI.Check += OnSoundToggleButtonChecked;
            _soundToggleButtonUI.Uncheck += OnSoundToggleButtonUnchecked;
        }        

        protected override void End()
        {
            base.End();

            _playButtonUI.Click -= OnPlayButtonClicked;
            _rouletteButtonUI.Click -= OnRouletteButtonClicked;
            _rulesButtonUI.Click -= OnRulesButtonClicked;

            _musicToggleButtonUI.Check -= OnMusicToggleButtonChecked;
            _musicToggleButtonUI.Uncheck -= OnMusicToggleButtonUnchecked;

            _soundToggleButtonUI.Check -= OnSoundToggleButtonChecked;
            _soundToggleButtonUI.Uncheck -= OnSoundToggleButtonUnchecked;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Goes to the Play Scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnPlayButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            var playersPopupScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<PlayersPopupScene>();
            if (playersPopupScene != null)
            {
                playersPopupScene.GameMode = GameModeEnum.PlayMode;

                playersPopupScene.Resume();

                Pause();
            }
        }

        /// <summary>
        /// Goes to the Roulette Scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnRouletteButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            var playersPopupScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<PlayersPopupScene>();
            if (playersPopupScene != null)
            {                
                playersPopupScene.GameMode = GameModeEnum.RouletteMode;

                playersPopupScene.Resume();

                Pause();
            }
        }

        private void OnRulesButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            var rulesScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<RulesScene>();
            if (rulesScene != null)
            {
                rulesScene.Resume();

                Pause();
            }
        }

        /// <summary>
        /// Enable Music on the game.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnMusicToggleButtonChecked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            _gameStorage.IsMusicEnabled = true;
            WaveServices.MusicPlayer.MusicEnabled = true;
            WaveServices.MusicPlayer.Volume = 0.6f;
            WaveServices.MusicPlayer.IsMuted = false;
        }

        /// <summary>
        /// Disable Music on the game.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnMusicToggleButtonUnchecked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            _gameStorage.IsMusicEnabled = false;
            WaveServices.MusicPlayer.MusicEnabled = false;
            WaveServices.MusicPlayer.Volume = 0;
            WaveServices.MusicPlayer.IsMuted = true;
        }

        /// <summary>
        /// Enable Sounds on the game.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnSoundToggleButtonChecked(object sender, EventArgs e)
        {
            _gameStorage.AreSoundsEnabled = true;

            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);
        }

        /// <summary>
        /// Disable Sounds on the game.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnSoundToggleButtonUnchecked(object sender, EventArgs e)
        {
            _gameStorage.AreSoundsEnabled = false;
        }

        #endregion
    }
}
