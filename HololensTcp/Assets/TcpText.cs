using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Application = UnityEngine.Application;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Security.Principal;
using UnityEngine.UIElements;
using System.Drawing;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using static NewBehaviourScript2;

public class TcpText : MonoBehaviour
{//连接到Tcp客户端并能够收发信息,断线重连,将客户端发送过来的信息显示在debugText中
 //手机开热点服务器ip 172.20.10.1 (不一定是这个,从网络设置中查看)
    public static bool ConnectedCompleted = false;//连接完成标志
    public static bool coloringsuccess = false;
    public Text debugText;
    public Socket m_socket;
    IPEndPoint m_endPoint;
    private SocketAsyncEventArgs m_connectSAEA;
    private SocketAsyncEventArgs m_sendSAEA;
    private SocketAsyncEventArgs m_sendSAEA1;
    public string ip = "10.206.75.19";//需要连接到的服务器ip
    public int port = 2077;
    private string preMsg = " ";
    bool needReconnect = false;
    string filePath;
    string filepath1;
    public static bool fileAccept = false;
    private void Start()
    {
        //Client();
        //Invoke("Client", 1f);
        filePath = Application.persistentDataPath;
        filepath1 = Application.dataPath;
    }

    private void Update()
    {//接收信息的回调函数上无法处理与Unity直接相关部分,在这里进行修改
        if (debugText && preMsg != " ") //接收消息
        {
            debugText.text = preMsg;//显示在Debug的UI上
            preMsg = " ";
        }
        if (needReconnect) //处理断线重连
        {
            Invoke("Client", 5f);
            needReconnect = false;
        }
    }

