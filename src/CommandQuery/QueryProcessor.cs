using CommandQuery.Exceptions;

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
        public async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic? handler = GetService(handlerType);

            if (handler is null)
            {
                throw new QueryProcessorException($"The query handler for '{query}' could not be found.");
            }

            return await handler.HandleAsync((dynamic)query, cancellationToken);
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

        /// <inheritdoc />
        public IQueryProcessor AssertConfigurationIsValid()
        {
            var errors = new List<string>();

            foreach (var queryType in GetQueryTypes())
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, queryType.GetResultType(typeof(IQuery<>)));

                try
                {
                    if (GetService(handlerType) is null)
                    {
                        errors.Add($"The query handler for '{queryType.AssemblyQualifiedName}' is not registered.");
                    }
                }
                catch (QueryProcessorException)
                {
                    errors.Add($"A single query handler for '{queryType.AssemblyQualifiedName}' could not be retrieved.");
                }
            }

            foreach (var handlerType in _serviceProvider
                .GetAllServiceTypes()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .ToList())
            {
                var queryType = handlerType.GetGenericArguments()[0];
                var supportedQueryType = _queryTypeProvider.GetQueryType(queryType.Name);

                if (supportedQueryType is null || supportedQueryType != queryType)
                {
                    errors.Add($"The query '{queryType.AssemblyQualifiedName}' is not registered.");
                }
            }

            if (errors.Any())
            {
                throw new QueryTypeException("The QueryProcessor configuration is not valid:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
            }

            return this;
        }

        private object? GetService(Type handlerType)
        {
            try
            {
                return _serviceProvider.GetSingleService(handlerType);
            }
            catch (InvalidOperationException exception)
            {
                throw new QueryProcessorException($"A single query handler for '{handlerType}' could not be retrieved.", exception);
            }
        }
    }
}
