# Created 2023-11-02 by NW

# Server socket implementation in Python [now with file transfer]

import os
import socket
import tqdm

SOCKET_FAMILY = socket.AF_INET # Internet
SOCKET_TYPE = socket.SOCK_STREAM # TCP socket

# Instantiate socket
print("Running Server")
tcp_server_socket = socket.socket(SOCKET_FAMILY, SOCKET_TYPE)

# Current device in network (destination for packets)
TCP_HOST_IP = "127.0.0.1" 
# Current application in network
TCP_PORT = 5500
# Max data size that can be sent and received
BUFFER_SIZE = 1024

SEPARATOR = "<SEPARATOR>"
EOM = "<EOM>"

# Bind the address to the socket
tcp_server_socket.bind((TCP_HOST_IP, TCP_PORT))

# Start listening for client requests to connect
print ("Listening for connection requests...")
tcp_server_socket.listen(1)

client_socket, client_address = tcp_server_socket.accept()

print("Received request from client.")
print("Accepted request from client.")
print("Connected to: ", client_address)

TEST_MESSAGE = "<|ACK|>"

data = client_socket.recv(BUFFER_SIZE)
decoded_message = data.decode()
filename, filesize = decoded_message.split(SEPARATOR)
filename = os.path.basename(filename)
filesize = int(filesize)

progress = tqdm.tqdm(range(filesize), f"Receiving \"{filename}\"", unit="B", unit_scale=True, unit_divisor=1024)

path = "../test_files_transfer/server/new_"
filepath = path + filename
with open(filepath, "wb") as f:
    # if client is taking too long to send image, timeout the server
    while True:
        bytes_read = client_socket.recv(BUFFER_SIZE)
        f.write(bytes_read)
        # THIS IS NOT AN IDEAL WAY TO DO THIS
        # BUT 'not bytes_read' wasn't working
        # IF AN IMAGE SIZE IS DIVISIBLE BY BUFFER_SIZE
        # THEN THIS WON'T WORK
        if len(bytes_read) < BUFFER_SIZE: break

print('-' * 20)
print("Received image from client: \""+filename+"\"" + " with a size of " + str(filesize) + " bytes")
print('=' * 20)

encoded_message = TEST_MESSAGE.encode()
client_socket.send(encoded_message)
print("Socket server sent acknowledgement: \""+TEST_MESSAGE+"\"")

print("Closing connection...")
client_socket.close()