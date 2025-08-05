using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;



public class dialog : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Assign DialogSmall_192x96.prefab")]
    private GameObject dialogPrefabSmall;
    /// <summary>
    /// Small Dialog example prefab to display
    /// </summary>
    public GameObject DialogPrefabSmall
    {
        get => dialogPrefabSmall;
        set => dialogPrefabSmall = value;
    }

 

    public static bool hasResponded = false;
    public static bool canload = false;
    // Start is called before the first frame update
    void Start()
    {
        OpenChoiceDialogSmall();
        
    }

    private float lastTime;   //计时器
    private float curTime;
    
    // Update is called once per frame
    void Update()
    {
        if (TcpText.ConnectedCompleted && !hasResponded)
        {
            // 当T为true且尚未响应时执行的代码
            /*Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, " ", "ConnectedCompleted , Waiting for file transfer", true);
            hasResponded = true;
            canload = true;*/
            GameObject importedPrefab = Resources.Load("process") as GameObject;
            importedPrefab = Instantiate(importedPrefab);
            importedPrefab.name = "process";

            canload = true;
            hasResponded = true;
        }
        if(NewBehaviourScript2.iscoloring==true) Destroy(GameObject.Find("process"));
    }

    

    public void OpenChoiceDialogSmall()
    {
        Dialog myDialog = Dialog.Open(DialogPrefabSmall, DialogButtonType.Yes | DialogButtonType.No, " ", "Connect?", true);
        if (myDialog != null)
        {
            myDialog.OnClosed += OnClosedDialogEvent;
        }
    }

    private void OnClosedDialogEvent(DialogResult obj)
    {
        if (obj.Result == DialogButtonType.Yes)
        {
            Debug.Log("开始连接");
            TcpText text = GetComponent<TcpText>();
            text.TCPConnect();

        }

    }


}
