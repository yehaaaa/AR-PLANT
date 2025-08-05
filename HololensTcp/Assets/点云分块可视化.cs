using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Debug = UnityEngine.Debug;
using Application = UnityEngine.Application;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Diagnostics;
using System;
using System.Linq;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UIElements;
using UnityEditor;
using static NewBehaviourScript1;
using Unity.VisualScripting;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection.Emit;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class 点云分块可视化 : MonoBehaviour
{
    private int[] SOM_List = new int[1000];
    private Color[] acolor = new Color[20];
    private int lineCount;
    public Material material;
    public struct pcdData
    {
        public float[] m_fX;
        public float[] m_fY;
        public float[] m_fZ;
        public int[] m_label;
    }

    private pcdData LoadFile_Point(string strFile)
    {
        pcdData pcdDataLoad;
        string[] strs = File.ReadAllLines(strFile);
        lineCount = strs.Length;
        pcdDataLoad.m_fX = new float[lineCount];
        pcdDataLoad.m_fY = new float[lineCount];
        pcdDataLoad.m_fZ = new float[lineCount];
        pcdDataLoad.m_label = new int[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            string[] data = strs[i].Split(' ');
            pcdDataLoad.m_fX[i] = float.Parse(data[0]);
            pcdDataLoad.m_fY[i] = float.Parse(data[1]);
            pcdDataLoad.m_fZ[i] = float.Parse(data[2]);
            pcdDataLoad.m_label[i] = int.Parse(data[3]);
        }

        return pcdDataLoad;
        
    }

    private GameObject CreatePointCloud (pcdData pcdData)
    {
        Mesh meshNeed = new Mesh();
        Material mat = material;
        GameObject pointObj = new GameObject();
        pointObj.transform.position = new Vector3(0, 0, 0);
        pointObj.AddComponent<MeshFilter>();
        pointObj.AddComponent<MeshRenderer>();
        pointObj.name = "pointcloud";

        Vector3[] points = new Vector3[lineCount];
        int[] indecies = new int[lineCount];
        Color[] colors = new Color[lineCount];
        for (int i = 0; i < lineCount; i++)
        {
            Vector3 xyz = new Vector3(pcdData.m_fX[i], pcdData.m_fY[i], pcdData.m_fZ[i])/100;
            points[i] = xyz;
            indecies[i] = i;
            colors[i] = acolor[pcdData.m_label[i]];
        }
        meshNeed.vertices = points;
        meshNeed.colors = colors;
        meshNeed.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshNeed.SetIndices(indecies, MeshTopology.Points, 0);
        
        pointObj.GetComponent<MeshFilter>().mesh = meshNeed;
        pointObj.GetComponent<MeshRenderer>().material = mat;
        pointObj.AddComponent<BoxCollider>();

        pointObj.AddComponent<ObjectManipulator>();
        pointObj.AddComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;

        return pointObj;

    }

    void Start()
    {
        // 定义 20 种固定的颜色
        Color[] fixedColors = new Color[]
        {
        Color.red,
        Color.green,
        Color.blue,
        new Color(1f, 0.5f, 0f), // 橙色
        new Color(0.5f, 0f, 0.5f), // 紫色
        Color.yellow,
        new Color(0f, 0.5f, 1f), // 天蓝色
        new Color(0.5f, 1f, 0f), // 浅绿色
        new Color(0.5f, 0.5f, 0f), // 橄榄色
        new Color(1f, 0f, 0.5f), // 粉色
        new Color(0f, 0.5f, 0.5f), // 青色
        new Color(0.5f, 0f, 1f), // 深紫色
        new Color(1f, 1f, 0f), // 明黄
        new Color(0.75f, 0.75f, 0.75f), // 灰色
        new Color(0.25f, 0.25f, 0.25f), // 深灰
        new Color(1f, 0.5f, 0.25f), // 橙红色
        new Color(0.5f, 0.25f, 0f), // 棕色
        new Color(0.25f, 1f, 0.5f), // 淡绿色
        new Color(0.5f, 0.5f, 1f), // 淡蓝色
        new Color(1f, 0.75f, 0f) // 金色
        };

        // 将固定的颜色填充到 acolor 数组
        for (int a = 0; a < acolor.Length; a++)
        {
            acolor[a] = fixedColors[a % fixedColors.Length];
        }

        pcdData data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_8.txt");

        GameObject gameObject8 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_7.txt");

        GameObject gameObject7 = CreatePointCloud(data);

        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_6.txt");

        GameObject gameObject6 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_5.txt");

        GameObject gameObject5 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_4.txt");

        GameObject gameObject4 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_3.txt");

        GameObject gameObject3 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_2.txt");

        GameObject gameObject2 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_1.txt");

        GameObject gameObject1 = CreatePointCloud(data);
        data = LoadFile_Point("C:/Users/dell/Desktop/分块点云/bbox_0.txt");

        GameObject gameObject0 = CreatePointCloud(data);

    }

}
