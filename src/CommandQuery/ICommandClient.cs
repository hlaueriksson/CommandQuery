using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandQuery
{
    public interface ICommandClient
    {
        string BaseUrl { get; set; }

        Task PostAsync(ICommand command);

        Task PostAsync(string slug, object command);
    }

    public class CommandClient : BaseClient, ICommandClient
    {
        public CommandClient(string baseUri) : base(baseUri)
        {
        }

        public async Task PostAsync(ICommand command)
        {
            await CommandPostAsync(command);
        }

        public async Task PostAsync(string slug, object command)
        {
            await CommandPostAsync(slug, command);
        }
    }

    public class CommandClientSettings
    {
        public Dictionary<string, string> BaseUrl { get; set; }
    }
}