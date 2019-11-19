namespace Carter.Tests.Modelbinding
{
    using System;
    using System.IO;
    using Carter.ModelBinding;
    using Carter.Response;

    public class BindModule : CarterModule
    {
        public BindModule()
        {
            this.Post("/bind", async (req, res) =>
            {
                var model = await req.Bind<TestModel>();
                await res.Negotiate(model);
            });

            this.Post("/bindandvalidate", async (req, res) =>
            {
                var model = await req.BindAndValidate<TestModel>();
                if (!model.ValidationResult.IsValid)
                {
                    res.StatusCode = 422;
                    await res.Negotiate(model.ValidationResult.GetFormattedErrors());
                    return;
                }

                await res.Negotiate(model.Data);
            });

            this.Post("/novalidator", async (req, res) =>
            {
                var model = await req.BindAndValidate<TestModelNoValidator>();
                if (!model.ValidationResult.IsValid)
                {
                    await res.Negotiate(model.ValidationResult.GetFormattedErrors());
                    return;
                }

                await res.Negotiate(model);
            });

            this.Post("/duplicatevalidator", async (req, res) =>
            {
                var model = await req.BindAndValidate<DuplicateTestModel>();
                if (!model.ValidationResult.IsValid)
                {
                    await res.Negotiate(model.ValidationResult.GetFormattedErrors());
                    return;
                }

                await res.Negotiate(model.Data);
            });

            this.Post("/bindandsave", async (req, res) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath);

                await res.Negotiate(new PathTestModel { Path = filePath });
            });

            this.Post("/bindandsavecustomname", async (req, res) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath, "mycustom.txt");

                await res.Negotiate(new PathTestModel { Path = filePath });
            });

            this.Post("/bindfail", async (req, res) =>
            {
                var model = await req.Bind<TestModel>();
                await res.Negotiate(model);
            });
        }
    }
}
