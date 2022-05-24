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
    [SerializeField] public float timeOffset;
    [SerializeField] float test;

    Vector2[] uvs;
    int[] tris;

 

    private void Start()
    {
        mesh = new Mesh();
        mesh.name = "Water Mesh";
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        uvs = new Vector2[resolution];
        DrawFirstMesh();
    }

    void DrawFirstMesh()
    {

        tris = new int[3 * (resolution / 2)];
        uvs = new Vector2[resolution];
        Vector3[] surfacePoints = new Vector3[resolution];
        float start = 0;
        float step = width / resolution;
        float waveStep = 180 / waveResolution;
        float heightIncrementPerStep = wavyness;
        float waveVariance = waveHeight / 5;
        for (int i = 0; i < resolution;)
        {
            for (int ii = 0; ii < waveResolution * 2; ii++)
            {
                surfacePoints[i] = new Vector3(start + i * step, Mathf.Sin(Mathf.Deg2Rad * waveStep * ii + (Time.timeSinceLevelLoad + timeOffset) * wavyness) * waveHeight, 0f);
                uvs[i] = new Vector2(i / test, 0);
                i++;
                if (i >= resolution)
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
            tris[i] = i - j;
            i++;
            tris[i] = i - j;
            i++;
            tris[i] = i - j;
            i++;
            tris[i] = i - 1 - j;
            i++;
            tris[i] = i - 3 - j;
            i++;
            tris[i] = i - 2 - j;
            j += 4;

        }
        mesh.Clear();
        mesh.vertices = surfacePoints;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private void Update()
    {
        DrawMesh();
    }

    void DrawMesh()
    {
        Vector3[] surfacePoints = new Vector3[resolution];
        float start = 0;
        float step = width / resolution;
        float waveStep = 180 / waveResolution;
        float heightIncrementPerStep = wavyness;
        float waveVariance = waveHeight / 5;
        for (int i = 0; i < resolution;)
        { 
            for(int ii = 0; ii < waveResolution*2; ii++)
            {
                surfacePoints[i] = new Vector3(start + i *step, Mathf.Sin(Mathf.Deg2Rad*waveStep*ii + (Time.timeSinceLevelLoad + timeOffset) * wavyness) * waveHeight , 0f);
                i++;
                if(i >= resolution)
                {
                    break;
                }
                surfacePoints[i] = new Vector3(start + i * step, -height, 0f);
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
        mesh.Clear();
        mesh.vertices = surfacePoints;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();


    }


}
