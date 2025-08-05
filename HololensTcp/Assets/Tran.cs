using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tran : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Create Vector3 vertices
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0.5f, 0, Mathf.Sqrt(0.75f));
        vertices[3] = new Vector3(0.5f, Mathf.Sqrt(0.75f), Mathf.Sqrt(0.75f) / 3);

        // Create Vector2 Array of UV coordinates
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0.5f, 1);
        uv[3] = new Vector2(0.5f, 0.5f);

        // Create Triangles
        int[] triangles = new int[12];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 1;
        triangles[6] = 1;
        triangles[7] = 3;
        triangles[8] = 2;
        triangles[9] = 0;
        triangles[10] = 2;
        triangles[11] = 3;

        // Create or get the Mesh
        GameObject gameObject = new GameObject();
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        Mesh mesh;
        if (mf == null)
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            mesh = gameObject.GetComponent<MeshFilter>().mesh;
            mesh.Clear();
        }
        else
        {
            mesh = mf.mesh;
            mesh.Clear();
        }

        // Assign Arrays to Mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
