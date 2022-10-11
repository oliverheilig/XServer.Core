using Newtonsoft.Json;
using Ptv.XServer.Controls.Map;
using Ptv.XServer.Controls.Map.Layers.Shapes;
using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.Localization;
using Ptv.XServer.Controls.Map.Symbols;
using Ptv.XServer.Controls.Map.TileProviders;
using Ptv.XServer.Controls.Map.Tools;
using RoutingClient.Model;
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

namespace PtvDeveloperTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string apiKey = ""; // Get your free key at https://developer.myptv.com/;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("You need an api key! Get your free key at https://developer.myptv.com/");
                Application.Current.Shutdown();
                return;
            }

            // Increase the number of parallel requests
            System.Net.ServicePointManager.DefaultConnectionLimit = 8;

            // The start and end locations Karlsrhe -> Berlin
            Point pStart = new Point(8.403951, 49.00921);
            Point pDest = new Point(13.408333, 52.518611);

            // Create a new shape layer containing the route result
            var shapeLayer = new ShapeLayer("Routing");
            Map.Layers.Add(shapeLayer);

            // Add markers for start and destination
            AddMarker(shapeLayer, pStart, Colors.Green, "Karlsruhe");
            AddMarker(shapeLayer, pDest, Colors.Red, "Berlin");

            // Set the map focus
            Map.SetEnvelope(new MapRectangle(pStart, pDest).Inflate(1.25));

            // Initialize the routing client
            var routingApi = new RoutingClient.Api.RoutingApi(new RoutingClient.Client.Configuration
            {
                ApiKey = new Dictionary<string, string>
                {
                    ["apiKey"] = apiKey
                }
            });

            // Calculate the route
            var routeResult = await routingApi.CalculateRoutePostAsync(new RouteRequest(waypoints: new List<Waypoint>
                {
                    new Waypoint{OffRoad = new OffRoadWaypoint{Longitude = pStart.X, Latitude = pStart.Y}},
                    new Waypoint{OffRoad = new OffRoadWaypoint{Longitude = pDest.X, Latitude = pDest.Y}}
                }),
                results: new List<Results> {
                    Results.POLYLINE,
            });

            // The result is GeoJson, need to parse it vis Json.NET
            dynamic polyline = JsonConvert.DeserializeObject(routeResult.Polyline);
            var points = new PointCollection();
            foreach (var c in polyline.coordinates)
                points.Add(new Point((double)c[0], (double)c[1]));

            // Add the route polygon
            var mp = new MapPolyline
            {
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                MapStrokeThickness = 25,
                ScaleFactor = .1,
                Points = points,
                Stroke = new SolidColorBrush(Colors.Blue),
                ToolTip = $"Distance: {routeResult.Distance / 1000} km, Travel Time: {routeResult.TravelTime / 60} min"
            };

            shapeLayer.Shapes.Add(mp);
        }
        private void AddMarker(ShapeLayer layer, Point p, Color color, string toolTip)
        {
            // craetae a pin-style symbol
            var pin = new Pin
            {
                Color = color,
                Width = 40,
                Height = 40,
                ToolTip = toolTip,
            };

            // move in-front of route
            ShapeCanvas.SetZIndex(pin, 10);

            // Sets Anchor and Location of the pin.
            ShapeCanvas.SetAnchor(pin, LocationAnchor.RightBottom);
            ShapeCanvas.SetLocation(pin, p);

            // Adds the pin to the layer.
            layer.Shapes.Add(pin);
        }
    private void MapStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Map.Layers.Remove(Map.Layers["Road"]);
            Map.Layers.Remove(Map.Layers["Satellite"]);

            switch ((e.AddedItems[0] as ContentControl)?.Content)
            {
                case "Road":
                    // Add the PTV-Developer raster map as base map
                    Map.Layers.Add(new TiledLayer("Road")
                    {
                        TiledProvider = new RemoteTiledProvider
                        {
                            MinZoom = 0,
                            MaxZoom = 22,
                            RequestBuilderDelegate = (x, y, z) =>
                               $"https://api.myptv.com/rastermaps/v1/image-tiles/{z}/{x}/{y}?style=silkysand&apiKey={apiKey}",
                        },
                        IsBaseMapLayer = true,
                        Copyright = "© 2022 PTV Group, HERE",
                        Caption = MapLocalizer.GetString(MapStringId.Background),
                        Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Background.png")
                    });
                    break;
                case "Satellite":
                    // Add the PTV-Developer satellite map as base map
                    Map.Layers.Add(new TiledLayer("Satellite")
                    {
                        TiledProvider = new RemoteTiledProvider
                        {
                            MinZoom = 0,
                            MaxZoom = 20,
                            RequestBuilderDelegate = (x, y, z) =>
                               $"https://api.myptv.com/rastermaps/v1/satellite-tiles/{z}/{x}/{y}?apiKey={apiKey}",
                        },
                        IsBaseMapLayer = true,
                        Copyright = "© 2022 PTV Group, HERE",
                        Caption = MapLocalizer.GetString(MapStringId.Aerials),
                        Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Aerials.png")
                    });
                    break;
                case "Hybrid":
                    // Add the PTV-Developer satellite map as base map
                    Map.Layers.Add(new TiledLayer("Satellite")
                    {
                        TiledProvider = new RemoteTiledProvider
                        {
                            MinZoom = 0,
                            MaxZoom = 20,
                            RequestBuilderDelegate = (x, y, z) =>
                               $"https://api.myptv.com/rastermaps/v1/satellite-tiles/{z}/{x}/{y}?apiKey={apiKey}",
                        },
                        IsBaseMapLayer = true,
                        Copyright = "© 2022 PTV Group, HERE",
                        Caption = MapLocalizer.GetString(MapStringId.Aerials),
                        Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Aerials.png")
                    });
                    // Add the PTV-Developer raster map as overlay
                    Map.Layers.Add(new TiledLayer("Road")
                    {
                        TiledProvider = new RemoteTiledProvider
                        {
                            MinZoom = 0,
                            MaxZoom = 22,
                            RequestBuilderDelegate = (x, y, z) =>
                               $"https://api.myptv.com/rastermaps/v1/image-tiles/{z}/{x}/{y}?style=silkysand&layers=transport,labels&apiKey={apiKey}",
                        },
                        IsBaseMapLayer = true,
                        Copyright = "© 2022 PTV Group, HERE",
                        Caption = MapLocalizer.GetString(MapStringId.Background),
                        Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Background.png")
                    });
                    break;
            }

        }
    }
}
