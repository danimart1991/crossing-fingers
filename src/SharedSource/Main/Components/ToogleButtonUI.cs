using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;

namespace CrossingFingers_Wave.Components
{
    [DataContract(Namespace = "WaveEngine.Components.Gestures")]
    public class ToggleButtonUI : Component
    {
        private static int _instances;

        private string _backgroundImage = null;

        private string _pressedBackgroundImage = null;

        private string _checkedBackgroundImage = null;

        private bool _backToBackgroundImage = false;

        private bool _isChecked = false;

        private bool _couldUncheckWithUI = true;

        private bool _couldCheckWithUI = true;

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

        [RenderPropertyAsAsset(AssetType.Texture)]
        [DataMember]
        public string CheckedBackgroundImage
        {
            get { return _checkedBackgroundImage; }
            set { _checkedBackgroundImage = value; }
        }

        [DataMember]
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                ManageChecks();
            }
        }

        [DataMember]
        public bool CouldUncheckWithUI
        {
            get { return _couldUncheckWithUI; }
            set { _couldUncheckWithUI = value; }
        }

        [DataMember]
        public bool CouldCheckWithUI
        {
            get { return _couldCheckWithUI; }
            set { _couldCheckWithUI = value; }
        }

        /// <summary> 
        /// Occurs when ToggleButtonUI is Clicked. 
        /// </summary> 
        public event EventHandler Click;

        /// <summary> 
        /// Occurs when ToggleButtonUI is Checked. 
        /// </summary> 
        public event EventHandler Check;

        /// <summary> 
        /// Occurs when ToggleButtonUI is Unchecked. 
        /// </summary> 
        public event EventHandler Uncheck;

        public ToggleButtonUI()
            : base("ToggleButtons" + _instances++)
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

            ManageChecks();
        }

        private void OnTouchGesturesTouchReleased(object sender, GestureEventArgs e)
        {
            if (IsChecked)
            {
                if (CouldUncheckWithUI)
                {
                    IsChecked = false;

                    if (Uncheck != null)
                    {
                        Uncheck(sender, e);
                    }
                }
            }
            else
            {
                if (CouldCheckWithUI)
                {
                    IsChecked = true;

                    if (Check != null)
                    {
                        Check(sender, e);
                    }
                }
            }

            if (Click != null)
            {
                Click(sender, e);
            }
        }

        private void ManageChecks()
        {
            if (IsChecked)
            {
                if (!string.IsNullOrWhiteSpace(_checkedBackgroundImage))
                {
                    ChangeBackgroundImage(_checkedBackgroundImage);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(_backgroundImage))
                {                    
                    ChangeBackgroundImage(_backgroundImage);
                }
            }

            _backToBackgroundImage = false;
        }

        private void OnTouchGesturesTouchPressed(object sender, GestureEventArgs e)
        {
            // Asking for !backToBackgroundImage avoids to execute the if when has been done once before 
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
