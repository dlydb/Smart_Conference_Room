function project(run_time)
cd 'C:\FALL_2018\research\LESA\Assets\Tests';
fileID = fopen('Waiting_Time.txt', 'r');
formatSpec = '%f';
cd 'C:\FALL_2018\research\LESA\Assets\Tests\Data';
wait_time = fscanf(fileID, formatSpec);
background = dlmread('background.txt');
background = imresize(background, 24, 'nearest');
people_inside = 0;
pre_position = [5, 30, 0];
for timer = 1:run_time
    disp(timer);
    if timer == 100
        pre_position = [6, 34, 0];
        people_inside = 1;
    end
    if timer == 224
        pre_position = [pre_position; 7 12 0];
        people_inside = 2;
    end
    if timer == 258
        pre_position = [pre_position;6, 34, 0];
        people_inside = 3;
    end
    if timer == 341
        pre_position = [pre_position;6, 34, 0];
        people_inside = 4;
    end
    if timer == 463
        pre_position = [pre_position; 7 12 0];
        people_inside = 5;
    end
    if timer == 638
        pre_position = [pre_position; 7 12 0];
        people_inside = 6;
    end
        
    file_name = "time" + int2str(timer) + ".txt";
    sub = dlmread(file_name);
    pre_position = find_person(sub, people_inside, pre_position);
    track = zeros(17, 45, 3);
    for i = 1:people_inside
        if pre_position(i,1) ~= -1
            %track(pre_position(i,1), pre_position(i,2), rem(i,3) + 1) = pre_position(i,3)/255;
            track(pre_position(i,1), pre_position(i,2), :) = pre_position(i,3)/255;
        end
    end
    track = imresize(track, 24, 'nearest');
    for i = 1:people_inside
       if pre_position(i,1) ~= -1
           track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+4, rem(i,3)+1) = 255;
           track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+21:(pre_position(i,2)-1)*24+24, rem(i,3)+1) = 255;
           track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+4, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+24, rem(i,3)+1) = 255;
           track((pre_position(i,1)-1)*24+21:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+24, rem(i,3)+1) = 255; 
           if i > 3
               track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+4, rem(i+1,3)+1) = 255;
               track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+21:(pre_position(i,2)-1)*24+24, rem(i+1,3)+1) = 255;
               track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+4, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+24, rem(i+1,3)+1) = 255;
               track((pre_position(i,1)-1)*24+21:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+24, rem(i+1,3)+1) = 255; 
               if i > 6
                   track((pre_position(i,1)-1)*24+1:(pre_position(i,1)-1)*24+24, (pre_position(i,2)-1)*24+1:(pre_position(i,2)-1)*24+4, rem(i+2,3)+1) = 255;
               end
           end
       end
    end
    %figure(1);
    %imshow(track, [0 255]);
    sub = imresize(sub, 24, 'nearest');
    
    baseFileName = "track" + int2str(timer + 1000) + ".png";
    fullFileName = fullfile('C:\FALL_2018\research\LESA\Assets\Tests\Data_Convert', char(baseFileName));
    imwrite(track, fullFileName);
    subFileName = "sub" + int2str(timer + 1000) + ".png";
    sub_fullFileName = fullfile('C:\FALL_2018\research\LESA\Assets\Tests\Data_Convert', char(subFileName));
    imwrite(uint8(sub), sub_fullFileName);
end


close all
cd 'C:\FALL_2018\research\LESA\Assets'
end