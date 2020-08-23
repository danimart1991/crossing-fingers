using System;

namespace CrossingFingers_Wave.Events
{
    /// <summary>
	/// Changed Event Handler delegate
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The <see cref="ListViewChangedEventArgs" /> instance containing the event data.</param>
	public delegate void ListViewChangedEventHandler(object sender, ListViewChangedEventArgs e);

    /// <summary>
    /// Changed events arguments class
    /// </summary>
    public class ListViewChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The old value
        /// </summary>
        public readonly int OldValue;

        /// <summary>
        /// The new value
        /// </summary>
        public readonly int NewValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ListViewChangedEventArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
