using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Common.Graphics;
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
    public class ListBoxBehavior : Behavior
    {
        #region Fields

        private readonly List<string> _listString = new List<string>();
        private int _needsInit, _needsUpdate;
        private StackPanel _panel = new StackPanel();

        #endregion

        public ListBoxBehavior(Color foreground)
        {
            Foreground = foreground;
            DrawListBox();
        }

        #region Properties

        public Color Foreground { get; set; }

        private float _itemHeight = 30f;

        public float ItemHeight
        {
            get { return _itemHeight; }
            set { _itemHeight = value; }
        }

        private float _posX;
        public float PosX
        {
            get { return _posX; }
            set
            {
                _posX = value;
                _panel.Margin = new Thickness(Margin.Left + PosX, Margin.Top + PosY, Margin.Right, Margin.Bottom);
            }
        }

        private float _posY;
        public float PosY
        {
            get { return _posY; }
            set
            {
                _posY = value;
                _panel.Margin = new Thickness(Margin.Left + PosX, Margin.Top + PosY, Margin.Right, Margin.Bottom);
            }
        }

        private Thickness _margin = Thickness.Zero;
        public Thickness Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                _margin = value;
                _needsUpdate = 1;
            }
        }

        private Orientation _orientation = Orientation.Horizontal;
        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                _needsUpdate = 1;
            }
        }

        public string FontPath { private get; set; }

        #endregion

        #region Public Methods

        #region Common List Methods

        public int Count
        {
            get { return _listString.Count; }
        }

        public string Last()
        {
            return _listString.Any() ? _listString.Last() : string.Empty;
        }

        public void Add(string a)
        {
            _listString.Add(a);

            _needsUpdate = 1;
        }

        public void Remove(string b)
        {
            _listString.Remove(b);

            _needsUpdate = 1;
        }

        public void RemoveAt(int index)
        {
            _listString.RemoveAt(index);

            _needsUpdate = 1;
        }

        public string GetAt(int index)
        {
            string text;

            if (index < 0) text = _listString[0];
            else if (index > _listString.Count - 1) text = _listString.Last();
            else text = _listString[index];

            return text;
        }

        #endregion

        public void DrawListBox()
        {
            _panel = new StackPanel
            {
                Orientation = Orientation,
                Margin = new Thickness(Margin.Left + PosX, Margin.Top + PosY, Margin.Right, Margin.Bottom)
            };
            _panel.Entity.FindComponent<StackPanelRenderer>().LayerType = DefaultLayers.Alpha;

            foreach (var text in _listString)
            {
                var item = new TextBlock
                {
                    Text = text,
                    Margin = new Thickness(0, 5, 0, 0),
                    Foreground = Foreground,
                    Height = ItemHeight
                };
                if (!string.IsNullOrWhiteSpace(FontPath))
                {
                    item.FontPath = FontPath;
                }
                item.Entity.FindChild("TextEntity").FindComponent<TextControlRenderer>().LayerType = DefaultLayers.Alpha;
                _panel.Add(item);
            }
        }

        #endregion

        #region Protected Methods

        protected override void Update(TimeSpan elapsedTime)
        {
            if (_needsInit == 0)
            {
                EntityManager.Add(_panel.Entity);

                _needsInit++;

                WaveServices.Layout.PerformLayout();
            }

            if (_needsUpdate == 1)
            {
                EntityManager.Remove(_panel.Entity.Name);
                DrawListBox();
                EntityManager.Add(_panel.Entity);

                _needsUpdate++;
            }
        }

        #endregion
    }
}
