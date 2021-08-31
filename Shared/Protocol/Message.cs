using System.Collections.Generic;

namespace rpcx.net.Shared.Protocol
{
    public class Message
    {
        public Header Header { get; set; }
        public string ServicePath { get; set; }
        public string ServiceMethod { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public byte[] Payload { get; set; }
        //public byte[] Data { get; set; }

        public Message(Header header) {
            Header = header;
        }
    }
}
