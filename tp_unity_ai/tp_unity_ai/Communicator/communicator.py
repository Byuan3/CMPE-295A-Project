import socket

import numpy as np

from .comserver import ComServer
from .comclient import ComClient
import cv2


class Communicator:
    def __init__(self, host=socket.gethostname(), py_port=10101, unity_port=20202):
        self.host = host
        self.py_port = py_port
        self.unity_port = unity_port
        self.server = ComServer(self.host, self.py_port)

    def close_msg(self):
        client = ComClient(self.host, self.unity_port)
        client.close_msg()

    def send_msg(self, msg_str):
        client = ComClient(self.host, self.unity_port)
        client.send_msg(msg_str)

    def send_image(self, img_bytes):
        client = ComClient(self.host, self.unity_port)
        bytes_array = bytes(cv2.imencode('.jpg', img_bytes)[1].tobytes())
        client.send_image(bytes_array)

    def imread_screen(self):
        client = ComClient(self.host, self.unity_port)
        img = client.imread_screen()
        img_np = cv2.imdecode(np.frombuffer(img, np.uint8), cv2.IMREAD_COLOR)
        return img_np

    def imread_file(self, file_path):
        client = ComClient(self.host, self.unity_port)
        img = client.imread_file(file_path)
        img_np = cv2.imdecode(np.frombuffer(img, np.uint8), cv2.IMREAD_COLOR)
        return img_np

    def start_server(self):
        self.server.server_closed = False
        self.server.start_server()

    def close_server(self):
        self.close_msg()

    def get_server_pipeline(self):
        if not self.server.server_closed:
            return self.server.get_data_pipeline()

    def get_server_data(self):
        if not self.server.server_closed:
            return self.server.get_last_req()


def cut_resolution_by(image, factor):
    imageResize = cv2.resize(image, (image.shape[1] // factor, image.shape[0] // factor))
    return imageResize
