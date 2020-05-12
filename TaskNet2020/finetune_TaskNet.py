# iSmart-VGG Python Code: Fine-tuning using ARI Dataset
import numpy as np
from PIL import Image
import pdb
import matplotlib.pyplot as plt
import sys
import time
import cv2

# UPDATE YOUR CAFFE PATH HERE
sys.path.append('/home/titan/caffe/python/') 
import caffe
caffe.set_mode_gpu()
caffe.set_device(0)

# UPDATE your Training Data Path
training_data_path = './DATASET/ARI/' 
size = 359	    # Sample Size
start = 10001	# Image File Name

# UPDATE Number of epoch
stepsize = 3590
run_epoch = 30

# Counters
start_time = time.time()
idx_counter = 0
count_ep = 1

# UPDATE SOLVER & MODEL HERE	
solver_path = './ARCHITECTURE/VGG16/VGG16_solver.prototxt'
old_model = './ARCHITECTURE/caffemodel/initialmodel_VGG16.caffemodel'
new_model = 'train_output/finetuned_TaskNet_{}.caffemodel'

solver = caffe.SGDSolver(solver_path)
solver.net.copy_from(old_model)
solver.net.save(new_model.format(idx_counter))

MEAN_VALUE = np.array([103.939, 116.779, 123.68]) # BGR
MEAN_VALUE = MEAN_VALUE[:,None, None]
fine_imgs = []
coarse_imgs = []
fix_imgs = []

print (' Start Training: TaskNet....')
print ('    Start Time: ' + str(time.ctime(start_time)))
print (' Solver: ' + solver_path)
print (' Model: ' + old_model)

# Preprocess: FINE Images
for i in range(start, start+size):
    # Load Image
    im = cv2.imread(training_data_path + 'images/' + str(i) + '.jpg')
    im = cv2.resize(im, (800,600), interpolation=cv2.INTER_LINEAR)	
    im = np.array(im, dtype=np.float32)	# in RGB    
    # Put channel dimension first [Length (0) x Width (1) x Channel (2)]
    im = np.transpose(im, (2,0,1))
    # Subtract mean
    im = im - MEAN_VALUE
    im = im[None,:]
    assert(im.shape == (1,3,600,800))
    # TEST - CONVERT TO DOUBLE
    im = im / 255
    im = im.astype(np.dtype(np.float32))
    fine_imgs.append(im)

# Preprocess: COARSE Images
for i in range(start, start+size):    
	# Load Image
    im = cv2.imread(training_data_path + 'images/' + str(i) + '.jpg')
    im = cv2.resize(im, (400,300), interpolation=cv2.INTER_AREA)	
    im = np.array(im, dtype=np.float32)	# in RGB    
	# Put channel dimension first [Length (0) x Width (1) x Channel (2)]
    im = np.transpose(im, (2,0,1))   
    # Subtract Mean
    im = im - MEAN_VALUE
    im = im[None,:]
    assert(im.shape == (1,3,300,400))
    # TEST - CONVERT TO DOUBLE
    im = im / 255
    im = im.astype(np.dtype(np.float32))
    coarse_imgs.append(im)

# Preprocess: Fixation Maps
for i in range(start, start+size):
	# Load Fixation Map
	im = cv2.imread(training_data_path + 'fixation_images/' + str(i) + '.jpg', cv2.IMREAD_GRAYSCALE)
	im = cv2.resize(im, (50,38), interpolation=cv2.INTER_AREA)
	im = np.array(im, dtype=np.float32)	# in RGB  	
	im = im[None,None,:]
	assert(im.shape == (1,1,38,50))
	# TEST - CONVERT TO DOUBLE
	im = im / 255
	im = im.astype(np.dtype(np.float32))
	fix_imgs.append(im)

# Check Lengths
assert(len(fix_imgs) == size)
assert(len(fix_imgs) == len(fine_imgs) and len(fine_imgs) == len(coarse_imgs))

# TRAINING THE ARCHITECTURE
while (count_ep % (run_epoch+1) != 0):    
	# Randomize Training Data
	sample = np.random.permutation(len(fix_imgs))
    
	for i in range(0, len(sample)):
		idx_counter = idx_counter + 1
		print ('  [EPOCH ' + str(count_ep) + '] Working on ' + str(i+1) + ' of ' + str(size))
		
		fine_img_to_process = fine_imgs[sample[i]]
		coarse_img_to_process = coarse_imgs[sample[i]]
		fix_img_to_process = fix_imgs[sample[i]]
		
		# print ('starts fine image loading ...')
		solver.net.blobs['fine_scale'].data[...] = fine_img_to_process
		# print ('starts coarse image loading ...')
		solver.net.blobs['coarse_scale'].data[...] = coarse_img_to_process
		# print ('starts fixation loading ...')
		solver.net.blobs['ground_truth'].data[...] = fix_img_to_process
		# print ('starts forward pass ...')
		solver.step(1)
		# print ('finished forward pass ...')
        
        # Save model after every Stepsizes
		if (int(idx_counter) % stepsize) == 0:
			print ('  Decrease Learning Rate...')
			solver.net.save(new_model.format(idx_counter))
    
	# Save model after every Epoch
	solver.net.save(new_model.format(idx_counter))
	count_ep = count_ep + 1	# Increase for the next epoch

print (' Done Training: TaskNet....')
print ('    Start Time: ' + str(time.ctime(start_time)))
print ('    End Time: ' + str(time.ctime(time.time())))
print (' Solver: ' + solver_path)
print (' Model: ' + old_model)
print (' New Model: ' + new_model)
