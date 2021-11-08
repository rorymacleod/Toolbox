using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Toolbox.ErrorHandling;

public class SocketExceptionInspector: DefaultExceptionInspector
{
    public override void GetData(Exception exception, IDictionary<string, string> data)
    {
        base.GetData(exception, data);

        if (exception is SocketException socEx)
        {
            data["ErrorCode"] = socEx.ErrorCode.ToString();
            data["NativeErrorCode"] = socEx.NativeErrorCode.ToString();
            data["SocketErrorCode"] = socEx.SocketErrorCode.ToString();
        }
    }

    protected override void AppendMessage(StringBuilder sb, Exception exception)
    {
        base.AppendMessage(sb, exception);
        if (exception is SocketException socEx)
        {
            string? err = this.GetSocketExceptionErrorMessage(socEx.SocketErrorCode);
            if (err != null)
            {
                sb.Append(' ');
                sb.Append(err);
            }
        }
    }

    public override bool IsTransient(Exception exception)
    {
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

        return base.IsTransient(exception);
    }

    private string? GetSocketExceptionErrorMessage(SocketError error)
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
                return "The non-blocking socket already has an operation in progress.";
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
                return "An operation on a non-blocking socket cannot be completed immediately.";
        }

        return null;
    }
}
