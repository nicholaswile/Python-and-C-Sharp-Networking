# Created 2023-10-30 by NW

# Client socket implementation in Python

import socket

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

TEST_MESSAGE = input("Enter a message to send to the server: ")
EOM = "<|EOM|>"
TEST_MESSAGE += EOM
encoded_message = TEST_MESSAGE.encode()

# Connect to TCP server
print("Requesting to connect to server...")
tcp_client_socket.connect((TCP_HOST_IP, TCP_PORT))
print("Connected to server.")

# Send message to server
print("Sending message to server...")
tcp_client_socket.send(encoded_message)
print("Socket client sent message to server: \""+TEST_MESSAGE.replace(EOM, "")+"\"")

data = tcp_client_socket.recv(BUFFER_SIZE)
decoded_message = data.decode()

print('-' * 20)
print("Received acknowledgement from server: \""+decoded_message+"\"")
print('=' * 20)

print("Closing connection...")
tcp_client_socket.close()