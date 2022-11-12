import helloworldtockenpackage as hw


if __name__ == '__main__':
    while True:
        message = input('Message --> ')
        print('Sending ...')
        hw.send_message(message)
