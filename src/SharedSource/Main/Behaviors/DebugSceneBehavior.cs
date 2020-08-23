using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

namespace CrossingFingers_Wave.Behaviors
{
    public class DebugSceneBehavior : SceneBehavior
    {
        #region Fields

        private Input _inputService;
        private KeyboardState _beforeKeyboardState;
        private bool _diagnostics, _wireframe;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugSceneBehavior" /> class.
        /// </summary>
        public DebugSceneBehavior()
            : base("DebugSceneBehavior")
        {
            _diagnostics = false;
            WaveServices.ScreenContextManager.SetDiagnosticsActive(_diagnostics);

            _wireframe = false;
            WaveServices.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
        }

        #region Protected Methods

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
#if DEBUG
            _inputService = WaveServices.Input;

            if (_inputService.KeyboardState.IsConnected)
            {
                if (_inputService.KeyboardState.F1 == ButtonState.Pressed && _beforeKeyboardState.F1 != ButtonState.Pressed)
                {
                    _diagnostics = !_diagnostics;
                    WaveServices.ScreenContextManager.SetDiagnosticsActive(_diagnostics);
                    Scene.RenderManager.DebugLines = _diagnostics;

                    _wireframe = !_wireframe;
                    WaveServices.GraphicsDevice.RenderState.FillMode = _wireframe ? FillMode.Wireframe : FillMode.Solid;
                }
            }

            _beforeKeyboardState = _inputService.KeyboardState;
#endif
        }

        #endregion
    }
}
