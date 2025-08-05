using UnityEngine;
using System.Collections;

public class Combine : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {
            CombineMesh();
        }
    }

    private void CombineMesh()
    {
        //��ȡ��������������������е� MeshRenderer ���
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        //����������
        Material[] materials = new Material[meshRenderers.Length];

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            materials[i] = meshRenderers[i].sharedMaterial;
        }

        // �ϲ� Mesh
        // ��ȥ����������������� MsehFilter ���
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combines = new CombineInstance[meshRenderers.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combines[i].mesh = meshFilters[i].sharedMesh;
            // ����Matrix������ռ�����ĵ�ת��������ռ�����ĵ�
            combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        //��������mesh
        MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        // �� MeshFilter ����� mesh ��ֵ
        meshFilter.mesh = new Mesh();
        //�ϲ�Mesh�� �ڶ������� false����ʾ�����ϲ�Ϊһ�����񣬶���һ���������б�
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combines, false);
        transform.gameObject.SetActive(true);

        //Ϊ�ϲ������Mesh ָ������
        //transform.GetComponent<MeshRenderer>().sharedMaterials = materials;

        MeshRenderer meshRender = transform.GetComponent<MeshRenderer>();
        if (meshRender == null)
        {
            meshRender = gameObject.AddComponent<MeshRenderer>();
        }
        meshRender.sharedMaterials = materials;
    }
}
