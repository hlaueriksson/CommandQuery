using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;

namespace CommandQuery
{
    /// <inheritdoc />
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IQueryTypeProvider _queryTypeProvider;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProcessor"/> class.
        /// </summary>
        /// <param name="queryTypeProvider">A provider of supported queries.</param>
        /// <param name="serviceProvider">A service provider with supported query handlers.</param>
        public QueryProcessor(IQueryTypeProvider queryTypeProvider, IServiceProvider serviceProvider)
        {
            _queryTypeProvider = queryTypeProvider;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic? handler = GetService(handlerType);

            if (handler is null)
            {
                throw new QueryProcessorException($"The query handler for '{query}' could not be found");
            }

            return await handler.HandleAsync((dynamic)query);
        }


        /// <inheritdoc />
        public Type? GetQueryType(string queryName)
        {
            return _queryTypeProvider.GetQueryType(queryName);
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetQueryTypes()
        {
            return _queryTypeProvider.GetQueryTypes();
        }

        private object? GetService(Type handlerType)
        {
            try
            {
                return _serviceProvider.GetSingleService(handlerType);
            }
            catch (InvalidOperationException)
            {
                throw new QueryProcessorException($"Multiple query handlers for '{handlerType}' was found");
            }
        }
    }
}
