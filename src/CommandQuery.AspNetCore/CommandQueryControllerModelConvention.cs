using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CommandQuery.AspNetCore
{
    public class CommandQueryControllerModelConvention : IControllerModelConvention
    {
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