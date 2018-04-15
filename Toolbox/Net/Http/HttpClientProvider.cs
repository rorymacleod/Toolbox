using System;
using System.Net.Http;
using System.Runtime.Caching;

namespace Toolbox.Net.Http
{
    /// <summary>
    /// Represents a source of <see cref="HttpClient"/> instances.
    /// </summary>
    public class HttpClientProvider : IHttpClientProvider
    {
        /// <summary>
        /// Cache of <see cref="HttpClient"/> instances.
        /// </summary>
        private readonly ObjectCache Cache;

        /// <summary>
        /// Synchronizes access to the cache by multiple threads. Note that thread-safety can only be guaranteed when
        /// <c>HttpClientProvider</c> creates its own cache, or when the supplied cache is not used by any other code.
        /// </summary>
        private readonly object SyncRoot = new object();

        /// <summary>
        /// Specifies the maximum lifetime for any cached <see cref="HttpClient"/> instance.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientProvider"/> class to use a private, in-memory, cache.
        /// </summary>
        public HttpClientProvider() : this(new MemoryCache("HttpClientProvider"))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientProvider"/> class using the specified cache.
        /// </summary>
        /// <param name="cache">An <see cref="ObjectCache"/> instance to hold the <see cref="HttpClient"/> 
        /// instances.</param>
        public HttpClientProvider(ObjectCache cache)
        {
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }



        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the 
        /// app-domain.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="baseUri"/> is a null-reference or empty 
        /// string.</exception>
        /// <exception cref="UriFormatException"><paramref name="baseUri"/> cannot be parsed by 
        /// <see cref="Uri"/>.</exception>
        public HttpClient GetHttpClient(string baseUri)
        {
            return GetHttpClient(baseUri, null);
        }

        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <param name="initializer">An optional <see cref="Action{T}"/> called to initialize a new 
        /// <see cref="HttpClient"/>, if one is created. Note that any other calls to <c>GetHttpClient</c> will be 
        /// blocked while the initializer is executing.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the 
        /// app-domain.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="baseUri"/> is a null-reference or empty 
        /// string.</exception>
        /// <exception cref="UriFormatException"><paramref name="baseUri"/> cannot be parsed by 
        /// <see cref="Uri"/>.</exception>
        public HttpClient GetHttpClient(string baseUri, Action<HttpClient> initializer)
        {
            if (string.IsNullOrEmpty(baseUri))
                throw new ArgumentNullException(nameof(baseUri));

            return GetHttpClient(new Uri(baseUri, UriKind.Absolute), initializer);
        }

        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the 
        /// app-domain.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="baseUri"/> is a null-reference.</exception>
        /// <exception cref="ArgumentException"><paramref name="baseUri"/> is not an absolute URI.</exception>
        public HttpClient GetHttpClient(Uri baseUri)
        {
            return GetHttpClient(baseUri, null);
        }

        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the 
        /// app-domain.</returns>
        /// <param name="initializer">An optional <see cref="Action{T}"/> called to initialize a new 
        /// <see cref="HttpClient"/>, if one is created. Note that any other calls to <c>GetHttpClient</c> will be 
        /// blocked while the initializer is executing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="baseUri"/> is a null-reference.</exception>
        /// <exception cref="ArgumentException"><paramref name="baseUri"/> is not an absolute URI.</exception>
        public HttpClient GetHttpClient(Uri baseUri, Action<HttpClient> initializer)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));
            if (!baseUri.IsAbsoluteUri)
                throw new ArgumentException("The URI must be absolute.", nameof(baseUri));

            var key = baseUri.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
            CacheItem item;
            lock (SyncRoot)
            {
                item = Cache.GetCacheItem(key);
                if (item == null)
                {
                    var client = new HttpClient {
                        BaseAddress = baseUri
                    };
                    initializer?.Invoke(client);
                    item = new CacheItem(key, client);
                    var policy = new CacheItemPolicy();
                    var ttl = TimeToLive;
                    if (ttl > TimeSpan.Zero)
                    {
                        policy.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(TimeToLive);
                    }
                    Cache.Set(item, policy);
                }
            }
            return (HttpClient)item.Value;
        }

        public void Invalidate(string baseUri)
        {
            throw new NotImplementedException();
        }

        public void Invalidate(Uri baseUri)
        {
            throw new NotImplementedException();
        }
    }
}