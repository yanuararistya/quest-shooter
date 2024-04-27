using MIConvexHull;
using System.Collections.Generic;
using UnityEngine;

public class MRUKVoronoiCellMesh : MonoBehaviour
{
    [SerializeField] private Material meshMaterial;
    [SerializeField] private bool includeShadow = true;
    [SerializeField] private Vector3 shadowOffset = new(0, -10f, 0f);
    [SerializeField] private Material shadowMaterial;

    public GameObject CreateMeshForCell(DefaultTriangulationCell<DefaultVertex> cell)
    {
        GameObject cellObject = new GameObject("VoronoiCell");
        cellObject.transform.SetParent(transform, false);

        var meshFilter = cellObject.AddComponent<MeshFilter>();
        var meshRenderer = cellObject.AddComponent<MeshRenderer>();
        var meshCollider = cellObject.AddComponent<MeshCollider>();
        var rigidbody = cellObject.AddComponent<Rigidbody>();
        Mesh mesh = new Mesh();

        List<Vector3> vertices = GetVerticesForCell(cell);

        int[] triangles = TriangulateCell(vertices);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshRenderer.material = meshMaterial;
        meshCollider.convex = true;
        meshCollider.sharedMesh = mesh;
        rigidbody.isKinematic = true;

        if (includeShadow) CreateShadowForCell(cellObject, mesh);

        return cellObject;
    }

    private void CreateShadowForCell(GameObject parentObject, Mesh originalMesh)
    {
        GameObject shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(parentObject.transform, false);

        Mesh shadowMesh = new Mesh();
        shadowMesh.vertices = ApplyOffsetToVertices(originalMesh.vertices, shadowOffset);
        shadowMesh.triangles = originalMesh.triangles;
        shadowMesh.RecalculateNormals();

        MeshFilter meshFilter = shadowObject.AddComponent<MeshFilter>();
        meshFilter.mesh = shadowMesh;

        MeshRenderer meshRenderer = shadowObject.AddComponent<MeshRenderer>();
        meshRenderer.material = shadowMaterial;
    }

    private Vector3[] ApplyOffsetToVertices(Vector3[] originalVertices, Vector3 offset)
    {
        Vector3[] offsetVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            offsetVertices[i] = originalVertices[i] + offset;
        }
        return offsetVertices;
    }

    private static List<Vector3> GetVerticesForCell(DefaultTriangulationCell<DefaultVertex> cell)
    {
        List<Vector3> vertices = new List<Vector3>();
        foreach (var vertex in cell.Vertices)
        {
            vertices.Add(new Vector3((float)vertex.Position[0], 0, (float)vertex.Position[1]));
        }
        return vertices;
    }

    private static int[] TriangulateCell(List<Vector3> vertices)
    {
        List<int> triangles = new List<int>();
        if (vertices.Count < 3) return triangles.ToArray();

        for (int i = 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        return triangles.ToArray();
    }
}
