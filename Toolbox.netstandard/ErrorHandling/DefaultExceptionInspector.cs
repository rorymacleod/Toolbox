using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Toolbox.ErrorHandling
{
    /// <summary>
    /// Provides analysis of exceptions from the .NET Framework, or a default value for any unrecognized exception.
    /// </summary>
    public class DefaultExceptionInspector : IExceptionInspector
    {
        /// <summary>
        /// Indicates whether the HTTP status code represents a transient error that might be resolved by retrying the
        /// request.
        /// </summary>
        /// <param name="status">The <see cref="HttpStatusCode"/>.</param>
        public static bool IsTransientHttpStatusCode(HttpStatusCode status)
        {
            switch (status)
            {
                case HttpStatusCode.BadGateway: // 502
                case HttpStatusCode.GatewayTimeout: // 504
                case HttpStatusCode.RequestTimeout: // 408
                case HttpStatusCode.ServiceUnavailable: // 503
                    return true;
            }

            return false;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultExceptionInspector"/> class.
        /// </summary>
        internal DefaultExceptionInspector()
        {
        }



        /// <summary>
        /// Gets a detailed error message built from the exception's own message and its properties.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        /// <param name="formatting">Optional <see cref="ExceptionFormatting"/> value specifying how to format the
        /// error message.</param>
        public string GetMessage(Exception exception, ExceptionFormatting formatting = ExceptionFormatting.SingleLine)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            // SingleLine formatting returns the exception Message by itself. Additional details are added for certain
            // exceptions that we know about. Multiline formatting adds the exception type (without the "Exception") to
            // the start of the line. Detailed formatting adds the contents of the Data dictionary and certain properties
            // from known exceptions on their own lines under the error message.
            //
            // Stack traces are not included. These messages are intended for display in UI or in logging. The full 
            // exception.ToString should still be logged separately.
            //
            // All of this only deals with the current exception. See the GetAllMessages extension method for combining
            // inner exceptions and AggregateExceptions.

            var sb = new StringBuilder();
            var detail = formatting == ExceptionFormatting.Detailed ? new StringBuilder() : null;
            // Basic exception message.
            if (formatting != ExceptionFormatting.SingleLine)
            {
                var exType = exception.GetType();
                var name = exType == typeof(Exception) ? exType.Name : exType.Name.TrimEnd("Exception",
                    StringComparison.InvariantCultureIgnoreCase);
                sb.AppendFormat("({0}) ", name);
            }
            sb.Append(exception.Message);

            // Special handling for known exceptions.
            if (exception is SocketException socEx)
            {
                detail?.AppendLine($"- ErrorCode: {socEx.ErrorCode}");
                detail?.AppendLine($"- NativeErrorCode: {socEx.NativeErrorCode}");
                detail?.AppendLine($"- SocketErrorCode: {socEx.SocketErrorCode}");

                string err = GetSocketExceptionErrorMessage(socEx.SocketErrorCode);
                sb.Append(err == null ? null : " " + err);
            }

            if (exception is WebException webEx)
            {
                detail?.AppendLine($"- Status: {webEx.Status}");

                if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse http)
                {
                    detail?.AppendLine($"- StatusCode: {(int)http.StatusCode}");
                    detail?.AppendLine($"- StatusDescription: {http.StatusDescription}");
                    sb.Append($" The web response was ({(int)http.StatusCode}) {http.StatusDescription}");
                }
                else
                {
                    string err = GetWebExceptionStatusMessage(webEx.Status);
                    sb.Append(err == null ? null : " " + err);
                }
            }

            // Add Data dictionary and message data values.
            if (detail != null)
            {
                var data = new SortedDictionary<string, string>();
                foreach (string key in exception.Data.Keys)
                {
                    data[key] = Convert.ToString(exception.Data[key]);
                }

                if (exception is ISupportsMessageData dataEx)
                {
                    dataEx.GetMessageData(data);
                }

                foreach (var pair in data)
                {
                    detail.AppendLine($"- {pair.Key}: {pair.Value}");
                }
            }


            // Add the extra details to the basic error message.
            if (detail?.Length > 0)
            {
                sb.AppendLine();
                sb.Append(detail.ToString().TrimEnd());
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns an additional error message for the given <see cref="SocketError"/>.
        /// </summary>
        private string GetSocketExceptionErrorMessage(SocketError error)
        {
            // See https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socketerror
            switch (error)
            {
                case SocketError.AccessDenied:
                    return "An attempt was made to access a socket in a way that is forbidden by its access permissions.";
                case SocketError.AddressAlreadyInUse:
                    return "Only one use of an address is normally permitted.";
                case SocketError.AddressFamilyNotSupported:
                    return "The address family specified is not supported.";
                case SocketError.AddressNotAvailable:
                    return "The selected IP address is not valid in this context.";
                case SocketError.AlreadyInProgress:
                    return "The nonblocking socket already has an operation in progress.";
                case SocketError.ConnectionAborted:
                    return "The connection was aborted by the .NET Framework or the underlying socket provider.";
                case SocketError.ConnectionRefused:
                    return "The remote host is actively refusing a connection.";
                case SocketError.ConnectionReset:
                    return "The connection was reset by the remote peer.";
                case SocketError.DestinationAddressRequired:
                    return "A required address was omitted from an operation on a Socket.";
                case SocketError.Disconnecting:
                    return "A graceful shutdown is in progress.";
                case SocketError.Fault:
                    return "An invalid pointer address was detected by the underlying socket provider.";
                case SocketError.HostDown:
                    return "The operation failed because the remote host is down.";
                case SocketError.HostNotFound:
                    return "No such host is known. The name is not an official host name or alias.";
                case SocketError.HostUnreachable:
                    return "There is no network route to the specified host.";
                case SocketError.InProgress:
                    return "A blocking operation is in progress.";
                case SocketError.Interrupted:
                    return "A blocking Socket call was canceled.";
                case SocketError.InvalidArgument:
                    return "An invalid argument was supplied to a Socket member.";
                case SocketError.IOPending:
                    return "The application has initiated an overlapped operation that cannot be completed immediately.";
                case SocketError.IsConnected:
                    return "The Socket is already connected.";
                case SocketError.MessageSize:
                    return "The datagram is too long.";
                case SocketError.NetworkDown:
                    return "The network is not available.";
                case SocketError.NetworkReset:
                    return "The application tried to set KeepAlive on a connection that has already timed out.";
                case SocketError.NetworkUnreachable:
                    return "No route to the remote host exists.";
                case SocketError.NoBufferSpaceAvailable:
                    return "No free buffer space is available for a Socket operation.";
                case SocketError.NoData:
                    return "The requested name or IP address was not found on the name server.";
                case SocketError.NoRecovery:
                    return "The error is unrecoverable or the requested database cannot be located.";
                case SocketError.NotConnected:
                    return "The application tried to send or receive data, and the Socket is not connected.";
                case SocketError.NotInitialized:
                    return "The underlying socket provider has not been initialized.";
                case SocketError.NotSocket:
                    return "A Socket operation was attempted on a non-socket.";
                case SocketError.OperationAborted:
                    return "The overlapped operation was aborted due to the closure of the Socket.";
                case SocketError.OperationNotSupported:
                    return "The address family is not supported by the protocol family.";
                case SocketError.ProcessLimit:
                    return "Too many processes are using the underlying socket provider.";
                case SocketError.ProtocolFamilyNotSupported:
                    return "The protocol family is not implemented or has not been configured.";
                case SocketError.ProtocolNotSupported:
                    return "The protocol is not implemented or has not been configured.";
                case SocketError.ProtocolOption:
                    return "An unknown, invalid, or unsupported option or level was used with a Socket.";
                case SocketError.ProtocolType:
                    return "The protocol type is incorrect for this Socket.";
                case SocketError.Shutdown:
                    return "A request to send or receive data was disallowed because the Socket has already been closed.";
                case SocketError.SocketError:
                    return "An unspecified Socket error has occurred.";
                case SocketError.SocketNotSupported:
                    return "The support for the specified socket type does not exist in this address family.";
                case SocketError.Success:
                    return "The Socket operation succeeded.";
                case SocketError.SystemNotReady:
                    return "The network subsystem is unavailable.";
                case SocketError.TimedOut:
                    return "The connection attempt timed out, or the connected host has failed to respond.";
                case SocketError.TooManyOpenSockets:
                    return "There are too many open sockets in the underlying socket provider.";
                case SocketError.TryAgain:
                    return "The name of the host could not be resolved. Try again later.";
                case SocketError.TypeNotFound:
                    return "The specified class was not found.";
                case SocketError.VersionNotSupported:
                    return "The version of the underlying socket provider is out of range.";
                case SocketError.WouldBlock:
                    return "An operation on a nonblocking socket cannot be completed immediately.";
            }

            return null;
        }

        /// <summary>
        /// Returns an additional error message for the given <see cref="WebExceptionStatus"/>.
        /// </summary>
        private string GetWebExceptionStatusMessage(WebExceptionStatus status)
        {
            // See https://docs.microsoft.com/en-us/dotnet/framework/network-programming/handling-errors
            switch (status)
            {
                case WebExceptionStatus.ConnectFailure:
                    return "The remote service could not be contacted at the transport level.";
                case WebExceptionStatus.ConnectionClosed:
                    return "The connection was closed prematurely.";
                case WebExceptionStatus.KeepAliveFailure:
                    return "The server closed a connection made with the Keep-alive header set.";
                case WebExceptionStatus.NameResolutionFailure:
                    return "The name service could not resolve the host name.";
                //case WebExceptionStatus.ProtocolError:
                //    return "The response received from the server was complete but indicated an error at the protocol level.";
                case WebExceptionStatus.ReceiveFailure:
                    return "A complete response was not received from the remote server.";
                case WebExceptionStatus.RequestCanceled:
                    return "The request was canceled.";
                case WebExceptionStatus.SecureChannelFailure:
                    return "An error occurred in a secure channel link.";
                case WebExceptionStatus.SendFailure:
                    return "A complete request could not be sent to the remote server.";
                case WebExceptionStatus.ServerProtocolViolation:
                    return "The server response was not a valid HTTP response.";
                case WebExceptionStatus.Success:
                    return "No error was encountered.";
                case WebExceptionStatus.Timeout:
                    return "No response was received within the time-out set for the request.";
                case WebExceptionStatus.TrustFailure:
                    return "A server certificate could not be validated.";
                case WebExceptionStatus.MessageLengthLimitExceeded:
                    return "A message was received that exceeded the specified limit when sending a request or receiving a response from the server.";
                case WebExceptionStatus.Pending:
                    return "An internal asynchronous request is pending.";
                case WebExceptionStatus.PipelineFailure:
                    return "This value supports the .NET Framework infrastructure and is not intended to be used directly in your code.";
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    return "The name resolver service could not resolve the proxy host name.";
                //case WebExceptionStatus.UnknownError:
                //    return "An exception of unknown type has occurred.";
            }

            return null;
        }

        /// <summary>
        /// Indicates whether the exception contains significant information, meaning that it is not a wrapper or
        /// aggregate of other exceptions.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        public bool IsSignificant(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            return !(exception is AggregateException);
        }

        /// <summary>
        /// Indicates whether the exception represents a transient error, meaning that the operation may succeed if it
        /// is retried.
        /// </summary>
        /// <param name="exception">The exception to inspect.</param>
        public bool IsTransient(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            // Allow custom exceptions to specify whether they are transient.
            if (exception is ISupportsTransient transientEx)
            {
                return transientEx.IsTransient;
            }

            // Some exceptions have error codes, some of which could be transient.
            if (exception is WebException webEx)
            {
                switch (webEx.Status)
                {
                    case WebExceptionStatus.TrustFailure:
                    case WebExceptionStatus.MessageLengthLimitExceeded:
                        return false;

                    case WebExceptionStatus.ProtocolError:
                        return webEx.Response is HttpWebResponse http && IsTransientHttpStatusCode(http.StatusCode);
                }
                return true;
            }

            // Some exceptions have error codes, some of which could be transient.
            if (exception is SocketException socEx)
            {
                switch (socEx.SocketErrorCode)
                {
                    case SocketError.AccessDenied:
                    case SocketError.AddressFamilyNotSupported:
                    case SocketError.MessageSize:
                        return false;
                }
                return true;
            }

            // Some exception types are always transient.
            return exception is ThreadAbortException || 
                exception is OperationCanceledException || 
                exception is TimeoutException ||
                // An exception that wraps a transient exception is itself transient.
                (exception.InnerException != null && IsTransient(exception.InnerException));
        }
    }
}
