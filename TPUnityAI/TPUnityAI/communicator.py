import cv2
import socket


def send_message(message):
    host = socket.gethostname()
    port = 9998

    client_socket = socket.socket()
    client_socket.connect((host, port))

    client_socket.send(message.encode())
    print('Send message to Server: ' + message)

    data = client_socket.recv(1024).decode()
    print('Received from server: ' + data)

    client_socket.close()


def send_image(path):
    _continue = True
    image = cv2.imread(path)
    host = socket.gethostname()
    port = 9998

    client = socket.socket()

    while _continue:
        try:

            client.connect((host, port))
            print("client connected")

            byteString = bytes(cv2.imencode('.jpg', image)[1].tobytes())
            fileSize = len(byteString)
            client.send(str(fileSize).encode())

            sizeConfirmation = client.recv(1024)

            totalSent = 0
            while totalSent < fileSize:
                totalSent += client.send(byteString[totalSent:])
                print("Send " + str(totalSent) + " Bytes")

            _continue = False
            print(str(fileSize), str(totalSent), sizeConfirmation.decode('utf-8'))

        except Exception as e:
            print(e)
            print("Shutting down.")
            _continue = False

    print("Exited.")