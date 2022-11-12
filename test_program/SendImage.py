import TPUnityAI as tp
import cv2


def cut_resolution_by(image, factor):
    imageResize = cv2.resize(image, (image.shape[1] // factor, image.shape[0] // factor))
    return imageResize


if __name__ == '__main__':
    image = cv2.imread(r"..\Images\IMG_4637.jpg")
    tp.send_image(cut_resolution_by(image, 8))
