using System;
using System.Net.Http;

namespace Toolbox.Net.Http
{
    /// <summary>
    /// Represents a source of <see cref="HttpClient"/> instances.
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the app-domain.</returns>
        HttpClient GetHttpClient(string baseUri);

        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <param name="initializer">A delegate that is called to initialize a new instance of <see cref="HttpClient"/>.
        /// This will be called once, only if a new instance is created.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the app-domain.</returns>
        HttpClient GetHttpClient(string baseUri, Action<HttpClient> initializer);

        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or 
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the app-domain.</returns>
        HttpClient GetHttpClient(Uri baseUri);

        /// <summary>
        /// Gets an <see cref="HttpClient"/> object that can be used to access resources on the specified server.
        /// </summary>
        /// <param name="baseUri">The URI of the server to access.</param>
        /// <param name="initializer">A delegate that is called to initialize a new instance of <see cref="HttpClient"/>.
        /// This will be called once, only if a new instance is created.</param>
        /// <returns>An <see cref="HttpClient"/> object for the specified URI. Note that this may be a cached or
        /// singleton instance - changing any properties on the <c>HttpClient</c> will affect all usages in the app-domain.</returns>
        HttpClient GetHttpClient(Uri baseUri, Action<HttpClient> initializer);

        /// <summary>
        /// Invalidates any cached <see cref="HttpClient"/> instance for the given URI so that any subsequent request for
        /// that URI will use a new instance.
        /// </summary>
        /// <param name="baseUri">The base URI assigned to the <c>HttpClient</c> instance to be invalidated.</param>
        void Invalidate(string baseUri);

        /// <summary>
        /// Invalidates any cached <see cref="HttpClient"/> instance for the given URI so that any subsequent request for
        /// that URI will use a new instance.
        /// </summary>
        /// <param name="baseUri">The base URI assigned to the <c>HttpClient</c> instance to be invalidated.</param>
        void Invalidate(Uri baseUri);
    }
}