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
        /// <param name="model">The <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationModels.ControllerModel" />.</param>
        public void Apply(ControllerModel model)
        {
            if (!model.ControllerType.IsGenericType) return;

            var openControllerType = model.ControllerType.GetGenericTypeDefinition();

            if (openControllerType != typeof(CommandController<>)
                && openControllerType != typeof(CommandWithResultController<,>)
                && openControllerType != typeof(QueryController<,>))
                return;

            model.ControllerName = model.ControllerType.GenericTypeArguments[0].Name;
        }
    }
}