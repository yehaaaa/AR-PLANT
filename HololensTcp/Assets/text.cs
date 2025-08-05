using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text : MonoBehaviour
{
  


    private BoxCollider boxCollider;


    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
  
        // ��ȡBoxCollider�ı߽��
        Bounds bounds = boxCollider.bounds;

        // ��ȡ�߽��ĸ����������
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        // ��������������
        Debug.Log("Center: " + center);
        Debug.Log("Extents: " + extents);
        Debug.Log("Min: " + min);
        Debug.Log("Max: " + max);

        var objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//����
        objCube.name = "Cude";
        objCube.transform.position = bounds.max;

    }


}
