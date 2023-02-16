from http.server import HTTPServer
from .req_handler import RequestHandler


class HTTPCommnServer:
    def __init__(self, host='', port=8080, env='unity'):
        self.host = host
        self.port = port
        self.env = env

    def run(self, server_class=HTTPServer, handler_class=RequestHandler):
        server_address = (self.host, self.port)
        httpd = server_class(server_address, handler_class)
        print('Server running at http://localhost:8080/')
        httpd.serve_forever()


if __name__ == '__main__':
    http_commn = HTTPCommnServer()
    http_commn.run()