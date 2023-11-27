// Created 2023-11-27 by NW 

// Server socket implementation in C# [now with file transfer]

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyNetwork {
    class Server {

        async public static Task Server_Main() {

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
            using Socket tcp_server_socket = new(SOCKET_FAMILY, SOCKET_TYPE, PROTOCOL_TYPE);
            
            // Bind the address to the socket
            tcp_server_socket.Bind(ip_endpoint);

            // Start listening for client requests to connect
            Console.WriteLine("Listening for connection requests...");
            tcp_server_socket.Listen(100);
        
            var handler = await tcp_server_socket.AcceptAsync();

            Console.WriteLine("Received request from client.");
            Console.WriteLine("Accepted request from client.");
            string client_address = "";
            client_address += handler.RemoteEndPoint;
            Console.WriteLine("Connected to: " + client_address);
            
            while (true)
            {
                // Receive message.
                var buffer = new byte[BUFFER_SIZE];
                var data = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var decoded_message = Encoding.UTF8.GetString(buffer, 0, data);
                
                var eom = "<|EOM|>";
                if (decoded_message.IndexOf(eom) > -1 /* is end of message */)
                {
                    Console.WriteLine(new string('-', 20));
                    Console.WriteLine($"Received message from client: \"{decoded_message.Replace(eom, "")}\"");
                    Console.WriteLine(new string('=', 20));

                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);

                    Console.WriteLine($"Socket server sent acknowledgment: \"{ackMessage}\"");

                    break;
                }
            }
            Console.WriteLine("Closing connection...");
        }
    }
}
