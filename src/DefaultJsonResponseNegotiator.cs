namespace Carter
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DefaultJsonResponseNegotiator : IResponseNegotiator
    {
        private readonly JsonSerializerSettings jsonSettings;

        public DefaultJsonResponseNegotiator()
        {
            var contractResolver = new SkipEmptyContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            this.jsonSettings = new JsonSerializerSettings { ContractResolver = contractResolver, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
        }

        public bool CanHandle(MediaTypeHeaderValue accept)
        {
            return accept.MediaType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public async Task Handle(HttpRequest req, HttpResponse res, object model, CancellationToken cancellationToken)
        {
            res.ContentType = "application/json; charset=utf-8";
            await res.WriteAsync(JsonConvert.SerializeObject(model, this.jsonSettings), cancellationToken);
        }
    }

    public class SkipEmptyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            bool isDefaultValueIgnored = ((property.DefaultValueHandling ?? DefaultValueHandling.Ignore) & DefaultValueHandling.Ignore) != 0;

            if (isDefaultValueIgnored && !typeof(string).IsAssignableFrom(property.PropertyType) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                Predicate<object> newShouldSerialize = obj =>
                {
                    var collection = property.ValueProvider.GetValue(obj) as ICollection;
                    return collection == null || collection.Count != 0;
                };

                Predicate<object> oldShouldSerialize = property.ShouldSerialize;

                property.ShouldSerialize = oldShouldSerialize != null ? o => oldShouldSerialize(o) && newShouldSerialize(o) : newShouldSerialize;
            }

            return property;
        }
    }
}
