using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    // Wave Generation Variables
    [Range(0,20)]public int waveCount;
    [Range(0,4)]public float steepnessModifier;
    [Range(0.5f,5)]public float wavelengthModifier;
    public Material waterMat;

    // Tile Generation Variables
    [Range(0,128)]public int tileResolution;
    [Range(0,16)] public int renderDistance;
    [Range(0.5f,10f)]public float tileScale;
    public GameObject waterPrefab;

    // Save wave info for height generation
    Vector4[] normWaves = new Vector4[20];


    // Start is called before the first frame update
    void Start()
    {

        SetupWaves();

        GenerateWaterTiles();
    }

    // Update is called once per frame
    void Update()
    {


        // Update slider variables
        waterMat.SetInt("_waveCount", waveCount);
        waterMat.SetFloat("_steepnessModifier", steepnessModifier);
        waterMat.SetFloat("_wavelengthModifier", wavelengthModifier);
    }

    void SetupWaves()
    {
        Vector4[] waves = new Vector4[100];
        for (int i=0;i<20;i++)
        {
            waves[i] = GenerateWave();
        }
        normWaves = NormalizeWaveAmps(waves);

        waterMat.SetInt("_waveCount", waveCount);
        waterMat.SetVectorArray("_waves", normWaves);
        waterMat.SetFloat("_steepnessModifier", steepnessModifier);
        waterMat.SetFloat("_wavelengthModifier", wavelengthModifier);
    }

    Vector4 GenerateWave()
    {
        float theta = Random.Range(0,Mathf.PI * 2);
        return new Vector4(
            Mathf.Cos(theta),
            Mathf.Sin(theta),
            Random.Range(.001f,2),
            Random.Range(1f,10f)
        );
    }

    Vector4[] NormalizeWaveAmps(Vector4[] waves)
    {
        Vector4[] tmp = waves;
        float ampSum = 0;
        for (int i=0;i<tmp.Length;i++)
        {
            ampSum += tmp[i].z;
        }
        if (ampSum > 1)
        {
            for (int i=0;i<tmp.Length;i++)
            {
                tmp[i].z = tmp[i].z / ampSum;
            }
        }
        return tmp;
    }

    void GenerateWaterTiles()
    {
        int[] triangles = MeshTrianglesGeneration();
        for (int i=-renderDistance;i<renderDistance;i++)
        {
            for (int j=-renderDistance;j<renderDistance;j++)
            {
                Mesh m = new Mesh();
                m.vertices = MeshVertexGeneration(i,j);
                m.uv = MeshUVGeneration(m.vertices);
                m.triangles = triangles;

                Transform tile = Instantiate(waterPrefab,this.transform).transform;
                tile.GetComponent<MeshFilter>().mesh = m;
                tile.GetComponent<MeshCollider>().sharedMesh = m;
            }
        }
    }

    Vector3[] MeshVertexGeneration(int offsetX, int offsetZ)
    {
        int edgeCount = tileResolution + 1; 
        Vector3[] verts = new Vector3[edgeCount * edgeCount];
        for (int i=0;i<edgeCount;i++)
        {
            for (int j=0;j<edgeCount;j++)
            {
                verts[(i*edgeCount)+j] = new Vector3(
                    (i + (tileResolution * offsetX))*tileScale,
                    0,
                    (j + (tileResolution * offsetZ))*tileScale
                );
            }
        }
        return verts;
    }

    Vector2[] MeshUVGeneration(Vector3[] vertices)
    {
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i=0;i<vertices.Length;i++)
        {
            uv[i] = new Vector2(vertices[i].x,vertices[i].z);
        }
        return uv;
    }

    int[] MeshTrianglesGeneration()
    {
        int edgeCount = tileResolution + 1; 
        int[] triangles = new int[tileResolution * tileResolution * 6];

        int triIndex = 0;
        int tlVert = 0;
        for (int i=0;i<tileResolution;i++)
        {
            for (int j=0;j<tileResolution;j++)
            {
                // Triangle 1
                triangles[triIndex+0] = tlVert;
                triangles[triIndex+1] = tlVert + 1;
                triangles[triIndex+2] = tlVert + edgeCount + 1;
                // Triangle 2
                triangles[triIndex+3] = tlVert;
                triangles[triIndex+4] = tlVert + edgeCount + 1;
                triangles[triIndex+5] = tlVert + edgeCount;

                triIndex += 6;
                tlVert += 1;
            }
            tlVert += 1;
        }
        return triangles;
    }

    public float GetHeight(float x, float z)
    {

        float y = 0;

        for (int i = 0; i < waveCount; i++)
        {
            float st = normWaves[i].z * steepnessModifier;
            float wl = normWaves[i].w * wavelengthModifier;
            float k = 2 * Mathf.PI / wl;
            float c = Mathf.Sqrt(9.8f / k);
            Vector2 d = new Vector2( normWaves[i].x, normWaves[i].y );
            d.Normalize(); 
            float f = k * ( Vector2.Dot(d, new Vector2(x,z)) - c * Shader.GetGlobalVector("_Time").y );
            float a = st / k;

            y += a * Mathf.Sin(f) * ( 20.0f / waveCount );
            
        }
        return y;

    }

    public void AdjustWaveCount (int newWaveCount)
    {
        waveCount = newWaveCount;
    }
    public void AdjustSteepness(float newSteepness)
    {
        steepnessModifier = newSteepness;
    }

    public void AdjustWavelength(int newWavelength)
    {
        wavelengthModifier = newWavelength;
    }
}
