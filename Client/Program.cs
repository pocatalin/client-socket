using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 8080;
        private const string IP = "192.168.1.122";
        
        static void Main()
        {
            Console.Title = "Client";
            ConnectToServer();
            RequestLoop();
            Exit();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected && attempts < 5)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    ClientSocket.Connect(IPAddress.Parse(IP), PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            if (ClientSocket.Connected)
            {
                Console.Clear();
                Console.WriteLine($"{ReceiveResponse()}");
            }
            else
            {
                Console.WriteLine("Failed to connect!");
                Exit();
            }
        }


        private static string ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return string.Empty;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            return Encoding.ASCII.GetString(data);
        }

        private static void RequestLoop()
        {
            Console.WriteLine(@"""exit"" to disconnect");

            while (true)
            {
                SendRequest();
                string response = ReceiveResponse();
                Console.WriteLine(response);
            }
        }


        private static void Exit()
        {
            SendString("exit");
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            Environment.Exit(0);
        }

        private static void SendRequest()
        {
            string request = Console.ReadLine();
            SendString(request);

            if (request.ToLower() == "exit")
            {
                Exit();
            }
        }

        private static void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
