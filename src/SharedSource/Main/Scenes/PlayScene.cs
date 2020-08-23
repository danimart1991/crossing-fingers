using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Common;
using CrossingFingers_Wave.Entities;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Transitions;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
using WaveEngine.Components;
using WaveEngine.Framework.Managers;
using CrossingFingers_Wave.Common.Enums;
using WaveEngine.Common.Physics2D;

namespace CrossingFingers_Wave.Scenes
{
    public class PlayScene : Scene
    {
        #region Fields

        private SimpleSoundService _soundManager;

        private GameStorage _gameStorage;

        private const int MinWaitTime = 5;
        private const int MaxWaitTime = 60;
        private const int IntervalWaitTime = 5;

        private TextBlock _textTimer;
        private Button _toggleAutomaticOn, _toggleAutomaticOff, _buttonTimerDecrease, _buttonTimerIncrease, _buttonBack;
        private Image _buttonTimerIncreaseDisabled, _buttonTimerDecreaseDisabled;

        private int _waitTime = 20;

        #endregion

        public PlayScene(NumberPlayerEnum numberOfPlayers)
        {
            NumberOfPlayers = numberOfPlayers;
        }

        #region Properties

        public TextBlock TimerLeftTextBlock;

        public NumberPlayerEnum NumberOfPlayers { get; set; }

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
            Load(WaveContent.Scenes.PlayScene);

            _gameStorage = Catalog.GetItem<GameStorage>();

            _soundManager = WaveServices.GetService<SimpleSoundService>();
            WaveServices.MusicPlayer.IsMuted = true;
            WaveServices.MusicPlayer.MusicEnabled = false;
            WaveServices.MusicPlayer.Volume = 0;

