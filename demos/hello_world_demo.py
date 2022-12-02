import tp_unity_ai.tp_unity_ai as tp


def interface():
    tp.commn.init()
    tp.commn.send_msg("Hello Unity Server")


if __name__ == '__main__':
    interface()
