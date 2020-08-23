using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Transitions;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Managers;
using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Components;

namespace CrossingFingers_Wave.Scenes
{
    public class PlayersPopupScene : Scene
    {
        #region Fields

        private NumberPlayerEnum _numberOfPlayers = NumberPlayerEnum.OnePlayer;

        private SimpleSoundService _soundManager;

        private Entity _goPlayButton, _goRouletteButton;

        private Entity _twoPlayersPlayToggleButton, _twoPlayersRouletteToggleButton;

        private Entity _touchWarningText;

        private ButtonUI _goPlayButtonUI, _goRouletteButtonUI;

        private ToggleButtonUI _onePlayerToggleButtonUI;

        private ToggleButtonUI _twoPlayersPlayToggleButtonUI, _twoPlayersRouletteToggleButtonUI;

        private ButtonUI _closeButtonUI;

        #endregion

        #region Properties

        public GameModeEnum GameMode;

        #endregion

        #region Scene Definition

        protected override void CreateScene()
        {
            Load(WaveContent.Scenes.PlayersPopupScene);

            _soundManager = WaveServices.GetService<SimpleSoundService>();

            Pause();
#if DEBUG
            AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }
               
        protected override void Start()
        {
            base.Start();

            _closeButtonUI = EntityManager.Find("closeButton").FindComponent<ButtonUI>();
            _closeButtonUI.Click += OnButtonCloseClicked;

            _goPlayButton = EntityManager.Find("goPlayButton");
            _goPlayButtonUI = _goPlayButton.FindComponent<ButtonUI>();
            _goPlayButtonUI.Click += OnButtonGoPlayClicked;

            _goRouletteButton = EntityManager.Find("goRouletteButton");
            _goRouletteButtonUI = _goRouletteButton.FindComponent<ButtonUI>();
            _goRouletteButtonUI.Click += OnButtonGoRouletteClicked;

            _onePlayerToggleButtonUI = EntityManager.Find("onePlayerToggleButton").FindComponent<ToggleButtonUI>();
            _onePlayerToggleButtonUI.Click += OnPlayerToggleButtonClicked;

            _twoPlayersPlayToggleButton = EntityManager.Find("twoPlayersPlayToggleButton");
            _twoPlayersPlayToggleButtonUI = _twoPlayersPlayToggleButton.FindComponent<ToggleButtonUI>();
            _twoPlayersPlayToggleButtonUI.Click += OnPlayersToggleButtonClicked;

            _twoPlayersRouletteToggleButton = EntityManager.Find("twoPlayersRouletteToggleButton");
            _twoPlayersRouletteToggleButtonUI = _twoPlayersRouletteToggleButton.FindComponent<ToggleButtonUI>();
            _twoPlayersRouletteToggleButtonUI.Click += OnPlayersToggleButtonClicked;

            _touchWarningText = EntityManager.Find("touchWarningText");
        }

        protected override void End()
        {
            base.End();

            _closeButtonUI.Click -= OnButtonCloseClicked;

            _goPlayButtonUI.Click -= OnButtonGoPlayClicked;
            _goRouletteButtonUI.Click -= OnButtonGoRouletteClicked;

            _onePlayerToggleButtonUI.Click -= OnPlayerToggleButtonClicked;
            _twoPlayersPlayToggleButtonUI.Click -= OnPlayersToggleButtonClicked;
            _twoPlayersRouletteToggleButtonUI.Click -= OnPlayersToggleButtonClicked;
        }

        public override void Pause()
        {
            base.Pause();

            IsVisible = false;

            HideItems();
        }

        public override void Resume()
        {
            base.Resume();

            IsVisible = true;

            ShowItems();

            ManageVisibility();

            _numberOfPlayers = NumberPlayerEnum.OnePlayer;
            _onePlayerToggleButtonUI.IsChecked = true;
            _twoPlayersPlayToggleButtonUI.IsChecked = false;
            _twoPlayersRouletteToggleButtonUI.IsChecked = false;
        }

        #endregion

        #region Private Methods

        private void ShowItems()
        {
            foreach (Entity entity in EntityManager.FindAllByTag("Common"))
            {
                entity.IsVisible = true;
            }
        }

        private void HideItems()
        {
            foreach (Entity entity in EntityManager.AllEntities)
            {
                entity.IsVisible = false;
            }
        }

        private void ManageVisibility()
        {
            switch (GameMode)
            {
                case GameModeEnum.PlayMode:
                    _touchWarningText.IsVisible = true;
                    _goPlayButton.IsVisible = true;
                    _goRouletteButton.IsVisible = false;
                    _twoPlayersPlayToggleButton.IsVisible = true;
                    _twoPlayersRouletteToggleButton.IsVisible = false;
                    break;
                case GameModeEnum.RouletteMode:
                    _touchWarningText.IsVisible = false;
                    _goPlayButton.IsVisible = false;
                    _goRouletteButton.IsVisible = true;
                    _twoPlayersPlayToggleButton.IsVisible = false;
                    _twoPlayersRouletteToggleButton.IsVisible = true;
                    break;
            }
        }

        /// <summary>
        /// Choose 1 Player.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnPlayerToggleButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            _numberOfPlayers = NumberPlayerEnum.OnePlayer;
            _onePlayerToggleButtonUI.IsChecked = true;
            _twoPlayersPlayToggleButtonUI.IsChecked = false;
            _twoPlayersRouletteToggleButtonUI.IsChecked = false;
        }

        /// <summary>
        /// Choose 2 Players.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnPlayersToggleButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            _numberOfPlayers = NumberPlayerEnum.TwoPlayers;
            _onePlayerToggleButtonUI.IsChecked = false;
            _twoPlayersPlayToggleButtonUI.IsChecked = true;
            _twoPlayersRouletteToggleButtonUI.IsChecked = true;
        }

        /// <summary>
        /// Goes Back to the Main Menu Scene, hiding the PlayersPopupScene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonCloseClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Back);

            var mainMenuScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<MainMenuScene>();
            if (mainMenuScene != null)
            {
                mainMenuScene.Resume();
                
                IsVisible = false;

                Pause();
            }
        }

        /// <summary>
        /// Goes to the Roulette Scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonGoRouletteClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            var screenContext = new ScreenContext(new PlayRouletteScene(_numberOfPlayers));
            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(1));
            WaveServices.ScreenContextManager.To(screenContext, transition);
        }

        /// <summary>
        /// Goes to the Play Scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonGoPlayClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            var screenContext = new ScreenContext(new PlayScene(_numberOfPlayers), new PlayGameOverPopupScene(_numberOfPlayers));
            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(1));
            WaveServices.ScreenContextManager.To(screenContext, transition);
        }

        #endregion
    }
}
