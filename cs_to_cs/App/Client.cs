// Created 2023-10-30 by NW

// Client socket implementation in C#

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyNetwork {
    class Client {

        async public static Task Client_Main() {

            // Current device in network (destination for packets)
            // IPHostEntry ip_host_info = await Dns.GetHostEntryAsync("host.contoso.com");
            // IPAddress TCP_HOST_IP = ip_host_info.AddressList[0];
            IPAddress TCP_HOST_IP = IPAddress.Parse("127.0.0.1");

            // Current application in network
            int TCP_PORT = 5500;
            // Max data size that can be sent and received
            int BUFFER_SIZE = 1024;

            // Create address which will be bound to the socket
            IPEndPoint ip_endpoint = new(TCP_HOST_IP, TCP_PORT);

            AddressFamily SOCKET_FAMILY = ip_endpoint.AddressFamily; // Internet
            SocketType SOCKET_TYPE = SocketType.Stream; // TCP socket
            ProtocolType PROTOCOL_TYPE = ProtocolType.Tcp; // TCP protocol
            
            // Create a socket 
            using Socket client = new(SOCKET_FAMILY, SOCKET_TYPE, PROTOCOL_TYPE);
            
            // Bind the address to the socket
            // Connect to TCP server
            Console.WriteLine("Requesting to connect to server...");
            await client.ConnectAsync(ip_endpoint);
            Console.WriteLine("Succesfully connected to server.");

            Console.Write("Enter a message to send to the server: ");
            var TEST_MESSAGE = "";
            TEST_MESSAGE += Console.ReadLine();
            TEST_MESSAGE += "<|EOM|>";
            var encoded_message = Encoding.UTF8.GetBytes(TEST_MESSAGE);

            while (true)
            {
                // Send message to server
                _ = await client.SendAsync(encoded_message, SocketFlags.None);
                Console.WriteLine($"Socket client sent message: \"{TEST_MESSAGE}\"");

                // Receive ack.
                var buffer = new byte[BUFFER_SIZE];
                var data = await client.ReceiveAsync(buffer, SocketFlags.None);
                var decoded_message = Encoding.UTF8.GetString(buffer, 0, data);
                if (decoded_message == "<|ACK|>")
                {
                    Console.WriteLine(new string('-', 20));
                    Console.WriteLine($"Received acknowledgement from server: \"{decoded_message}\"");
                    Console.WriteLine(new string('=', 20));
                    break;
                }
            }
            Console.WriteLine("Closing connection...");
            client.Shutdown(SocketShutdown.Both);
        }
    }

}