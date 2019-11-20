using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carter.OpenApi
{
    public sealed class ApiSchemaAttributes : Attribute
    {
        public string Format { get; set; }
    }
}
