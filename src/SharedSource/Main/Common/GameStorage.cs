namespace CrossingFingers_Wave.Common
{
    /// <summary>
    /// This class is used to make a save game for all the things we
    /// need in a next execution of the game.
    /// </summary>
    public class GameStorage
    {
        /// <summary>
        /// Record of Movements in Play Mode 1 Player.
        /// </summary>
        public int Record { get; set; }

        /// <summary>
        /// Determines if the music on the game is enabled.
        /// </summary>
        public bool IsMusicEnabled { get; set; }

        /// <summary>
        /// Determines if sounds on the game are enabled.
        /// </summary>
        public bool AreSoundsEnabled { get; set; }
    }
}
