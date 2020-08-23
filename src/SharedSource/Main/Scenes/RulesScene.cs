using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Common;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.Managers;
using CrossingFingers_Wave.Components;

namespace CrossingFingers_Wave.Scenes
{
    public class RulesScene : Scene
    {
        #region Fields

        private SimpleSoundService _soundManager;

        private ButtonUI _closeButtonUI;

        #endregion

        #region Scene Definition

        protected override void CreateScene()
        {
            Load(WaveContent.Scenes.RulesScene);

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
        }

        protected override void End()
        {
            base.End();

            _closeButtonUI.Click -= OnButtonCloseClicked;
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
        }

        #endregion

        #region Private Methods

        private void ShowItems()
        {
            foreach (Entity entity in EntityManager.AllEntities)
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
        /// Goes Back to the Main Menu Scene, hiding the RulesScene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonCloseClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Back);

            MainMenuScene mainMenuScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<MainMenuScene>();
            if (mainMenuScene != null)
            {
                mainMenuScene.Resume();

                IsVisible = false;

                Pause();
            }
        }

        #endregion
    }
}
