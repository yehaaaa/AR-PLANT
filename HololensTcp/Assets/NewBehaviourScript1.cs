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

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NewBehaviourScript1 : MonoBehaviour
{
    public static bool iscoloring = false;
    public static int label4num=0;
    public static bool tip=true;
    public static int[] Tag, Tagafter;
    public int tagmode=1;
    public static int colornum = 1;
    public static int taggingmode = 1;
    public static Color[] colortonum = new Color[100];
    public static Color[] colors = new Color[10000];
    public static Color[] acolor = new Color[100];



    private ObjectManipulator objectManipulator;
    public struct pcdData
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
        public  enum FiledNames { X, Y, Z, NorX, NorY, NorZ, Curvature };

        public void PcdFileParser()
        {
        }
       
        /// <summary>
        /// 加载*.pcd点云数据格式文件
        /// </summary>
        /// <param name="加载的PCD文件绝对路径"></param>
        public bool LoadFile(string strFile)
        {
            if (!File.Exists(strFile))
            {
             
              
                return false;
            }



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
            m_pcdHeader.m_nPoints = 10000;
            m_pcdHeader.m_strData = strs[9];

            m_pcdHeader.m_pcdData = new pcdData();
            m_pcdHeader.m_pcdData.m_fX = new float[m_pcdHeader.m_nPoints];
            m_pcdHeader.m_pcdData.m_fY = new float[m_pcdHeader.m_nPoints];
            m_pcdHeader.m_pcdData.m_fZ = new float[m_pcdHeader.m_nPoints];
            m_pcdHeader.m_pcdData.m_label=new float[m_pcdHeader.m_nPoints];
        
            m_pcdHeader.m_pcdData.m_fNorX = new float[m_pcdHeader.m_nPoints];
            m_pcdHeader.m_pcdData.m_fNorY = new float[m_pcdHeader.m_nPoints];
            m_pcdHeader.m_pcdData.m_fNorZ = new float[m_pcdHeader.m_nPoints];
            m_pcdHeader.m_pcdData.m_fCurvature = new float[m_pcdHeader.m_nPoints];
            for (int i = 0; i < m_pcdHeader.m_nPoints; i++)
            {
                string[] data = strs[i + 10].Split(' ');
                m_pcdHeader.m_pcdData.m_fX[i] = float.Parse(data[0]);
                m_pcdHeader.m_pcdData.m_fY[i] = float.Parse(data[1]);
                m_pcdHeader.m_pcdData.m_fZ[i] = float.Parse(data[2]);
            if (data.Length >= 4)
            {
                // Check if the string at index 3 is not empty
                if (!String.IsNullOrWhiteSpace(data[3]))
                {
                    
                    m_pcdHeader.m_pcdData.m_label[i] = float.Parse(data[3]);
                }
            }
            else
            {
                
                m_pcdHeader.m_pcdData.m_label[i] = 1;
            }

            }
        
        Mesh meshNeed = new Mesh();
        Material mat = new Material(Shader.Find("Custom/VertexColor"));
        int num = m_pcdHeader.m_nPoints;
        Tag = new int[num];
        Tagafter = new int[num];
        GameObject pointObj = new GameObject();
         
        if(TcpText.coloringsuccess==false) pointObj.name = "new";else pointObj.name = "new1";
        
            pointObj.transform.position= new Vector3(0, 1.8f, 0.5f);
            pointObj.AddComponent<MeshFilter>();
            pointObj.AddComponent<MeshRenderer>();
            
           // pointObj.GetComponent<MeshFilter>().mesh = meshNeed;
           // pointObj.GetComponent<MeshRenderer>().material = mat;
            Vector3[] points = new Vector3[num];
            Color[] colors = new Color[num];
        int[] indecies = new int[num];
        for (int i = 0; i < num; ++i)
            {
                Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[i], m_pcdHeader.m_pcdData.m_fY[i], m_pcdHeader.m_pcdData.m_fZ[i])/100 ;
                points[i] = xyz;
                indecies[i] = i;
                int g = (int)m_pcdHeader.m_pcdData.m_label[i];
                colors[i] = acolor[g];    
        }

       
            //计算label1的数量
            for (int i = 0; i < num; ++i)
            {
                if (m_pcdHeader.m_pcdData.m_label[i] == 4) 
                { 

                    label4num++; 
                }
            }

            //绘制点云
            meshNeed.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshNeed.vertices = points;
            meshNeed.colors = colors;
            meshNeed.SetIndices(indecies, MeshTopology.Points, 0);
            pointObj.GetComponent<MeshFilter>().mesh = meshNeed;
            pointObj.GetComponent<MeshRenderer>().material = mat;
            pointObj.AddComponent<BoxCollider>();
            pointObj.AddComponent<ObjectManipulator>();
            pointObj.AddComponent<BoundsControl>();

        /*var objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
        objCube.name = "定位球";
        objCube.transform.position = new Vector3(0,0,0.5f);
        objCube.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        objCube.AddComponent<BoxCollider>();
        objCube.AddComponent<ObjectManipulator>();
        objCube.AddComponent<ConstraintManager>();*/

        // 获取目标物体的BoxCollider组件
        BoxCollider boxCollider = pointObj.GetComponent<BoxCollider>();
        Vector3 center = boxCollider.bounds.center;
        Vector3 size = boxCollider.bounds.extents;
    
        // 生成一个空对象
        GameObject newEmptyObject = new GameObject("EmptyObject");

        // 设置生成空对象的位置
        newEmptyObject.transform.position = new Vector3(0,0,0.5f);

        if(TcpText.coloringsuccess==false) newEmptyObject.transform.SetParent(GameObject.Find("new").transform); else newEmptyObject.transform.SetParent(GameObject.Find("new1").transform);

        Vector3 location = center + size + new Vector3(0.1f, 0.1f, 0.1f);
        //GameObject importedPrefab = Resources.Load<GameObject>(Application.dataPath + "/Button"); // 替换为预制件的路径
        GameObject importedPrefab = Resources.Load("Button") as GameObject;
        importedPrefab=Instantiate(importedPrefab, location, Quaternion.identity);
        importedPrefab.name = "固定按钮";

        if(TcpText.coloringsuccess==false) importedPrefab.transform.SetParent(GameObject.Find("new").transform); else importedPrefab.transform.SetParent(GameObject.Find("new1").transform);

        importedPrefab.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate () {
            this.Frozen();
        });
        return true;
        }
    // }


    public bool LoadFile_Sphere(string strFile)
    {
        if (!File.Exists(strFile))
        {

            return false;
        }

        if (strFile.LastIndexOf(".pcd") == -1)
        {


            return false;
        }

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
        m_pcdHeader.m_nPoints = int.Parse(strs[7].Split(' ')[1]);
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
        for (int i = 0; i < m_pcdHeader.m_nPoints; i++)
        {
            string[] data = strs[i + 10].Split(' ');
            m_pcdHeader.m_pcdData.m_fX[i] = float.Parse(data[0]);
            m_pcdHeader.m_pcdData.m_fY[i] = float.Parse(data[1]);
            m_pcdHeader.m_pcdData.m_fZ[i] = float.Parse(data[2]);
            if (data.Length >= 4)
            {
                // Check if the string at index 3 is not empty
                if (!String.IsNullOrWhiteSpace(data[3]))
                {
                    m_pcdHeader.m_pcdData.m_label[i] = float.Parse(data[3]);
                }
            }
            else
            {
                m_pcdHeader.m_pcdData.m_label[i] = 0;
            }

        }
        int num = m_pcdHeader.m_nPoints;
        Tag = new int[num];
        Tagafter = new int[num];
        GameObject pointObj = new GameObject();

        if (TcpText.coloringsuccess == false) pointObj.name = "new"; else pointObj.name = "new1";

        pointObj.transform.position = new Vector3(0, 0, 0.5f);


        float[] labels = m_pcdHeader.m_pcdData.m_label;
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
        int labelListSize = labelList.Count;
        Debug.Log("Size of labelList: " + labelListSize.ToString());

        int a = 0;
        GameObject father = new GameObject(); 
        if (TcpText.coloringsuccess == false)
            father = GameObject.Find("new");
        else
            father = GameObject.Find("new1");

        

        foreach (KeyValuePair<float, int> label in labelCount)
        {

            int g = (int)label.Key;
            Color newColor = acolor[g];
            /*var objCube = new GameObject();
            objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
            objCube.name = label.Key.ToString();
            objCube.transform.position = new Vector3(10, 10, 10);
            objCube.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            objCube.GetComponent<MeshRenderer>().material.color = newColor;*/
            GameObject importedPrefab1 = Resources.Load("点") as GameObject;
            importedPrefab1.GetComponent<MeshRenderer>().sharedMaterial.color = newColor;

            List<int> indices = labelIndices[label.Key];
            for (int i=0; i < label.Value; ++i)
            {

                Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[indices[i]], m_pcdHeader.m_pcdData.m_fY[indices[i]], m_pcdHeader.m_pcdData.m_fZ[indices[i]]) / 100;

                GameObject newObj = Instantiate(importedPrefab1); // 实例化游戏对象
                newObj.name = (i+a).ToString();

                if (TcpText.coloringsuccess == false)
                    newObj.transform.SetParent(father.transform);
                else
                    newObj.transform.SetParent(father.transform);

                newObj.GetComponent<Transform>().position = xyz + pointObj.transform.position;

            }
            a += label.Value;
            Debug.Log(label.Key);
            Debug.Log(label.Value);
            Debug.Log(newColor);
        }


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
        newEmptyObject.transform.position = new Vector3(0, 0, 0.5f);

        if (TcpText.coloringsuccess == false) newEmptyObject.transform.SetParent(GameObject.Find("new").transform); else newEmptyObject.transform.SetParent(GameObject.Find("new1").transform);

        Vector3 location = center + size + new Vector3(0.1f, 0.1f, 0.1f);
        GameObject importedPrefab = Resources.Load("Button") as GameObject;
        importedPrefab = Instantiate(importedPrefab, location, Quaternion.identity);
        importedPrefab.name = "固定按钮";

        if (TcpText.coloringsuccess == false) importedPrefab.transform.SetParent(GameObject.Find("new").transform); else importedPrefab.transform.SetParent(GameObject.Find("new1").transform);

        importedPrefab.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate () {
            this.Frozen();
        });
        return true;
    }
    // }


    public bool LoadFile_Sphere_meshcomb(string strFile)
    {
        if (!File.Exists(strFile))
        {

            return false;
        }

        if (strFile.LastIndexOf(".pcd") == -1)
        {


            return false;
        }

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
        m_pcdHeader.m_nPoints = 10000;
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
        for (int i = 0; i < m_pcdHeader.m_nPoints; i++)
        {
            string[] data = strs[i + 10].Split(' ');
            m_pcdHeader.m_pcdData.m_fX[i] = float.Parse(data[0]);
            m_pcdHeader.m_pcdData.m_fY[i] = float.Parse(data[1]);
            m_pcdHeader.m_pcdData.m_fZ[i] = float.Parse(data[2]);
            if (data.Length >= 4)
            {
                // Check if the string at index 3 is not empty
                if (!String.IsNullOrWhiteSpace(data[3]))
                {
                    m_pcdHeader.m_pcdData.m_label[i] = float.Parse(data[3]);
                }
            }
            else
            {
                m_pcdHeader.m_pcdData.m_label[i] = 0;
            }

        }
        int num = m_pcdHeader.m_nPoints;
        Tag = new int[num];
        Tagafter = new int[num];
        GameObject pointObj = new GameObject();

        if (TcpText.coloringsuccess == false) pointObj.name = "new"; else pointObj.name = "new1";

        pointObj.transform.position = new Vector3(0, 0, 0.5f);

        Vector3[] points = new Vector3[num];
        for (int i = 0; i < num; ++i)
        {
            Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[i], m_pcdHeader.m_pcdData.m_fY[i], m_pcdHeader.m_pcdData.m_fZ[i]) / 100;
            int g = (int)m_pcdHeader.m_pcdData.m_label[i];
            colors[i] = acolor[g];

            GameObject importedPrefab1 = Resources.Load("点") as GameObject;
            importedPrefab1 = Instantiate(importedPrefab1);
            importedPrefab1.name = i.ToString();

            if (TcpText.coloringsuccess == false)
                importedPrefab1.transform.SetParent(GameObject.Find("new").transform);
            else importedPrefab1.transform.SetParent(GameObject.Find("new1").transform);


            importedPrefab1.GetComponent<Transform>().position = xyz + pointObj.transform.position;
            importedPrefab1.GetComponent<MeshRenderer>().material.color = acolor[g];



        }

        pointObj.AddComponent<BoxCollider>();
        pointObj.AddComponent<ObjectManipulator>();
        pointObj.AddComponent<BoundsControl>();

        /*
        // 获取目标物体的BoxCollider组件
        BoxCollider boxCollider = pointObj.GetComponent<BoxCollider>();
        Vector3 center = boxCollider.bounds.center;
        Vector3 size = boxCollider.bounds.extents;
       
        // 生成一个空对象
        GameObject newEmptyObject = new GameObject("EmptyObject");

        // 设置生成空对象的位置
        newEmptyObject.transform.position = new Vector3(0, 0, 0.5f);

        if (TcpText.coloringsuccess == false) newEmptyObject.transform.SetParent(GameObject.Find("new").transform); else newEmptyObject.transform.SetParent(GameObject.Find("new1").transform);

        Vector3 location = center + size + new Vector3(0.1f, 0.1f, 0.1f);
        GameObject importedPrefab = Resources.Load("Button") as GameObject;
        importedPrefab = Instantiate(importedPrefab, location, Quaternion.identity);
        importedPrefab.name = "固定按钮";

        if (TcpText.coloringsuccess == false) importedPrefab.transform.SetParent(GameObject.Find("new").transform); else importedPrefab.transform.SetParent(GameObject.Find("new1").transform);

        importedPrefab.GetComponent<ButtonConfigHelper>().OnClick.AddListener(delegate () {
            this.Frozen();
        });//*/
        return true;
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
        if (TcpText.ConnectedCompleted && dialog.hasResponded&&!only1&&dialog.canload&&TcpText.fileAccept)
        {
            iscoloring = true;
            LoadFile(filepath11 + "/flowerWithoutLabel.pcd");
            //LoadFile_Sphere_meshcomb(filepath11 + "/flowerWithoutLabel.pcd");
            
            GameObject.Find("new").GetComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
            only1 = true;
        }
        if (TcpText.ConnectedCompleted && dialog.hasResponded && !only2 && dialog.canload && TcpText.fileAccept&&TcpText.coloringsuccess)
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
            
            GameObject.Find("new1").GetComponent<BoundsControl>().BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
            only2 = true;
        }
    }




    
    public void Frozen() 
    {

        if (TcpText.coloringsuccess == false)
        {
            objectManipulator = GameObject.Find("new").GetComponent<ObjectManipulator>();
            objectManipulator.enabled = !objectManipulator.enabled;

        }
        else
        {
            objectManipulator = GameObject.Find("new1").GetComponent<ObjectManipulator>();
            objectManipulator.enabled = !objectManipulator.enabled;
        }

            
    }



    public void ColorChange_Sphere(int[] a)
    {

        Vector3[] points = new Vector3[m_pcdHeader.m_nPoints];
        Color[] colors = new Color[m_pcdHeader.m_nPoints];
        int[] indecies = new int[m_pcdHeader.m_nPoints];
        for (int i = 0; i < m_pcdHeader.m_nPoints; ++i)
        {
            Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[i], m_pcdHeader.m_pcdData.m_fY[i], m_pcdHeader.m_pcdData.m_fZ[i]) / 100;
            points[i] = xyz;
            indecies[i] = i;
            if (a[i] == Tagafter[i] && Tagafter[i] == 0) colors[i] = acolor[0];
            if (a[i] == Tagafter[i] && Tagafter[i] != 0) colors[i] = colortonum[Tagafter[i]];
            if (a[i] != Tagafter[i] && Tagafter[i] == 0) colors[i] = colortonum[a[i]];
            if (a[i] != Tagafter[i] && Tagafter[i] != 0) colors[i] = colortonum[a[i]];
        }
        for (int i = 0; i < m_pcdHeader.m_nPoints; ++i) 
        {
            GameObject sphere = GameObject.Find(i.ToString());
            sphere.GetComponent<MeshRenderer>().material.color = colors[i];
        }

            for (int i = 0; i < m_pcdHeader.m_nPoints; ++i)
        {
            Tagafter[i] = a[i];
        }

    }



    public void ColorChange(int[] a) 
    {
        Mesh meshNeed = new Mesh();
        Material mat = new Material(Shader.Find("Custom/VertexColor"));
        Vector3[] points = new Vector3[m_pcdHeader.m_nPoints];
        Color[] colors = new Color[m_pcdHeader.m_nPoints];
        int[] indecies = new int[m_pcdHeader.m_nPoints];
        for (int i = 0; i < m_pcdHeader.m_nPoints; ++i)
        {
            GameObject sphere = GameObject.Find(i.ToString());
            Vector3 xyz= sphere.transform.position;
            //Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[i], m_pcdHeader.m_pcdData.m_fY[i], m_pcdHeader.m_pcdData.m_fZ[i]) / 200;
            points[i] = xyz;
            indecies[i] = i;
            if (a[i] == Tagafter[i] && Tagafter[i] == 0) colors[i] = acolor[1];
            if (a[i] == Tagafter[i] && Tagafter[i] != 0) colors[i] = colortonum[Tagafter[i]];
            if (a[i] != Tagafter[i] && Tagafter[i] == 0) colors[i] = colortonum[a[i]];
            if (a[i] != Tagafter[i] && Tagafter[i] != 0) colors[i] = colortonum[a[i]];
        }

        meshNeed.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshNeed.vertices = points;
        meshNeed.colors = colors;
        meshNeed.SetIndices(indecies, MeshTopology.Points, 0);
        GameObject.Find("new").GetComponent<MeshFilter>().mesh = meshNeed;
        GameObject.Find("new").GetComponent<MeshRenderer>().material = mat;
        GameObject.Find("new").GetComponent<MeshFilter>().mesh.colors= colors;
        for (int i = 0; i < m_pcdHeader.m_nPoints; ++i)
        {
            Tagafter[i] = a[i];
        }

    }

    public void Tagging()
    {

        float distance;
        Vector3 xyz_R = Printlocation();

        float z = GameObject.Find("new").GetComponent<Transform>().localScale.x;
        float r = GameObject.Find("球").GetComponent<Transform>().localScale.x / 2;
        Vector3 qiu_size = GameObject.Find("球").GetComponent<Transform>().localScale;
        Vector3 MXsize = xyz_R + 0.5f * qiu_size;
        Vector3 MNsize = xyz_R - 0.5f * qiu_size;


        for (int i = 0; i < m_pcdHeader.m_nPoints; ++i)
        {
            Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[i]* z, m_pcdHeader.m_pcdData.m_fZ[i]*z, m_pcdHeader.m_pcdData.m_fY[i]* z) / 100;
            distance = Vector3.Distance(xyz, xyz_R);
            if(taggingmode==1)if (distance < r) Tag[i] = colornum;
            if (taggingmode == 0) 
            {
                if (xyz.x < MXsize.x & xyz.y < MXsize.y & xyz.z < MXsize.z) 
                {
                    if(xyz.x>MNsize.x&xyz.y>MNsize.y&xyz.z>MNsize.z) Tag[i] = colornum;
                }
            }
            if (taggingmode == 2) 
            {
                float cylinderHeight = 1f * qiu_size.x;
                float cylinderRadius = 0.5f * qiu_size.x;
                Vector3 pointXZ= new Vector3(xyz.x,0,xyz.y);
                Vector3 cylinderBaseCenterXZ = new Vector3(xyz_R.x, 0, xyz_R.y);
                if (xyz.z > xyz_R.z - cylinderHeight & xyz.z < xyz_R.z + cylinderHeight) 
                {
                    if(Vector3.Distance(pointXZ,cylinderBaseCenterXZ)<cylinderRadius) Tag[i] = colornum;
                }
            }

        }
        
        ColorChange_Sphere(Tag);
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
        colortonum[colornum] = acolor[colornum];
        GameObject.Find("球").GetComponent<MeshRenderer>().material.color = colortonum[colornum];

        GameObject parent1 = GameObject.Find("changecolor"); // 把这里的Parent1替换成你的父对象的名字
        //GameObject child = FindChildByName(parent1, "Cylinder"); // 把这里的ChildName替换成你的子对象的名字
        GameObject child = parent1.transform.GetChild(0).GetChild(1).gameObject;
        child.GetComponent<Renderer>().material.color= colortonum[colornum];

        Debug.Log(colornum);
        Debug.Log(acolor[colornum]);

    }

    public void LocalChange(SliderEventData eventData)
    {
        Vector3 o = new Vector3(0.1f, 0.1f, 0.1f);
        float value1 = GameObject.Find("Pinch").GetComponent<PinchSlider>().SliderValue * 2;
         Vector3 local = GameObject.Find("球").GetComponent<Transform>().localScale;
        GameObject.Find("球").GetComponent<Transform>().localScale = o * value1;
    }

    public void Change2Cube()
    {
        if (GameObject.Find("球")) 
        {
            Destroy(GameObject.Find("球"));
            var objCube = GameObject.CreatePrimitive(PrimitiveType.Cube);//类型
        objCube.name = "球";
        objCube.transform.position = new Vector3(0,0,0.5f);
        objCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        objCube.AddComponent<BoxCollider>();
        objCube.AddComponent<ObjectManipulator>();
        objCube.AddComponent<ConstraintManager>();
            objCube.GetComponent<MeshRenderer>().material.color = colortonum[colornum];
            taggingmode = 0;
        }
    }
    public void Change2Sphere()
    {
        if (GameObject.Find("球"))
        {
            Destroy(GameObject.Find("球"));
            var objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
            objCube.name = "球";
            objCube.transform.position = new Vector3(0, 0, 0.5f);
            objCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            objCube.AddComponent<BoxCollider>();
            objCube.AddComponent<ObjectManipulator>();
            objCube.AddComponent<ConstraintManager>();
            objCube.GetComponent<MeshRenderer>().material.color = colortonum[colornum];
            taggingmode = 1;
        }
    }
    public void Change2Cylinder()
    {
        if (GameObject.Find("球"))
        {
            Destroy(GameObject.Find("球"));
            var objCube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);//类型
            objCube.name = "球";
            objCube.transform.position = new Vector3(0, 0, 0.5f);
            objCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            objCube.AddComponent<BoxCollider>();
            objCube.AddComponent<ObjectManipulator>();
            objCube.AddComponent<ConstraintManager>();
            objCube.GetComponent<MeshRenderer>().material.color = colortonum[colornum];
            taggingmode = 2;
        }
    }


    public void Tagmode() 
    {

      
        if (GameObject.Find("球"))
        {
            Destroy(GameObject.Find("球"));
            Destroy(GameObject.Find("Pinch"));
            Destroy(GameObject.Find("tagging"));
            Destroy(GameObject.Find("changecolor"));
            Destroy(GameObject.Find("menu"));
        } 
        else if(GameObject.Find("new"))
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
            /*importedPrefab1.AddComponent<Orbital>();
            importedPrefab1.GetComponent<Orbital>().LocalOffset = new Vector3(0.1f, -0.13f, 0.5f);*/


            GameObject importedPrefab2 = Resources.Load("tagging") as GameObject;
            importedPrefab2 = Instantiate(importedPrefab2);
            importedPrefab2.name = "tagging";
            importedPrefab2.GetComponent<Transform>().position = center - new Vector3(0, size.y, size.z) + new Vector3(0.242f, -0.06f, -0.02f);
            importedPrefab2.transform.GetChild(0).GetComponent<Interactable>().OnClick.AddListener(Tagging);
            importedPrefab2.AddComponent<SolverHandler>();
            importedPrefab2.transform.SetParent(GameObject.Find("new").transform);
            /*importedPrefab2.AddComponent<Orbital>();
            importedPrefab2.GetComponent<Orbital>().LocalOffset = new Vector3(-0.1f, -0.13f, 0.5f);*/

            GameObject importedPrefab3 = Resources.Load("Pinch") as GameObject;
            importedPrefab3 = Instantiate(importedPrefab3);
            importedPrefab3.name = "Pinch";
            importedPrefab3.GetComponent<Transform>().position = center - new Vector3(0, size.y, size.z) + new Vector3(0, -0.06f, -0.02f);
            importedPrefab3.GetComponent<PinchSlider>().OnValueUpdated.AddListener(LocalChange);
            importedPrefab3.AddComponent<SolverHandler>();
            importedPrefab3.transform.SetParent(GameObject.Find("new").transform);
            /*importedPrefab3.AddComponent<Orbital>();
            importedPrefab3.GetComponent<Orbital>().LocalOffset = new Vector3(0.02f, 0.05f, 0.5f);*/

            GameObject importedPrefab4 = Resources.Load("menu") as GameObject;
            importedPrefab4 = Instantiate(importedPrefab4);
            importedPrefab4.name = "menu";
            importedPrefab4.GetComponent<Transform>().position = center - new Vector3(size.x, size.y, size.z) + new Vector3(-0.1f, size.y, size.z);
            importedPrefab4.AddComponent<SolverHandler>();
            importedPrefab4.transform.SetParent(GameObject.Find("new").transform);
            /*importedPrefab4.AddComponent<Orbital>();
            importedPrefab4.GetComponent<Orbital>().LocalOffset = new Vector3(-0.24f, 0.1f, 0.5f);*/
            GameObject child1=importedPrefab4.transform.GetChild(1).GetChild(0).gameObject;
            child1.AddComponent<Interactable>().OnClick.AddListener(Change2Cube);
            GameObject child2 = importedPrefab4.transform.GetChild(1).GetChild(1).gameObject;
            child2.AddComponent<Interactable>().OnClick.AddListener(Change2Sphere);
            GameObject child3 = importedPrefab4.transform.GetChild(1).GetChild(2).gameObject;
            child3.AddComponent<Interactable>().OnClick.AddListener(Change2Cylinder);

            GameObject importedPrefab5 = Resources.Load("球") as GameObject;
            importedPrefab5 = Instantiate(importedPrefab5);
            importedPrefab5.name = "球";
            importedPrefab5.GetComponent<Transform>().position = new Vector3(0, 0, 0.5f);
            importedPrefab5.AddComponent<BoxCollider>();
            importedPrefab5.AddComponent<ObjectManipulator>();
            importedPrefab5.AddComponent<ConstraintManager>();

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

                    sw.WriteLine(m_pcdHeader.m_pcdData.m_fX[j].ToString() + " " + m_pcdHeader.m_pcdData.m_fY[j].ToString() + " " + m_pcdHeader.m_pcdData.m_fZ[j].ToString() + " " + Tag[j].ToString()+" "+"-1");

                }
                sw.Flush();
                fs.Close();
            }

        }
       
    }

    public void box()
    {

         GameObject targetObject=GameObject.Find("new");

    
        // 获取目标物体的BoxCollider组件
        BoxCollider boxCollider = targetObject.GetComponent<BoxCollider>();

       
            // 获取盒装碰撞器的中点
            Vector3 center = boxCollider.bounds.center;

            // 获取盒装碰撞器的尺寸
            Vector3 size = boxCollider.bounds.extents;

            // 计算底面中点
            Vector3 bottomCenter = center - new Vector3(0f, size.y, 0f);



        // 生成一个空对象
        GameObject newEmptyObject = new GameObject("EmptyObject");

        // 设置生成空对象的位置
        newEmptyObject.transform.position = bottomCenter;

        Transform parentObject=GameObject.Find("new").transform;
        newEmptyObject.transform.SetParent(parentObject);

        //生成小球测试
        /*var objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);//类型
        objCube.name = "Cude";       
        objCube.transform.position = bottomCenter;
        objCube.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        objCube.transform.SetParent(parentObject); */
    }


    private Vector3 Printlocation()
    {

        Transform targetTransform = GameObject.Find("EmptyObject").GetComponent<Transform>();
        Transform referenceTransform = GameObject.Find("球").GetComponent<Transform>();
        //Vector3 relativePosition = referenceTransform.InverseTransformPoint(targetTransform.position);
        Vector3 distance = referenceTransform.transform.position - targetTransform.transform.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, targetTransform.transform.right.normalized);
        relativePosition.z = Vector3.Dot(distance, targetTransform.transform.up.normalized);
        relativePosition.y = Vector3.Dot(distance, targetTransform.transform.forward.normalized);


        return relativePosition;
    }








}


