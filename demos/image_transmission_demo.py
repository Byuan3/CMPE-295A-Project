import tp_unity_ai.tp_unity_ai as tp
import cv2


def interface():
    tp.commn.init()
    img = cv2.imread('../image/IMG_4637.jpg')
    downscaled_img = tp.cv.downscale(img, 4)
    tp.commn.send_img(downscaled_img)


if __name__ == '__main__':
    interface()
