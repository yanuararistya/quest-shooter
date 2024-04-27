using UnityEngine;

public class MRUKDestructibleMesh : MonoBehaviour
{
    public GameObject prefabToInstantiate;

    public void InitializeMRUKVoronoiGeneration()
    {
        GameObject effectMesh = GameObject.Find("GLOBAL_MESH_EffectMesh");
        if (effectMesh != null) effectMesh.SetActive(false);

        // Find all Transform components in the scene, as every GameObject has a Transform component
        Transform[] allTransforms = FindObjectsOfType<Transform>();

        foreach (Transform tr in allTransforms)
        {
            // Check if the GameObject's name contains "CEILING_EffectMesh" or "WALL_FACE_EffectMesh"
            if (tr.name.ToLower().Contains("ceiling_effectmesh") || tr.name.ToLower().Contains("wall_face_effectmesh"))
            {
                // Add the MRUKDestroyWalls component to the GameObject
                tr.gameObject.AddComponent<MRUKDestroyWalls>();
            }
            else if (tr.name.ToLower().Contains("floor_effectmesh"))
            {
                tr.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
