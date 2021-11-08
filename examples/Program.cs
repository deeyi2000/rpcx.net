using rpcx.net.Client;
using rpcx.net.Shared;
using rpcx.net.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace examples {
    public class Args {
        public long A { get; set; }
        public long B { get; set; }
    }

    public class Reply {
        public long C { get; set; }
    }

    class Program {
        static void Main(string[] args) {
            // NOTE: 必须将证书通过如下命令合并后才能使用。
            // https://blog.csdn.net/yiquan_yang/article/details/113251552
            // openssl pkcs12 -export -in client.pem -inkey client.key -out client.pfx

            // 双向证书
            var pfx = Path.Combine("conf", "normal", "client.pfx");
            using var clientCert = new X509Certificate2(pfx);

            var c = new Client(new Option() {
                //ClienetCert = clientCert, // 客户端证书
                // TargetHost = "01-matrix", // 如果同时客户端证书, 则该参数无效
                Types = Header.eType.MessagePack, // | rpcx.net.Shared.Protocol.Header.eType.Gzip,
            });

            c.Connect("tcp", "127.0.0.1:8972");
            c.OnServerMessage += C_OnServerMessage;

            var md = new Dictionary<string, string> { { "ab", "1" }, { "cd", "2" } };

            var ctx1 = Context.WithMetadata(null, md);
            var ar = new Args() { A = 10, B = 20 };
            var t1 = c.Go<Args, Reply>(ctx1, "Arith", "Mul", ar).Result;
            PrintReply(ctx1, t1);

            var ctx2 = Context.WithMetadata(null, md);
            ar = new Args() { A = 3, B = 5 };
            var t2 = c.Go<Args, Reply>(ctx2, "Arith", "Mul", ar).Result;
            PrintReply(ctx2, t2);

            var ctx3 = Context.WithMetadata(null, md);
            ar = new Args() { A = 7, B = 8 };
            var t3 = c.Go<Args, Reply>(ctx3, "Arith", "Mul", ar).Result;
            PrintReply(ctx3, t3);

            Console.WriteLine("Press Any Key to exit...");
            Console.ReadKey();
        }

        private static void PrintReply(IContext ctx, Reply reply) {
            Console.WriteLine("-----------------------------------------------------");
            while (ctx != null) {
                var key = ctx.Key();
                var val = ctx.Value(key);
                var json = Encoding.UTF8.GetString(Utf8Json.JsonSerializer.Serialize(val));
                Console.WriteLine($"Key = {key} Value = {json}");

                ctx = ctx.Parent();
            }

            Console.WriteLine($"Reply = {reply.C}");
            Console.WriteLine();
        }

        private static void C_OnServerMessage(object sender, rpcx.net.Shared.Protocol.Message msg) {
            throw new NotImplementedException();
        }
    }
}
