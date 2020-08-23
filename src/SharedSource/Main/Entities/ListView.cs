using System;
using CrossingFingers_Wave.Behaviors;
using CrossingFingers_Wave.Events;
using WaveEngine.Common.Graphics;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.UI;

namespace CrossingFingers_Wave.Entities
{
    /// <summary>
    /// [Deprecated] UI ListView decorate class
    /// </summary>
    public class ListView : UIBase
    {
        #region Fields

        /// <summary>
        /// The instances
        /// </summary>
        private static int _instances;

        private float _height, _width;

        private Thickness _margin = Thickness.Zero;

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
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value
        {
            get
            {
                return entity.FindComponent<ListViewBehavior>().Value;
            }
            set
            {
                entity.FindComponent<ListViewBehavior>().Value = value;
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
                return entity.FindComponent<ListViewBehavior>().Orientation;
            }
            set
            {
                entity.FindComponent<ListViewBehavior>().Orientation = value;
            }
        }

        /// <summary>
        /// Gets or sets the margin.
        /// </summary>
        /// <value>
        /// The margin.
        /// </value>
        protected Thickness Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                _margin = value;
                entity.FindComponent<PanelControl>().Margin = value;
                entity.FindComponent<ListBoxBehavior>().Margin = value;
                entity.FindComponent<ListViewBehavior>().UpdateLayer();
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                entity.FindComponent<PanelControl>().Width = value;
                entity.FindComponent<ListViewBehavior>().UpdateWidth = value;
                entity.FindComponent<ListViewBehavior>().UpdateLayer();
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public float Height
        {
            get { return _height; }
            set
            {
                if (_height.Equals(0)) value = value - Margin.Top - Margin.Bottom;

                _height = value;
                
                entity.FindComponent<PanelControl>().Height = value;
                entity.FindComponent<ListViewBehavior>().UpdateHeight = value;
                entity.FindComponent<ListViewBehavior>().UpdateLayer();
            }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public Color Foreground
        {
            get
            {
                return entity.FindComponent<ListBoxBehavior>().Foreground;
            }
            set
            {
                entity.FindComponent<ListBoxBehavior>().Foreground = value;
            }
        }

        /// <summary>
        /// Gets or sets the Scroll Bar Bullet Color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color ScrollBarBulletColor
        {
            get
            {
                return entity.FindChild("BulletEntity").FindComponent<ImageControl>().TintColor;
            }
            set
            {
                entity.FindChild("BulletEntity").FindComponent<ImageControl>().TintColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Scroll Bar Foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public Color ScrollBarForeground
        {
            get
            {
                return entity.FindChild("ForegroundEntity").FindComponent<ImageControl>().TintColor;
            }
            set
            {
                entity.FindChild("ForegroundEntity").FindComponent<ImageControl>().TintColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Scroll Bar Background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public Color ScrollBarBackground
        {
            get
            {
                return entity.FindChild("BackgroundEntity").FindComponent<ImageControl>().TintColor;
            }
            set
            {
                entity.FindChild("BackgroundEntity").FindComponent<ImageControl>().TintColor = value;
            }
        }

        /// <summary>
        /// Sets the font.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public string FontPath
        {
            set
            {
                entity.FindComponent<ListBoxBehavior>().FontPath = value;
            }
        }

        /// <summary>
        /// Sets the Height of each Item.
        /// </summary>
        /// <value>
        /// The Height of each Item.
        /// </value>
        public float ItemHeight
        {
            set
            {
                entity.FindComponent<ListBoxBehavior>().ItemHeight = value;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ListView" /> class.
        /// </summary>
        public ListView(Thickness margin) : this("ListView" + _instances++, margin)
        {
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ListView" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="margin">The margin</param>
        public ListView(string name, Thickness margin)
        {
            entity = new Entity(name)
                .AddComponent(new Transform2D())
                .AddComponent(new RectangleCollider2D())
                .AddComponent(new TouchGestures(false))
                .AddComponent(new PanelControl())
                .AddComponent(new PanelControlRenderer
                {
                    LayerType = DefaultLayers.Alpha
                })
                .AddComponent(new ListViewBehavior())
                .AddComponent(new ListBoxBehavior(Color.Black))
                .AddChild(new Entity("BackgroundEntity")
                    .AddComponent(new Transform2D
                    {
                        DrawOrder = 0.5f
                    })
                    .AddComponent(new ImageControl(Color.Blue, 1, 1))
                    .AddComponent(new ImageControlRenderer
                    {
                        LayerType = DefaultLayers.Alpha
                    }))
                .AddChild(new Entity("ForegroundEntity")
                    .AddComponent(new Transform2D
                    {
                        DrawOrder = 0.45f
                    })
                    .AddComponent(new ImageControl(Color.LightBlue, 1, 1))
                    .AddComponent(new ImageControlRenderer
                    {
                        LayerType = DefaultLayers.Alpha
                    }))
                .AddChild(new Entity("BulletEntity")
                    .AddComponent(new Transform2D
                    {
                        DrawOrder = 0.4f
                    })
                    .AddComponent(new ImageControl(Color.White, 1, 1))
                    .AddComponent(new ImageControlRenderer
                    {
                        LayerType = DefaultLayers.Alpha
                    }));

            // Events
            entity.FindComponent<ListViewBehavior>().ValueChanged -= OnSliderValueChanged;
            entity.FindComponent<ListViewBehavior>().RealTimeValueChanged -= OnSliderRealTimeValueChanged;
            entity.FindComponent<ListViewBehavior>().ValueChanged += OnSliderValueChanged;
            entity.FindComponent<ListViewBehavior>().RealTimeValueChanged += OnSliderRealTimeValueChanged;

            Margin = margin;
        }

        public override void Dispose()
        {
            base.Dispose();

            entity.FindComponent<ListViewBehavior>().ValueChanged -= OnSliderValueChanged;
            entity.FindComponent<ListViewBehavior>().RealTimeValueChanged -= OnSliderRealTimeValueChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the specified Text.
        /// </summary>
        /// <param name="text">The Text to add.</param>
        /// <exception cref="System.ArgumentNullException">text component is empty.</exception>
        public void Add(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException(text, "text is empty.");

            entity.FindComponent<ListBoxBehavior>().Add(text);

            entity.FindComponent<ListViewBehavior>().ScrollToEndAtAddIfNeeded();
        }

        public int Count()
        {
            return entity.FindComponent<ListBoxBehavior>().Count;
        }

        public void RemoveAt(int index)
        {
            entity.FindComponent<ListBoxBehavior>().RemoveAt(index);

            entity.FindComponent<ListViewBehavior>().ScrollToEndAtRemoveIfNeeded();
        }

        public string GetAt(int index)
        {
            return entity.FindComponent<ListBoxBehavior>().GetAt(index);
        }

        public string Last()
        {
            return entity.FindComponent<ListBoxBehavior>().Last();
        }

        /// <summary>
        /// Removes the specified Text.
        /// </summary>
        /// <param name="text">The Text to remove.</param>
        /// <exception cref="System.ArgumentNullException">text component is empty.</exception>
        public void Remove(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                entity.FindComponent<ListBoxBehavior>().Remove(text);

                entity.FindComponent<ListViewBehavior>().ScrollToEndAtRemoveIfNeeded();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the ValueChanged event of the Slider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnSliderValueChanged(object sender, ListViewChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, e);
            }
        }

        /// <summary>
        /// Handles the RealTimeValueChanged event of the Slider control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ListViewChangedEventArgs" /> instance containing the event data.</param>
        private void OnSliderRealTimeValueChanged(object sender, ListViewChangedEventArgs e)
        {
            if (RealTimeValueChanged != null)
            {
                RealTimeValueChanged.Invoke(this, e);
            }
        }

        #endregion
    }
}
