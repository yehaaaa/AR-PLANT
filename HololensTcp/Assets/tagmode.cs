using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tagmode : MonoBehaviour
{

    public static int colornum = 1;
    public static Color[] colortonum = new Color[100];

    private Interactable asmp;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame


    public void colornumchange()
    {
        colornum += 1;
        colortonum[colornum] = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, UnityEngine.Random.Range(0.7f, 1f));
        GameObject.Find("Çò").GetComponent<MeshRenderer>().material.color = colortonum[colornum];

    }




}