            Entity rouletteImage = new Entity("rouletteImage")
                .AddComponent(new Transform2D
                {
                    DrawOrder = 1,
                    Origin = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(0.5f, 0.5f),
                    X = 7 * (VirtualScreenManager.VirtualWidth / 8) - 30,
                    Y = (VirtualScreenManager.VirtualHeight / 4f) + 30
                })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Roulette.Roulette_png))
                .AddComponent(new SpriteRenderer());
            EntityManager.Add(rouletteImage);

            Entity needleImage = new Entity("needleImage")
                .AddComponent(new Transform2D
                {
                    DrawOrder = 0,
                    Origin = new Vector2(0.5f, 0.5f),
                    Scale = new Vector2(0.5f, 0.5f),
                    X = 7 * (VirtualScreenManager.VirtualWidth / 8) - 30,
                    Y = (VirtualScreenManager.VirtualHeight / 4f) + 30
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
                    Scale = new Vector2(0.5f, 0.5f),
                    X = 7 * (VirtualScreenManager.VirtualWidth / 8) - 30,
                    Y = (VirtualScreenManager.VirtualHeight / 4f) + 30
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

            CreateMatCircles();

            #region HUD

            // Automatic Toggle Button OFF
            _toggleAutomaticOff = new Button
            {
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ToggleButtonAutomaticOff_png,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ToggleButtonAutomaticOver_png,
                Margin = new Thickness(150, 10, 0, 0),
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
                Margin = new Thickness(150, 10, 0, 0),
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
                Margin = new Thickness(20, 25, 0, 0),
                Foreground = Color.Black,
                Text = "Automatic:"
            };
            EntityManager.Add(textAutomatic.Entity);

            // Timer Text
            TextBlock textTime = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness((VirtualScreenManager.VirtualWidth / 4) - 20, 25, 0, 0),
                Foreground = Color.Black,
                Text = "Timer:"
            };
            EntityManager.Add(textTime.Entity);

            // Decrease Time Button Disabled
            _buttonTimerDecreaseDisabled = new Image(WaveContent.Assets.Textures.Buttons.ButtonLeftDisabled_png)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 50,
                Height = 50,
                IsBorder = false,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness((VirtualScreenManager.VirtualWidth / 4) + 60, 10, 0, 0),
                IsVisible = true
            };
            EntityManager.Add(_buttonTimerDecreaseDisabled.Entity);

            // Decrease Time Button
            _buttonTimerDecrease = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonLeftOver_png,
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonLeft_png,
                Margin = new Thickness((VirtualScreenManager.VirtualWidth / 4) + 60, 10, 0, 0),
                Text = "",
                Width = 50,
                Height = 50,
                IsBorder = false,
                IsVisible = false
            };
            _buttonTimerDecrease.Click += OnButtonTimerDecreaseClicked;
            EntityManager.Add(_buttonTimerDecrease.Entity);

            // Timer Time Text
            _textTimer = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness((VirtualScreenManager.VirtualWidth / 4) + 115, 25, 0, 0),
                Foreground = Color.Black,
                TextAlignment = TextAlignment.Center,
                Width = 50,
                Text = WaitTime + " s"
            };
            EntityManager.Add(_textTimer.Entity);

            // Increase Time Button Disabled
            _buttonTimerIncreaseDisabled = new Image(WaveContent.Assets.Textures.Buttons.ButtonRightDisabled_png)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 50,
                Height = 50,
                IsBorder = false,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness((VirtualScreenManager.VirtualWidth / 4) + 170, 10, 0, 0),
                IsVisible = true
            };
            EntityManager.Add(_buttonTimerIncreaseDisabled.Entity);

            // Increase Time Button
            _buttonTimerIncrease = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                PressedBackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonRightOver_png,
                BackgroundImage = WaveContent.Assets.Textures.Buttons.ButtonRight_png,
                Margin = new Thickness((VirtualScreenManager.VirtualWidth / 4) + 170, 10, 0, 0),
                Text = "",
                Width = 50,
                Height = 50,
                IsBorder = false,
                IsVisible = false
            };
            _buttonTimerIncrease.Click += OnButtonTimerIncreaseClicked;
            EntityManager.Add(_buttonTimerIncrease.Entity);

            // Time left for next round Text
            TextBlock textTimeLeftRound = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Margin = new Thickness(2 * (VirtualScreenManager.VirtualWidth / 4), 25, 0, 0),
                Foreground = Color.Black,
                Text = "Time left for the next round:"
            };
            EntityManager.Add(textTimeLeftRound.Entity);

            TimerLeftTextBlock = new TextBlock("timerLeftTextBlock")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                TextAlignment = TextAlignment.Right,
                FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                Foreground = Color.Black,
                Text = "- s",
                Margin = new Thickness(2 * (VirtualScreenManager.VirtualWidth / 4) + 310, 25, 0, 0)
            };
            EntityManager.Add(TimerLeftTextBlock.Entity);

            if (NumberOfPlayers == NumberPlayerEnum.OnePlayer)
            {
                TextBlock textRecord = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    TextAlignment = TextAlignment.Right,
                    FontPath = WaveContent.Assets.Fonts.HelveticaRounded_16_ttf,
                    Margin = new Thickness(0, 25, 20, 0),
                    Foreground = Color.Black,
                    Text = "Record: " + _gameStorage.Record
                };
                EntityManager.Add(textRecord.Entity);
            }

            Grid gridUpMovementList = new Grid
            {
                DrawOrder = 0.9f,
                BackgroundColor = Color.White,
                Width = 360,
                Height = 60,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, VirtualScreenManager.TopEdge, 0, 0)
            };
            EntityManager.Add(gridUpMovementList);

            // Back button for return to Main Menu.
            _buttonBack = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
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

            #endregion

            AddSceneBehavior(new PlaySceneBehavior(), SceneBehavior.Order.PostUpdate);

#if DEBUG
            AddSceneBehavior(new DebugSceneBehavior(), SceneBehavior.Order.PostUpdate);
