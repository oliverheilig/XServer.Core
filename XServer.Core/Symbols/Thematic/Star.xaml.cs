﻿using System.Windows;
using System.Windows.Media;


namespace Ptv.XServer.Controls.Map.Symbols
{
	/// <summary> Interaction logic for Star.xaml. </summary>
    public partial class Star
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="Star"/> class.
        /// </summary>
		public Star()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Gets or sets the color of this star.
        /// </summary>
		public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the stroke of this star.
        /// </summary>
        public Color Stroke
        {
            get => (Color)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        /// <summary> Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...</summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Star), new PropertyMetadata(Colors.Blue));

        /// <summary> Using a DependencyProperty as the backing store for Stroke Color.  This enables animation, styling, binding, etc...</summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Color), typeof(Star), new PropertyMetadata(Colors.Black));
    }
}