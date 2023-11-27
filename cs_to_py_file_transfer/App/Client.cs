// Created 2023-11-27 by NW

// Client socket implementation in C# [now with file transfer]

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyNetwork {
    class Client {

        async public static Task Client_Main() {

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

            if (Directory.Exists(path)) {
                string [] file_names = Directory.GetFiles(path);
                foreach (string file_path in file_names) {
                    // Only display the file name, not the path to the file
                    string file_name = file_path.Remove(0,path.Length);
                    Console.WriteLine(file_name);
                }
            }

            bool file_not_found = true;

            string filepath = "";
            long size = 0;
            var filename = "";

            while (file_not_found) {
                try {
                    Console.Write("Enter the name of a file to send to the server: ");
                    filename = Console.ReadLine();
                    filepath = path + filename;
                    size = new FileInfo(filepath).Length;

                    // Get the file size
                    if (size > 0) {
                        Console.WriteLine("Found file \"" + filename + "\" with size of " + size + " bytes");
                        file_not_found = false;
                    } 
                }
                catch (Exception e) {
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
            while (true) {
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

}