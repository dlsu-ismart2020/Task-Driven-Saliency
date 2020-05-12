import time
import cv2
from scipy.misc import imsave
from generate_saliencymap import generate_saliencymap
start_time = time.time()

# UPDATE PROTOTXT & MODEL
prototxtpath = './ARCHITECTURE/VGG16/VGG16.prototxt'
model = 'model_files/TaskNet1.caffemodel'

# UPDATE FOLDER PATH
input_path	= './Dataset/TaskFix/test_images/'
output_path	= './output_salmaps/'

# UPDATE COUNTERS
start = 10719
size = 718

gen_map = generate_saliencymap(prototxtpath, model)

print ("Model: " + model)
print ("Prototxt: " + prototxtpath)
print ("Output: " + output_path)

count = 1
for ctr in range(start, start+size):	
	salmap = gen_map.compute_saliency(input_path + str(ctr) + '.jpg')	
	imsave(output_path + str(ctr) + '.jpg', salmap)
	print ("   Processing (" + str(count) + " of " + str(size) + "): " + str(ctr) + '.jpg')
	count = count + 1	

print ("DONE SAVING...")
print ('  Start Time: ' + str(time.ctime(start_time)))
print ('  End Time: ' + str(time.ctime(time.time())))
print ("Model: " + model)
print ("Prototxt: " + prototxtpath)
print ("Output: " + output_path)

