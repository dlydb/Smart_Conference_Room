function pre_position = find_person(cur_m, people_inside, pre_position)
row = 17;
col = 45;
position = [];
if people_inside == 0
    return
end
for r = 1:row
    for c = 1:col
        if (cur_m(r,c) ~= 0)
            position = [position; r c cur_m(r, c)];
        end
    end
end
shape = size(position);
num = shape(1,1);
if (num > 1)
    for i = 1:num
        for j = i + 1:num
            if ((position(i, 1) - 3 <= position(j, 1) && (position(j, 1) <= position(i, 1) + 3)) && ...
                    (position(i, 2) - 3 <= position(j, 2) && (position(j, 2) <= position(i, 2) + 3)) == 1)
                if (position(i, 3) >= position(j, 3))
                    position(j, 3) = 0;
                else
                    position(i, 3) = 0;
                end
            end
        end
    end
end
group = 0;
cur_pos = [];
for i = 1:num
    if (position(i, 3) ~= 0)
        group = group + 1;
        cur_pos = [cur_pos;position(i, 1) position(i, 2) position(i, 3)];
    end
end
hung = ones(group, people_inside);
if (group ~= 0)
    for i = 1:group
        for j = 1:people_inside
            hung(i, j) = sqrt((cur_pos(i,1) - pre_position(j,1))^2 + (cur_pos(i,2) - pre_position(j,2))^2);
        end
    end
[assign, cost] = assignmentoptimal(hung);
num = size(assign);
num = num(1,1);
for i = 1:num
    pre_position(assign(i,1),:) = cur_pos(i,:);
end
for i = 1:people_inside
    if ismember(i,assign) ~= 1
    pre_position(i,:) = pre_position(i,:);
    end
end
end
end