using System.Collections.Generic;
using UnityEngine;

public class Chunk01 : MonoBehaviour
{
    public Vector3 Size = new Vector3(20, 10, 20);
    public GridPoint[,,] p = null;
    public Material material = null;
    public float zoom = 1.5f;
    public float surfacelevel = 0.3f;
    public float pointSize = 0.5f;
    public float Editspeed = 0.5f;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uv = new List<Vector2>();
    private GridCell cell = new GridCell();
    private bool bUpdateRequest = false;

    private void Start()
    {
        p = new GridPoint[(int)Size.x, (int)Size.y, (int)Size.z];

        MakeGridPoints();
        MarchCubes();
    }
    private void Update()
    {
        if (bUpdateRequest == true)
        {
            MarchCubes();
            bUpdateRequest = false;
        }
    }
    private void OnEnable()
    {
        GridPoint.OnPointStayErase += OnPointErase;
        GridPoint.OnPointStayAdd += OnPointAdd;
    }
    private void OnDisable()
    {
        GridPoint.OnPointStayErase -= OnPointErase;
        GridPoint.OnPointStayAdd -= OnPointAdd;
    }
    private void MakeGridPoints()
    {
        for (int x = 0; x < p.GetLength(0); x++)
        {
            for (int y = 0; y < p.GetLength(1); y++)
            {
                for (int z = 0; z < p.GetLength(2); z++)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.transform.parent = this.transform;
                    go.transform.localPosition = new Vector3(x, y, z);
                    go.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
                    go.gameObject.GetComponent<Collider>().isTrigger = true;
                    go.layer = 2; //Ignore Raycast (2)

                    float noise = MarchingCube.Perlin2D(Size, new Vector3(x, y, z), zoom);

                    p[x, y, z] = go.AddComponent<GridPoint>();
                    p[x, y, z].Chunk = this.transform.position;
                    p[x, y, z].Value = noise;
                    p[x, y, z].Color = noise;
                    p[x, y, z].Visible = false;
                }
            }
        }
    }
    private void MarchCubes()
    {
        GameObject go = this.gameObject;
        MarchingCube.GetMesh(ref go, ref material, true);

        vertices.Clear();
        triangles.Clear();
        uv.Clear();

        /*  vertex 8 (0-7)
              E4-------------F5         7654-3210
              |               |         HGFE-DCBA
              |               |
        H7-------------G6     |
        |     |         |     |
        |     |         |     |
        |     A0--------|----B1  
        |               |
        |               |
        D3-------------C2               */
        for (int x = 0; x < p.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < p.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < p.GetLength(2) - 1; z++)
                {
                    cell.Reset();
                    cell.A0 = p[x, y, z + 1];
                    cell.B1 = p[x + 1, y, z + 1];
                    cell.C2 = p[x + 1, y, z];
                    cell.D3 = p[x, y, z];
                    cell.E4 = p[x, y + 1, z + 1];
                    cell.F5 = p[x + 1, y + 1, z + 1];
                    cell.G6 = p[x + 1, y + 1, z];
                    cell.H7 = p[x, y + 1, z];
                    MarchingCube.IsoFaces(ref cell, surfacelevel);
                    CreateCell();
                }
            }
        }

        Vector3[] av = vertices.ToArray();
        int[] at = triangles.ToArray();
        Vector2[] au = uv.ToArray();
        MarchingCube.SetMesh(ref go, ref av, ref at, ref au);
    }
    private void CreateCell()
    {
        bool uvAlternate = false;
        for (int i = 0; i < cell.numtriangles; i++)
        {
            vertices.Add(cell.triangle[i].p[0]);
            vertices.Add(cell.triangle[i].p[1]);
            vertices.Add(cell.triangle[i].p[2]);

            triangles.Add(vertices.Count - 1);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 3);

            /*  A ------ B
                |        |
                |        |
                D ------ C  */
            if (uvAlternate == true)
            {
                uv.Add(UVCoord.A);
                uv.Add(UVCoord.C);
                uv.Add(UVCoord.D);
            }
            else
            {
                uv.Add(UVCoord.A);
                uv.Add(UVCoord.B);
                uv.Add(UVCoord.C);
            }
            uvAlternate = !uvAlternate;
        }
    }
    private void OnPointErase(Vector3 chunk)
    {
        //if (chunk == this.transform.position)
            bUpdateRequest = true;
    }
    private void OnPointAdd(Vector3 chunk)
    {
        //if (chunk == this.transform.position)
            bUpdateRequest = true;
    }
}
