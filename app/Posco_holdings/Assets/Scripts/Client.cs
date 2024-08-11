using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketTest
{
    class Client
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a message to send.");
                return;
            }

            string message = args[0];

            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(new IPEndPoint(IPAddress.Parse("192.168.0.6"), 5000));

                var data = Encoding.UTF8.GetBytes(message);

                client.Send(BitConverter.GetBytes(data.Length));
                client.Send(data);

                data = new byte[4];
                client.Receive(data, data.Length, SocketFlags.None);
                Array.Reverse(data);

                data = new byte[BitConverter.ToInt32(data, 0)];
                client.Receive(data, data.Length, SocketFlags.None);

                Console.WriteLine(Encoding.UTF8.GetString(data));
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}