namespace Botwin.Request
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {

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
