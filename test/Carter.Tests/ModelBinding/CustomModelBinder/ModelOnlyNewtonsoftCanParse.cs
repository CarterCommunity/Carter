namespace Carter.Tests.ModelBinding.CustomModelBinder
{
    public class ModelOnlyNewtonsoftCanParse
    {
        public ModelOnlyNewtonsoftCanParse(string onlyString)
        {
            this.PrivateSetterProperty = onlyString;
            this.PublicSetterProperty = "world";
        }

        public string PublicSetterProperty { get; set; }
        public string PrivateSetterProperty { get; private set; }
    }
}
