using System;
using System.Collections.Generic;
using System.Text;

namespace src.proto.rpcHandler
{
    internal abstract class RPCHandler
    {
        protected StreamJsonRpc.JsonRpc rpcServer { get; init; }
        protected RPCHandler(StreamJsonRpc.JsonRpc rpcServer)
        {
            this.rpcServer = rpcServer;
            this.rpcServer.StartListening();
        }

    }
}
