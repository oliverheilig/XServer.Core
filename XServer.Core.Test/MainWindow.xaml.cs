using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.TileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
            Map.SetMapLocation(new Point(8.4, 49), 10);

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
                Copyright = $"Map © OpenStreetMap contributors"
            });

        }
    }
}
