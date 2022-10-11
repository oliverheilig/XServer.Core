using Ptv.XServer.Controls.Map.Layers.Shapes;
using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.Layers.Untiled;
using Ptv.XServer.Controls.Map.Localization;
using Ptv.XServer.Controls.Map.Symbols;
using Ptv.XServer.Controls.Map.TileProviders;
using Ptv.XServer.Controls.Map.Tools;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XMap1Test
{
    public partial class Form1 : Form
    {
        private static readonly string token = ""; // Your xserver-internet token;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("You need an xserver-internet token to run this sample.");
                Application.ExitThread();
                return;
            }

            // Create the ElementHost control for hosting the
            // WPF UserControl.
            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            // Create the WPF UserControl.
            var map =
                new Ptv.XServer.Controls.Map.WpfMap();

            // Assign the WPF UserControl to the ElementHost control's
            // Child property.
            host.Child = map;

            // Add the ElementHost control to the form's
            // collection of child controls.
            this.Controls.Add(host);

            map.Layers.Add(new TiledLayer("Background")
            {
                TiledProvider = new RemoteTiledProvider
                {
                    MinZoom = 0,
                    MaxZoom = 22,
                    RequestBuilderDelegate = (x, y, z) =>
                            $"https://api{1 + (x + y) % 4}-test.cloud.ptvgroup.com/WMS/GetTile/xmap-silkysand-bg/{x}/{y}/{z}.png",
                },
                Copyright = $"© {DateTime.Now.Year} PTV AG, TomTom",
                IsBaseMapLayer = true,
                Caption = MapLocalizer.GetString(MapStringId.Background),
                Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Background.png")
            });

            // -fg layers require the xServer-internet token
            map.Layers.Add(new UntiledLayer("Labels")
            {
                UntiledProvider = new WmsUntiledProvider(
                    $"https://api-test.cloud.ptvgroup.com/WMS/WMS?xtok={token}&service=WMS&request=GetMap&version=1.1.1&layers=xmap-silkysand-fg&styles=&format=image%2Fpng&transparent=true&srs=EPSG%3A3857", 19),
                Copyright = $"© {DateTime.Now.Year} PTV AG, TomTom",
                Caption = MapLocalizer.GetString(MapStringId.Labels),
                Icon = ResourceHelper.LoadBitmapFromResource("Ptv.XServer.Controls.Map;component/Resources/Labels.png")
            });

            // go to Karlsruhe castle
            var myLocation = new System.Windows.Point(8.4044, 49.01405);
            map.SetMapLocation(myLocation, 14);

            var myLayer = new ShapeLayer("MyLayer")
            {
                LocalOffset = myLocation // this new property eliminates jitter at deep zoom levels
            };
            map.Layers.Add(myLayer);

            // add push-pin
            var pin = new Pin
            {
                Width = 50, // regarding scale fatoring
                Height = 50,
                ToolTip = "I am a pin!",
                Color = Colors.Red
            };
            ShapeCanvas.SetLocation(pin, myLocation);
            ShapeCanvas.SetAnchor(pin, LocationAnchor.RightBottom);
            ShapeCanvas.SetScaleFactor(pin, 0.1);
            System.Windows.Controls.Panel.SetZIndex(pin, 100);
            myLayer.Shapes.Add(pin);

            // add geographic circle
            double radius = 435; // radius in meters
            double cosB = Math.Cos(myLocation.Y / 360.0 * (2 * Math.PI)); // factor depends on latitude
            double ellipseSize = Math.Abs(1.0 / cosB * radius) * 2; // size mercator units

            var ellipse = new Ellipse
            {
                Width = ellipseSize,
                Height = ellipseSize,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 0, 0, 255)),
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