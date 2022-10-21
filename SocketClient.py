import socket


def send_message(message):
    host = socket.gethostname()
    port = 9998

    client_socket = socket.socket()
    client_socket.connect((host, port))

    client_socket.send(message.encode())
    print('Send message to Server: ' + message)

    data = client_socket.recv(1024).decode()
    print('Received from server: ' + data)

    client_socket.close()


if __name__ == '__main__':
    send_message("Hello World")
