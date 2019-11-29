using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandQuery.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CommandQuery.AspNetCore
{
    public class CommandControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _types;

        public CommandControllerFeatureProvider(params Assembly[] assemblies)
        {
            _types = assemblies.GetTypesAssignableTo(typeof(ICommand)).Concat(assemblies.GetTypesAssignableTo(typeof(ICommand<>))).ToArray();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var commandType in _types.Where(x => typeof(ICommand).IsAssignableFrom(x)))
            {
                var controllerType = typeof(CommandController<>).MakeGenericType(commandType);

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }

            foreach (var commandType in _types.Where(x => x.IsAssignableToGenericType(typeof(ICommand<>))))
            {
                var controllerType = typeof(CommandWithResultController<,>).MakeGenericType(commandType, commandType.GetResultType(typeof(ICommand<>)));

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}