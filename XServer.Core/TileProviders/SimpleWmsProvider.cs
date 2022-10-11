using System.Globalization;
using System.IO;
using System.Net;
using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.Tools;
using Ptv.XServer.Controls.Map.Layers.Untiled;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Threading;

namespace Ptv.XServer.Controls.Map.TileProviders
{
    /// <summary> Provider delivering imagery from a wms service. </summary>
    public class WmsUntiledProvider : IUntiledProvider
    {
        /// <summary> Url of the wms service. </summary>
        protected string baseUrl;

        /// <summary> Conversion factor from Google Mercator to PTV Mercator. </summary>
        private const double GOOGLE_TO_PTV = 6371000.0 / 6378137.0;
        
        /// <summary> Gets or sets the minimum level where tiles are available. </summary>
        public int MinZoom { get; set; }

        /// <summary> Gets or sets the maximum level where tiles are available. </summary>
        public int MaxZoom { get; set; }
        
        #region constructor
        /// <summary> Initializes a new instance of the <see cref="WmsUntiledProvider"/> class. </summary>
        /// <param name="baseUrl"> Url of the wms service. </param>
        /// <param name="maxZoom"> Maximum zoom at which map tiles are available. </param>
        public WmsUntiledProvider(string baseUrl, int maxZoom)
        {
            this.baseUrl = baseUrl;
            MinZoom = 1;
            MaxZoom = maxZoom;
        }
        #endregion

        public HttpClient httpClient = new HttpClient();

        /// <summary> Reads the content from a given url and returns it as a stream. </summary>
        /// <param name="url"> The url to look for. </param>
        /// <returns> The url content as a stream. </returns>
        public async Task<Stream> ReadURL(string url, CancellationToken ct)
        {
            try
            {
                var b = await httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead, ct);
                if (b.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                else
                    return await b.Content.ReadAsStreamAsync(ct);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
        
        #region IUntiledProvider Members
        /// <inheritdoc/>
        public async Task<Stream> GetImageStream(double left, double top, double right, double bottom, int width, int height, CancellationToken ct)
        {
            var url = string.Format(
                "{0}&BBOX={1},{2},{3},{4}&WIDTH={5}&HEIGHT={6}",
                baseUrl,
                System.Convert.ToString(left / GOOGLE_TO_PTV, NumberFormatInfo.InvariantInfo),
                System.Convert.ToString(top / GOOGLE_TO_PTV, NumberFormatInfo.InvariantInfo),
                System.Convert.ToString(right / GOOGLE_TO_PTV, NumberFormatInfo.InvariantInfo),
                System.Convert.ToString(bottom / GOOGLE_TO_PTV, NumberFormatInfo.InvariantInfo),
                System.Convert.ToString(width, NumberFormatInfo.InvariantInfo),
                System.Convert.ToString(height, NumberFormatInfo.InvariantInfo));

            try
            {
                return await ReadURL(url, ct);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }

    /// <summary> Provider delivering imagery from a wms service. </summary>
    public class WmsProvider : WmsUntiledProvider, ITiledProvider
    {
        #region constructor
        /// <summary> Initializes a new instance of the <see cref="WmsUntiledProvider"/> class. </summary>
        /// <param name="baseUrl"> Url of the wms service. </param>
        /// <param name="maxZoom"> Maximum zoom at which map tiles are available. </param>
        public WmsProvider(string baseUrl, int maxZoom) : base(baseUrl, maxZoom)
        {
            
        }
        #endregion

        #region ITiledProvider Members
        /// <inheritdoc/>
        public Task<Stream> GetImageStream(int x, int y, int zoom, CancellationToken ct)
        {
            // calc rect from tile key
            var rect = GeoTransform.TileToPtvMercatorAtZoom(x, y, zoom);

            return GetImageStream(rect.Left, rect.Top, rect.Right, rect.Bottom, 256, 256, ct);
        }

        public string CacheId
        {
            get { return baseUrl; }
        }
        #endregion
    }
}
