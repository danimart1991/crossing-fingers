using System.Runtime.Serialization;
using WaveEngine.Components.Gestures;
using WaveEngine.Framework;

namespace CrossingFingers_Wave.Components
{
    [DataContract(Namespace = "WaveEngine.Components.Gestures")]
    public class TouchTransform : Component
    {
        /// <summary> 
        /// Number of instances of this component created. 
        /// </summary> 
        private static int _instances;

        [RequiredComponent]
        public TouchGestures TouchGestures = null;

        public GestureSample TouchGesture { get; private set; }

        public TouchTransform()
            : base("TouchTransforms" + _instances++)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            TouchGestures.TouchMoved -= OnTouchGesturesTouchMoved;
            TouchGestures.TouchMoved += OnTouchGesturesTouchMoved;
            TouchGestures.TouchReleased -= OnTouchGesturesTouchReleased;
            TouchGestures.TouchReleased += OnTouchGesturesTouchReleased;
        }

        private void OnTouchGesturesTouchReleased(object sender, GestureEventArgs e)
        {
            TouchGesture = new GestureSample();
        }

        private void OnTouchGesturesTouchMoved(object sender, GestureEventArgs e)
        {
            TouchGesture = e.GestureSample;
        }
    }
}
