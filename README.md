# Task-Driven-Saliency
The Task Driven Saliency repository contains the used eye tracking program, constructed task-driven fixation dataset, and the fine-tuned task-driven saliency models for the thesis "Investigating a Saliency-based Model for an Abnormal Road Traffic Incident Detection System (iSmart)."

### Eye Tracking Program
The eye tracking program is based on https://github.com/commanderking/EyeTrackerWidget. There are two folders in the Eye Tracking Program: Eye Fixation Data Gathering and Modified Files. The Eye Fixation Data Gathering folder contains the files needed for the data gathering. Run the program "EyeFixDataGathering.exe" to collect eye fixation data. The Modified Files are the files that were added or edited to create the image presentation for the eye fixation data gathering. Other files needed to connect the eye tracker and collect the eye fixation data are found in the EyeTrackerWidget repository.

### Fixation Data Post-Processing
The Fixation Data Post-Processing folder contains two files: fixation.m and SplitFixationData.py. Please refer to the user or technical manuals on how to use these.

### TaskNet Saliency Model
TaskNet Model is a task-driven saliency model that aims to predict road traffic incidents. This model outputs a saliency map that simulates the eye fixation of a human and represents the visual saliency on road traffic scenarios. 
