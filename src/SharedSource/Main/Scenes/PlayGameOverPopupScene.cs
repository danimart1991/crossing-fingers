using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Framework.Managers;
using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Components;
using WaveEngine.Components.Toolkit;
using WaveEngine.Components.Graphics2D;

namespace CrossingFingers_Wave.Scenes
{
    public class PlayGameOverPopupScene : Scene
    {
        #region Fields

        private SimpleSoundService _soundManager;

        private readonly NumberPlayerEnum _numberOfPlayers;

        private Entity _onePlayerPointsText;

        private ButtonUI _goBackButtonUI, _playAgainButtonUI;

        private TextComponent _popupTitleTextComponent, _onePlayerPointsTextComponent;

        private Sprite _playResultImageSprite;

        #endregion

        public PlayGameOverPopupScene(NumberPlayerEnum numberOfPlayers)
        {
            _numberOfPlayers = numberOfPlayers;
        }

        #region Properties

        public int MovementsCount { get; set; }

        public int PlayerPlaying { get; set; }

        public int MovementsRecord { get; set; }

        #endregion

        #region Scene Definition

        protected override void CreateScene()
        {
            Load(WaveContent.Scenes.PlayGameOverPopupScene);

            _soundManager = WaveServices.GetService<SimpleSoundService>();

            Pause();
#if DEBUG
            AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }

        protected override void Start()
        {
            base.Start();

            _goBackButtonUI = EntityManager.Find("goBackButton").FindComponent<ButtonUI>();
            _goBackButtonUI.Click += OnGoBackButtonClicked;

            _playAgainButtonUI = EntityManager.Find("playAgainButton").FindComponent<ButtonUI>();
            _playAgainButtonUI.Click += OnPlayAgainButtonClicked;

            _popupTitleTextComponent = EntityManager.Find("popupTitleText").FindComponent<TextComponent>();

            _onePlayerPointsText = EntityManager.Find("onePlayerPointsText");
            _onePlayerPointsTextComponent = _onePlayerPointsText.FindComponent<TextComponent>();

            _playResultImageSprite = EntityManager.Find("playResultImage").FindComponent<Sprite>();
        }

        protected override void End()
        {
            base.End();

            _goBackButtonUI.Click -= OnGoBackButtonClicked;
            _playAgainButtonUI.Click -= OnPlayAgainButtonClicked;
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

            ShowCommonTaggedItems();

            if (_numberOfPlayers == NumberPlayerEnum.OnePlayer)
            {
                ShowOnePlayerPopup();
            }
            else
            {
                ShowTwoPlayersPopup();
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void ShowCommonTaggedItems()
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

        /// <summary>
        /// Play again the same scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnPlayAgainButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            var screenContext = new ScreenContext(new PlayScene(_numberOfPlayers), new PlayGameOverPopupScene(_numberOfPlayers));
            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(1));
            WaveServices.ScreenContextManager.To(screenContext, transition);
        }

        /// <summary>
        /// Goes Back to the Main Menu Scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnGoBackButtonClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Back);

            var screenContext = new ScreenContext(new MainMenuScene(), new PlayersPopupScene(), new RulesScene());
            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(1));
            WaveServices.ScreenContextManager.To(screenContext, transition);
        }

        private void ShowOnePlayerPopup()
        {
            _onePlayerPointsText.IsVisible = true;

            if (MovementsCount > MovementsRecord)
            {
                _popupTitleTextComponent.Text = "Congratulations! You win!";
                _playResultImageSprite.TexturePath = WaveContent.Assets.Textures.PlayGameOverPopupWin_png;
                _onePlayerPointsTextComponent.Text = "You did " + MovementsCount + " points! Record was " + MovementsRecord + " points.";
            }
            else
            {
                _popupTitleTextComponent.Text = "You lose. Good luck next time.";
                _playResultImageSprite.TexturePath = WaveContent.Assets.Textures.PlayGameOverPopupLose_png;
                _onePlayerPointsTextComponent.Text = "You did " + MovementsCount + " points. Record is " + MovementsRecord + " points.";
            }
        }

        private void ShowTwoPlayersPopup()
        {
            if (PlayerPlaying == 1)
            {
                _popupTitleTextComponent.Text = "Player 2 Wins !";
                _playResultImageSprite.TexturePath = WaveContent.Assets.Textures.PlayGameOverPopupPlayer2Win_png;
            }
            else
            {
                _popupTitleTextComponent.Text = "Player 1 Wins !";
                _playResultImageSprite.TexturePath = WaveContent.Assets.Textures.PlayGameOverPopupPlayer1Win_png;
            }
        }

        #endregion
    }
}
