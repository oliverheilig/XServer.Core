// This source file is covered by the LICENSE.TXT file in the root folder of the SDK.

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ptv.XServer.Controls.Map.Layers.Tiled;
using Ptv.XServer.Controls.Map.Tools;

namespace Ptv.XServer.Controls.Map.TileProviders
{
    /// <summary> Provider loading tiled bitmaps from a given url. </summary>
    public class RemoteTiledProvider : ITiledProvider
    {
        public HttpClient httpClient = new HttpClient();

        /// <summary> Initializes a new instance of the <see cref="RemoteTiledProvider"/> class. </summary>
        public RemoteTiledProvider()
        {
            MinZoom = 0;
            MaxZoom = 19;
        }

        /// <inheritdoc/>
        public async Task<Stream> GetImageStream(int tileX, int tileY, int zoom, CancellationToken ct)
        {
            return await ReadURL(RequestBuilderDelegate(tileX, tileY, zoom), ct);
        }

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
                    return await b.Content.ReadAsStreamAsync();
            }
            catch(OperationCanceledException)
            {
                return null;
            }
            catch(HttpRequestException)
            {
                return null;
            }
        }

        /// <summary> Gets or sets a method which can be used to build a request. </summary>
        public RequestBuilder RequestBuilderDelegate { get; set; }

        /// <summary> Method building a request by the given tile information. </summary>
        /// <param name="x"> X coordinate of the requested tile. </param>
        /// <param name="y"> Y coordinate of the requested tile. </param>
        /// <param name="level"> Zoom level of the requested tile. </param>
        /// <returns> The request string. </returns>
        public delegate string RequestBuilder(int x, int y, int level);

        /// <inheritdoc/>
        public string CacheId => RequestBuilderDelegate(0, 0, 0);

        /// <inheritdoc/>
        public int MinZoom {get; set; }

        /// <inheritdoc/>
        public int MaxZoom { get; set; }

        /// <summary> Logging restricted to this class. </summary>
        private static readonly Logger logger = new Logger("RemoteTiledProvider");
    }
}
