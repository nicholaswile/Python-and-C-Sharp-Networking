// 2023-11-27 by NW

// Client socket implementation in C#

// NETWORKING
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// UI
using TMPro;
using UnityEngine.UI;

// IO
using System;
using System.IO;

public class TCP_Client_FTP : MonoBehaviour
{
    [SerializeField] private TMP_Text display_text;
    [SerializeField] private Button button;

    static string path = "Assets/Screenshots";

    private void Display_Text(string s)
    {
        Debug.Log(s);
        display_text.text += s + "\n";
    }

    public async System.Threading.Tasks.Task Client_Main()
    {
        // Current device in network (destination for packets)
        string TCP_HOST_IP = "127.0.0.1";

        // Current application in network
        int TCP_PORT = 5500;
        // Max data size that can be sent and received
        int BUFFER_SIZE = 1024;

        string SEPARATOR = "<SEPARATOR>";
        string message = "";

        // The path of the file we want to send
        message = "Files you can send: ";
        Debug.Log(message);

        string filepath = "";
        long size = 0;
        string filename = "";

        if (Directory.Exists(path))
        {
            string[] file_names = Directory.GetFiles(path);
            foreach (string file_path in file_names)
            {
                if (file_path.Contains("meta"))
                {
                    continue;
                }
                // The last file in the list will be sent to the server
                filepath = file_path;
                filename = filepath.Remove(0, path.Length);
                size = new FileInfo(filepath).Length;
                // Only display the file name, not the path to the file
                Debug.Log(filename);
            }
        }

        // Create a socket 
        message = ("Requesting to connect to server...");
        Display_Text(message);

        using TcpClient client = new();

        // Bind the address to the socket
        // Connect to TCP server
        await client.ConnectAsync(TCP_HOST_IP, TCP_PORT);
        message = "Connected to server.";
        Display_Text(message);

        // Send encoded file to server
        message = ("Sending file to server...");
        Display_Text(message);  

        string file_string = filepath + SEPARATOR + size;
        var encoded = Encoding.UTF8.GetBytes(file_string);
        client.Client.Send(encoded);
        StreamWriter sWriter = new(client.GetStream());
        sWriter.Flush();
        client.Client.SendFile(filepath);
        message = ("Socket client sent file to server: \"" + filepath + "\"");
        Display_Text(message);

        // Receive ack.
        while (true)
        {
            var buffer = new byte[BUFFER_SIZE];
            var data = await client.Client.ReceiveAsync(buffer, SocketFlags.None);
            var decoded_message = Encoding.UTF8.GetString(buffer, 0, data);
            if (decoded_message == "<|ACK|>")
            {
                message = (new string('-', 20));
                message += ($"Received acknowledgement from server: \"{decoded_message}\"");
                message = (new string('=', 20));
                Display_Text(message);
                break;
            }
        }

        message = ("Closing connection...");
        Display_Text(message);
        client.Client.Shutdown(SocketShutdown.Both);
    }


    private async void Connect_Server()
    {
        Debug.Log("Client");
        await Client_Main();
    }

    private void Start()
    {
        display_text.text = "";
        button.onClick.AddListener(Connect_Server);
    }

}