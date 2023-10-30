# Created 2023-10-30 by NW

# Socket implementation in Python

import socket

SOCKET_FAMILY = socket.AF_INET # Internet
SOCKET_TYPE = socket.SOCK_STREAM # TCP socket

tcp_server_socket = socket.socket(SOCKET_FAMILY, SOCKET_TYPE)

# Current device in network (destination for packets)
TCP_HOST_IP = "127.0.0.1" 
# Current application in network
TCP_PORT = 5500
# Max data size that can be sent and received
BUFFER_SIZE = 1024

# Bind the address to the socket
tcp_server_socket.bind((TCP_HOST_IP, TCP_PORT))

# Start listening for client requests to connect
print ("Listening for connection requests...")
tcp_server_socket.listen(1)

client_socket, client_address = tcp_server_socket.accept()

print("Received request from client.")
print("Accepted request from client.")
print("Connected to: ", client_address)

TEST_MESSAGE = "Received data: "

while True:
    data = client_socket.recv(BUFFER_SIZE)
    decoded_message = data.decode()
    TEST_MESSAGE += decoded_message
    
    encoded_message = TEST_MESSAGE.encode()

    if not data: break

    print('-' * 20)
    print("Received message from client: ", decoded_message)
    print('=' * 20)

    client_socket.send(encoded_message)

print("Closing connection...")
client_socket.close()