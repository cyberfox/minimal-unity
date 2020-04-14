using System.Linq;
using UnityEngine;
using DAZ;

public class Cubist : MonoBehaviour
{
    public Material DefaultMaterial;
    private float scalefactor = 10.0f;
    public bool doubleSided = false;

    public string Filepath = 
        "/Users/mrs/Documents/DAZ 3D/Studio/My Daz Connect Library/data/cloud/1_52197/data/daz 3d/chic fashion gothic outfit/collar/go_collar_19856.dsf";
    //"/Users/mrs/Projects/Power Offense/Assets/chic fashion gothic collar.duf"

    private void Start()
    {
        DSF dsf = Utility.LoadDSF(Filepath);

        Debug.Log(dsf.asset_info.contributor.author);
        Debug.Log(dsf.file_version);

        GameObject simple_mesh = new GameObject("DSF_Object");
        generateMesh(simple_mesh, dsf, out Vector3[] vertexData, out int[] indices);
        if (doubleSided)
        {
            GameObject second_mesh = new GameObject("DSF_Object_inverse");
            Vector3 object_pos = new Vector3(0, 0.5f, 0);
            Quaternion object_rot = new Quaternion(0, 0, 0, 0);
            second_mesh.transform.position = object_pos;
            second_mesh.transform.rotation = object_rot;
            second_mesh.AddComponent<MeshFilter>();
            second_mesh.AddComponent<MeshRenderer>();
            second_mesh.GetComponent<MeshRenderer>().material = DefaultMaterial;
            Mesh mesh = second_mesh.GetComponent<MeshFilter>().mesh;
            mesh.name = "inverted";
            mesh.vertices = vertexData;
            for (int i = 0; i < indices.Length; i+=3)
            {
                int temp = indices[i];
                indices[i] = indices[i+1];
                indices[i+1] = temp;
            }
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.RecalculateNormals();
        }
    }

    private void generateMesh(GameObject simple_mesh, DSF dsf, out Vector3[] vertex_data, out int[] indices)
    {
        Vector3 object_pos = new Vector3(0, 0.5f, 0);
        Quaternion object_rot = new Quaternion(0, 0, 0, 0);
        simple_mesh.transform.position = object_pos;
        simple_mesh.transform.rotation = object_rot;
        simple_mesh.AddComponent<MeshFilter>();
        simple_mesh.AddComponent<MeshRenderer>();
        simple_mesh.GetComponent<MeshRenderer>().material = DefaultMaterial;
        vertex_data = null;
        indices = null;

        int submesh = 0;
        foreach (Geometry g in dsf.geometry_library)
        {
            Vertices vertices = g.vertices;
            vertex_data = new Vector3[vertices.count];

            for (int i = 0; i < vertices.count; i++)
            {
                vertex_data[i] = new Vector3(vertices.values[i][0], vertices.values[i][1], vertices.values[i][2]);
            }

            indices = buildIndices(g.polylist, out var isTriangles);

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
            submesh++;
        }
    }

    private int[] buildIndices(PolyList polys, out bool isTriangles)
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
            int baseCount = quadCount * 6 + triCount * 3;
            int currentIndex = 0;

            indices = new int[baseCount];

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
}
