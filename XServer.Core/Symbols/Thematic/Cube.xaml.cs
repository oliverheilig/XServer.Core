﻿using System.Windows;
using System.Windows.Media;


namespace Ptv.XServer.Controls.Map.Symbols
{
    /// <summary> Interaction logic for Cube.xaml. </summary>
	public partial class Cube
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cube"/> class.
        /// </summary>
		public Cube()
		{
			// Required to initialize variables
			InitializeComponent();

			SetValue(LightColorProperty, Color.Lighten(1.5f));
            SetValue(DarkColorProperty, Color.Lighten(0.5f));
		}

        /// <summary>
        /// Gets or sets the color of this cube.
        /// </summary>
		public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the stroke of this cube.
        /// </summary>
        public Color Stroke
        {
            get => (Color)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        /// <summary> Documentation in progress... </summary>
        /// <param name="d"> Documentation in progress... </param>
        /// <param name="e"> Documentation in progress... </param>
        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
             d.SetValue(LightColorProperty, ((Color)e.NewValue).Lighten(1.5f));
             d.SetValue(DarkColorProperty, ((Color)e.NewValue).Lighten(0.5f));
		}

        /// <summary> Using a DependencyProperty as the backing store for DarkColorProperty.  This enables animation, styling, binding, etc...</summary>
        public static readonly DependencyProperty DarkColorProperty =   
            DependencyProperty.Register("DarkColor", typeof(Color), typeof(Cube));

        /// <summary> Using a DependencyProperty as the backing store for LightColor.  This enables animation, styling, binding, etc...</summary>
        public static readonly DependencyProperty LightColorProperty =
            DependencyProperty.Register("LightColor", typeof(Color), typeof(Cube));

        /// <summary> Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...</summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Cube), new PropertyMetadata(Colors.Blue, ColorChanged));

        /// <summary> Using a DependencyProperty as the backing store for Stroke Color.  This enables animation, styling, binding, etc...</summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Color), typeof(Cube), new PropertyMetadata(Colors.Black));
	}
}