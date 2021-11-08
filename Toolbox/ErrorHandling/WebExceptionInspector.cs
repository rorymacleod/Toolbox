using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Toolbox.ErrorHandling;

public class WebExceptionInspector: DefaultExceptionInspector
{
    public override void GetData(Exception exception, IDictionary<string, string> data)
    {
        base.GetData(exception, data);

        if (exception is WebException webEx)
        {
            data["Status"] = webEx.Status.ToString();

            if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse http)
            {
                data["StatusCode"] = ((int)http.StatusCode).ToString();
                data["StatusDescription"] = http.StatusDescription;
            }
        }
    }


    protected override void AppendMessage(StringBuilder sb, Exception exception)
    {
        base.AppendMessage(sb, exception);

        if (exception is WebException webEx)
        {
            if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response is HttpWebResponse http)
            {
                sb.Append($" The web response was ({(int)http.StatusCode}) {http.StatusDescription}");
            }
            else
            {
                string? err = this.GetWebExceptionStatusMessage(webEx.Status);
                if (err != null)
                {
                    sb.Append(' ');
                    sb.Append(err);
                }
            }
        }
    }

    public override bool IsTransient(Exception exception)
    {
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

        return base.IsTransient(exception);
    }

    private string? GetWebExceptionStatusMessage(WebExceptionStatus status)
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
            case WebExceptionStatus.ProtocolError:
                return "The response received from the server was complete but indicated an error at the protocol level.";
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

    private bool IsTransientHttpStatusCode(HttpStatusCode status)
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
}
