import tp_unity_ai.tp_unity_ai as tp
import cv2


def interface():
    tp.init_communicator()
    img = tp.imread_file('Assets\logo.jpg')
    cv2.imshow('image', img)
    cv2.waitKey(0)


if __name__ == '__main__':
    interface()