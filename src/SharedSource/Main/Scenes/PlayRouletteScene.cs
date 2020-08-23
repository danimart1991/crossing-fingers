using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Common;
using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Entities;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace CrossingFingers_Wave.Scenes
{
    public class PlayRouletteScene : Scene
    {
        #region Fields

        private SimpleSoundService _soundManager;

        private const int MinWaitTime = 5;
        private const int MaxWaitTime = 60;
        private const int IntervalWaitTime = 5;

        private TextBlock _textTimer;
        private Button _toggleAutomaticOn, _toggleAutomaticOff, _buttonTimerDecrease, _buttonTimerIncrease, _buttonBack;
        private Image _buttonTimerIncreaseDisabled, _buttonTimerDecreaseDisabled;

        private int _waitTime = 20;

        #endregion

        public PlayRouletteScene(NumberPlayerEnum numberOfPlayers)
        {
            NumberOfPlayers = numberOfPlayers;
        }

        #region Properties

        public TextBlock TimerLeftTextBlock;

        public NumberPlayerEnum NumberOfPlayers { get; private set; }

        public int WaitTime
        {
            get { return _waitTime; }
            set
            {
                if (value.Equals(_waitTime)) return;
                _waitTime = value;
                _textTimer.Text = value + " s";
            }
        }

        public GameStateEnum GameState = GameStateEnum.Manual;

        #endregion

        #region Scene Definition

        protected override void CreateScene()
        {
            Load(WaveContent.Scenes.PlayRouletteScene);

            _soundManager = WaveServices.GetService<SimpleSoundService>();
            WaveServices.MusicPlayer.IsMuted = true;
            WaveServices.MusicPlayer.MusicEnabled = false;
            WaveServices.MusicPlayer.Volume = 0;

            Entity rouletteImage = new Entity("rouletteImage")
                .AddComponent(new Transform2D
                {
                    DrawOrder = 1,
                    Origin = new Vector2(0.5f, 0.5f),
                    X = 3 * (VirtualScreenManager.VirtualWidth / 4),
                    Y = (VirtualScreenManager.VirtualHeight / 2f) + 30
                })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Roulette.Roulette_png))
                .AddComponent(new SpriteRenderer());
            EntityManager.Add(rouletteImage);

            Entity needleImage = new Entity("needleImage")
                .AddComponent(new Transform2D
                {
                    DrawOrder = 0,
                    Origin = new Vector2(0.5f, 0.5f),
                    X = 3 * (VirtualScreenManager.VirtualWidth / 4),
                    Y = (VirtualScreenManager.VirtualHeight / 2f) + 30
                })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Roulette.Needle_png))
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new CircleCollider2D()
                {
                    DebugLineColor = Color.Blue,
                    Density = 0.05f
                })
                .AddComponent(new RigidBody2D
                {
                    PhysicBodyType = RigidBodyType2D.Dynamic,
                    LinearDamping = 1,
                    AngularDamping = 1,
                    GravityScale = 0
                });
            EntityManager.Add(needleImage);

            Entity needlePin = new Entity("needlePin")
                .AddComponent(new Transform2D
                {
                    DrawOrder = 1,
                    Origin = new Vector2(0.5f, 0.5f),
                    X = 3 * (VirtualScreenManager.VirtualWidth / 4),
                    Y = (VirtualScreenManager.VirtualHeight / 2f) + 30
                })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Pixel_png))
                .AddComponent(new SpriteRenderer(DefaultLayers.Opaque))
                .AddComponent(new RectangleCollider2D())
                .AddComponent(new RigidBody2D()
                {
                    PhysicBodyType = RigidBodyType2D.Kinematic
                })
                .AddComponent(new RevoluteJoint2D()
                {
                    EnableMotor = false,
                    MaxMotorTorque = 1000f,
                    ConnectedEntityPath = "needleImage"
                })
                .AddComponent(new MotorBehavior()
                {
                    IsActive = true
                });
            EntityManager.Add(needlePin);

            // Automatic Toggle Button OFF
            _toggleAutomaticOff = new Button
            {
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ToggleButtonAutomaticOff_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ToggleButtonAutomaticOver_png,
                Margin = new Thickness(210, 10, 0, 0),
                Height = 50,
                Text = "",
                IsVisible = true
            };
            _toggleAutomaticOff.Click += OnToggleAutomaticOffClicked;
            EntityManager.Add(_toggleAutomaticOff);

            // Automatic Toggle Button ON
            _toggleAutomaticOn = new Button
            {
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ToggleButtonAutomaticOn_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ToggleButtonAutomaticOver_png,
                Margin = new Thickness(210, 10, 0, 0),
                Height = 50,
                IsBorder = false,
                Text = "",
                IsVisible = false
            };
            _toggleAutomaticOn.Click += OnToggleAutomaticOnClicked;
            EntityManager.Add(_toggleAutomaticOn);

            // Automatic Text
            TextBlock textAutomatic = new TextBlock
            {
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness(75, 25, 0, 0),
                Foreground = Color.Black,
                Text = "Automatic:"
            };
            EntityManager.Add(textAutomatic.Entity);

            // Timer Text
            TextBlock textTime = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness(0, 25, 130, 0),
                Foreground = Color.Black,
                Text = "Timer:"
            };
            EntityManager.Add(textTime.Entity);

            // Timer Time Text
            _textTimer = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness(0, 25, 0, 0),
                Foreground = Color.Black,
                TextAlignment = TextAlignment.Center,
                Width = 50,
                Text = WaitTime + " s"
            };
            EntityManager.Add(_textTimer.Entity);

            // Increase Time Button Disabled
            _buttonTimerIncreaseDisabled = new Image(WaveContent.Assets.Textures.Buttons.ButtonRightDisabled_png)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 50,
                Height = 50,
                IsBorder = false,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(60, 10, 0, 0),
                IsVisible = true
            };
            EntityManager.Add(_buttonTimerIncreaseDisabled.Entity);

            // Increase Time Button
            _buttonTimerIncrease = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonRightOver_png,
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonRight_png,
                Margin = new Thickness(60, 10, 0, 0),
                Text = "",
                Width = 50,
                Height = 50,
                IsBorder = false,
                IsVisible = false
            };
            _buttonTimerIncrease.Click += OnButtonTimerIncreaseClicked;
            EntityManager.Add(_buttonTimerIncrease.Entity);

            // Decrease Time Button Disabled
            _buttonTimerDecreaseDisabled = new Image(WaveContent.Assets.Textures.Buttons.ButtonLeftDisabled_png)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 50,
                Height = 50,
                IsBorder = false,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(0, 10, 60, 0),
                IsVisible = true
            };
            EntityManager.Add(_buttonTimerDecreaseDisabled.Entity);

            // Decrease Time Button
            _buttonTimerDecrease = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonLeftOver_png,
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonLeft_png,
                Margin = new Thickness(0, 10, 60, 0),
                Text = "",
                Width = 50,
                Height = 50,
                IsBorder = false,
                IsVisible = false
            };
            _buttonTimerDecrease.Click += OnButtonTimerDecreaseClicked;
            EntityManager.Add(_buttonTimerDecrease.Entity);

            // Time left for next round Text
            TextBlock textTimeLeftRound = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness(0, 25, 320, 0),
                Foreground = Color.Black,
                Text = "Time left for the next round:"
            };
            EntityManager.Add(textTimeLeftRound.Entity);

            TimerLeftTextBlock = new TextBlock("timerLeftTextBlock")
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Foreground = Color.Black,
                Text = "- s",
                Margin = new Thickness(0, 25, 0, 0)
            };
            EntityManager.Add(TimerLeftTextBlock.Entity);

            // Back button for return to Main Menu.
            _buttonBack = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(10),
                Text = "",
                FontPath = WaveContent.Assets.Fonts.Kabel_Dm_BT_18_TTF,
                Width = 163,
                Height = 82,
                IsBorder = false,
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonGoBack_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonGoBackOver_png
            };
            _buttonBack.Click += OnButtonBackClicked;
            EntityManager.Add(_buttonBack.Entity);

            Grid gridUpMovementList = new Grid
            {
                DrawOrder = 0.9f,
                BackgroundColor = Color.White,
                Width = 550,
                Height = 80,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            EntityManager.Add(gridUpMovementList);

            AddSceneBehavior(new PlayRouletteSceneBehavior(), SceneBehavior.Order.PostUpdate);

#if DEBUG
            AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }

        protected override void End()
        {
            base.End();

            WaveServices.TimerFactory.RemoveAllTimers();

            _toggleAutomaticOff.Click -= OnToggleAutomaticOffClicked;
            _toggleAutomaticOn.Click -= OnToggleAutomaticOnClicked;

            _buttonTimerIncrease.Click -= OnButtonTimerIncreaseClicked;
            _buttonTimerDecrease.Click -= OnButtonTimerDecreaseClicked;

            _buttonBack.Click -= OnButtonBackClicked;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method executed when any Automatic Mode Toggle Buttons is pressed.
        /// Recalculate the buttons that must be visible.
        /// </summary>
        private void TimerButtonsChanged()
        {
            if (_toggleAutomaticOn.IsVisible)
            {
                if (WaitTime > MinWaitTime)
                {
                    _buttonTimerDecreaseDisabled.IsVisible = false;
                    _buttonTimerDecrease.IsVisible = true;
                }
                else
                {
                    _buttonTimerDecreaseDisabled.IsVisible = true;
                    _buttonTimerDecrease.IsVisible = false;
                }

                if (WaitTime < MaxWaitTime)
                {
                    _buttonTimerIncreaseDisabled.IsVisible = false;
                    _buttonTimerIncrease.IsVisible = true;
                }
                else
                {
                    _buttonTimerIncreaseDisabled.IsVisible = true;
                    _buttonTimerIncrease.IsVisible = false;
                }
            }
            else
            {
                _buttonTimerIncreaseDisabled.IsVisible = true;
                _buttonTimerIncrease.IsVisible = false;
                _buttonTimerDecreaseDisabled.IsVisible = true;
                _buttonTimerDecrease.IsVisible = false;
            }
        }

        /// <summary>
        /// Initializes the Automatic State Mode of the game.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnToggleAutomaticOnClicked(object sender, EventArgs e)
        {
            _soundManager.StopAllSounds();
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            GameState = GameStateEnum.Manual;
            _toggleAutomaticOn.IsVisible = false;
            _toggleAutomaticOff.IsVisible = true;

            TimerButtonsChanged();
        }

        /// <summary>
        /// Initializes the Manual State Mode of the game.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnToggleAutomaticOffClicked(object sender, EventArgs e)
        {
            _soundManager.StopAllSounds();
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            GameState = GameStateEnum.Automatic;
            _toggleAutomaticOff.IsVisible = false;
            _toggleAutomaticOn.IsVisible = true;

            TimerButtonsChanged();
        }

        /// <summary>
        /// Decreases <see cref="WaitTime" /> in <see cref="IntervalWaitTime" /> until <see cref="MinWaitTime" /> is reached
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonTimerDecreaseClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            if (WaitTime > MinWaitTime)
            {
                WaitTime -= IntervalWaitTime;
            }

            TimerButtonsChanged();
        }

        /// <summary>
        /// Increases <see cref="WaitTime" /> in <see cref="IntervalWaitTime" /> until <see cref="MaxWaitTime" /> is reached
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonTimerIncreaseClicked(object sender, EventArgs e)
        {
            _soundManager.PlaySound(SimpleSoundService.SoundType.Button);

            if (WaitTime < MaxWaitTime)
            {
                WaitTime += IntervalWaitTime;
            }

            TimerButtonsChanged();
        }

        /// <summary>
        /// Goes Back to the Main Menu Scene.
        /// </summary>
        /// <param name="sender">The object that sends the event</param>
        /// <param name="e">The parameters of the event.</param>
        private void OnButtonBackClicked(object sender, EventArgs e)
        {
            _soundManager.StopAllSounds();
            _soundManager.PlaySound(SimpleSoundService.SoundType.Back);

            var screenContext = new ScreenContext(new MainMenuScene(), new PlayersPopupScene(), new RulesScene());
            var transition = new ColorFadeTransition(Color.White, TimeSpan.FromSeconds(1));
            WaveServices.ScreenContextManager.To(screenContext, transition);
        }

        #endregion
    }
}
