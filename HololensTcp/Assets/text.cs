using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text : MonoBehaviour
{
  


    private BoxCollider boxCollider;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
  
        // 获取BoxCollider的边界框
        Bounds bounds = boxCollider.bounds;

        // 获取边界框的各个点的坐标
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        // 输出各个点的坐标
        Debug.Log("Center: " + center);
        Debug.Log("Extents: " + extents);
        Debug.Log("Min: " + min);
        Debug.Log("Max: " + max);

        var objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
        objCube.name = "Cude";
        objCube.transform.position = bounds.max;

    }


}
