using System;
using CrossingFingers_Wave.Events;
using WaveEngine.Common.Math;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

namespace CrossingFingers_Wave.Behaviors
{
    /// <summary>
    /// [Deprecated]
    /// </summary>
    public class ListViewBehavior : FocusBehavior
    {
        #region Constants

        /// <summary>
        /// The default unchecked image
        /// </summary>
        private const int DefaultSliderWeight = 40;

        #endregion

        #region Fields

        /// <summary>
        /// The gestures
        /// </summary>
        [RequiredComponent]
        public TouchGestures Gestures;

        /// <summary>
        /// The bullet image
        /// </summary>
        private ImageControl _bulletImage;

        /// <summary>
        /// The foreground image
        /// </summary>
        private ImageControl _foregroundImage;

        /// <summary>
        /// The foreground transform
        /// </summary>
        private Transform2D _foregroundTransform;

        /// <summary>
        /// The background image
        /// </summary>
        private ImageControl _backgroundImage;

        /// <summary>
        /// The bullet transform
        /// </summary>
        private Transform2D _bulletTransform;

        /// <summary>
        /// The text control
        /// </summary>
        private ListBoxBehavior _listBoxControl;

        /// <summary>
        /// The orientation
        /// </summary>
        private Orientation _orientation;

        /// <summary>
        /// The maximum value
        /// </summary>
        private int _maximum;

        /// <summary>
        /// The minimum value
        /// </summary>
        private int _minimum;

        /// <summary>
        /// The current value
        /// </summary>
        private int _value;

        #region Cached values

        /// <summary>
        /// The cached difference between maximum and minimum
        /// </summary>
        private int _difference;

        /// <summary>
        /// The maximum offset
        /// </summary>
        private float _maximunOffset;

        /// <summary>
        /// The maximum offset over2
        /// </summary>
        private float _maximunOffsetOver2;

        /// <summary>
        /// The bullet with over2
        /// </summary>
        private float _bulletWeightOver2;

        /// <summary>
        /// The old cached value
        /// </summary>
        private int _oldCachedValue1, _oldCachedValue2;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event ListViewChangedEventHandler ValueChanged;

        /// <summary>
        /// Occurs when [real time value changed].
        /// </summary>
        public event ListViewChangedEventHandler RealTimeValueChanged;

        #endregion

        #region Properties

        /// <summary>
        /// The panel
        /// </summary>
        [RequiredComponent]
        public PanelControl Panel;

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
                UpdateDifference();
            }
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public int Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                _minimum = value;
                UpdateDifference();
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value
        {
            get { return _value; }

            set
            {
                if (value > _maximum) value = _maximum;
                if (value < _minimum) value = _minimum;

                if (_value != value)
                {
                    _value = value;

                    if (_bulletTransform != null)
                    {
                        float result;
                        switch (_orientation)
                        {
                            case Orientation.Vertical:
                                result = _maximunOffset * (value + _minimum) / _difference;
                                _bulletTransform.Y = result;
                                _foregroundTransform.YScale = -result;
                                break;
                            case Orientation.Horizontal:
                                result = _maximunOffset * (value - _minimum) / _difference;
                                _bulletTransform.X = result;
                                _foregroundTransform.XScale = result;
                                break;
                        }
                    }

                    // Events

                    // Stable Change event
                    if (ValueChanged != null && _oldCachedValue1 != _value)
                    {
                        ValueChanged(this, new ListViewChangedEventArgs(_oldCachedValue1, value));
                        _oldCachedValue1 = _value;
                    }

                    // RealTime Change event
                    if (RealTimeValueChanged != null && _oldCachedValue2 != _value)
                    {
                        RealTimeValueChanged(this, new ListViewChangedEventArgs(_oldCachedValue2, _value));
                        _oldCachedValue2 = _value;
                    }
                }
            }
        }

