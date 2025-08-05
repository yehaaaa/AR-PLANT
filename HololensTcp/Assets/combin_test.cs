using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using static NewBehaviourScript1;
using Unity.VisualScripting;

public class combin_test : MonoBehaviour
{
    public static bool iscoloring = false;
    public static int label4num = 0;
    public static bool tip = true;
    public static int[] Tag, Tagafter;
    public int tagmode = 1;
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
    public enum FiledNames { X, Y, Z, NorX, NorY, NorZ, Curvature };

    public void PcdFileParser()
    {
    }



    // Start is called before the first frame update
    void Start()
    {
        LoadFile_Sphere(Application.dataPath + "/benthi1_save.pcd");
        meah_pointcloud(m_pcdHeader.m_pcdData.m_label);
        /*GameObject pointObj = new GameObject();
        pointObj.name = "father";
        for (int i = 0; i < 200; i++) 
        {
            GameObject importedPrefab1 = Resources.Load("点") as GameObject;
            importedPrefab1 = Instantiate(importedPrefab1);
            importedPrefab1.name = i.ToString();
            importedPrefab1.transform.position =new Vector3(0,0.002f,0)*i;
            importedPrefab1.transform.SetParent(pointObj.transform);
        }*/

        //MergeMesh_pro();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MergeMesh()
    {
        MeshFilter[] meshFilters = GameObject.Find("father").GetComponentsInChildren<MeshFilter>();   //获取 所有子物体的网格
        Debug.Log(meshFilters.Length);
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length]; //新建一个合并组，长度与 meshfilters一致

        for (int i = 0; i < meshFilters.Length; i++)                                  //遍历
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;                   //将共享mesh，赋值
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix; //本地坐标转矩阵，赋值
        }

        Mesh newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;//声明一个新网格对象
        newMesh.CombineMeshes(combineInstances);                    //将combineInstances数组传入函数
        GameObject.Find("father").AddComponent<MeshFilter>().mesh = newMesh; //给当前空物体，添加网格组件；将合并后的网格，给到自身网格
        //到这里，新模型的网格就已经生成了。运行模式下，可以点击物体的 MeshFilter 进行查看网格
        GameObject.Find("father").AddComponent<MeshRenderer>().material= GameObject.Find("2").GetComponent<MeshRenderer>().material;


    }

    private void MergeMesh_pro()
    {

        CombineInstance[] combineInstances = new CombineInstance[200]; //新建一个合并组，长度与 meshfilters一致

        for (int i = 0; i < 200; i++)                                  //遍历
        {
            GameObject prefab = Resources.Load("点") as GameObject;
            MeshFilter prefabMesh = prefab.GetComponent<MeshFilter>();

            combineInstances[i].mesh = prefabMesh.sharedMesh;
            combineInstances[i].transform = Matrix4x4.TRS(new Vector3(0, 0.002f, 0) * i, Quaternion.identity, new Vector3(0.02f, 0.02f, 0.02f));
        }

        Mesh newMesh = new Mesh();
        newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;//声明一个新网格对象
        newMesh.CombineMeshes(combineInstances);                    //将combineInstances数组传入函数
        GameObject combinedObject = new GameObject("CombinedObject");
        combinedObject.AddComponent<MeshFilter>().mesh = newMesh; //给当前空物体，添加网格组件；将合并后的网格，给到自身网格

        combinedObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/VertexColor")); 


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

        return true;

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
        int labelListSize = labelList.Count;
        Debug.Log("Size of labelList: " + labelListSize.ToString());
        int a = 0;
        GameObject ma = new GameObject();
        ma = GameObject.Find("11111");
        MeshRenderer mr = ma.GetComponent<MeshRenderer>();
        foreach (KeyValuePair<float, int> label in labelCount)
        {

            int g = (int)label.Key;
            Color newColor = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, UnityEngine.Random.Range(0.9f, 1f));

            GameObject importedPrefab1 = Resources.Load("点") as GameObject;
            List<int> indices = labelIndices[label.Key];

            CombineInstance[] combineInstances = new CombineInstance[label.Value];
            for (int i = 0; i < label.Value; ++i)
            {

                GameObject prefab = Resources.Load("点") as GameObject;
                MeshFilter prefabMesh = prefab.GetComponent<MeshFilter>();
                Vector3 xyz = new Vector3(m_pcdHeader.m_pcdData.m_fX[indices[i]], m_pcdHeader.m_pcdData.m_fY[indices[i]], m_pcdHeader.m_pcdData.m_fZ[indices[i]]) / 200;
                combineInstances[i].mesh = prefabMesh.sharedMesh;
                combineInstances[i].transform = Matrix4x4.TRS(xyz + pointObj.transform.position, Quaternion.identity, new Vector3(0.002f, 0.002f, 0.002f));

            }

            Mesh newMesh = new Mesh();
            newMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; //声明一个新网格对象
            newMesh.CombineMeshes(combineInstances); //将combineInstances数组传入函数
            GameObject combinedObject = new GameObject(label.Key.ToString());
            combinedObject.transform.position = father.transform.position;
            combinedObject.transform.SetParent(father.transform);
            combinedObject.AddComponent<MeshFilter>().mesh = newMesh; //给当前空物体，添加网格组件；将合并后的网格，给到自身网格
            combinedObject.AddComponent<MeshRenderer>().material = mr.material;
            combinedObject.GetComponent<MeshRenderer>().material.color = newColor;

            a += label.Value;

        }
    }

    private void meah_pointcloud(float[] labels) 
    {
        GameObject pointObj = new GameObject();

        if (TcpText.coloringsuccess == false) pointObj.name = "new"; else pointObj.name = "new1";

        pointObj.transform.position = new Vector3(0, 0, 0.5f);

        GameObject father = new GameObject();
        if (TcpText.coloringsuccess == false)
            father = GameObject.Find("new");
        else
            father = GameObject.Find("new1");

        mesh_creat(labels,father,pointObj);


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

        newEmptyObject.transform.SetParent(father.transform);

        Vector3 location = center + size + new Vector3(0.1f, 0.1f, 0.1f);
        GameObject importedPrefab = Resources.Load("Button") as GameObject;
        importedPrefab = Instantiate(importedPrefab, location, Quaternion.identity);
        importedPrefab.name = "固定按钮";

        importedPrefab.transform.SetParent(father.transform);
    }

}
