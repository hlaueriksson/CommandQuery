using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommandQuery.Client
{
    public interface ICommandClient
    {
        void Post(ICommand command);
        Task PostAsync(ICommand command);
    }

    public class CommandClient : BaseClient, ICommandClient
    {
        public CommandClient(string baseUrl, int timeoutInSeconds = 10) : base(baseUrl, timeoutInSeconds) { }
        public CommandClient(string baseUrl, Action<HttpClient> configAction) : base(baseUrl, configAction) { }

        public void Post(ICommand command) => BasePost(command);
        public async Task PostAsync(ICommand command) => await BasePostAsync(command);
    }
}