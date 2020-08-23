using System;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics2D;
using System.Runtime.Serialization;

namespace CrossingFingers_Wave.Behaviors
{
    /// <summary>
    /// Motor Behavior Class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Components.Shared")]
    public class MotorBehavior : Behavior
    {
        #region Fields

        // Motor Speed Increase
        private const float SpeedInterval = 0.5f;

        // Motor Max Speed
        private const float MaxSpeed = 2f;

        // Motor Revolute Joint
        [RequiredComponent]
        private RevoluteJoint2D _revoluteJoint;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Behavior.</param>
        public MotorBehavior(string name)
            : base(name)
        {
            _revoluteJoint = null;
        }

        public MotorBehavior()
        {
            _revoluteJoint = null;
        }

        #region Protected Methods

        /// <summary>
        /// Update Method
        /// </summary>
        /// <param name="gameTime">Current Game Time</param>
        protected override void Update(TimeSpan gameTime)
        {
            // If motor is activated speed increases until maxSpeed
            if (_revoluteJoint != null && _revoluteJoint.EnableMotor)
            {
                if (_revoluteJoint.MotorSpeed <= MaxSpeed) _revoluteJoint.MotorSpeed += SpeedInterval;
            }
        }

        #endregion
    }
}
