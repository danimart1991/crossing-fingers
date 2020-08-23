using System;
using WaveEngine.Framework.Services;
using Android.App;
using Android.Content.PM;
using Android.Views;

namespace CrossingFingers_Wave
{
    [Activity(Label = "CrossingFingers_Wave",
            Icon = "@drawable/icon",
            ScreenOrientation = ScreenOrientation.Landscape,
            MainLauncher = true,
            LaunchMode = LaunchMode.SingleTask,
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AndroidActivity : WaveEngine.Adapter.Application
    {
        private CrossingFingers_Wave.Game game;

        public AndroidActivity()
        {
            FullScreen = true;

            DefaultOrientation = WaveEngine.Common.Input.DisplayOrientation.LandscapeLeft;
            SupportedOrientations = WaveEngine.Common.Input.DisplayOrientation.LandscapeLeft | WaveEngine.Common.Input.DisplayOrientation.LandscapeRight;

            // Set the app layout
            LayoutId = Resource.Layout.Main;
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                int options = (int)Window.DecorView.SystemUiVisibility;
                options |= (int)SystemUiFlags.LowProfile;
                options |= (int)SystemUiFlags.HideNavigation;

                if ((int)Android.OS.Build.VERSION.SdkInt >= 19)
                {
                    options |= (int)2048; // SystemUiFlags.Inmersive;
                    options |= (int)4096; // SystemUiFlags.ImmersiveSticky;
                }

                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)options;
            }
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
        }

        public override void Initialize()
        {
            game = new CrossingFingers_Wave.Game();
            game.Initialize(this);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            game.UpdateFrame(elapsedTime);
        }

        public override void Draw(TimeSpan elapsedTime)
        {
            game.DrawFrame(elapsedTime);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && WaveServices.Platform != null)
            {
                WaveServices.Platform.Exit();
            }

            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnPause()
        {
            if (game != null)
            {
                game.OnDeactivated();
            }

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (game != null)
            {
                game.OnActivated();
            }
        }
    }
}

