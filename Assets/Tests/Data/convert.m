background = dlmread('time225.txt');
background(7, 12) = 255;
b = imresize(background, 24, 'nearest');
imwrite(uint8(b), 'a.png');