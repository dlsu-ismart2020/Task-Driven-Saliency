import numpy as np
from PIL import Image
import cv2
import pdb
import matplotlib.pyplot as plt
import time
import scipy.ndimage 
import scipy.misc

# UPDATE YOUR CAFFE PATH HERE
import sys
sys.path.append('/home/titan/caffe/python/') 
import caffe
caffe.set_mode_gpu()
caffe.set_device(0)

MEAN_VALUE = np.array([103.939, 116.779, 123.68])   # BGR
MEAN_VALUE = MEAN_VALUE[:,None, None]
FINE_SCALE = np.array([1,3,600,800], dtype=np.float32)
COARSE_SCALE = np.array([1,3,300,400], dtype=np.float32)

class generate_saliencymap:
    def __init__(self, prototxtpath, model):
        self.net = caffe.Net(prototxtpath, model, caffe.TEST) 
        
    def process_the_image(self, im):
        # Put channel dimension first
        im = np.transpose(im, (2,0,1))        
        # Subtract mean
        im = im - MEAN_VALUE
        im = im[None,:]
        # Convert to Float precision
        im = im / 255 
        return im
    
    def compute_saliency(self, image_path):
        im = fine = cv2.imread(image_path) # in BGR
        im = self.process_the_image(im)
        coarse_img = scipy.ndimage.interpolation.zoom(im,tuple(COARSE_SCALE / np.asarray(im.shape, dtype=np.float32)), np.dtype(np.float32), mode='nearest')
        assert(coarse_img.shape == (1,3,300,400))
        fine_img = scipy.ndimage.interpolation.zoom(im,tuple(FINE_SCALE / np.asarray(im.shape, dtype=np.float32)), np.dtype(np.float32), mode='nearest')
        assert(fine_img.shape == (1,3,600,800))
        self.net.blobs['fine_scale'].data[...] = fine_img
        self.net.blobs['coarse_scale'].data[...] = coarse_img
        self.net.forward()
        sal_map = self.net.blobs['saliency_map_out'].data
        sal_map = sal_map[0,0,:,:]
        sal_map = sal_map - np.amin(sal_map)
        sal_map = sal_map / np.amax(sal_map)    
        sal_map = scipy.ndimage.interpolation.zoom(sal_map,tuple(np.asarray(im.shape[2:], dtype=np.float32) / np.asarray(sal_map.shape,dtype=np.float32)), np.dtype(np.float32), mode='nearest')
        return sal_map
