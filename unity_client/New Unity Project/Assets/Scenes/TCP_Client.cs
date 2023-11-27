// 2023-11-02 by NW

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

public class TCP_Client : MonoBehaviour
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
