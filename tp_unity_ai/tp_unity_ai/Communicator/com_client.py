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

    def send_command(self, command):
        self.client_socket.send(command.encode())
        command_ack = self.client_socket.recv(1024).decode()
        return command_ack

    def close_connection(self):
        if not self.isConnected:
            self.client_socket.close()
            self.isConnected = False

    def close_msg(self):
        if not self.isConnected:
            self.connect_to_unity()

        command_ack = self.send_command('0.Close')

        self.close_connection()

    def send_msg(self, msg):
        if not self.isConnected:
            self.connect_to_unity()

        command_ack = self.send_command('1.Message')

        self.client_socket.send(msg.encode())
        response = self.client_socket.recv(1024).decode()
        print('Msg Response: ' + str(response))

        self.close_connection()

    def send_image(self, img):
        if not self.isConnected:
            self.connect_to_unity()

        command_ack = self.send_command('2.Image')

        imageSize = len(img)
        self.client_socket.send(str(imageSize).encode())
        imageSize_ack = self.client_socket.recv(1024).decode()

        chunk_size = 1024
        totalSent = 0
        while totalSent < imageSize:
            totalSent += self.client_socket.send(img[totalSent:totalSent + chunk_size])
            # print("Send " + str(totalSent) + " Bytes")
        img_ack = self.client_socket.recv(1024).decode()
        print('Image Response: ' + str(img_ack))

        self.close_connection()

    def rec_img(self):
        imageSize = int(self.client_socket.recv(1024).decode())
        imageSize_ack = 'Img_Size_ACK: ' + str(imageSize) + ' Bytes'
        print(imageSize)
        print(imageSize_ack)
        self.client_socket.send(str(imageSize).encode())

        chunk_size = 1024
        img = bytes()
        totalReceived = 0
        while len(img) < imageSize:
            img += self.client_socket.recv(chunk_size)
            totalReceived = len(img)
            # print("Received " + str(totalReceived) + " Bytes")
        img_ack = 'Img_ACK: ' + str(totalReceived) + ' Bytes'
        self.client_socket.send(img_ack.encode())

        return img

    def imread_screen(self):
        if not self.isConnected:
            self.connect_to_unity()

        command_ack = self.send_command('3.imread_screen')
        print(command_ack)
        img = self.rec_img()
        self.close_connection()

        return img

    def imread_file(self, filePath):
        if not self.isConnected:
            self.connect_to_unity()

        command_ack = self.send_command('4.imread_file')

        self.client_socket.send(filePath.encode())
        img = self.rec_img()

        self.close_connection()

        return img
