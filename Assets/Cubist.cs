using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using DAZ;

public class Cubist : MonoBehaviour
{
    public Material DefaultMaterial;
    private float scalefactor = 10.0f;

    class MeshFace
    {
        public int _vid { get; set; }
        public int _uid { get; set; }
        public int _nid { get; set; }

        public MeshFace(int vid,int uid,int nid) {
            _vid = vid; _uid = uid; _nid = nid;
        }
    }

    //// Exported data from 3DsMax using Trimesh API [YZ flip is applied]
    //// It's in *.obj data-type format
    private Vector3[] _3dsmax_vertexdata = {
      new Vector3(-5.000000f,-5.000000f,5.000000f),
      new Vector3(5.000000f,-5.000000f,5.000000f),
      new Vector3(-5.000000f,-5.000000f,-5.000000f),
      new Vector3(5.000000f,-5.000000f,-5.000000f),
      new Vector3(-5.000000f,5.000000f,5.000000f),
      new Vector3(5.000000f,5.000000f,5.000000f),
      new Vector3(-5.000000f,5.000000f,-5.000000f),
      new Vector3(5.000000f,5.000000f,-5.000000f)
    };
    private Vector3[] _3dsmax_normaldata = {
      new Vector3(0.000000f,-1.000000f,0.000000f),
      new Vector3(0.000000f,-1.000000f,0.000000f),
      new Vector3(0.000000f,-1.000000f,0.000000f),
      new Vector3(0.000000f,-1.000000f,0.000000f),
      new Vector3(0.000000f,1.000000f,0.000000f),
      new Vector3(0.000000f,1.000000f,0.000000f),
      new Vector3(0.000000f,1.000000f,0.000000f),
      new Vector3(0.000000f,1.000000f,0.000000f),
      new Vector3(0.000000f,0.000000f,1.000000f),
      new Vector3(0.000000f,0.000000f,1.000000f),
      new Vector3(0.000000f,0.000000f,1.000000f),
      new Vector3(0.000000f,0.000000f,1.000000f),
      new Vector3(1.000000f,0.000000f,0.000000f),
      new Vector3(1.000000f,0.000000f,0.000000f),
      new Vector3(1.000000f,0.000000f,0.000000f),
      new Vector3(1.000000f,0.000000f,0.000000f),
      new Vector3(0.000000f,0.000000f,-1.000000f),
      new Vector3(0.000000f,0.000000f,-1.000000f),
      new Vector3(0.000000f,0.000000f,-1.000000f),
      new Vector3(0.000000f,0.000000f,-1.000000f),
      new Vector3(-1.000000f,0.000000f,0.000000f),
      new Vector3(-1.000000f,0.000000f,0.000000f),
      new Vector3(-1.000000f,0.000000f,0.000000f),
      new Vector3(-1.000000f,0.000000f,0.000000f)
    };
    private Vector2[] _3dsmax_uvdata = {
      new Vector2(0.000000f,0.000000f),
      new Vector2(1.000000f,0.000000f),
      new Vector2(0.000000f,1.000000f),
      new Vector2(1.000000f,1.000000f),
      new Vector2(0.000000f,0.000000f),
      new Vector2(1.000000f,0.000000f),
      new Vector2(0.000000f,1.000000f),
      new Vector2(1.000000f,1.000000f),
      new Vector2(0.000000f,0.000000f),
      new Vector2(1.000000f,0.000000f),
      new Vector2(0.000000f,1.000000f),
      new Vector2(1.000000f,1.000000f)
        };
    private MeshFace[] _3dsmax_facedata = {
      new MeshFace(4,11,1),
      new MeshFace(2,9,2),
      new MeshFace(1,10,3),

      new MeshFace(1,10,3),
      new MeshFace(3,12,4),
      new MeshFace(4,11,1),

      new MeshFace(8,12,5),
      new MeshFace(7,11,6),
      new MeshFace(5,9,7),

      new MeshFace(5,9,7),
      new MeshFace(6,10,8),
      new MeshFace(8,12,5),

      new MeshFace(6,8,9),
      new MeshFace(5,7,10),
      new MeshFace(1,5,11),

      new MeshFace(1,5,11),
      new MeshFace(2,6,12),
      new MeshFace(6,8,9),

      new MeshFace(8,4,13),
      new MeshFace(6,3,14),
      new MeshFace(2,1,15),

      new MeshFace(2,1,15),
      new MeshFace(4,2,16),
      new MeshFace(8,4,13),

      new MeshFace(7,8,17),
      new MeshFace(8,7,18),
      new MeshFace(4,5,19),

      new MeshFace(4,5,19),
      new MeshFace(3,6,20),
      new MeshFace(7,8,17),

      new MeshFace(5,4,21),
      new MeshFace(7,3,22),
      new MeshFace(3,1,23),

      new MeshFace(3,1,23),
      new MeshFace(1,2,24),
      new MeshFace(5,4,21),
    };

    public string Filepath = 
        "/Users/mrs/Documents/DAZ 3D/Studio/My Daz Connect Library/data/cloud/1_52197/data/daz 3d/chic fashion gothic outfit/collar/go_collar_19856.dsf";
    //"/Users/mrs/Projects/Power Offense/Assets/chic fashion gothic collar.duf"

