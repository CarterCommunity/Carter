namespace Carter.Tests.ModelBinding.NewtonsoftBinding
{
    using Carter.ModelBinding;
    using Carter.Response;
    using Carter.Tests.Modelbinding;

    public class NewtonsoftModule : CarterModule
    {
        public NewtonsoftModule()
        {
            this.Post("/bind", async (req, res) =>
            {
                var model = await req.Bind<ModelOnlyNewtonsoftCanParse>();
                await res.Negotiate(model);
            });
        }
    }
}