    public void Client()
    {
        m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress iPAddress = IPAddress.Parse(ip);
        m_endPoint = new IPEndPoint(iPAddress, port);
        m_connectSAEA = new SocketAsyncEventArgs { RemoteEndPoint = m_endPoint };
        m_connectSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectedCompleted);
        m_socket.ConnectAsync(m_connectSAEA);

    }

    private void OnConnectedCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError != SocketError.Success) { needReconnect = true; return; }
        Socket socket = sender as Socket;
        string iPRemote = socket.RemoteEndPoint.ToString();


        SocketAsyncEventArgs receiveSAEA = new SocketAsyncEventArgs();
        byte[] receiveBuffer = new byte[1024 * 1024 * 16];
        byte[] temporaryreceiveBuffer = new byte[4096];
        receiveSAEA.SetBuffer(temporaryreceiveBuffer, 0, temporaryreceiveBuffer.Length);

        receiveSAEA.Completed += OnReceiveCompleted;
        receiveSAEA.RemoteEndPoint = m_endPoint;
        socket.ReceiveAsync(receiveSAEA);
        ConnectedCompleted = true;
        Send("hello");

    }

    private void WriteListToTextFile(List<string> list, string txtFile)

    {

        //创建一个文件流，用以写入或者创建一个StreamWriter 

        FileStream fs = new FileStream(txtFile, FileMode.OpenOrCreate, FileAccess.Write);

        StreamWriter sw = new StreamWriter(fs);

        sw.Flush();

        // 使用StreamWriter来往文件中写入内容 

        sw.BaseStream.Seek(0, SeekOrigin.Begin);

        for (int i = 0; i < list.Count; i++) sw.Write(list[i]);

        //关闭此文件 

        sw.Flush();

        sw.Close();

        fs.Close();

    }

    List<byte> fileBuffer = new List<byte>();
    private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.OperationAborted) return;
        Socket socket = sender as Socket;
        if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
        {
            string ipAdress = socket.RemoteEndPoint.ToString();
            int lengthBuffer = e.BytesTransferred;//获取接收的数据长度

            byte[] receiveBuffer = e.Buffer;

            //读取指定位数的信息
            byte[] data = new byte[lengthBuffer];
            Array.Copy(receiveBuffer, 0, data, 0, lengthBuffer);

            string str = System.Text.Encoding.Default.GetString(data);
            if (str != "12345" && str != "123456789")
            {
                fileBuffer.AddRange(data);
            }
            if (str == "12345")
            {



                WriteListToTextFile(fileBuffer.Select(b => Convert.ToChar(b).ToString()).ToList(), filePath + "/flowerWithoutLabel.pcd");
                //WriteListToTextFile(fileBuffer.Select(b => Convert.ToChar(b).ToString()).ToList(), filepath1 + "/flowerWithoutLabel.pcd");
    
                fileAccept = true;
                fileBuffer.Clear();
            }
            if (str == "123456789")
            {

                WriteListToTextFile(fileBuffer.Select(b => Convert.ToChar(b).ToString()).ToList(), filePath + "/flowerWithLabel.pcd");
                // WriteListToTextFile(fileBuffer.Select(b => Convert.ToChar(b).ToString()).ToList(), filepath1 + "/flowerWithLabel.pcd");
    
                coloringsuccess = true;
                fileBuffer.Clear();
            }
            string newstr = str;
            //Debug.Log(newstr);
            preMsg = newstr;//这里直接赋值给debugText.text无法更新,通过update中检测的方式更新信息

            socket.ReceiveAsync(e);
        }
        else if (e.BytesTransferred == 0) //连接断开的处理
        {
            if (e.SocketError == SocketError.Success)
            {
                Debug.Log("主动断开连接 ");
                //DisConnect();
            }
            else
            {
                Debug.Log("被动断开连接 ");
            }
            needReconnect = true;//通过update中检测的方式更新信息
        }
        else
        {
            return;
        }
    }




    #region 发送
    void Send(string msg)
    {
        byte[] sendBuffer = Encoding.Default.GetBytes(msg);
        if (m_sendSAEA == null)
        {
            m_sendSAEA = new SocketAsyncEventArgs();
            m_sendSAEA.Completed += OnSendCompleted;

        }

        m_sendSAEA.SetBuffer(sendBuffer, 0, sendBuffer.Length);
        if (m_socket != null)
        {
            m_socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, sendCallBack, m_socket);
        }

    }

    void Send1(byte[] msg)
    {
        byte[] sendBuffer = msg;
        if (m_sendSAEA == null)
        {
            m_sendSAEA.Completed += OnSendCompleted1;

        }

        m_sendSAEA.SetBuffer(sendBuffer, 0, sendBuffer.Length);
        if (m_socket != null)
        {
            m_socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, sendCallBack, m_socket);
        }
    }

    private void sendCallBack(IAsyncResult ar)

    {
        string msg = (string)ar.AsyncState;
        Debug.Log(msg);


    }

    void OnSendCompleted1(object sender1, SocketAsyncEventArgs e1)
    {
        if (e1.SocketError != SocketError.Success) return;
        Socket socket1 = sender1 as Socket;
        byte[] sendBuffer = e1.Buffer;
        string sendMsg = Encoding.Default.GetString(sendBuffer);



    }


    void OnSendCompleted(object sender, SocketAsyncEventArgs e1)
    {

        if (e1.SocketError != SocketError.Success) return;
        Socket socket = sender as Socket;
        byte[] sendBuffer = e1.Buffer;

        string sendMsg = Encoding.Default.GetString(sendBuffer);



    }
    #endregion
    #region 断开连接
    void DisConnect()
    {
        
        if (m_socket != null)
        {
            try
            {
                m_socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException excep)
            {
            }
            finally
            {
                m_socket.Close();
            }
        }
    }
    #endregion
    public int connectlab = 0;
    public int outlab = 0;
    public void TCPConnect()
    {
        if (connectlab == 1)
        {
            Invoke("Client", 1f);
            connectlab = 0;

        }
        else
        {
            DisConnect();
            connectlab = 1;
        }
    }

    public void SendLocation()
    {

        if (connectlab == 0)
        {

            Vector3 vertex3 = Printlocation();
            Send(vertex3.ToString());
        }
        else
        {
            
        }

        if (outlab == 1)
        {
            Send("outofboundry");
            outlab = 0;
        }

    }

    public void Startcoloring()
    {
        Destroy(GameObject.Find("定位球"));
        Destroy(GameObject.Find("new"));

        GameObject importedPrefab1 = Resources.Load("process") as GameObject;
        importedPrefab1 = Instantiate(importedPrefab1);
        importedPrefab1.name = "process1";
        Send("start-coloring");
        
    }

    public void SendTagPcd()
    {

    }

    private NewBehaviourScript2 NewBehaviourScript2;
    public void Savepcd() 
    {
        string strDir = Application.persistentDataPath;
        
        NewBehaviourScript2 = GameObject.Find("GameObject").GetComponent<NewBehaviourScript2>();
        
        if (File.Exists(string.Format("{0}\\{1}.pcd", strDir, "savepcdaaa")))
        {
            File.Delete(string.Format("{0}\\{1}.pcd", strDir, "savepcdaaa"));
            
        }
        NewBehaviourScript2.SaveToPcd();
        Send("file");
        string path = filePath + "/flowerWithLabel.pcd";
        string path1 = filePath + "/savepcdaaa.pcd";
        byte[] data = File.ReadAllBytes(path1);
        Send1(data);
        

        Send("OKOKOK");

      
    }

  

        public Vector3 Printlocation()
    {
        GameObject P;
        if (coloringsuccess == false) P = GameObject.Find("new"); else P = GameObject.Find("new1");
        BoxCollider boxCollider = P.GetComponent<BoxCollider>();
        // 获取盒装碰撞器的中点
        Vector3 center = boxCollider.bounds.center;
        // 获取盒装碰撞器的尺寸
        Vector3 size = boxCollider.bounds.extents;
        Vector3 location = center - size;

        Transform targetTransform = GameObject.Find("EmptyObject").GetComponent<Transform>();
        Transform referenceTransform = GameObject.Find("球").GetComponent<Transform>();
        //Vector3 relativePosition = referenceTransform.InverseTransformPoint(targetTransform.position);
        Vector3 boundarypoint = GameObject.Find("固定按钮").transform.position-new Vector3(0.1f,0.1f,0.1f);
        Vector3 cubepoint=referenceTransform.position;
        Vector3 distance = referenceTransform.transform.position - targetTransform.transform.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, targetTransform.transform.right.normalized)*200;
        relativePosition.z = Vector3.Dot(distance, targetTransform.transform.up.normalized)*200;
        relativePosition.y = Vector3.Dot(distance, targetTransform.transform.forward.normalized) * 200;
       

        return relativePosition;
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

}
