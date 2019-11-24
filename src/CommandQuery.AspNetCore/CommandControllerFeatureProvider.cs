using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CommandQuery.AspNetCore
{
    public class CommandControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Type[] _types;

        public CommandControllerFeatureProvider(params Assembly[] assemblies)
        {
            _types = assemblies.SelectMany(x => x.GetExportedTypes())
                .Where(t => !t.IsAbstract)
                .Where(t => t.BaseType != null)
                .Where(t => typeof(ICommand).IsAssignableFrom(t) || typeof(ICommand<>).IsAssignableFrom(t))
                .ToArray();
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var commandType in _types)
            {
                var controllerType = commandType.BaseType.IsGenericType
                    ? typeof(CommandWithResultController<,>).MakeGenericType(commandType, commandType.BaseType.GetGenericArguments()[0])
                    : typeof(CommandController<>).MakeGenericType(commandType);

                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}