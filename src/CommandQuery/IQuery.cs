using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;
using Newtonsoft.Json.Linq;

namespace CommandQuery
{
    public interface IQuery<TResult>
    {
    }

    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }

    public interface IQueryProcessor
    {
        Task<TResult> ProcessAsync<TResult>(string queryName, string json);

        Task<TResult> ProcessAsync<TResult>(string queryName, JObject json);

        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);

        IEnumerable<Type> GetQueries();
    }

    public class QueryProcessor : IQueryProcessor
    {
        private readonly ITypeCollection _typeCollection;
        private readonly IServiceProvider _serviceProvider;

        public QueryProcessor(IQueryTypeCollection typeCollection, IServiceProvider serviceProvider)
        {
            _typeCollection = typeCollection;
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> ProcessAsync<TResult>(string queryName, string json)
        {
            return await ProcessAsync<TResult>(queryName, JObject.Parse(json));
        }

        public async Task<TResult> ProcessAsync<TResult>(string queryName, JObject json)
        {
            var queryType = _typeCollection.GetType(queryName);

            if (queryType == null) throw new QueryProcessorException($"The query type '{queryName}' could not be found");

            var query = json.SafeToObject(queryType);

            if (query == null) throw new QueryProcessorException("The json could not be converted to an object");

            return await ProcessAsync((dynamic)query);
        }

        public async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.GetService(handlerType);

            if (handler == null) throw new QueryProcessorException($"The query handler for '{query}' could not be found");

            return await handler.HandleAsync((dynamic)query);
        }

        public IEnumerable<Type> GetQueries()
        {
            return _typeCollection.GetTypes();
        }
    }
}