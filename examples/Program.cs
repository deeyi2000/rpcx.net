using rpcx.net.Client;
using rpcx.net.Shared.Protocol;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace examples {
    public class Args : WithMetadata {
        public long A { get; set; }
        public long B { get; set; }
    }

    public class Reply : WithMetadata {
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

            var ar = new Args() { A = 10, B = 20 };
            // Metadata
            ar["ab"] = "1";
            ar["cd"] = "2";
            var t1 = c.Go<Args, Reply>("Arith", "Mul", ar).Result;

            ar = new Args() { A = 3, B = 5 };
            // Metadata
            ar["cd"] = "3";
            ar["ef"] = "4";
            var t2 = c.Go<Args, Reply>("Arith", "Mul", ar).Result;

            ar = new Args() { A = 7, B = 8 };
            // Metadata
            ar["gh"] = "5";
            ar["ij"] = "6";
            var t3 = c.Go<Args, Reply>("Arith", "Mul", ar).Result;

            PrintReply(t1, t2, t3);

            Console.WriteLine("Press Any Key to exit...");
            Console.ReadKey();
        }

        private static void PrintReply(params Reply[] replies) {
            foreach (var reply in replies) {
                foreach (var k in reply) {
                    Console.WriteLine($"Key={k.Key}, Value={k.Value}");
                }
                Console.WriteLine($"Reply={reply.C}");
                Console.WriteLine();
            }
        }

        private static void C_OnServerMessage(object sender, rpcx.net.Shared.Protocol.Message msg) {
            throw new NotImplementedException();
        }
    }
}
