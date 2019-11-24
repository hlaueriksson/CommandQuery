using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CommandQuery.AspNetCore
{
    public class QueryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _types;

        public QueryControllerFeatureProvider(params Assembly[] assemblies)
        {
            _types = assemblies.SelectMany(x => x.GetExportedTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => t.BaseType != null)
                .Where(t => typeof(IQuery<>).IsAssignableFrom(t))
                .ToArray();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var queryType in _types)
            {
                var controllerType = typeof(QueryController<,>).MakeGenericType(queryType, queryType.BaseType.GetGenericArguments()[0]);

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }

    /*
            services
                .AddMvc(o =>
                {
                    o.Conventions.Add(new GenericControllerRouteConvention());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)

                // This will create controllers based on base template types
                .ConfigureApplicationPartManager(m =>
                {
                    m.FeatureProviders.Add(new CommandControllerFeatureProvider<SampleCommandBase>());
                    m.FeatureProviders.Add(new QueryControllerFeatureProvider<SampleQueryBase>());
                });
     */
}