namespace Carter.Request
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        /// <summary>
        /// Gets the <see cref="HttpRequest"/> Body <see cref="Stream"/> as <see cref="String"/> in the optional <see cref="Encoding"/>
        /// </summary>
        /// <param name="stream">Current <see cref="Stream"/></param>
        /// <param name="encoding">The character encoding to use or <see cref="Encoding.UTF8"/> by default</param>
        /// <returns>Current content of the Body</returns>
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
        /// Gets the <see cref="HttpRequest" /> Body <see cref="Stream"/> as <see cref="String"/> asynchronously in the optional <see cref="Encoding"/>
        /// </summary>
        /// <param name="stream">Current <see cref="Stream"/></param>
        /// <param name="encoding">The character encoding to use or <see cref="Encoding.UTF8"/> by default</param>
        /// <param name="cancellationToken">The cancellation instruction if required</param>
        /// <returns>Awaited <see cref="Task{String}"/></returns>
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
