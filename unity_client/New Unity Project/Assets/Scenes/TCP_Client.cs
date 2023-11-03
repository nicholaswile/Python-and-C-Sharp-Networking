// Created 2023-10-30 and adapted 2023-11-02 by NW

// Client socket implementation in C#

using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TCP_Client : MonoBehaviour
{
    async public static Task Client_Main()
    {
        // Current device in network (destination for packets)
        // IPHostEntry ip_host_info = await Dns.GetHostEntryAsync("host.contoso.com");
        // IPAddress TCP_HOST_IP = ip_host_info.AddressList[0];
        IPAddress TCP_HOST_IP = IPAddress.Parse("127.0.0.1");

        // Current application in network
        int TCP_PORT = 5500;
        // Max data size that can be sent and received
        int BUFFER_SIZE = 1024;

        Debug.Log("Enter a message to send to the server: ");
        var TEST_MESSAGE = "This is a test message from Unity!";
        //TEST_MESSAGE += Console.ReadLine();
        var EOM = "<|EOM|>";
        TEST_MESSAGE += EOM;
        var encoded_message = Encoding.UTF8.GetBytes(TEST_MESSAGE);

        // Create address which will be bound to the socket
        IPEndPoint ip_endpoint = new(TCP_HOST_IP, TCP_PORT);

        AddressFamily SOCKET_FAMILY = ip_endpoint.AddressFamily; // Internet
        SocketType SOCKET_TYPE = SocketType.Stream; // TCP socket
        ProtocolType PROTOCOL_TYPE = ProtocolType.Tcp; // TCP protocol

        // Create a socket 
        using Socket client = new(SOCKET_FAMILY, SOCKET_TYPE, PROTOCOL_TYPE);

        // Bind the address to the socket
        // Connect to TCP server
        Debug.Log("Requesting to connect to server...");
        await client.ConnectAsync(ip_endpoint);
        Debug.Log("Connected to server.");

        while (true)
        {
            // Send message to server
            _ = await client.SendAsync(encoded_message, SocketFlags.None);
            Debug.Log("Sending message to server...");
            Debug.Log($"Socket client sent message: \"{TEST_MESSAGE.Replace(EOM, "")}\"");

            // Receive ack.
            var buffer = new byte[BUFFER_SIZE];
            var data = await client.ReceiveAsync(buffer, SocketFlags.None);
            var decoded_message = Encoding.UTF8.GetString(buffer, 0, data);
            if (decoded_message == "<|ACK|>")
            {
                Debug.Log(new string('-', 20));
                Debug.Log($"Received acknowledgement from server: \"{decoded_message}\"");
                Debug.Log(new string('=', 20));
                break;
            }
        }
        Debug.Log("Closing connection...");
        client.Shutdown(SocketShutdown.Both);
    }

    private void Start()
    {
        Debug.Log("Press SPACE to connect client");
    }

    // Update is called once per frame
    async private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Client");
            await Client_Main();
        }
    }
}
