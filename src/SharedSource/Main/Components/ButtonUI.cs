using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace CrossingFingers_Wave.Components
{
    [DataContract(Namespace = "WaveEngine.Components.Gestures")]
    public class ButtonUI : Component
    {
        private static int _instances;

        private string _backgroundImage = null;

        private string _pressedBackgroundImage = null;

        private bool _backToBackgroundImage = false;

        [RequiredComponent]
        public TouchGestures TouchGestures = null;

        [RequiredComponent]
        public Sprite Sprite = null;

        [RenderPropertyAsAsset(AssetType.Texture)]
        [DataMember]
        public string BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                ChangeBackgroundImage(value);
            }
        }

        [RenderPropertyAsAsset(AssetType.Texture)]
        [DataMember]
        public string PressedBackgroundImage
        {
            get { return _pressedBackgroundImage; }
            set { _pressedBackgroundImage = value; }
        }

        /// <summary> 
        /// Occurs when ButtonUI is Clicked. 
        /// </summary> 
        public event EventHandler Click;

        public ButtonUI()
            : base("Buttons" + _instances++)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            TouchGestures.TouchPressed -= OnTouchGesturesTouchPressed;
            TouchGestures.TouchPressed += OnTouchGesturesTouchPressed;
            TouchGestures.TouchReleased -= OnTouchGesturesTouchReleased;
            TouchGestures.TouchReleased += OnTouchGesturesTouchReleased;

            if (!string.IsNullOrWhiteSpace(_backgroundImage))
            {
                ChangeBackgroundImage(_backgroundImage);
            }
        }

        private void OnTouchGesturesTouchReleased(object sender, GestureEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_backgroundImage) && _backToBackgroundImage)
            {
                _backToBackgroundImage = false;
                ChangeBackgroundImage(_backgroundImage);
            }

            if (Click != null)
            {
                Click(sender, e);
            }
        }

        private void OnTouchGesturesTouchPressed(object sender, GestureEventArgs e)
        {
            // Asking for !this.backToBackgroundImage avoids to execute the if when has been done once before 
            if (!string.IsNullOrWhiteSpace(_pressedBackgroundImage) && !_backToBackgroundImage)
            {
                ChangeBackgroundImage(_pressedBackgroundImage);
                _backToBackgroundImage = true;
            }

        }

        /// <summary> 
        /// Modifies the background image with the new asset path. 
        /// </summary> 
        /// <param name="imagePath">Path to the background image</param> 
        private void ChangeBackgroundImage(string imagePath)
        {
            if (Sprite != null)
            {
                Sprite.TexturePath = imagePath;
            }
        }
    }
}
