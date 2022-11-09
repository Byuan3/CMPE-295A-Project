import socket
import time
import com_client
import com_server


class Communicator:
    def __init__(self, host=socket.gethostname(), py_port=10101, unity_port=20202):
        self.host = host
        self.py_port = py_port
        self.unity_port = unity_port
        self.server = com_server.Com_server(self.host, self.py_port)

    def send_msg(self, msg):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        client.send_msg(msg)

    def close_msg(self):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        client.close_msg()

    def start_server(self):
        self.server.server_closed = False
        self.server.start_server()

    def close_server(self):
        self.close_msg()


if __name__ == '__main__':
    c1 = Communicator()
    c1.start_server()

    time.sleep(3)
    c1.send_msg('Hello World')

    time.sleep(3)
    c1.close_server()