    private void Start()
    {
        DSF dsf = Utility.LoadDSF(Filepath);

        Debug.Log(dsf.asset_info.contributor.author);
        Debug.Log(dsf.file_version);

        GameObject simple_mesh = new GameObject("DSF_Object");
        Vector3 object_pos = new Vector3(0, 0.5f, 0);
        Quaternion object_rot = new Quaternion(0,0,0,0);
        simple_mesh.transform.position = object_pos;
        simple_mesh.transform.rotation = object_rot;
        simple_mesh.AddComponent<MeshFilter>();
        simple_mesh.AddComponent<MeshRenderer>();

        int submesh = 0;
        foreach (Geometry g in dsf.geometry_library)
        {
            Debug.Log(g.vertices.count);
            Debug.Log(g.polylist.count);
            Node n = dsf.node_library.First();
            Debug.Log(n.name);
            Vertices vertices = g.vertices;
            Vector3[] vertex_data   =   new Vector3[vertices.count];
            for (int i = 0; i < vertices.count; i++)
            {
                vertex_data[i] = new Vector3(vertices.values[i][0], vertices.values[i][1], vertices.values[i][2]);
            }

            PolyList polys = g.polylist;
            int[] indices = buildIndices(polys, out var isTriangles);

            Mesh mesh = simple_mesh.GetComponent<MeshFilter>().mesh;
            mesh.name = g.name;
            mesh.vertices = vertex_data;
            if (isTriangles)
            {
                mesh.SetIndices(indices, MeshTopology.Triangles, submesh);
            }
            else
            {
                mesh.SetIndices(indices, MeshTopology.Quads, submesh);
            }
            mesh.RecalculateNormals();
            simple_mesh.GetComponent<MeshRenderer>().material = DefaultMaterial;
            submesh++;
        }
    }

    private static int[] buildIndices(PolyList polys, out bool isTriangles)
    {
        int quadCount = 0;
        int triCount = 0;
        for (int i = 0; i < polys.count; i++)
        {
            if (polys.values[i].Count == 5) triCount++;
            if (polys.values[i].Count == 6) quadCount++;
        }

        Debug.Log(quadCount + " quads, and " + triCount + " triangles.");
        int[] indices;
        if (quadCount == polys.count)
        {
            indices = new int[quadCount * 4];
            for (int i = 0; i < polys.count; i++)
            {
                indices[i * 4 + 0] = polys.values[i][2];
                indices[i * 4 + 1] = polys.values[i][3];
                indices[i * 4 + 2] = polys.values[i][4];
                indices[i * 4 + 3] = polys.values[i][5];
            }
        }
        else if (triCount == polys.count)
        {
            indices = new int[triCount * 3];
            for (int i = 0; i < polys.count; i++)
            {
                indices[i * 4 + 0] = polys.values[i][2];
                indices[i * 4 + 1] = polys.values[i][3];
                indices[i * 4 + 2] = polys.values[i][4];
            }
        }
        else
        {
            indices = new int[quadCount * 6 + triCount * 3];
            int currentIndex = 0;
            for (int i = 0; i < polys.count; i++)
            {
                if (polys.values[i].Count == 5)
                {
                    indices[currentIndex++] = polys.values[i][2];
                    indices[currentIndex++] = polys.values[i][3];
                    indices[currentIndex++] = polys.values[i][4];
                }
                else
                {
                    indices[currentIndex++] = polys.values[i][2];
                    indices[currentIndex++] = polys.values[i][3];
                    indices[currentIndex++] = polys.values[i][5];
                    indices[currentIndex++] = polys.values[i][5];
                    indices[currentIndex++] = polys.values[i][3];
                    indices[currentIndex++] = polys.values[i][4];
                }
            }
        }

        isTriangles = triCount != 0;
        return indices;
    }

    void OldStart()
    {
        uint v_count = 8; uint n_count = 24;
        uint t_count = 12; uint f_count = 36;

        //// Extracted From 3DsMax
        string obj_name = "Box_001";
        Vector3 object_pos = new Vector3(0, 0.5f, 0);
        Quaternion object_rot = new Quaternion(0,0,0,0);

        GameObject simple_mesh = new GameObject(obj_name);
        simple_mesh.transform.position = object_pos;
        simple_mesh.transform.rotation = object_rot;

        simple_mesh.AddComponent<MeshFilter>();
        simple_mesh.AddComponent<MeshRenderer>();

        Mesh mesh = simple_mesh.GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertex_data   =   new Vector3[f_count];
        Vector3[] normal_data   =   new Vector3[f_count];
        Vector2[] texcoord_data =   new Vector2[f_count];
        int[] triangles         =   new int[f_count];

        for (int i = 0; i < f_count; i++)
        {
            triangles[i] = i;
            var face = _3dsmax_facedata[i];
            vertex_data[i] = _3dsmax_vertexdata[face._vid-1];
            normal_data[i] = _3dsmax_normaldata[face._nid-1];
            texcoord_data[i] = _3dsmax_uvdata[face._uid-1];
        }

        mesh.Clear();
        mesh.vertices = vertex_data;
        mesh.normals = normal_data;
        mesh.uv = texcoord_data;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        simple_mesh.GetComponent<MeshRenderer>().material = DefaultMaterial;
    }

    public static void Main(string[] args)
    {
        Debug.Log("done");
    }
}
