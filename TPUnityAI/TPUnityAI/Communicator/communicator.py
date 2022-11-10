import socket
import time

import numpy as np

import com_client
import com_server
import cv2


class Communicator:
    def __init__(self, host=socket.gethostname(), py_port=10101, unity_port=20202):
        self.host = host
        self.py_port = py_port
        self.unity_port = unity_port
        self.server = com_server.Com_server(self.host, self.py_port)

    def close_msg(self):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        client.close_msg()

    def send_msg(self, msg_str):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        client.send_msg(msg_str)

    def send_image(self, img_bytes):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        bytesArray = bytes(cv2.imencode('.jpg', img_bytes)[1].tobytes())
        client.send_image(bytesArray)

    def imread_screen(self):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        return client.imread_screen()

    def imread_file(self, filepath):
        # Testing with Python
        # Change py_port to unity_port before testing with Unity
        client = com_client.Com_client(self.host, self.py_port)
        return client.imread_file(filepath)

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


if __name__ == '__main__':
    c1 = Communicator()
    c1.start_server()

    time.sleep(1)
    c1.send_msg('Hello World')
    time.sleep(3)
    msg = c1.get_server_data()
    print('Req msg: ' + str(msg))
    print('-----')

    image = cv2.imread(r"../../../Images/unity_logo.jpg")
    c1.send_image(image)
    time.sleep(3)
    img = c1.get_server_data()
    print('Req msg: ' + str(len(img)))
    print('-----')
    img_np = cv2.imdecode(np.frombuffer(img, np.uint8), cv2.IMREAD_COLOR)
    cv2.imshow('image', img_np)
    cv2.waitKey(0)

    time.sleep(2)
    c1.close_server()
