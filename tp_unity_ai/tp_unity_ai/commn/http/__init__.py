from .server import HTTPCommnServer

server = HTTPCommnServer()
default_host = ""
default_port = 8080
default_env = "Unity"


def config(host=default_host, server_port=default_port, env=default_env):
    global server, default_host, default_port, default_env
    default_host = host
    default_port = server_port
    default_env = env
    server = HTTPCommnServer(host, server_port, env)


def run():
    server.run()
