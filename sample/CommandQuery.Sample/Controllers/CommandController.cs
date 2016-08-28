using Microsoft.AspNetCore.Mvc;

namespace CommandQuery.Sample.Controllers
{
    [Route("api/[controller]")]
    public class CommandController : BaseCommandController
    {
        public CommandController(ICommandProcessor commandProcessor) : base(commandProcessor)
        {
        }
    }
}