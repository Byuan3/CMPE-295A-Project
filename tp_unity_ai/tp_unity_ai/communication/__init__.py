import socket
from .communicator import Communicator

communicator = Communicator()
commn_host = socket.gethostname()
commn_server_port = 10101
commn_env_port = 20202
simulation_env = 'unity'


def init(init_host=socket.gethostname(), init_py_port=10101, init_env_port=20202, init_env='unity'):
    global communicator, commn_host, commn_server_port, commn_env_port, simulation_env
    commn_host = init_host
    commn_server_port = init_py_port
    commn_env_port = init_env_port
    simulation_env = init_env
    communicator = Communicator(commn_host, commn_server_port, commn_env_port, simulation_env)


def config(host=commn_host, server_port=commn_server_port, env_port=commn_env_port, env=simulation_env):
    global communicator, commn_host, commn_server_port, commn_env_port, simulation_env
    commn_host = host
    commn_server_port = server_port
    commn_env_port = env_port
    simulation_env = env

    communicator.close_server()
    communicator = Communicator(commn_host, commn_server_port, commn_env_port, simulation_env)


def listen():
    communicator.start_server()


def close():
    communicator.close_server()


def send_msg(msg_str):
    communicator.send_msg(msg_str)


def send_img(img_bytes):
    communicator.send_image(img_bytes)


def imread_screen():
    return communicator.imread_screen()


def imread_file(file_path):
    return communicator.imread_file(file_path)


def get_data_pipeline():
    return communicator.get_server_pipeline()


def get_recent_data():
    return communicator.get_server_data()

