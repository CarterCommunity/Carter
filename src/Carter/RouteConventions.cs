namespace Carter
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Builder;

    internal class RouteConventions : IEndpointConventionBuilder
    {
        private readonly List<Action<EndpointBuilder>> _actions = new List<Action<EndpointBuilder>>();

        public void Add(Action<EndpointBuilder> convention)
        {
            this._actions.Add(convention);
        }

        public void Apply(IEndpointConventionBuilder builder)
        {
            foreach (var a in this._actions)
            {
                builder.Add(a);
            }
        }
    }
}
