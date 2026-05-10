using Microsoft.VisualStudio.Threading;
using Nerdbank.Streams;
using StreamJsonRpc;
using System.IO;

namespace src.proto.rpcHandler
{


    internal class MemoryRPCHandler : RPCHandler
    {
        // We pass the 'server' side of a stream pair here
        public Stream ClientStream { get; }

        private MemoryRPCHandler(Stream serverStream, Stream clientStream)
            : base(new JsonRpc(serverStream))
        {
            ClientStream = clientStream;
        }

        // Factory method to create the pair and the handler
        public static MemoryRPCHandler CreatePair()
        {
            var (clientSide, serverSide) = FullDuplexStream.CreatePair();
            return new MemoryRPCHandler(serverSide, clientSide);
        }
    }
}
