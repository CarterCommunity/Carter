namespace Carter.Tests.ModelBinding.CustomModelBinder
{
    using System.Reflection;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Converters;

    public static class NewtonsoftJsonUtils
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterCamelCasePropertyNamesContractResolver(),
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };
        
        public static bool IsPropertyWithSetter(this MemberInfo member)
        {
            var property = member as PropertyInfo;

            return property?.SetMethod != null;
        }
    }
}
