namespace Carter.Tests.ModelBinding.Newtonsoft
{
    using Carter.ModelBinding;
    using Carter.Response;

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
