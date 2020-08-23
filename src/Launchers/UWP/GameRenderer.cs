using System;
using WaveEngine.Adapter;
using WaveEngine.Common.Input;
using Windows.System.Display;
using Windows.UI.Xaml.Controls;

namespace CrossingFingers_Wave
{
    public class GameRenderer : Application
    {
        private DisplayRequest displayRequest;

        private Game game;

        public GameRenderer(SwapChainPanel panel)
            : base(panel)
        {
            FullScreen = true;
        }

        public override void Update(TimeSpan gameTime)
        {  
            game.UpdateFrame(gameTime);
        }

        public override void Draw(TimeSpan gameTime)
        {
            game.DrawFrame(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();

            Adapter.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            displayRequest = new DisplayRequest();
            displayRequest.RequestActive();

            game = new Game();
            game.Initialize(this);
        }

        public override void OnResuming()
        {
            base.OnResuming();

            game.OnActivated();
        }

        public override void OnSuspending()
        {
            base.OnSuspending();

            game.OnDeactivated();
        }
    }
}
