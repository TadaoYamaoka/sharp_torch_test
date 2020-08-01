import torch
import numpy as np
import ctypes

def function(pointer, size):
    print(type(pointer))
    array = (ctypes.c_float * size).from_address(pointer)
    data = np.ctypeslib.as_array(array)
    print(data)
    tensor = torch.as_tensor(data, dtype=torch.float32)
    print(tensor)

    tensor[0] = 10
