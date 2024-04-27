using UnityEngine;

public class MRUKDestroyWalls : MonoBehaviour
{
    private MRUKDestructibleMesh destructibleMesh;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Start()
    {
        destructibleMesh = FindObjectOfType<MRUKDestructibleMesh>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        CreateAndFitPrefab();
    }

    private void CreateAndFitPrefab()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        GameObject instantiatedPrefab = Instantiate(destructibleMesh.prefabToInstantiate, transform);
        Bounds bounds = meshFilter.sharedMesh.bounds;

        float prefabInitialSizeX = 10f;
        float prefabInitialSizeZ = 10f;

        float scaleX = bounds.size.x / prefabInitialSizeX;
        float scaleZ = bounds.size.y / prefabInitialSizeZ;
        float scaleY = 0.001f;

        instantiatedPrefab.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        instantiatedPrefab.transform.localPosition = new Vector3(0, 0, 0);
        instantiatedPrefab.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        if (meshRenderer != null) meshRenderer.enabled = false;
        if (meshCollider != null) meshCollider.enabled = false;
    }
}
