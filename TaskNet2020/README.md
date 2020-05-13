# TaskNet Saliency Model
TaskNet Model is a task-driven saliency model that aims to predict road traffic incidents. This model outputs a saliency map that simulates the eye fixation of a human and represents the visual saliency on road traffic scenarios. 

The model has three architectures integrated with different Deep Neural Networks pre-trained for object detection, specifically VGG16, GoogLeNet22, and ResNet50. Each architecture is fine-tuned using three variations of the TaskFix dataset, which is available at this **[link](https://tinyurl.com/TaskFix2020)**. The model has a total of nine versions, caffemodels can be downloaded **[here](https://tinyurl.com/TaskNetCaffemodels)**.

### Using TaskNet

#### Requirements
1.  **Python**

    The pycaffe is compatible with Python 2.7. or Python 3.3+.
    The following packages should be installed as well: numpy, PIL, matplotlib, and scipy.
2.  **Caffe and pycaffe**

    Instructions and requirements for the installation of Caffe and pycaffe can be found at **[link](http://caffe.berkeleyvision.org/installation.html)**.
    After installing, go to ```` makefile.config ```` file and uncomment the line ```` WITH_PYTHON_LAYER := 1 ```` to enable the Python layer.
    
#### Generating Saliency Map
1.  Download the needed files.
2.  Update the path in ```` generate_saliencymap.py ```` where caffe python is installed.
3.  Update the following in the run_generatemap.py file:
**IMPORTANT:**	always edit and check the file/folder path if the directory is manually changed.
    * prototxt file path
    * Tasknet caffemodel file path
    * input & output folder path  (images must be a jpg file and filename should be numeric)	
    * variable size and start (start is for filename of the image)	
4.  Run python and execute the command: 
    ```` python 
         python run_generatemap.py 
    ````
5.  The output saliency maps are found at the variable ```` output_path ```` indicated in the ```` run_generatemap.py ```` file.
    
    

