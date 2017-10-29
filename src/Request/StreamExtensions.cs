namespace Botwin.Request
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        /// <summary>
        /// Gets the RequestBody Stream as string in the specified <see cref="Encoding"/> (optional)
        /// </summary>
        /// <param name="stream">Current <see cref="Stream"/></param>
        /// <param name="encoding">The character encoding to use or <see cref="Encoding.UTF8"/> by default</param>
        /// <returns>Current string content of the RequestBody Stream</returns>
        public static string AsString(this Stream stream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                var readStream = reader.ReadToEnd();

                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                return readStream;
            }
        }

        /// <summary>
        /// Gets the RequestBody Stream as string asynchronously in the specified <see cref="Encoding"/> (optional)
        /// </summary>
        /// <param name="stream">Current <see cref="Stream"/></param>
        /// <param name="encoding">The character encoding to use or <see cref="Encoding.UTF8"/> by default</param>
        /// <param name="cancellationToken">The cancellation instruction if required</param>
        /// <returns>Awaited <see cref="Task"/></returns>
        public static async Task<string> AsStringAsync(this Stream stream, Encoding encoding = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                var readStream = await reader.ReadToEndAsync();

                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                return readStream;
            }
        }
    }
}
