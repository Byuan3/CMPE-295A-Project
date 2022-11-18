import tp_unity_ai.tp_unity_ai as tp
import cv2


def interface():
    tp.init_communicator()
    img = cv2.imread('../image/unity_logo.jpg')
    tp.send_img(img)


if __name__ == '__main__':
    interface()