        public void ScrollToEndAtAddIfNeeded()
        {
            if (Panel != null && _listBoxControl != null && Value == 100)
            {
                switch (_orientation)
                {
                    case Orientation.Vertical:
                        if (Panel.Height < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                        {
                            _bulletTransform.Opacity = 1;
                            _foregroundTransform.Opacity = 1;

                            var maxVerticalOffset = (Panel.Height - (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count);
                            _listBoxControl.PosY = maxVerticalOffset;
                        }
                        else
                        {
                            _bulletTransform.Opacity = 0;
                            _foregroundTransform.Opacity = 0;
                        }
                        break;
                    case Orientation.Horizontal:
                        if (Panel.Width < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                        {
                            _bulletTransform.Opacity = 1;
                            _foregroundTransform.Opacity = 1;

                            var maxHorizontalOffset = -(((_listBoxControl.ItemHeight + 5) * _listBoxControl.Count) - Panel.Width);
                            _listBoxControl.PosX = maxHorizontalOffset;
                        }
                        else
                        {
                            _bulletTransform.Opacity = 0;
                            _foregroundTransform.Opacity = 0;
                        }
                        break;
                }

            }
        }

        public void ScrollToEndAtRemoveIfNeeded()
        {
            if (Panel != null && _listBoxControl != null && Value == 100)
            {
                switch (_orientation)
                {
                    case Orientation.Vertical:
                        if (Panel.Height < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                        {
                            _bulletTransform.Opacity = 1;
                            _foregroundTransform.Opacity = 1;

                            var maxVerticalOffset = (Panel.Height - (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count);
                            _listBoxControl.PosY = maxVerticalOffset;
                        }
                        else
                        {
                            _bulletTransform.Opacity = 0;
                            _foregroundTransform.Opacity = 0;
                        }
                        break;
                    case Orientation.Horizontal:
                        if (Panel.Width < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                        {
                            _bulletTransform.Opacity = 1;
                            _foregroundTransform.Opacity = 1;

                            var maxHorizontalOffset = -(((_listBoxControl.ItemHeight + 5) * _listBoxControl.Count) - Panel.Width);
                            _listBoxControl.PosX = maxHorizontalOffset;
                        }
                        else
                        {
                            _bulletTransform.Opacity = 0;
                            _foregroundTransform.Opacity = 0;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the width of the update.
        /// </summary>
        /// <value>
        /// The width of the update.
        /// </value>
        public float UpdateWidth
        {
            set
            {
                if (_backgroundImage != null && _orientation == Orientation.Horizontal && value > 0)
                {
                    _backgroundImage.Width = value;
                }
            }
        }

        /// <summary>
        /// Sets the height of the update.
        /// </summary>
        /// <value>
        /// The height of the update.
        /// </value>
        public float UpdateHeight
        {
            set
            {
                if (_backgroundImage != null && _orientation == Orientation.Vertical && value > 0)
                {
                    _backgroundImage.Height = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value; UpdateLayer();
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderBehavior" /> class.
        /// </summary>
        public ListViewBehavior() : base("ListViewBehavior")
        {
            _maximum = 100;
            _minimum = 0;
            _difference = _maximum - _minimum;
            _value = _minimum;
            _orientation = Orientation.Horizontal;

            _oldCachedValue1 = -1;
            _oldCachedValue2 = -1;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            Gestures.TouchMoved -= OnGesturesTouchMoved;
            Gestures.TouchMoved += OnGesturesTouchMoved;
            Gestures.TouchReleased -= OnGesturesTouchReleased;
            Gestures.TouchReleased += OnGesturesTouchReleased;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            var bulletEntity = Owner.FindChild("BulletEntity");
            _bulletTransform = bulletEntity.FindComponent<Transform2D>();
            _bulletTransform.Opacity = 0;
            _bulletImage = bulletEntity.FindComponent<ImageControl>();
            _bulletImage.Width = DefaultSliderWeight;
            _bulletImage.Height = DefaultSliderWeight;

            var foregroundEntity = Owner.FindChild("ForegroundEntity");
            _foregroundImage = foregroundEntity.FindComponent<ImageControl>();
            _foregroundTransform = foregroundEntity.FindComponent<Transform2D>();
            _foregroundTransform.Opacity = 0;

            _backgroundImage = Owner.FindChild("BackgroundEntity").FindComponent<ImageControl>();

            _listBoxControl = Owner.FindComponent<ListBoxBehavior>();

            // Default parameters
            UpdateLayer();

            float result;
            // Initialization value
            switch (_orientation)
            {
                case Orientation.Vertical:
                    result = _maximunOffset * (_value + _minimum) / _difference;
                    _bulletTransform.Y = result;
                    _foregroundTransform.YScale = -result;
                    break;
                case Orientation.Horizontal:
                    result = _maximunOffset * (_value - _minimum) / _difference;
                    _bulletTransform.X = result;
                    _foregroundTransform.XScale = result;
                    break;
            }
        }

        /// <summary>
        /// Handles the TouchMoved event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void OnGesturesTouchMoved(object sender, GestureEventArgs e)
        {
            switch (_orientation)
            {
                case Orientation.Vertical:
                    if (Panel.Height < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                    {
                        _bulletTransform.Opacity = 1;
                        _foregroundTransform.Opacity = 1;

                        var offsetY = e.GestureSample.Position.Y - _bulletTransform.Rectangle.Y;
                        UpdateWidthVerticalOffset(offsetY);
                    }
                    break;
                case Orientation.Horizontal:
                    if (Panel.Width < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                    {
                        _bulletTransform.Opacity = 1;
                        _foregroundTransform.Opacity = 1;

                        var offsetX = e.GestureSample.Position.X - _bulletTransform.Rectangle.X;
                        UpdateWidthHorizontalOffset(offsetX);
                    }
                    break;
            }

            // RealTime Change event
            if (RealTimeValueChanged != null && _oldCachedValue2 != _value)
            {
                RealTimeValueChanged(this, new ListViewChangedEventArgs(_oldCachedValue2, _value));
                _oldCachedValue2 = _value;
            }
        }

        /// <summary>
        /// Handles the TouchReleased event of the Gestures control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GestureEventArgs" /> instance containing the event data.</param>
        private void OnGesturesTouchReleased(object sender, GestureEventArgs e)
        {
            switch (_orientation)
            {
                case Orientation.Horizontal:
                    if (Panel.Width < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                    {
                        _bulletTransform.Opacity = 1;
                        _foregroundTransform.Opacity = 1;

                        var maxHorizontalOffset = -(((_listBoxControl.ItemHeight + 5) * _listBoxControl.Count) - Panel.Width);

                        if (_listBoxControl.PosX > 0)
                        {
                            _listBoxControl.PosX = 0;
                        }
                        else if (_listBoxControl.PosX < maxHorizontalOffset)
                        {
                            _listBoxControl.PosX = maxHorizontalOffset;
                        }
                    }
                    break;
                case Orientation.Vertical:
                    if (Panel.Height < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
                    {
                        _bulletTransform.Opacity = 1;
                        _foregroundTransform.Opacity = 1;

                        var maxVerticalOffset = (Panel.Height - (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count);

                        if (_listBoxControl.PosY > 0)
                        {
                            _listBoxControl.PosY = 0;
                        }
                        else if (_listBoxControl.PosY < maxVerticalOffset)
                        {
                            _listBoxControl.PosY = maxVerticalOffset;
                        }
                    }
                    break;
            }

            // Stable Change event
            if (ValueChanged != null && _oldCachedValue1 != _value)
            {
                ValueChanged(this, new ListViewChangedEventArgs(_oldCachedValue1, _value));
            }

            // RealTime Change event
            if (RealTimeValueChanged != null && _oldCachedValue2 != _value)
            {
                RealTimeValueChanged(this, new ListViewChangedEventArgs(_oldCachedValue2, _value));
                _oldCachedValue2 = _value;
            }
        }

        /// <summary>
        /// Updates the difference.
        /// </summary>
        private void UpdateDifference()
        {
            _difference = _maximum - _minimum;
            UpdateValue();
        }

        /// <summary>
        /// Updates the value.
        /// </summary>
        private void UpdateValue()
        {
            if (_bulletTransform != null)
            {
                switch (_orientation)
                {
                    case Orientation.Vertical:
                        _value = (int)(_minimum + ((_bulletTransform.Y * _difference) / _maximunOffset));
                        break;
                    case Orientation.Horizontal:
                        _value = (int)(_minimum + ((_bulletTransform.X * _difference) / _maximunOffset));
                        break;
                }
            }
            else
            {
                _value = _minimum;
            }
        }

        /// <summary>
        /// Updates the width horizontal offset.
        /// </summary>
        /// <param name="offsetX">The offset X.</param>
        private void UpdateWidthHorizontalOffset(float offsetX)
        {
            float result;

            if (offsetX < _maximunOffsetOver2)
            {
                if (offsetX <= _bulletWeightOver2)
                {
                    result = 0;
                }
                else
                {
                    result = offsetX - _bulletWeightOver2;
                }
            }
            else
            {
                result = _maximunOffset;
            }

            if (Panel.Width < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
            {
                var maxTextOffset = (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count - Panel.Width;
                var resultText = (int)(0 - ((offsetX * (maxTextOffset - 0)) / Panel.Width));
                _listBoxControl.PosX = resultText;
            }

            _bulletTransform.X = result;
            _foregroundTransform.XScale = result;

            UpdateValue();
        }

        /// <summary>
        /// Updates the width vertical offset.
        /// </summary>
        /// <param name="offsetY">The offset Y.</param>
        public void UpdateWidthVerticalOffset(float offsetY)
        {
            float result;
            if (offsetY < _maximunOffsetOver2)
            {
                if (offsetY > _bulletWeightOver2)
                {
                    result = offsetY - _bulletWeightOver2;
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                result = _maximunOffset;
            }

            if (Panel.Height < (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count)
            {
                var maxTextOffset = Panel.Height - (_listBoxControl.ItemHeight + 5) * _listBoxControl.Count;
                var resultText = (int)(0 - ((offsetY * (maxTextOffset - 0)) / Panel.Height));
                _listBoxControl.PosY = -resultText;
            }

            _bulletTransform.Y = result;
            _foregroundTransform.YScale = -result;

            UpdateValue();
        }

        /// <summary>
        /// Updates the Layer.
        /// </summary>
        public void UpdateLayer()
        {
            switch (_orientation)
            {
                case Orientation.Vertical:

                    if (_backgroundImage != null && _foregroundImage != null && _foregroundTransform != null && _bulletImage != null &&
                        _bulletTransform != null && Panel != null && _listBoxControl != null)
                    {
                        _maximunOffset = Panel.Height - _bulletImage.Height;
                        _bulletWeightOver2 = _bulletImage.Height / 2;
                        _maximunOffsetOver2 = Panel.Height - _bulletImage.Height - _bulletWeightOver2;

                        _backgroundImage.HorizontalAlignment = HorizontalAlignment.Right;
                        _backgroundImage.VerticalAlignment = VerticalAlignment.Top;
                        _backgroundImage.Width = DefaultSliderWeight;
                        _backgroundImage.Height = Panel.Height;

                        _foregroundImage.Width = DefaultSliderWeight;
                        _foregroundImage.Height = 1;
                        _foregroundImage.HorizontalAlignment = HorizontalAlignment.Right;
                        _foregroundImage.VerticalAlignment = VerticalAlignment.Top;
                        _foregroundImage.Margin = Thickness.Zero;

                        _foregroundTransform.Origin = Vector2.UnitX / 2;
                        _foregroundTransform.Rotation = MathHelper.Pi;
                        _foregroundTransform.XScale = 1;

                        _bulletImage.HorizontalAlignment = HorizontalAlignment.Right;
                        _bulletImage.VerticalAlignment = VerticalAlignment.Top;
                        _bulletImage.Margin = Thickness.Zero;

                        _bulletTransform.X = 0;
                        _bulletTransform.Y = 0;

                        _listBoxControl.Orientation = Orientation.Vertical;
                        _listBoxControl.Margin = new Thickness(Panel.Margin.Left, Panel.Margin.Top,
                                                               Panel.Margin.Right + DefaultSliderWeight + 5, Panel.Margin.Bottom);
                        _listBoxControl.PosX = 0;
                        _listBoxControl.PosY = 0;
                    }

                    break;
                case Orientation.Horizontal:

                    if (_backgroundImage != null && _foregroundImage != null && _foregroundTransform != null && _bulletImage != null &&
                        _bulletTransform != null && Panel != null && _listBoxControl != null)
                    {
                        _maximunOffset = Panel.Width - _bulletImage.Width;
                        _bulletWeightOver2 = _bulletImage.Width / 2;
                        _maximunOffsetOver2 = Panel.Width - _bulletWeightOver2;

                        _backgroundImage.HorizontalAlignment = HorizontalAlignment.Left;
                        _backgroundImage.VerticalAlignment = VerticalAlignment.Bottom;
                        _backgroundImage.Width = Panel.Width;
                        _backgroundImage.Height = DefaultSliderWeight;

                        _foregroundImage.Height = DefaultSliderWeight;
                        _foregroundImage.Width = 1;
                        _foregroundImage.HorizontalAlignment = HorizontalAlignment.Left;
                        _foregroundImage.VerticalAlignment = VerticalAlignment.Bottom;
                        _foregroundImage.Margin = Thickness.Zero;

                        _foregroundTransform.Origin = Vector2.Zero;
                        _foregroundTransform.Rotation = 0;
                        _foregroundTransform.YScale = 1;

                        _bulletImage.HorizontalAlignment = HorizontalAlignment.Left;
                        _bulletImage.VerticalAlignment = VerticalAlignment.Bottom;
                        _bulletImage.Margin = Thickness.Zero;

                        _bulletTransform.X = 0;
                        _bulletTransform.Y = 0;

                        _listBoxControl.Orientation = Orientation.Horizontal;
                        _listBoxControl.Margin = new Thickness(Panel.Margin.Left, Panel.Margin.Top, Panel.Margin.Right,
                                                               Panel.Margin.Bottom + DefaultSliderWeight + 5);
                        _listBoxControl.PosX = 0;
                        _listBoxControl.PosY = 0;
                    }

                    break;
            }
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="Component" />, or the <see cref="Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            WaveServices.Layout.PerformLayout();
        }

        #endregion
    }
}
