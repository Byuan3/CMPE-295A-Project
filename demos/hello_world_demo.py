import tp_unity_ai.tp_unity_ai as tp


def interface():
    tp.init_communicator()
    tp.send_msg("Hello Unity Server")


if __name__ == '__main__':
    interface()
