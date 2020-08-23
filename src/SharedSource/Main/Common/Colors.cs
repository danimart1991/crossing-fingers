using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Graphics;

namespace CrossingFingers_Wave.Common
{
    public static class Colors
    {
        public static readonly Color Yellow = new Color("#FEF650");
        public static readonly Color Green = new Color("#72B755");
        public static readonly Color Red = new Color("#EE2524");
        public static readonly Color Blue = new Color("#648BCA");
        public static readonly Color Orange = new Color("#DD9944");
        public static readonly Color Gray = new Color("#9ABBC6");
        public static readonly Color DarkGray = new Color("#292929");

        public static readonly Dictionary<Color, string> List = new Dictionary<Color, string>
        {
            { Yellow, "Yellow" },
            { Green, "Green"},
            { Red, "Red" },
            { Blue, "Blue" },
            { Orange, "Orange" },
            { Gray, "Gray" },
            { DarkGray, "DarkGray" }
        };

        public static string NameOf(Color color)
        {
            string name = "Other";

            KeyValuePair<Color, string> colorList = List.FirstOrDefault(x => x.Key == color);

            if (colorList.Key == color)
            {
                name = colorList.Value;
            }

            return name;
        }
    }
}
