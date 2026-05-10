using System.Net.Sockets;
using StreamJsonRpc;

namespace src.proto.rpcHandler
{
    internal class SocketRPCHandler : RPCHandler
    {
        private readonly Socket _socket;

        public SocketRPCHandler(Socket socket)
            : base(new JsonRpc(new NetworkStream(socket, ownsSocket: true)))
        {
            _socket = socket;
        }

        // Helper to create a handler from a hostname/port
        public static SocketRPCHandler Connect(string host, int port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);
            return new SocketRPCHandler(socket);
        }
    }
}
