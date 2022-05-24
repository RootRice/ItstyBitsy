using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEditor : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    [SerializeField] int resolution;
    [SerializeField] float width;
    [SerializeField] float height;

    [SerializeField] float wavyness;
    [SerializeField] public float waveHeight;
    [SerializeField] int waveResolution;
    [SerializeField] float timeOffset;
    [SerializeField] float test;


    private void Start()
    {
        mesh = new Mesh();
        mesh.name = "Water Mesh";
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        DrawMesh();
    }

    private void Update()
    {
        DrawMesh();
    }

    void DrawMesh()
    {
        Vector3[] surfacePoints = new Vector3[resolution];
        Vector2[] uvs = new Vector2[resolution];
        float start = 0;
        float step = width / resolution;
        float waveStep = 180 / waveResolution;
        float heightIncrementPerStep = wavyness;
        int[] triangles = new int[3*(resolution/2)];
        float waveVariance = waveHeight / 5;
        for (int i = 0; i < resolution;)
        { 
            for(int ii = 0; ii < waveResolution*2; ii++)
            {
                surfacePoints[i] = new Vector3(start + i *step, Mathf.Sin(Mathf.Deg2Rad*waveStep*ii + (Time.timeSinceLevelLoad + timeOffset) * wavyness) * waveHeight , 0f);
                uvs[i] = new Vector2(i /test, 0);
                i++;
                if(i >= resolution)
                {
                    break;
                }
                surfacePoints[i] = new Vector3(start + i * step, -height, 0f);
                uvs[i] = new Vector2(i / test, 1);
                i++;
                if (i >= resolution)
                {
                    break;
                }
            }
            if (0 > waveResolution)
            {
                break;
            }
        }
        int j = 0;
        for (int i = 0; i < (resolution / 2) * 3; i++)
        {
            triangles[i] = i - j;
            i++;
            triangles[i] = i - j;
            i++;
            triangles[i] = i - j;
            i++;
            triangles[i] = i - 1 - j;
            i++;
            triangles[i] = i - 3 - j;
            i++;
            triangles[i] = i - 2 - j;
            j += 4;

        }
        int max = 0, min = 0;
        for(int i = 0; i < (resolution / 2) * 3; i++)
        {
            max = Mathf.Max(max, triangles[i]);
            min = Mathf.Min(min, triangles[i]);
        }
        mesh.Clear();
        mesh.vertices = surfacePoints;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();


    }

}
