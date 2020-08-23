using WaveEngine.Framework;
using CrossingFingers_Wave.Common;
using WaveEngine.Common.Graphics;
using System.Text;
using System.Runtime.Serialization;
using CrossingFingers_Wave.Common.Enums;

namespace CrossingFingers_Wave.Components
{
    [DataContract(Namespace = "WaveEngine.Components.Shared")]
    public class Movement : Component
    {
        #region Properties

        public int Number { get; set; }

        public MovementFingersEnum Finger { get; set; }

        public Color Color { get; set; }

        public NumberPlayerEnum NumberOfPlayers { get; set; }

        public int PlayerNumber
        {
            get
            {
                var modCount = Number % 2;
                if (modCount == 0) modCount = 2;

                return modCount;
            }
        }

        public Entity CirclePressed { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            var text = new StringBuilder();

            text.Append(Number);
            text.Append(". ");

            if (NumberOfPlayers == NumberPlayerEnum.TwoPlayers)
            {
                text.Append("P");
                text.Append(PlayerNumber);
                text.Append(": ");
            }

            text.Append(Finger);
            text.Append(" Finger to ");
            text.Append(Colors.NameOf(Color));

            return text.ToString();
        }

        #endregion
    }
}
