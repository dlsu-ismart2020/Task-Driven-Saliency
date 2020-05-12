% The ground-truth fixation maps are constructed by convolving a Gaussian kernel with a standard deviation of one degree of visual angle 
clear;

% Open folder selection dialog box, for selecting input and output folders
indir = uigetdir(cd, 'Select INPUT Folder');
directory = dir([indir, '\', '*.csv']);

for i = 1 : length(directory)
    
    fprintf(directory(i).name + "\n")
    filename = extractBetween(directory(i).name,1,strlength(directory(i).name)-4);
    
    % CREATE BLACK IMAGE
    blackImage = 0 * ones(768,1024); %(768,1024)
    imgsize = size(blackImage);

    % READ CSV FILE
    pixel_val = readtable([indir, '\', directory(i).name], 'HeaderLines',1);
    pixel_val = table2array(pixel_val);

    % PLOTTING X & Y FIXATION COORDINATES
    for k=1:1:size(pixel_val)
        if(pixel_val(k,2) >= 1 && pixel_val(k,1)-171 >= 1)
            if (pixel_val(k,1)-171 <= imgsize(2) && pixel_val(k,2) <= imgsize(1))
                blackImage(pixel_val(k,2), pixel_val(k,1)-171) = 255;
            end
        end
    end
    
    % SAVE FIXATION LOCATION IMAGE
    fixLoc = blackImage;
    imwrite(fixLoc, "fix_" + filename + ".jpg");
    
    % GAUSSIAN FILTER
    sd = 24;
    img_gauss = imgaussfilt(blackImage,sd);
    
    % NORMALIZE
    min1 = min(min(img_gauss));
    max1 = max(max(img_gauss));
    img_gauss = uint8(255 .* ((double(img_gauss)-double(min1))) ./ double(max1-min1));
    
    % SAVE FIXATION MAP IMAGE
    imwrite(img_gauss, filename + ".jpg");
end
