namespace CarterSample.Features.Actors.OpenApi
{
    using Carter.OpenApi;
    using System;
    using System.Net;
    using System.Net.Http;

    public class PostFileForm : RouteMetaData
    {
        public override string Tag { get; } = "Files";
        public override string Description { get; } = "Create a File.";
        public override RouteMetaDataRequest[] Requests { get; } =
        {
            new RouteMetaDataRequest
            {
                MediaType = "multipart/form-data",
                Request = typeof(FileForm),
                Description = "JSON file object, followed by a binary blob"
            }
        };
        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = (int)HttpStatusCode.Created,
                Description = "Created File.",
            },
            new RouteMetaDataResponse
            {
                Code = (int)HttpStatusCode.UnprocessableEntity,
                Description = "Cannot process entity."
            },
            new RouteMetaDataResponse
            {
                Code = (int)HttpStatusCode.InternalServerError,
                Description = "Internal server error."
            }
        };
        public override Type Content { get; } = typeof(MultipartContent);
        public override string OperationId { get; } = "PostFile";
    }
}