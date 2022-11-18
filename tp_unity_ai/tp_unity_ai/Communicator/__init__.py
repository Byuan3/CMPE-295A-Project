import socket
from .communicator import Communicator

c = Communicator()


def init_communicator(host=socket.gethostname(), py_port=10101, unity_port=20202):
    global c
    c = Communicator(host, py_port, unity_port)


def start_listening():
    c.start_server()


def send_msg(msg_str):
    c.send_msg(msg_str)


def send_img(img_bytes):
    c.send_image(img_bytes)


def imread_screen():
    return c.imread_screen()


def imread_file(file_path):
    return c.imread_file(file_path)


def get_data_pipeline():
    return c.get_server_pipeline()


def get_recent_data():
    return c.get_server_data()

