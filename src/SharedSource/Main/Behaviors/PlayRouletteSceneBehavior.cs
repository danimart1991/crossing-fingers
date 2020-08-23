using System;
using CrossingFingers_Wave.Common;
using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Common.Helpers;
using CrossingFingers_Wave.Components;
using CrossingFingers_Wave.Scenes;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Common.Media;
using WaveEngine.Components.Toolkit;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace CrossingFingers_Wave.Behaviors
{
    public class PlayRouletteSceneBehavior : SceneBehavior
    {
        #region Fields

        private SimpleSoundService _soundManager;
        private SoundInstance _sound;

        // Timming in acceleration and round time.
        private Timer _timer, _timer2;
        private int _time, _aceleratingTime = AcelerationTime;
        private const int AcelerationTime = 3;

        // Play Roulette Scene
        [RequiredComponent]
        private PlayRouletteScene _scene;

        private TextComponent _movementsTextBlockText;
        private Entity _noMovementsTextBlock;

        // input variables
        private TouchPanelState _touchState;
        private int _touchingId;

        // Entities with their components
        private Entity _needleImage;
        private RigidBody2D _needleImageRigidBody2D;

        // Physic components
        private MouseJoint2D _mouseJoint;
        private RevoluteJoint2D _revoluteJoint;

        // Mouse position optimization
        private Vector2 _touchPosition = Vector2.Zero;

        private bool _stopRepeat = true;

        private int _movementsCount = 1;

        #endregion

        public PlayRouletteSceneBehavior()
            : base("PlayRouletteSceneBehavior")
        {
        }

        #region Private Methods

        /// <summary>
        /// Update Method
        /// </summary>
        /// <param name="gameTime">Current Game Time</param>
        protected override void Update(TimeSpan gameTime)
        {
            WaveServices.Layout.PerformLayout();

            switch (_scene.GameState)
            {
                case GameStateEnum.Automatic:
                    if (_timer == null)
                    {
                        _soundManager.PlaySound(SimpleSoundService.SoundType.Timer, 1f, true);

                        _timer = WaveServices.TimerFactory.CreateTimer("TimeToWait", TimeSpan.FromSeconds(1f), () =>
                        {
                            _time--;
                            _scene.TimerLeftTextBlock.Text = _time + " s";
                        });
                    }
                    else if (_time <= 0)
                    {
                        WaveServices.TimerFactory.RemoveTimer("TimeToWait");
                        _scene.TimerLeftTextBlock.Text = "- s";

                        if (_timer2 == null)
                        {
                            _soundManager.StopAllSounds();

                            _revoluteJoint.EnableMotor = true;

                            _timer2 = WaveServices.TimerFactory.CreateTimer("TimeToAceletare", TimeSpan.FromSeconds(1f), () => { _aceleratingTime--; });
                        }

                        if (_aceleratingTime <= 0)
                        {
                            WaveServices.TimerFactory.RemoveTimer("TimeToAceletare");

                            _revoluteJoint.EnableMotor = false;
                        }
                    }

                    break;

                case GameStateEnum.Manual:
                    WaveServices.TimerFactory.RemoveAllTimers();
                    _timer = null;
                    _timer2 = null;
                    _scene.TimerLeftTextBlock.Text = "- s";
                    _revoluteJoint.EnableMotor = false;

                    _touchState = WaveServices.Input.TouchPanelState;
                    if (_touchState.IsConnected)
                    {
                        // Checks Mouse Left Button Click and anyone entity linked
                        if (_touchState.Count > 0 && _mouseJoint == null)
                        {
                            foreach (var touch in _touchState)
                            {
                                // Updates Mouse Position
                                _touchPosition = touch.Position;

                                // Adjust the position to the Viewport.
                                _scene.VirtualScreenManager.ToVirtualPosition(ref _touchPosition);

                                // Collider Test
                                var collider = _needleImage.FindComponent<CircleCollider2D>();
                                if (collider != null && collider.Contain(_touchPosition))
                                {
                                    var rigidBody = _needleImage.FindComponent<RigidBody2D>();
                                    if (rigidBody != null)
                                    {
                                        _touchingId = touch.Id;

                                        // Create Mouse Joint
                                        _mouseJoint = new MouseJoint2D()
                                        {
                                            Target = _touchPosition
                                        };
                                        _needleImage.AddComponent(_mouseJoint);

                                        // We break after collide because no more than one touch can be Joint to entity.
                                        break;
                                    }
                                }
                            }
                        }

                        // If joint exists then update joint anchor position.
                        // If touchReleased Then touchFound = false; so Remove the Joint to conserve physics.
                        if (_mouseJoint != null)
                        {
                            TouchLocation touchLocation;

                            if (_touchState.TryGetTouch(_touchingId, out touchLocation))
                            {
                                _touchPosition = touchLocation.Position;
                                _scene.VirtualScreenManager.ToVirtualPosition(ref _touchPosition);

                                _mouseJoint.Target = _touchPosition;
                            }
                            else
                            {
                                if (!_needleImage.IsDisposed)
                                {
                                    _needleImage.RemoveComponent<MouseJoint2D>();
                                }

                                _mouseJoint = null;
                            }
                        }
                    }

                    break;
            }

            // Collision with pieces when the needle stops.
            float angularVelocity = _needleImageRigidBody2D.AngularVelocity;
            if (!_stopRepeat && angularVelocity > -0.005f && angularVelocity < 0.005f)
            {
                _soundManager.StopAllSounds();

                Movement movement = RouletteHelper.GetMovementFromRotation(_needleImage.FindComponent<Transform2D>().Rotation);

                movement.Number = _movementsCount++;
                movement.NumberOfPlayers = _scene.NumberOfPlayers;

                _noMovementsTextBlock.IsVisible = false;
                _movementsTextBlockText.Text += "\n" + movement.ToString();

                if (_scene.GameState == GameStateEnum.Automatic)
                {
                    ResetAutomaticTimers();
                }

                _soundManager.PlaySound(SimpleSoundService.SoundType.Pick);

                _stopRepeat = true;
            }

            // When needle start to ride, it's time to another movement
            if (angularVelocity > 0.01f || angularVelocity < -0.01f)
            {
                _stopRepeat = false;
            }

            if (angularVelocity > 2f || angularVelocity < -2f)
            {
                if (_sound == null || _sound.State == SoundState.Stopped)
                {
                    _sound = _soundManager.PlaySound(SimpleSoundService.SoundType.Rotating);
                }
            }
        }

        protected override void ResolveDependencies()
        {
            _scene = Scene as PlayRouletteScene;

            _soundManager = WaveServices.GetService<SimpleSoundService>();

            _needleImage = _scene.EntityManager.Find("needleImage");
            _needleImageRigidBody2D = _needleImage.FindComponent<RigidBody2D>();

            _movementsTextBlockText = _scene.EntityManager.Find("movementsTextBlock").FindComponent<TextComponent>();
            _noMovementsTextBlock = _scene.EntityManager.Find("noMovementsTextBlock");

            Entity _needlePin = _scene.EntityManager.Find("needlePin");
            _revoluteJoint = _needlePin.FindComponent<RevoluteJoint2D>();
        }

        public void ResetAutomaticTimers()
        {
            _time = _scene.WaitTime + 1;
            _aceleratingTime = AcelerationTime;
            WaveServices.TimerFactory.RemoveAllTimers();
            _timer = null;
            _timer2 = null;
        }

        #endregion
    }
}
