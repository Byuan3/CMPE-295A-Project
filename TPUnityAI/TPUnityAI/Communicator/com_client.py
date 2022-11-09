import socket


class Com_client:
    def __init__(self, host=socket.gethostname(), unity_port=20202):
        self.host = host
        self.unity_port = unity_port
        self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.isConnected = False

    def connect_to_unity(self):
        if not self.isConnected:
            self.client_socket.connect((self.host, self.unity_port))
            self.isConnected = True
            print('Client connected')
        else:
            print('Client already connected')

    def send_msg(self, msg):
        if not self.isConnected:
            self.connect_to_unity()
        self.client_socket.send('1.Message'.encode())
        command_ack = self.client_socket.recv(1024).decode()
        self.client_socket.send(msg.encode())
        response = self.client_socket.recv(1024).decode()
        print('Response: ' + str(response))
        self.client_socket.close()
        self.isConnected = False

    def close_msg(self):
        if not self.isConnected:
            self.connect_to_unity()
        self.client_socket.send('0.Close'.encode())
        command_ack = self.client_socket.recv(1024).decode()
        self.client_socket.close()
        self.isConnected = False

