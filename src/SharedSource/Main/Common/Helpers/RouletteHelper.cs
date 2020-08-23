using CrossingFingers_Wave.Common.Enums;
using CrossingFingers_Wave.Components;
using CrossingFingers_Wave.Extensions;
using WaveEngine.Common.Math;

namespace CrossingFingers_Wave.Common.Helpers
{
    public static class RouletteHelper
    {
        public static Movement GetMovementFromRotation(float rotation)
        {
            float rotationDegrees = MathHelper.ToDegrees(rotation);

            Movement movement = new Movement();

            if (rotationDegrees.Between(-180, -157.5f))
            {
                movement.Color = Colors.Green;
                movement.Finger = MovementFingersEnum.Thumb;
            }
            else if (rotationDegrees.Between(-157.5f, -135))
            {
                movement.Color = Colors.Red;
                movement.Finger = MovementFingersEnum.Thumb;
            }
            else if (rotationDegrees.Between(-135, -112.5f))
            {
                movement.Color = Colors.Yellow;
                movement.Finger = MovementFingersEnum.Thumb;
            }
            else if (rotationDegrees.Between(-112.5f, -90))
            {
                movement.Color = Colors.Blue;
                movement.Finger = MovementFingersEnum.Thumb;
            }
            else if (rotationDegrees.Between(-90, -67.5f))
            {
                movement.Color = Colors.Green;
                movement.Finger = MovementFingersEnum.Index;
            }
            else if (rotationDegrees.Between(-67.5f, -45))
            {
                movement.Color = Colors.Red;
                movement.Finger = MovementFingersEnum.Index;
            }
            else if (rotationDegrees.Between(-45, -22.5f))
            {
                movement.Color = Colors.Yellow;
                movement.Finger = MovementFingersEnum.Index;
            }
            else if (rotationDegrees.Between(-22.5f, 0))
            {
                movement.Color = Colors.Blue;
                movement.Finger = MovementFingersEnum.Index;
            }
            else if (rotationDegrees.Between(0, 22.5f))
            {
                movement.Color = Colors.Green;
                movement.Finger = MovementFingersEnum.Middle;
            }
            else if (rotationDegrees.Between(22.5f, 45))
            {
                movement.Color = Colors.Red;
                movement.Finger = MovementFingersEnum.Middle;
            }
            else if (rotationDegrees.Between(45, 67.5f))
            {
                movement.Color = Colors.Yellow;
                movement.Finger = MovementFingersEnum.Middle;
            }
            else if (rotationDegrees.Between(67.5f, 90))
            {
                movement.Color = Colors.Blue;
                movement.Finger = MovementFingersEnum.Middle;
            }
            else if (rotationDegrees.Between(90, 112.5f))
            {
                movement.Color = Colors.Green;
                movement.Finger = MovementFingersEnum.Third;
            }
            else if (rotationDegrees.Between(112.5f, 135))
            {
                movement.Color = Colors.Red;
                movement.Finger = MovementFingersEnum.Third;
            }
            else if (rotationDegrees.Between(135, 157.5f))
            {
                movement.Color = Colors.Yellow;
                movement.Finger = MovementFingersEnum.Third;
            }
            else if (rotationDegrees.Between(157.5f, 180))
            {
                movement.Color = Colors.Blue;
                movement.Finger = MovementFingersEnum.Third;
            }

            return movement;
        }
    }
}
