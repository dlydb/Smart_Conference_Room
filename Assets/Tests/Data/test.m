function test(run_time)
for a = 90:100
    disp(a);
   file_name = "time" + int2str(a) + ".txt";
   sub = dlmread(file_name);
   m = imresize(sub, 24, 'nearest');
   figure(1);
   imshow(m, [0 255]);
   %baseFileName = "img" + int2str(a + 2500) + ".png";
   %fullFileName = fullfile('C:\SPR 2018\research\Data', char(baseFileName));
   %imwrite(uint8(m), fullFileName);
   %drawnow;
   pause(0.05);
end
end