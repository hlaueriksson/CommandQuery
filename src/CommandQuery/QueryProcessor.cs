﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommandQuery.Exceptions;
using CommandQuery.Internal;

namespace CommandQuery
{
    /// <summary>
    /// Process queries by invoking the corresponding handler.
    /// </summary>
    public interface IQueryProcessor
    {
        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="query">The query</param>
        /// <returns>The result of the query</returns>
        Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query);

        /// <summary>
        /// Returns the types of queries that can be processed.
        /// </summary>
        /// <returns>Supported query types</returns>
        IEnumerable<Type> GetQueryTypes();

        /// <summary>
        /// Returns the type of query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <returns>The query type</returns>
        Type GetQueryType(string queryName);
    }

    /// <summary>
    /// Process queries by invoking the corresponding handler.
    /// </summary>
    public class QueryProcessor : IQueryProcessor
    {
        private readonly ITypeCollection _typeCollection;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProcessor" /> class.
        /// </summary>
        /// <param name="typeCollection">A collection of supported queries</param>
        /// <param name="serviceProvider">A service provider with supported query handlers</param>
        public QueryProcessor(IQueryTypeCollection typeCollection, IServiceProvider serviceProvider)
        {
            _typeCollection = typeCollection;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Process a query.
        /// </summary>
        /// <typeparam name="TResult">The type of result</typeparam>
        /// <param name="query">The query</param>
        /// <returns>The result of the query</returns>
        public async Task<TResult> ProcessAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = GetService(handlerType);

            if (handler == null) throw new QueryProcessorException($"The query handler for '{query}' could not be found");

            return await handler.HandleAsync((dynamic)query);
        }

        /// <summary>
        /// Returns the types of queries that can be processed.
        /// </summary>
        /// <returns>Supported query types</returns>
        public IEnumerable<Type> GetQueryTypes()
        {
            return _typeCollection.GetTypes();
        }

        /// <summary>
        /// Returns the type of query.
        /// </summary>
        /// <param name="queryName">The name of the query</param>
        /// <returns>The query type</returns>
        public Type GetQueryType(string queryName)
        {
            return _typeCollection.GetType(queryName);
        }

        private object GetService(Type handlerType)
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