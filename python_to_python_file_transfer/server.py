# Created 2023-11-02 by NW

# Server socket implementation in Python [now with file transfer]

import socket

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

while True:
    data = client_socket.recv(BUFFER_SIZE)
    decoded_message = data.decode()
    encoded_message = TEST_MESSAGE.encode()

    if not data: break

    # If message comes from a C# client
    eom = "<|EOM|>"
    decoded_message = decoded_message.replace(eom, "")

    print('-' * 20)
    print("Received message from client: \""+decoded_message+"\"")
    print('=' * 20)

    client_socket.send(encoded_message)
    print("Socket server sent acknowledgement: \""+TEST_MESSAGE+"\"")

print("Closing connection...")
client_socket.close()