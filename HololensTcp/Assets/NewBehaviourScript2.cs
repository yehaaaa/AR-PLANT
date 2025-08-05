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



[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NewBehaviourScript2 : MonoBehaviour
{
    public static bool iscoloring = false;
    public static bool tip = true;
    public static int[] Tag;
    public int tagmode = 1;
    public int vessel = 1;//0=SOM
    public static int colornum = 1;
    public static int taggingmode = 1;
    public static Color[] acolor = new Color[100];
    public static bool b2_Sign = false;
    int SOM_NUM = 1000;
    private int[] SOM_List = new int[1000];
    int local = 100;

    private ObjectManipulator objectManipulator;
    public  struct pcdData
    {
        public float[] m_fX;
        public float[] m_fY;
        public float[] m_fZ;
        public float[] m_fNorX;
        public float[] m_fNorY;
        public float[] m_fNorZ;
        public float[] m_fCurvature;
        public float[] m_label;
    }

    public struct PcdInfo
    {
        public string m_strVerInfo;
        public string m_strFileds;
        public string m_strSize;
        public string m_strType;
        public string m_strCount;
        public string m_strWidth;
        public string m_strHeight;
        public string m_strViewPoint;
        public int m_nPoints;
        public string m_points;
        public string m_strData;
        public pcdData m_pcdData;
    }



    //class PcdFileParser
    // {
    public PcdInfo m_pcdHeader;
    public enum FiledNames { X, Y, Z, NorX, NorY, NorZ, Curvature };

    public void PcdFileParser()
    {
    }



    public bool LoadFile_Sphere(string strFile)
    {


        string[] strs = File.ReadAllLines(strFile);
        m_pcdHeader.m_strVerInfo = strs[0];
        m_pcdHeader.m_strFileds = strs[1];
        m_pcdHeader.m_strSize = strs[2];
        m_pcdHeader.m_strType = strs[3];
        m_pcdHeader.m_strCount = strs[4];
        m_pcdHeader.m_strWidth = strs[5];
        m_pcdHeader.m_strHeight = strs[6];
        m_pcdHeader.m_strViewPoint = strs[8];
        m_pcdHeader.m_points = strs[7];
        m_pcdHeader.m_nPoints = 10000-10;
        m_pcdHeader.m_strData = strs[9];

        m_pcdHeader.m_pcdData = new pcdData();
        m_pcdHeader.m_pcdData.m_fX = new float[m_pcdHeader.m_nPoints];
        m_pcdHeader.m_pcdData.m_fY = new float[m_pcdHeader.m_nPoints];
        m_pcdHeader.m_pcdData.m_fZ = new float[m_pcdHeader.m_nPoints];
        m_pcdHeader.m_pcdData.m_label = new float[m_pcdHeader.m_nPoints];

        m_pcdHeader.m_pcdData.m_fNorX = new float[m_pcdHeader.m_nPoints];
        m_pcdHeader.m_pcdData.m_fNorY = new float[m_pcdHeader.m_nPoints];
        m_pcdHeader.m_pcdData.m_fNorZ = new float[m_pcdHeader.m_nPoints];
        m_pcdHeader.m_pcdData.m_fCurvature = new float[m_pcdHeader.m_nPoints];
        int f = 0;
        for (int i = 11; i < m_pcdHeader.m_nPoints; i++)
        {
            string[] data = strs[i+10].Split(' ');
            m_pcdHeader.m_pcdData.m_fX[i] = float.Parse(data[0]);
            m_pcdHeader.m_pcdData.m_fY[i] = float.Parse(data[1]);
            m_pcdHeader.m_pcdData.m_fZ[i] = float.Parse(data[2]);
            m_pcdHeader.m_pcdData.m_label[i] = float.Parse(data[3]);
            if (data.Length >= 5)
            {
                if (float.Parse(data[4]) >= 1 )
                {
                    SOM_List[f] = i;
                    f++;
                }
            }

            
        }

        int num = m_pcdHeader.m_nPoints;

        Tag = new int[num];

        return true;

    }



    private void mesh_pointcloud(float[] labels)
    {
        GameObject pointObj = new GameObject();

        if (TcpText.coloringsuccess == false) pointObj.name = "new"; else pointObj.name = "new1";


        pointObj.transform.position = new Vector3(0, 1.8f, 0.5f);

        GameObject father ;
        if (TcpText.coloringsuccess == false)
            father = GameObject.Find("new");
        else
            father = GameObject.Find("new1");

        mesh_creat(labels, father, pointObj);

        pointObj.AddComponent<BoxCollider>();
        pointObj.AddComponent<ObjectManipulator>();
        pointObj.AddComponent<BoundsControl>();


        // 获取目标物体的BoxCollider组件
        BoxCollider boxCollider = pointObj.GetComponent<BoxCollider>();
        Vector3 center = boxCollider.bounds.center;
        Vector3 size = boxCollider.bounds.extents;

        // 生成一个空对象
        GameObject newEmptyObject = new GameObject("EmptyObject");

        // 设置生成空对象的位置
        newEmptyObject.transform.position = new Vector3(0, 1.8f, 0.5f);

        newEmptyObject.transform.SetParent(father.transform);

    }

    private void mesh_creat(float[] labels, GameObject father, GameObject pointObj)
    {
        Dictionary<float, int> labelCount = new Dictionary<float, int>();
        Dictionary<float, List<int>> labelIndices = new Dictionary<float, List<int>>();
        // Iterate over the labels.
        for (int i = 0; i < labels.Length; i++)
        {
            // If the label is not in the dictionary, add it.
            if (!labelCount.ContainsKey(labels[i]))
            {
                labelCount[labels[i]] = 0;
                labelIndices[labels[i]] = new List<int>();
            }
            // Increase the count of the current label.
            labelCount[labels[i]]++;
            labelIndices[labels[i]].Add(i);
        }
        List<KeyValuePair<float, int>> labelList = new List<KeyValuePair<float, int>>(labelCount);
        int a = 0;
        GameObject ma;
        ma = GameObject.Find("11111");
        MeshRenderer mr = ma.GetComponent<MeshRenderer>();
        foreach (KeyValuePair<float, int> label in labelCount)
        {

            int g = (int)label.Key;
            Color newColor = acolor[g];

            GameObject importedPrefab1 = Resources.Load("点") as GameObject;
            List<int> indices = labelIndices[label.Key];

            CombineInstance[] combineInstances = new CombineInstance[label.Value];
            for (int i = 0; i < label.Value; ++i)
            {

                GameObject prefab = Resources.Load("点") as GameObject;
                MeshFilter prefabMesh = prefab.GetComponent<MeshFilter>();
                Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[indices[i]], m_pcdHeader.m_pcdData.m_fY[indices[i]], m_pcdHeader.m_pcdData.m_fZ[indices[i]]) /local;
                combineInstances[i].mesh = prefabMesh.sharedMesh;
                combineInstances[i].transform = Matrix4x4.TRS(xyz, Quaternion.identity, new Vector3(0.0012f, 0.0012f, 0.0012f));

            }

            Mesh newMesh = new Mesh();
            newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            newMesh.CombineMeshes(combineInstances);
            GameObject combinedObject = new GameObject("label_" + label.Key.ToString());
            combinedObject.transform.position = father.transform.position;
            combinedObject.transform.SetParent(father.transform);
            combinedObject.AddComponent<MeshFilter>().mesh = newMesh;
            combinedObject.AddComponent<MeshRenderer>().material = mr.material;
            combinedObject.GetComponent<MeshRenderer>().material.color = newColor;

            a += label.Value;

        }
    }

    private void mesh_pointcloud_change(float[] labels)
    {
        Dictionary<float, int> labelCount = new Dictionary<float, int>();
        Dictionary<float, List<int>> labelIndices = new Dictionary<float, List<int>>();
        GameObject father;
        if (TcpText.coloringsuccess == false)
            father = GameObject.Find("new");
        else
            father = GameObject.Find("new1");

        // Iterate over the labels.
        for (int i = 0; i < labels.Length; i++)
        {
            // If the label is not in the dictionary, add it.
            if (!labelCount.ContainsKey(labels[i]))
            {
                labelCount[labels[i]] = 0;
                labelIndices[labels[i]] = new List<int>();
            }
            // Increase the count of the current label.
            labelCount[labels[i]]++;
            labelIndices[labels[i]].Add(i);
        }

        GameObject ma = GameObject.Find("11111");
        MeshRenderer mr = ma.GetComponent<MeshRenderer>();

        Vector3 fatherscale = father.transform.localScale;

        foreach (KeyValuePair<float, int> label in labelCount)
        {
            // Check and destroy old objects
            GameObject oldObject = GameObject.Find("label_" + label.Key.ToString());
            if (oldObject != null)
            {
                MeshFilter oldMeshFilter = oldObject.GetComponent<MeshFilter>();
                if (oldMeshFilter != null && oldMeshFilter.mesh != null)
                {
                    DestroyImmediate(oldMeshFilter.mesh); // 立即销毁旧的 Mesh
                }
                DestroyImmediate(oldObject); // 立即销毁旧的 GameObject
            }

            int g = (int)label.Key;
            Color newColor = acolor[g];

            List<int> indices = labelIndices[label.Key];

            CombineInstance[] combineInstances = new CombineInstance[label.Value];
            for (int i = 0; i < label.Value; ++i)
            {
                GameObject prefab = Resources.Load("点") as GameObject;
                MeshFilter prefabMesh = prefab.GetComponent<MeshFilter>();
                Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[indices[i]] * fatherscale.x, m_pcdHeader.m_pcdData.m_fY[indices[i]] * fatherscale.y, m_pcdHeader.m_pcdData.m_fZ[indices[i]] * fatherscale.z) /local;
                combineInstances[i].mesh = prefabMesh.sharedMesh;
                combineInstances[i].transform = Matrix4x4.TRS(xyz, Quaternion.identity, new Vector3(0.0012f * fatherscale.x, 0.0012f * fatherscale.y, 0.0012f * fatherscale.z));
            }

            Mesh newMesh = new Mesh();
            newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            newMesh.CombineMeshes(combineInstances);

            GameObject combinedObject = new GameObject("label_" + label.Key.ToString());
            combinedObject.transform.position = father.transform.position;
            combinedObject.transform.rotation = father.transform.rotation;
            combinedObject.transform.SetParent(father.transform);
            combinedObject.AddComponent<MeshFilter>().mesh = newMesh;
            combinedObject.AddComponent<MeshRenderer>().material = mr.material;
            combinedObject.GetComponent<MeshRenderer>().material.color = newColor;
        }

        // 手动调用垃圾回收
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }






    string filepath11;

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

        filepath11 = Application.persistentDataPath;

    }

    private bool only1 = false;
    private bool only2 = false;
    void Update()
    {
        if (TcpText.ConnectedCompleted && dialog.hasResponded && !only1 && dialog.canload && TcpText.fileAccept)
        {
            iscoloring = true;
            LoadFile_Sphere(filepath11 + "/flowerWithoutLabel.pcd");
            mesh_pointcloud(m_pcdHeader.m_pcdData.m_label);

            GameObject.Find("new").GetComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
            only1 = true;
        }
        if (TcpText.ConnectedCompleted && dialog.hasResponded && !only2 && dialog.canload && TcpText.fileAccept && TcpText.coloringsuccess)
        {

            for (int j = 0; j < m_pcdHeader.m_nPoints; j++)
            {
                m_pcdHeader.m_pcdData.m_fX[j] = 0;
                m_pcdHeader.m_pcdData.m_fY[j] = 0;
                m_pcdHeader.m_pcdData.m_fZ[j] = 0;
                m_pcdHeader.m_pcdData.m_label[j] = 0;
            }
            Destroy(GameObject.Find("process1"));
            LoadFile_Sphere(filepath11 + "/flowerWithLabel.pcd");
            mesh_pointcloud(m_pcdHeader.m_pcdData.m_label);

            GameObject.Find("new1").GetComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
            only2 = true;
        }
    }



    private BoxCollider boxCollider;
    private BoundsControl boundscontrol;
    public void Frozen()
    {

        boxCollider = GameObject.Find("new").GetComponent<BoxCollider>();
        boundscontrol= GameObject.Find("new").GetComponent<BoundsControl>();
        if (boxCollider != null)
        {
            // 关闭BoxCollider的碰撞检测
            boxCollider.enabled = !boxCollider.enabled;
            boundscontrol.enabled= !boundscontrol.enabled;
        }

    }


    public void B2() 
    {
        Dictionary<float, int> labelCount = new Dictionary<float, int>();
        float[] labels = m_pcdHeader.m_pcdData.m_label;
        GameObject father = GameObject.Find("new1");
        // Iterate over the labels.
        for (int i = 0; i < labels.Length; i++)
        {
            // If the label is not in the dictionary, add it.
            if (!labelCount.ContainsKey(labels[i]))
            {
                labelCount[labels[i]] = 0;

            }
            // Increase the count of the current label.
            labelCount[labels[i]]++;
        }
        if (!b2_Sign) 
        {
            
            foreach (KeyValuePair<float, int> label in labelCount)
            {
                GameObject test = GameObject.Find("label_" + label.Key.ToString());
                test.AddComponent<BoxCollider>();
                test.AddComponent<ObjectManipulator>();
                test.AddComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
                test.transform.SetParent(null, true);
                //Destroy(father.GetComponent<BoundsControl>());
            }
            Destroy(father);

        }
        if (b2_Sign)
        {

            foreach (KeyValuePair<float, int> label in labelCount)
            {
                GameObject test = GameObject.Find("label_"+label.Key.ToString());
                Destroy(test);
            }

            LoadFile_Sphere(filepath11 + "/flowerWithLabel.pcd");
            mesh_pointcloud(m_pcdHeader.m_pcdData.m_label);

            GameObject.Find("new1").GetComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;

        }
        b2_Sign = !b2_Sign;
    }



 

    private void colorChange_mesh(int[] a) 
    {

        float[] floatArray = new float[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            floatArray[i] = (float)a[i];
        }

        mesh_pointcloud_change(floatArray);

    }


    public void Tagging(Vector3 xyz_l,float r_l,Vector3 qiu_size_l)
    {

        float distance;
        Vector3 xyz_R = xyz_l;

        float z = GameObject.Find("new").GetComponent<Transform>().localScale.x;
        float r = r_l / 2;
        Vector3 qiu_size = qiu_size_l;
        Vector3 MXsize = xyz_R + 0.5f * qiu_size;
        Vector3 MNsize = xyz_R - 0.5f * qiu_size;


        for (int i = 0; i < m_pcdHeader.m_nPoints; ++i)
        {
            Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[i] * z, m_pcdHeader.m_pcdData.m_fZ[i] * z, m_pcdHeader.m_pcdData.m_fY[i] * z) /local;
            distance = Vector3.Distance(xyz, xyz_R);
            if (taggingmode == 1) if (distance < r) Tag[i] = colornum;
            if (taggingmode == 0)
            {
                if (xyz.x < MXsize.x & xyz.y < MXsize.y & xyz.z < MXsize.z)
                {
                    if (xyz.x > MNsize.x & xyz.y > MNsize.y & xyz.z > MNsize.z) Tag[i] = colornum;
                }
            }
            if (taggingmode == 2)
            {
                float cylinderHeight = 1f * qiu_size.x;
                float cylinderRadius = 0.5f * qiu_size.x;
                Vector3 pointXZ = new Vector3(xyz.x, 0, xyz.y);
                Vector3 cylinderBaseCenterXZ = new Vector3(xyz_R.x, 0, xyz_R.y);
                if (xyz.z > xyz_R.z - cylinderHeight & xyz.z < xyz_R.z + cylinderHeight)
                {
                    if (Vector3.Distance(pointXZ, cylinderBaseCenterXZ) < cylinderRadius) Tag[i] = colornum;
                }
            }

        }

        colorChange_mesh(Tag);
    }

    public void Tagging1()
    {
        GameObject sphere = GameObject.Find("Sphere");
        Vector3 xyz_l = Printlocation();
        float r_l = sphere.GetComponent<Transform>().localScale.x;
        Vector3 qiu_size_l = sphere.GetComponent<Transform>().localScale;

        // 调用另一个脚本中的Tagging方法
       Tagging(xyz_l, r_l, qiu_size_l);
    }


    GameObject FindChildByName(GameObject parent, string name)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
            GameObject result = FindChildByName(child.gameObject, name);
            if (result != null)
            {
                return result;
            }
        }
        return null; // 如果没有找到，返回null
    }

    public void colornumchange()
    {
        colornum += 1;
        /*
        for (int i = 0; i < 100; i++)
        {
            GameObject.Find(i.ToString()).GetComponent<MeshRenderer>().material.color = acolor[colornum];
        }
        */
        GameObject parent1 = GameObject.Find("changecolor"); 
        //GameObject child = FindChildByName(parent1, "Cylinder"); // 把这里的ChildName替换成你的子对象的名字
        GameObject child = parent1.transform.GetChild(0).GetChild(1).gameObject;
        child.GetComponent<Renderer>().material.color = acolor[colornum];
        if(GameObject.Find("Sphere"))
        {
            GameObject.Find("Sphere").GetComponent<Renderer>().material.color = acolor[colornum];
        }
        Debug.Log(colornum);
        Debug.Log(acolor[colornum]);
    }

    public void LocalChange(SliderEventData eventData)
    {
        if(GameObject.Find("KON"))
        {
            Vector3 o = new Vector3(0.01f, 0.01f, 0.01f);
            float value1 = GameObject.Find("Pinch").GetComponent<PinchSlider>().SliderValue*12;
            for(int i = 0; i <SOM_NUM; i++)
            {
                GameObject.Find(i.ToString()).GetComponent<Transform>().localScale = o * value1;
            }

        }
        if(GameObject.Find("Sphere"))
        {
            Vector3 o = new Vector3(0.05f, 0.05f, 0.05f);
            float value1 = GameObject.Find("Pinch").GetComponent<PinchSlider>().SliderValue * 2;
            GameObject.Find("Sphere").GetComponent<Transform>().localScale = o * value1;
        }
        
    }

    public void Change2Cube()
    {
        if (GameObject.Find("Sphere"))
        {
            Destroy(GameObject.Find("Sphere"));
            var objCube = GameObject.CreatePrimitive(PrimitiveType.Cube);//类型
            objCube.name = "Sphere";
            objCube.transform.position = new Vector3(0, 1.8f, 0.5f);
            objCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            objCube.AddComponent<BoxCollider>();
            objCube.AddComponent<ObjectManipulator>();
            objCube.AddComponent<ConstraintManager>();
            objCube.GetComponent<MeshRenderer>().material.color = acolor[colornum];
            taggingmode = 0;
        }
    }
    public void Change2Sphere()
    {
        if (GameObject.Find("Sphere"))
        {
            Destroy(GameObject.Find("Sphere"));
            var objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
            objCube.name = "Sphere";
            objCube.transform.position = new Vector3(0, 1.8f, 0.5f);
            objCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            objCube.AddComponent<BoxCollider>();
            objCube.AddComponent<ObjectManipulator>();
            objCube.AddComponent<ConstraintManager>();
            objCube.GetComponent<MeshRenderer>().material.color = acolor[colornum];
            taggingmode = 1;
        }
    }
    public void Change2Cylinder()
    {
        if (GameObject.Find("Sphere"))
        {
            Destroy(GameObject.Find("Sphere"));
            var objCube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);//类型
            objCube.name = "Sphere";
            objCube.transform.position = new Vector3(0, 1.8f, 0.5f);
            objCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            objCube.AddComponent<BoxCollider>();
            objCube.AddComponent<ObjectManipulator>();
            objCube.AddComponent<ConstraintManager>();
            objCube.GetComponent<MeshRenderer>().material.color = acolor[colornum];
            taggingmode = 2;
        }
    }

    public void Tagmode()
    {
        
        if (GameObject.Find("KON"))
        {
            Destroy(GameObject.Find("KON"));
            Destroy(GameObject.Find("Pinch"));
            Destroy(GameObject.Find("tagging"));
            Destroy(GameObject.Find("changecolor"));
            Destroy(GameObject.Find("menu"));
        }
        else if (GameObject.Find("new"))
        {
            BoxCollider boxCollider = GameObject.Find("new").GetComponent<BoxCollider>();
            Vector3 center = boxCollider.bounds.center;
            Vector3 size = boxCollider.bounds.extents;

            Vector3 location = center + size + new Vector3(0.1f, 0.1f, 0.1f);

            GameObject importedPrefab1 = Resources.Load("changecolor") as GameObject;
            importedPrefab1 = Instantiate(importedPrefab1);
            importedPrefab1.name = "changecolor";
            importedPrefab1.GetComponent<Transform>().position = center - new Vector3(0, size.y, size.z) + new Vector3(-0.242f, -0.06f, -0.02f);
            importedPrefab1.transform.GetChild(0).GetComponent<Interactable>().OnClick.AddListener(colornumchange);
            importedPrefab1.AddComponent<SolverHandler>();
            importedPrefab1.transform.SetParent(GameObject.Find("new").transform);



            GameObject importedPrefab2 = Resources.Load("tagging") as GameObject;
            importedPrefab2 = Instantiate(importedPrefab2);
            importedPrefab2.name = "tagging";
            importedPrefab2.GetComponent<Transform>().position = center - new Vector3(0, size.y, size.z) + new Vector3(0.242f, -0.06f, -0.02f);
            importedPrefab2.transform.GetChild(0).GetComponent<Interactable>().OnClick.AddListener(Tagging1);
            importedPrefab2.AddComponent<SolverHandler>();
            importedPrefab2.transform.SetParent(GameObject.Find("new").transform);


            GameObject importedPrefab3 = Resources.Load("Pinch") as GameObject;
            importedPrefab3 = Instantiate(importedPrefab3);
            importedPrefab3.name = "Pinch";
            importedPrefab3.GetComponent<Transform>().position = center - new Vector3(0, size.y, size.z) + new Vector3(0, -0.06f, -0.02f);
            importedPrefab3.GetComponent<PinchSlider>().OnValueUpdated.AddListener(LocalChange);
            importedPrefab3.AddComponent<SolverHandler>();
            importedPrefab3.transform.SetParent(GameObject.Find("new").transform);

            GameObject Cube_father = new GameObject();
            Cube_father.GetComponent<Transform>().position = GameObject.Find("new").GetComponent<Transform>().position;
            Cube_father .transform.SetParent(GameObject.Find ("new").transform);
            Cube_father.name = "KON";
            
            for (int i = 0; i < SOM_NUM; i++)
            {
                //Debug.Log(SOM_List[i]);
                Vector3 P_List = new Vector3(m_pcdHeader.m_pcdData.m_fX[SOM_List[i]], m_pcdHeader.m_pcdData.m_fY[SOM_List[i]], m_pcdHeader.m_pcdData.m_fZ[SOM_List[i]]) / local;
                GameObject importedPrefab = Resources.Load("球") as GameObject;
                importedPrefab = Instantiate(importedPrefab);
                importedPrefab.name=i.ToSafeString();
                importedPrefab.GetComponent<Transform>().position = P_List+ Cube_father.GetComponent<Transform>().position;
                //importedPrefab.GetComponent<Transform>().localScale = new Vector3(0.05f, 0.05f, 0.05f);
                importedPrefab.transform.SetParent(Cube_father.transform);

            }
            
        }
    }



    public void SaveToPcd()//保存PCD
    {

        string strDir = Application.persistentDataPath;

        if (m_pcdHeader.m_nPoints > 0)
        {

            string strFile = string.Format("{0}\\{1}.pcd", strDir, "savepcdaaa");
            using (FileStream fs = new FileStream(strFile, FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(m_pcdHeader.m_strVerInfo);
                sw.WriteLine(m_pcdHeader.m_strFileds);
                sw.WriteLine(m_pcdHeader.m_strSize);
                sw.WriteLine(m_pcdHeader.m_strType);
                sw.WriteLine(m_pcdHeader.m_strCount);
                sw.WriteLine(m_pcdHeader.m_strWidth);
                sw.WriteLine(m_pcdHeader.m_strHeight);
                sw.WriteLine(m_pcdHeader.m_points);
                sw.WriteLine(m_pcdHeader.m_strViewPoint);
                sw.WriteLine(m_pcdHeader.m_strData);
                for (int j = 0; j < m_pcdHeader.m_nPoints; j++)
                {

                    sw.WriteLine(m_pcdHeader.m_pcdData.m_fX[j].ToString() + " " + m_pcdHeader.m_pcdData.m_fY[j].ToString() + " " + m_pcdHeader.m_pcdData.m_fZ[j].ToString() + " " + Tag[j].ToString() + " " + "-1");

                }
                sw.Flush();
                fs.Close();
            }

        }

    }


    public void modeChange()
    {
        if (GameObject.Find("Sphere"))
        {
            Destroy(GameObject.Find("Sphere"));
            Destroy(GameObject.Find("menu"));

            GameObject Cube_father = new GameObject();
            Cube_father.GetComponent<Transform>().position = GameObject.Find("new").GetComponent<Transform>().position;
            Cube_father.transform.SetParent(GameObject.Find("new").transform);
            Cube_father.name = "KON";

            for (int i = 0; i < SOM_NUM; i++)
            {
                //Debug.Log(SOM_List[i]);
                Vector3 P_List = new Vector3(m_pcdHeader.m_pcdData.m_fX[SOM_List[i]], m_pcdHeader.m_pcdData.m_fY[SOM_List[i]], m_pcdHeader.m_pcdData.m_fZ[SOM_List[i]]) / local;
                GameObject importedPrefab = Resources.Load("球") as GameObject;
                importedPrefab = Instantiate(importedPrefab);
                importedPrefab.name = i.ToSafeString();
                importedPrefab.GetComponent<Transform>().position = P_List + Cube_father.GetComponent<Transform>().position;
                //importedPrefab.GetComponent<Transform>().localScale = new Vector3(0.05f, 0.05f, 0.05f);
                importedPrefab.transform.SetParent(Cube_father.transform);

            }
            vessel = 0;
        }
        else if (GameObject.Find("new"))
        {
            BoxCollider boxCollider = GameObject.Find("new").GetComponent<BoxCollider>();
            Vector3 center = boxCollider.bounds.center;
            Vector3 size = boxCollider.bounds.extents;

            Destroy(GameObject.Find("KON"));

            GameObject importedPrefab4 = Resources.Load("menu") as GameObject;
            importedPrefab4 = Instantiate(importedPrefab4);
            importedPrefab4.name = "menu";
            importedPrefab4.GetComponent<Transform>().position = center - new Vector3(size.x, size.y, size.z) + new Vector3(-0.1f, size.y, size.z);
            importedPrefab4.AddComponent<SolverHandler>();
            importedPrefab4.transform.SetParent(GameObject.Find("new").transform);

            GameObject child1 = importedPrefab4.transform.GetChild(1).GetChild(0).gameObject;
            child1.AddComponent<Interactable>().OnClick.AddListener(Change2Cube);
            GameObject child2 = importedPrefab4.transform.GetChild(1).GetChild(1).gameObject;
            child2.AddComponent<Interactable>().OnClick.AddListener(Change2Sphere);
            GameObject child3 = importedPrefab4.transform.GetChild(1).GetChild(2).gameObject;
            child3.AddComponent<Interactable>().OnClick.AddListener(Change2Cylinder);

            GameObject importedPerfab=Resources.Load("Sphere") as GameObject;
            importedPerfab = Instantiate(importedPerfab);
            importedPerfab.name = "Sphere";
            importedPerfab.transform.position = center;
            importedPerfab.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            vessel = 1;
            importedPerfab.AddComponent<BoxCollider>();
            importedPerfab.AddComponent<ObjectManipulator>();
            importedPerfab.AddComponent<ConstraintManager>();
        }
    }

    private Vector3 Printlocation()
    {

        Transform targetTransform = GameObject.Find("EmptyObject").GetComponent<Transform>();
        Transform referenceTransform = GameObject.Find("Sphere").GetComponent<Transform>();
        //Vector3 relativePosition = referenceTransform.InverseTransformPoint(targetTransform.position);
        Vector3 distance = referenceTransform.transform.position - targetTransform.transform.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, targetTransform.transform.right.normalized);
        relativePosition.z = Vector3.Dot(distance, targetTransform.transform.up.normalized);
        relativePosition.y = Vector3.Dot(distance, targetTransform.transform.forward.normalized);
        

        return relativePosition;
    }


}
