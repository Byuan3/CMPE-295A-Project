import numpy as np
import cv2 as cv
import matplotlib.pyplot as plt
img = cv.imread('/Users/manishmapakshi/Desktop/side-view-lion-walking-looking-camera-panthera-leo-side-view-lion-walking-looking-camera-panthera-leo-103837962.jpeg',0)
edges = cv.Canny(img,100,200)
plt.subplot(121),plt.imshow(img,cmap = 'gray')
plt.title('Original Image'), plt.xticks([]), plt.yticks([])
plt.subplot(122),plt.imshow(edges,cmap = 'gray')
plt.title('Edge Image'), plt.xticks([]), plt.yticks([])
plt.show()
