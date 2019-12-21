namespace Carter.Tests.ModelBinding.NewtonsoftBinding
{
    using Carter.ModelBinding;
    using Carter.Response;
    using Carter.Tests.Modelbinding;

    public class NewtonsoftBinderModule : CarterModule
    {
        public NewtonsoftBinderModule()
        {
            this.Post("/bind", async (req, res) =>
            {
                var model = await req.Bind<TestModel>();
                await res.Negotiate(model);
            });
        }
    }
}
