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
            this.Post("/bind", (req, res, routeData) =>
            {
                var model = req.Bind<TestModel>();
                return res.Negotiate(model);
            });

            this.Post("/bindandvalidate", (req, res, routeData) =>
            {
                var model = req.BindAndValidate<TestModel>();
                if (!model.ValidationResult.IsValid)
                {
                    return res.Negotiate(model.ValidationResult.GetFormattedErrors());
                }

                return res.Negotiate(model.Data);
            });

            this.Post("/novalidator", (req, res, routeData) =>
            {
                var model = req.BindAndValidate<TestModelNoValidator>();
                if (!model.ValidationResult.IsValid)
                {
                    return res.Negotiate(model.ValidationResult.GetFormattedErrors());
                }

                return res.Negotiate(model);
            });

            this.Post("/duplicatevalidator", (req, res, routeData) =>
            {
                var model = req.BindAndValidate<DuplicateTestModel>();
                if (!model.ValidationResult.IsValid)
                {
                    return res.Negotiate(model.ValidationResult.GetFormattedErrors());
                }

                return res.Negotiate(model.Data);
            });

            this.Post("/bindandsave", async (req, res, routeData) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath);

                await res.Negotiate(new PathTestModel { Path = filePath });
            });

            this.Post("/bindandsavecustomname", async (req, res, routeData) =>
            {
                var filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

                await req.BindAndSaveFile(filePath, "mycustom.txt");

                await res.Negotiate(new PathTestModel { Path = filePath });
            });
        }
    }
}