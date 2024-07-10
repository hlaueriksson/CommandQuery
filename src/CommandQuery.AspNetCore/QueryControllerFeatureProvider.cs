using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Populates the list of controller types in an MVC application with controllers for all queries found in the provided assemblies.
    /// </summary>
    public class QueryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryControllerFeatureProvider"/> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with queries to create controllers for.</param>
        public QueryControllerFeatureProvider(params Assembly[] assemblies)
        {
            _types = assemblies.GetTypesAssignableTo(typeof(IQuery<>)).ToArray();
        }

        /// <summary>
        /// Populates the list of controller types in an MVC application with controllers for all queries found in the provided assemblies.
        /// </summary>
        /// <param name="parts">The list of <see cref="ApplicationPart"/> instances in the application.
        /// </param>
        /// <param name="feature">The feature instance to populate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="feature"/> is <see langword="null"/>.</exception>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            ArgumentNullException.ThrowIfNull(feature);

            foreach (var queryType in _types)
            {
                var controllerType = typeof(QueryController<,>).MakeGenericType(queryType, queryType.GetResultType(typeof(IQuery<>))!);

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}
