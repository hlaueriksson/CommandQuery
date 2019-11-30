using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CommandQuery.AspNetCore
{
    public class QueryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _types;

        public QueryControllerFeatureProvider(params Assembly[] assemblies)
        {
            _types = assemblies.GetTypesAssignableTo(typeof(IQuery<>)).ToArray();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var queryType in _types)
            {
                var controllerType = typeof(QueryController<,>).MakeGenericType(queryType, queryType.GetResultType(typeof(IQuery<>)));

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}