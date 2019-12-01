using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Populates the list of controller types in an MVC application with controllers for all commands found in the provided assemblies.
    /// </summary>
    public class CommandControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandControllerFeatureProvider" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies with commands to create controllers for</param>
        public CommandControllerFeatureProvider(params Assembly[] assemblies)
        {
            _types = assemblies.GetTypesAssignableTo(typeof(ICommand)).Concat(assemblies.GetTypesAssignableTo(typeof(ICommand<>))).ToArray();
        }

        /// <summary>
        /// Populates the list of controller types in an MVC application with controllers for all commands found in the provided assemblies.
        /// </summary>
        /// <param name="parts">The list of <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPart" /> instances in the application.
        /// </param>
        /// <param name="feature">The feature instance to populate.</param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var commandType in _types.Where(x => x.IsAssignableTo(typeof(ICommand))))
            {
                var controllerType = typeof(CommandController<>).MakeGenericType(commandType);

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }

            foreach (var commandType in _types.Where(x => x.IsAssignableTo(typeof(ICommand<>))))
            {
                var controllerType = typeof(CommandWithResultController<,>).MakeGenericType(commandType, commandType.GetResultType(typeof(ICommand<>)));

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}