# Created 2023-11-02 by NW

# Client socket implementation in Python [now with file transfer]

import os
import socket
import tqdm # File transfer progress bar

SOCKET_FAMILY = socket.AF_INET # Internet
SOCKET_TYPE = socket.SOCK_STREAM # TCP socket

print("Running Client")
tcp_client_socket = socket.socket(SOCKET_FAMILY, SOCKET_TYPE)

# Current device in network (destination for packets)
TCP_HOST_IP = "127.0.0.1" 
# Current application in network
TCP_PORT = 5500
# Max data size that can be sent and received
BUFFER_SIZE = 1024

SEPERATOR = "<SEPARATOR>"
EOM = "<EOM>"

# The path of the file we want to send
path = "../test_files_transfer/client/"
print("Files you can send: ")
dir_list = os.listdir(path)
print(dir_list)

file_not_found = True

while (file_not_found):
    try:   
        filename = input("Enter the name of a file to send to the server: ")
        filepath = path + filename

        # Get the file size
        filesize = os.path.getsize(filepath)
        if (filesize > 0):
            print("Found file \"" + filename + "\" with size of " + str(filesize) + " bytes")
            file_not_found = False

    except:
        print("Could not find file \"" + filename + "\"")
        print("Try a different file name. Make sure to select a file from the directory listed above, and include the file extension.")
        file_not_found = True

# Connect to TCP server
print("Requesting to connect to server...")
tcp_client_socket.connect((TCP_HOST_IP, TCP_PORT))
print("Connected to server.")

# Send encoded file to server
print("Sending file to server...")
tcp_client_socket.send(f"{filepath}{SEPARATOR}{filesize}{EOM}".encode())
print("Socket client sent file to server: \""+filepath+"\"")

# Progress bar sending the file
progress = tqdm.tqdm(range(filesize), f"Sending {filename}", unit="B", unit_scale=True, unit_divisor=BUFFER_SIZE)
with open (filename, "rb") as f:
    while True:
        # read bytes from file
        bytes_read = f.read(BUFFER_SIZE)
        if not bytes_read:
            # Done sending the file
            break
        tcp_socket_client.sendall(bytes_read)
        # update bar
        progress.update(len(bytes_read))

data = tcp_client_socket.recv(BUFFER_SIZE)
decoded_message = data.decode()

print('-' * 20)
print("Received acknowledgement from server: \""+decoded_message+"\"")
print('=' * 20)

print("Closing connection...")
tcp_client_socket.close()