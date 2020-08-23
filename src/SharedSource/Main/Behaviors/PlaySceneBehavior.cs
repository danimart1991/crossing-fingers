using CrossingFingers_Wave.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using CrossingFingers_Wave.Scenes;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;
using CrossingFingers_Wave.Common;
using WaveEngine.Components.Graphics2D;
using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Common.Helpers;
using WaveEngine.Common.Media;
using WaveEngine.Components.Toolkit;

namespace CrossingFingers_Wave.Behaviors
{
    public class PlaySceneBehavior : SceneBehavior
    {
        #region Fields

        private SimpleSoundService _soundManager;
        private SoundInstance _sound;

        // Timming in acceleration and round time.
        private Timer _timer, _timer2;
        private int _time, _aceleratingTime = AcelerationTime;
        private const int AcelerationTime = 3;

        // Play Scene
        [RequiredComponent]
        private PlayScene _scene;

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
        private List<Movement> _movementsList = new List<Movement>();

        private NumberPlayerEnum _numberOfPlayers;

        private Movement _movementToRemove;

        private List<Entity> _lastCirclesPressed = new List<Entity>();

        // Alter this between 1 and 2.
        private int _playerPlaying = 2;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PlaySceneBehavior()
            : base("PlaySceneBehavior")
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

                            _timer2 = WaveServices.TimerFactory.CreateTimer("TimeToAceletare", TimeSpan.FromSeconds(1f), () =>
                            {
                                _aceleratingTime--;
                            });
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
            var angularVelocity = _needleImageRigidBody2D.AngularVelocity;
            if (!_stopRepeat && angularVelocity > -0.005f && angularVelocity < 0.005f)
            {
                _soundManager.StopAllSounds();

                Movement movement = RouletteHelper.GetMovementFromRotation(_needleImage.FindComponent<Transform2D>().Rotation);

                movement.Number = _movementsCount++;
                movement.NumberOfPlayers = _numberOfPlayers;

                Movement repeatedFingerMovement = null;
                switch (_numberOfPlayers)
                {
                    case NumberPlayerEnum.OnePlayer:
                        repeatedFingerMovement = _movementsList.Find(x => x.Finger == movement.Finger);
                        if (repeatedFingerMovement != null)
                        {
                            if (repeatedFingerMovement.Color != movement.Color)
                            {
                                _movementToRemove = repeatedFingerMovement;
                                _movementsList.Add(movement);
                            }
                        }
                        else
                        {
                            _movementsList.Add(movement);
                        }
                        break;
                    case NumberPlayerEnum.TwoPlayers:
                        repeatedFingerMovement = _movementsList.Find(x => x.PlayerNumber != _playerPlaying && x.Finger == movement.Finger);
                        if (repeatedFingerMovement != null)
                        {
                            if (repeatedFingerMovement.Color != movement.Color)
                            {
                                _movementToRemove = repeatedFingerMovement;
                                _movementsList.Add(movement);
                            }
                        }
                        else
                        {
                            _movementsList.Add(movement);
                        }
                        break;
                }

                _noMovementsTextBlock.IsVisible = false;
                _movementsTextBlockText.Text += "\n" + movement.ToString();

                if (_numberOfPlayers == NumberPlayerEnum.TwoPlayers)
                {
                    if (_playerPlaying == 1)
                    {
                        _playerPlaying++;
                    }
                    else
                    {
                        _playerPlaying--;
                    }
                }

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

                if (_movementsList.Any(x => x.CirclePressed == null))
                {
                    GameOver();
                }
            }

            if (angularVelocity > 2f || angularVelocity < -2f)
            {
                if (_sound == null || _sound.State == SoundState.Stopped)
                {
                    _sound = _soundManager.PlaySound(SimpleSoundService.SoundType.Rotating);
                }
            }

            List<Entity> circlesPressed = new List<Entity>();
            // We check the Circles to assign the collider with touch.
            _touchState = WaveServices.Input.TouchPanelState;
            if (_touchState.IsConnected)
            {
                foreach (Entity entity in _scene.EntityManager.FindAllByTag("MatCircles"))
                {
                    var opacity = 1f;

                    foreach (var touch in _touchState)
                    {
                        // Updates Mouse Position
                        _touchPosition = touch.Position;

                        _scene.VirtualScreenManager.ToVirtualPosition(ref _touchPosition);

                        var collider = entity.FindComponent<CircleCollider2D>();
                        if (collider != null && collider.Contain(_touchPosition))
                        {
                            circlesPressed.Add(entity);
                            opacity = 0.5f;

                            break;
                        }
                    }

                    entity.FindComponent<Transform2D>().Opacity = opacity;
                }
            }

            // Check if user adds a new circle touch.
            foreach (var circle in circlesPressed)
            {
                if (!_movementsList.Any(x => x.CirclePressed == circle))
                {
                    var emptyMovement = _movementsList.Find(x => x.CirclePressed == null);
                    if (emptyMovement != null && emptyMovement.Color == circle.FindComponent<Sprite>().TintColor)
                    {
                        emptyMovement.CirclePressed = circle;
                        _movementsList.Remove(_movementToRemove);
                        _movementToRemove = null;
                    }
                    else
                    {
                        GameOver();
                    }
                }
            }

            // Check if user removes a circle touch.
            if (circlesPressed.Count < _lastCirclesPressed.Count)
            {
                Entity circleRemoved = null;

                foreach (var lastCircle in _lastCirclesPressed)
                {
                    if (!circlesPressed.Contains(lastCircle))
                    {
                        circleRemoved = lastCircle;
                        break;
                    }
                }

                if (circleRemoved != null)
                {
                    var movement = _movementsList.Find(x => x.CirclePressed == circleRemoved);

                    if (movement != null && movement != _movementToRemove)
                    {
                        GameOver();
                    }
                }
            }

            _lastCirclesPressed = circlesPressed.ToList();
        }

        public void GameOver()
        {
            if (_numberOfPlayers == NumberPlayerEnum.OnePlayer)
            {
                _scene.OpenGameOver1PlayerPopup(_movementsCount - 1);
            }
            else
            {
                _scene.OpenGameOver2PlayersPopup(_playerPlaying);
            }
        }

        /// <summary>
        /// To Resolve Dependencies of Required Components
        /// </summary>
        protected override void ResolveDependencies()
        {
            _scene = Scene as PlayScene;

            _soundManager = WaveServices.GetService<SimpleSoundService>();

            _needleImage = _scene.EntityManager.Find("needleImage");
            _needleImageRigidBody2D = _needleImage.FindComponent<RigidBody2D>();

            _movementsTextBlockText = _scene.EntityManager.Find("movementsTextBlock").FindComponent<TextComponent>();
            _noMovementsTextBlock = _scene.EntityManager.Find("noMovementsTextBlock");

            Entity needlePin = _scene.EntityManager.Find("needlePin");
            _revoluteJoint = needlePin.FindComponent<RevoluteJoint2D>();

            _numberOfPlayers = _scene.NumberOfPlayers;
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
