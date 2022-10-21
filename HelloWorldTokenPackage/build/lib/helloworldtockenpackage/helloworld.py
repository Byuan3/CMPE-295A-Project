# -----------------------------------------------------------
# helloworld module file of helloworldtockenpackage
#
# (C) 2022 SJSU-CMPE295-Team3- Prof. Harry Li
#
# -----------------------------------------------------------

import socket;
def printhelloworld():
    """
        A user program can call this function using helloworldpackage.
        This function returns the string 'HelloWorld'
    """
    return "Hello World!"


def printhelloname(name):
    """
        A user program can call this function using helloworldpackage.
        Return the variable passed with the string 'Hello'
    """
    return "Hello " + name

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