#endif
        }

        #endregion

        #region Public Methods

        public void OpenGameOver1PlayerPopup(int movementsCount)
        {
            Pause();

            var playGameOverPopupScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<PlayGameOverPopupScene>();
            playGameOverPopupScene.MovementsRecord = _gameStorage.Record;
            if (movementsCount > _gameStorage.Record)
            {
                _soundManager.PlaySound(SimpleSoundService.SoundType.Win);
                _gameStorage.Record = movementsCount;
            }
            else
            {
                _soundManager.PlaySound(SimpleSoundService.SoundType.Lose);
            }
            playGameOverPopupScene.MovementsCount = movementsCount;
            playGameOverPopupScene.Resume();
        }

        public void OpenGameOver2PlayersPopup(int playerPlaying)
        {
            Pause();

            _soundManager.PlaySound(SimpleSoundService.SoundType.Win);

            var playGameOverPopupScene = WaveServices.ScreenContextManager.CurrentContext.FindScene<PlayGameOverPopupScene>();
            playGameOverPopupScene.PlayerPlaying = playerPlaying;
            playGameOverPopupScene.Resume();
        }

        #endregion

        #region Private Methods

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

        /// <summary> 
        /// Creates a group of Circles for the Mate that collides with Fingers using a CircleFactory. 
        /// </summary>
        private void CreateMatCircles()
        {
            // Color1 Mat Circles
            EntityManager.Add(MatCircleFactory("Color1Circle1", Colors.Yellow, 117f, 305f));
            EntityManager.Add(MatCircleFactory("Color1Circle2", Colors.Yellow, 254f, 305f));
            EntityManager.Add(MatCircleFactory("Color1Circle3", Colors.Yellow, 391f, 305f));
            EntityManager.Add(MatCircleFactory("Color1Circle4", Colors.Yellow, 528f, 305f));
            EntityManager.Add(MatCircleFactory("Color1Circle5", Colors.Yellow, 665f, 305f));
            EntityManager.Add(MatCircleFactory("Color1Circle6", Colors.Yellow, 802f, 305f));

            // Color2 Mat Circles
            EntityManager.Add(MatCircleFactory("Color2Circle1", Colors.Green, 117f, 132f));
            EntityManager.Add(MatCircleFactory("Color2Circle2", Colors.Green, 254f, 132f));
            EntityManager.Add(MatCircleFactory("Color2Circle3", Colors.Green, 391f, 132f));
            EntityManager.Add(MatCircleFactory("Color2Circle4", Colors.Green, 528f, 132f));
            EntityManager.Add(MatCircleFactory("Color2Circle5", Colors.Green, 665f, 132f));
            EntityManager.Add(MatCircleFactory("Color2Circle6", Colors.Green, 802f, 132f));

            // Color3 Mat Circles
            EntityManager.Add(MatCircleFactory("Color3Circle1", Colors.Red, 117f, 651f));
            EntityManager.Add(MatCircleFactory("Color3Circle2", Colors.Red, 254f, 651f));
            EntityManager.Add(MatCircleFactory("Color3Circle3", Colors.Red, 391f, 651f));
            EntityManager.Add(MatCircleFactory("Color3Circle4", Colors.Red, 528f, 651f));
            EntityManager.Add(MatCircleFactory("Color3Circle5", Colors.Red, 665f, 651f));
            EntityManager.Add(MatCircleFactory("Color3Circle6", Colors.Red, 802f, 651f));

            // Color4 Mat Circles
            EntityManager.Add(MatCircleFactory("Color4Circle1", Colors.Blue, 117f, 478f));
            EntityManager.Add(MatCircleFactory("Color4Circle2", Colors.Blue, 254f, 478f));
            EntityManager.Add(MatCircleFactory("Color4Circle3", Colors.Blue, 391f, 478f));
            EntityManager.Add(MatCircleFactory("Color4Circle4", Colors.Blue, 528f, 478f));
            EntityManager.Add(MatCircleFactory("Color4Circle5", Colors.Blue, 665f, 478f));
            EntityManager.Add(MatCircleFactory("Color4Circle6", Colors.Blue, 802f, 478f));
        }

        private static Entity MatCircleFactory(string name, Color spriteColor, float x, float y)
        {
            var circle = new Entity(name) { Tag = "MatCircles" }
                .AddComponent(new CircleCollider2D() { DebugLineColor = Color.Green })
                .AddComponent(new Sprite(WaveContent.Assets.Textures.Circle_png) { TintColor = spriteColor })
                .AddComponent(new SpriteRenderer(DefaultLayers.Alpha))
                .AddComponent(new Transform2D
                {
                    DrawOrder = 0f,
                    Origin = Vector2.Center,
                    X = x,
                    Y = y
                });

            return circle;
        }

        #endregion
    }
}
