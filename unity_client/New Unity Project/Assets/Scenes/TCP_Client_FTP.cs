// 2023-11-27 by NW

// Client socket implementation in C#


// NETWORKING
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// UI
using TMPro;
using UnityEngine.UI;
using System.IO;
using System;

public class TCP_Client_FTP : MonoBehaviour
{
    [SerializeField] private TMP_Text display_text, msg_text;
    [SerializeField] private Button button;

    private string TEST_MESSAGE = "";

    private void Display_Text(string s)
    {
        display_text.text += s + "\n";
    }

    async private Task Client_Main()
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
        //var TEST_MESSAGE = "This is a test message from Unity!";
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
        string message = "Requesting to connect to server...";
        Debug.Log(message);
        Display_Text(message);

        await client.ConnectAsync(ip_endpoint);
        message = "Connected to server.";
        Debug.Log(message);
        Display_Text(message);

        while (true)
        {
            // Send message to server
            _ = await client.SendAsync(encoded_message, SocketFlags.None);
            message = "Sending message to server...";
            Debug.Log(message);
            Display_Text(message);

            message = $"Socket client sent message: \"{TEST_MESSAGE.Replace(EOM, "")}\"";
            Debug.Log(message);
            Display_Text(message);

            // Receive ack.
            var buffer = new byte[BUFFER_SIZE];
            var data = await client.ReceiveAsync(buffer, SocketFlags.None);
            var decoded_message = Encoding.UTF8.GetString(buffer, 0, data);
            if (decoded_message == "<|ACK|>")
            {
                message = new string('-', 20);
                Debug.Log(message);
                Display_Text(message);

                message = $"Received acknowledgement from server: \"{decoded_message}\"";
                Debug.Log(message);
                Display_Text(message);

                message = new string('=', 20);
                Debug.Log(message);
                Display_Text(message);

                break;
            }
        }
        message = "Closing connection...";
        Debug.Log(message);
        Display_Text(message);

        client.Shutdown(SocketShutdown.Both);
    }

    private async void Connect_Server()
    {
        Debug.Log("Client");
        TEST_MESSAGE = msg_text.text;
        await Client_Main();
    }

    private void Start()
    {
        display_text.text = "";
        // Debug.Log("Press SPACE to connect client");
        button.onClick.AddListener(Connect_Server);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Connect_Server();
        }

    }
}

// Created 2023-11-27 by NW

// Client socket implementation in C# [now with file transfer]

//namespace MyNetwork
//{
    class Client
    {

        async public static Task Client_Main()
        {

            // Current device in network (destination for packets)
            string TCP_HOST_IP = "127.0.0.1";

            // Current application in network
            int TCP_PORT = 5500;
            // Max data size that can be sent and received
            int BUFFER_SIZE = 1024;

            string SEPARATOR = "<SEPARATOR>";

            // The path of the file we want to send
            string path = "../../test_files_transfer/client/";
            Console.WriteLine("Files you can send: ");

            if (Directory.Exists(path))
            {
                string[] file_names = Directory.GetFiles(path);
                foreach (string file_path in file_names)
                {
                    // Only display the file name, not the path to the file
                    string file_name = file_path.Remove(0, path.Length);
                    Console.WriteLine(file_name);
                }
            }

            bool file_not_found = true;

            string filepath = "";
            long size = 0;
            var filename = "";

            while (file_not_found)
            {
                try
                {
                    Console.Write("Enter the name of a file to send to the server: ");
                    filename = Console.ReadLine();
                    filepath = path + filename;
                    size = new FileInfo(filepath).Length;

                    // Get the file size
                    if (size > 0)
                    {
                        Console.WriteLine("Found file \"" + filename + "\" with size of " + size + " bytes");
                        file_not_found = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Could not find file \"" + filename + "\"");
                    Console.WriteLine("Try a different file name. Make sure to select a file from the directory listed above, and include the file extension.");
                    file_not_found = true;
                    size = 0;
                }
            }

            // Create a socket 
            Console.WriteLine("Requesting to connect to server...");
            using TcpClient client = new();

            // Bind the address to the socket
            // Connect to TCP server
            await client.ConnectAsync(TCP_HOST_IP, TCP_PORT);
            Console.WriteLine("Connected to server.");

            // Send encoded file to server
            Console.WriteLine("Sending file to server...");
            string file_string = filepath + SEPARATOR + size;
            var encoded = Encoding.UTF8.GetBytes(file_string);
            client.Client.Send(encoded);
            StreamWriter sWriter = new StreamWriter(client.GetStream());
            sWriter.Flush();
            client.Client.SendFile(filepath);
            Console.WriteLine("Socket client sent file to server: \"" + filepath + "\"");

            // Receive ack.
            while (true)
            {
                var buffer = new byte[BUFFER_SIZE];
                var data = await client.Client.ReceiveAsync(buffer, SocketFlags.None);
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
            client.Client.Shutdown(SocketShutdown.Both);
        }
    }

//}