namespace Carter.Tests.ModelBinding.NewtonsoftBinding
{
    using Carter.ModelBinding;
    using Carter.Response;

    public class CustomModelBinderModule : CarterModule
    {
        public CustomModelBinderModule()
        {
            this.Post("/bind", async (req, res) =>
            {
                var model = await req.Bind<ModelOnlyNewtonsoftCanParse>();
                await res.Negotiate(model);
            });
        }
    }
}
