import socket
from threading import Thread


class Com_server:
    def __init__(self, host=socket.gethostname(), py_port=10101, unity_port=20202):
        self.host = host
        self.py_port = py_port
        self.unity_port = unity_port
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_thread = Thread(target=self.server_thread)
        self.server_closed = True

    def server_thread(self):
        self.server_socket.bind((self.host, self.py_port))
        self.server_socket.listen(5)

        while not self.server_closed:
            print('Python server is listening...')
            self.client_socket, address = self.server_socket.accept()
            print('Got connection from', address)
            command = self.client_socket.recv(1024).decode()
            print('Got Command: ' + str(command))
            self.ack('Got Command: ' + str(command))
            self.command_parse(command)

    def rec_msg(self):
        msg = self.client_socket.recv(1024).decode()
        print('Got Message: ' + str(msg))
        self.ack("Hello, This is Python server")
        self.client_socket.close()
        return msg

    def ack(self, msg):
        self.client_socket.send(msg.encode())
        return msg

    def start_server(self):
        self.server_closed = False
        self.server_thread.start()

    def command_parse(self, msg):
        if msg == '0.Close':
            self.server_closed = True
            self.server_socket.close()
            return 'Server Closed'
        elif msg == '1.Message':
            return self.rec_msg()


