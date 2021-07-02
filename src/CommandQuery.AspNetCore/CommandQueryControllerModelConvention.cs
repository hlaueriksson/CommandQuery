using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CommandQuery.AspNetCore
{
    /// <summary>
    /// Customizes the name of controllers for commands and queries.
    /// </summary>
    public class CommandQueryControllerModelConvention : IControllerModelConvention
    {
        /// <summary>
        /// Applies the naming convention of controllers for commands and queries.
        /// </summary>
        /// <param name="controller">The <see cref="ControllerModel"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="controller"/> is <see langword="null"/>.</exception>
        public void Apply(ControllerModel controller)
        {
            if (controller is null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (!controller.ControllerType.IsGenericType)
            {
                return;
            }

            var openControllerType = controller.ControllerType.GetGenericTypeDefinition();

            if (openControllerType != typeof(CommandController<>)
                && openControllerType != typeof(CommandWithResultController<,>)
                && openControllerType != typeof(QueryController<,>))
            {
                return;
            }

            controller.ControllerName = controller.ControllerType.GenericTypeArguments[0].Name;
        }
    }
}
