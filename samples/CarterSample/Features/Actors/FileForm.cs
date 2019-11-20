using Carter.OpenApi;

namespace CarterSample.Features.Actors
{
    public class FileForm
    {
        public File File { get; set; }

        [ApiSchemaAttributes(Format = "binary")]
        public string FileBinary { get; set; }
    }
}
