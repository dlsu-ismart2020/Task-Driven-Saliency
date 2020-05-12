import os
import glob
import pandas as pd
import csv

#user input
path = input("Enter path: ")
img_num = []
print("Enter image numbers (enter 'end' to finish input): ")
temp = ""
while temp != "end":
    temp = input()
    if(temp != "end"):
        img_num.append(temp)

#get files in directory
os.chdir(path)
extension = 'csv'
all_files = [i for i in glob.glob('*.{}'.format(extension))]

#create csv file
for img in img_num:
    print("Now parsing: " + img + ".jpg")
    for f in all_files:
        values = []
        copy = False
        print(os.path.basename(f))
        csv_file = csv.reader(open(f, "r"))
        for row in csv_file:
            if "file" in row[0] and img not in row[0]:
                copy = False
            if copy == True:
                values.append([int(row[0]),int(row[1]),int(row[2])])
            if "file" in row[0] and img in row[0]:
                copy = True
        if values:
            values.insert(0, ["X Gaze Data", "Y Gaze Data", "Time"])
            df = pd.DataFrame(values)
            df.to_csv(img + '_' + os.path.basename(f)[:2] + ".csv", index = False, header = False)
    print(img + ".jpg csv generated.")
    
print("Splitting complete.")
