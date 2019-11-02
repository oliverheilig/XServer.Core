using Ptv.XServer.Controls.Map.Layers.Shapes;
using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.Symbols;
using Ptv.XServer.Controls.Map.TileProviders;
using Ptv.XServer.Controls.Map.Tools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XServer.Core.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // go to Karlsruhe castle
            var myLocation = new Point(8.4044, 49.01405);
            Map.SetMapLocation(myLocation, 14);

            // add OSM tiles
            Map.Layers.Add(new TiledLayer("OSM_DE")
            {
                Caption = "OpenStreetMap.DE",
                IsBaseMapLayer = true,
                TiledProvider = new RemoteTiledProvider
                {
                    MinZoom = 0,
                    MaxZoom = 18,
                    RequestBuilderDelegate = (x, y, level) =>
                        $"https://{"abc"[(x ^ y) % 3]}.tile.openstreetmap.de/tiles/osmde/{level}/{x}/{y}.png",
                },
                Copyright = $"Map © OpenStreetMap contributors",
                Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Background.png")
            });

            // add stuff
            var myLayer = new ShapeLayer("MyLayer");
            Map.Layers.Add(myLayer);

            // add push-pin
            Control pin = new Pin
            {
                Width = 50, // regarding scale fatoring
                Height = 50,
                ToolTip = "I am a pin!",
                Color = Colors.Red
            };
            ShapeCanvas.SetLocation(pin, myLocation);
            ShapeCanvas.SetAnchor(pin, LocationAnchor.RightBottom);
            ShapeCanvas.SetScaleFactor(pin, 0.125);
            Panel.SetZIndex(pin, 100);
            myLayer.Shapes.Add(pin);

            // add geographic circle
            double radius = 435; // radius in meters
            double cosB = Math.Cos(myLocation.Y / 360.0 * (2 * Math.PI)); // factor depends on latitude
            double ellipseSize = Math.Abs(1.0 / cosB * radius) * 2; // size mercator units

            var ellipse = new Ellipse
            {
                Width = ellipseSize,
                Height = ellipseSize,
                Fill = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255)),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = radius / 20,
                ToolTip = "I am a circle!"
            };

            ShapeCanvas.SetScaleFactor(ellipse, 1); // scale linear
            ShapeCanvas.SetLocation(ellipse, myLocation);
            myLayer.Shapes.Add(ellipse);
        }
    }
}
