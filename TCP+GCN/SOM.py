import numpy as np
import matplotlib.pyplot as plt
import random
import os
import pandas as pd

# Return the (g,h) index of the BMU in the grid
def find_BMU(SOM, x):
    distSq = (np.square(SOM - x)).sum(axis=1)
    return np.unravel_index(np.argmin(distSq, axis=None), distSq.shape)


# Update the weights of the SOM cells when given a single training example
# and the model parameters along with BMU coordinates as a tuple
def update_weights(SOM, train_ex, learn_rate, radius_sq,
                   BMU_coord, step=3):
    g, h = BMU_coord
    # if radius is close to zero then only BMU is changed
    if radius_sq < 1e-3:
        SOM[g, h, :] += learn_rate * (train_ex - SOM[g, h, :])
        return SOM
    # Change all cells in a small neighborhood of BMU
    for i in range(max(0, g - step), min(SOM.shape[0], g + step)):
        for j in range(max(0, h - step), min(SOM.shape[1], h + step)):
            dist_sq = np.square(i - g) + np.square(j - h)
            dist_func = np.exp(-dist_sq / 2 / radius_sq)
            SOM[i, j, :] += learn_rate * dist_func * (train_ex - SOM[i, j, :])
    return SOM
def update_weights1(SOM, train_ex, learn_rate, radius_sq,
                   BMU_coord, step=1):
    g = BMU_coord[0]
    g=SOM[g]
    # if radius is close to zero then only BMU is changed
    if radius_sq < 1e-3:
        SOM[g, :] += learn_rate * (train_ex - SOM[g, :])
        return SOM

    dis=(np.square(SOM-g)).sum(axis=1)
    indices = np.argpartition(dis, step)[:step]
    indices = indices[np.argsort(dis[indices])]
    dist_sq = np.square(SOM[indices] - g)
    dist_func = np.exp(-dist_sq / 2 / radius_sq)
    SOM[indices, :] += learn_rate * dist_func * (train_ex - SOM[indices, :])
    return SOM

def update_weights2(SOM, train_ex, learn_rate, radius_sq,
                   BMU_coord, step=1):
    g = BMU_coord[0]
    # if radius is close to zero then only BMU is change

    SOM[g, :] += learn_rate * (train_ex - SOM[g, :])
    return SOM



# Main routine for training an SOM. It requires an initialized SOM grid
# or a partially trained grid as parameter
def train_SOM(SOM, train_data, learn_rate=.01, radius_sq=1,
              lr_decay=.1, radius_decay=.1, epochs=10):
    learn_rate_0 = learn_rate
    radius_0 = radius_sq
    for epoch in np.arange(0, epochs):
        random.shuffle(train_data)
        for train_ex in train_data:
            g, h = find_BMU(SOM, train_ex)
            SOM = update_weights(SOM, train_ex,
                                 learn_rate, radius_sq, (g, h))
        # Update learning rate and radius
        learn_rate = learn_rate_0 * np.exp(-epoch * lr_decay)
        radius_sq = radius_0 * np.exp(-epoch * radius_decay)
    return SOM
def train_SOM1(SOM, train_data, learn_rate=1, radius_sq=1.5,
              lr_decay=.1, radius_decay=.1, epochs=10):
    learn_rate_0 = learn_rate
    radius_0 = radius_sq
    total_epochs=epochs
    for epoch in np.arange(0, epochs):

        for train_ex in train_data:
            g = find_BMU(SOM, train_ex)
            SOM = update_weights1(SOM, train_ex,
                                 learn_rate, radius_sq, g)
        # Update learning rate and radius
        learn_rate = learn_rate_0 * np.exp(-epoch * lr_decay)
        radius_sq = radius_0 * np.exp(-epoch * radius_decay)
        #learn_rate=learn_rate_0*(1+2*epoch/total_epochs)
        #radius_sq=radius_0*(1+2*epoch/total_epochs)
        SOM,point_list=fix_point(SOM,train_data)
    return SOM,point_list

def fix_point(SOM,train_data):
    point_list=[]
    for i,p in enumerate(SOM):
        distSq = (np.square(train_data - p)).sum(axis=1)
        idx=np.argmin(distSq)
        SOM[i]=train_data[idx]
        point_list.append(idx)
    return SOM,point_list


def process_point_cloud(input_path, output_path, n_som=25, epochs=40):
    import numpy as np
    import pandas as pd
    rand = np.random.RandomState(0)
    # 读取点云数据
    pcd_data = pd.read_csv(input_path, delimiter=" ", skiprows=10, header=None)
    train_data = pcd_data.to_numpy(dtype=np.dtype(float))[:, :3]
    idx = rand.randint(0, len(train_data), (n_som, 1))
    SOM = train_data[idx]
    SOM = SOM[:, :3].squeeze(axis=1)
    SOM, list_idx = train_SOM1(SOM, train_data, epochs=epochs)
    zeros1 = np.zeros(len(train_data))
    zeros2 = np.zeros(len(train_data))
    zeros2[list_idx] = 1
    augmented_data = np.column_stack((train_data, zeros1, zeros2))
    # 读取原文件前10行
    with open(input_path, 'r', encoding='utf-8') as f:
        header_lines = [next(f) for _ in range(10)]
    # 保存新文件
    with open(output_path, 'w', encoding='utf-8') as f:
        f.writelines(header_lines)
        np.savetxt(f, augmented_data, fmt="%.6f %.6f %.6f %d %d")





