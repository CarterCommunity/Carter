namespace Botwin.Request
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        public static string AsString(this Stream stream)
        {
            var reader = new StreamReader(stream);

            var readStream = reader.ReadToEnd();

            stream.Position = 0;

            return readStream;
        }

        public static string AsString(this Stream stream, Encoding encoding)
        {
            var reader = new StreamReader(stream, encoding);

            var readStream = reader.ReadToEnd();

            stream.Position = 0;

            return readStream;
        }

        public static async Task<string> AsStringAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            var reader = new StreamReader(stream);

            var readStream = await reader.ReadToEndAsync();

            stream.Position = 0;

            return readStream;
        }

        public static async Task<string> AsStringAsync(this Stream stream, Encoding encoding, CancellationToken cancellationToken = default)
        {
            var reader = new StreamReader(stream, encoding);

            var readStream = await reader.ReadToEndAsync();

            stream.Position = 0;

            return readStream;
        }
    }
}
