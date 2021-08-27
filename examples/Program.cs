using rpcx.net.Client;
using rpcx.net.Client.ServiceDiscovery;
using System;

namespace examples
{
    public class Args
    {
        public long A { get; set; }
        public long B { get; set; }
    }

    public class Reply
    {
        public long C { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var c = new Client(new Option()
            {
                Types = rpcx.net.Shared.Protocol.Header.eType.MessagePack, // | rpcx.net.Shared.Protocol.Header.eType.Gzip,
            });
            c.Connect("tcp", "127.0.0.1:8972");
            c.OnServerMessage += C_OnServerMessage;

            var ar = new Args() { A = 10, B = 20 };
            var t1 = c.Go<Args, Reply>("Arith", "Mul", ar);
            ar = new Args() { A = 3, B = 5 };
            var t2 = c.Go<Args, Reply>("Arith", "Mul", ar);
            ar = new Args() { A = 7, B = 8 };
            var t3 = c.Go<Args, Reply>("Arith", "Mul", ar);
            var r1 = t1.Result;
            var r2 = t2.Result;
            var r3 = t3.Result;

            Console.ReadLine();
        }

        private static void C_OnServerMessage(object sender, rpcx.net.Shared.Protocol.Message msg)
        {
            throw new NotImplementedException();
        }
    }
}
