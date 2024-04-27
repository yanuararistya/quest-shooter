using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MIConvexHull;

public class MRUKVoronoiDiagram : MonoBehaviour
{
    [SerializeField] private int gridRows = 7;
    [SerializeField] private int gridColumns = 7;
    [SerializeField] private bool includeEdgeSites = true;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool applyExclusionArea = false;
    [SerializeField] private float exclusionHeightFromBottom = 2.0f;
    [SerializeField] private LayerMask cellLayer;
    [SerializeField] private LayerMask exclusionZoneCellLayer;

    private List<Vector3> sites = new List<Vector3>();
    private List<DefaultTriangulationCell<DefaultVertex>> voronoiCells = new List<DefaultTriangulationCell<DefaultVertex>>();
    private MRUKVoronoiCellMesh voronoiCellMeshGenerator;

    private void Start()
    {
        voronoiCellMeshGenerator = GetComponent<MRUKVoronoiCellMesh>();
        GenerateSitesBasedOnMeshBounds();
        GenerateVoronoiDiagram();
    }

    private void GenerateSitesBasedOnMeshBounds()
    {
        Bounds bounds = GetComponent<MeshRenderer>().localBounds;
        Transform meshTransform = GetComponent<Transform>();

        float cellWidth = bounds.size.x / gridColumns;
        float cellHeight = bounds.size.z / gridRows;

        for (int i = 0; i < gridRows; i++)
        {
            for (int j = 0; j < gridColumns; j++)
            {
                Vector3 sitePosition = new Vector3(bounds.min.x + j * cellWidth + Random.Range(0f, cellWidth), bounds.center.y, bounds.min.z + i * cellHeight + Random.Range(0f, cellHeight));

                if (applyExclusionArea && sitePosition.z - bounds.min.z < exclusionHeightFromBottom && !IsEdge(i, j, gridRows, gridColumns))
                {
                    continue;
                }

                Vector3 localSitePosition = meshTransform.InverseTransformPoint(sitePosition);
                sites.Add(meshTransform.TransformPoint(localSitePosition));
            }
        }

        if (includeEdgeSites)
        {
            AddEdgeSites(bounds);
        }
    }

    bool IsEdge(int rowIndex, int columnIndex, int totalRows, int totalColumns)
    {
        return rowIndex == 0 || columnIndex == 0 || rowIndex == totalRows - 1 || columnIndex == totalColumns - 1;
    }

    void AddEdgeSites(Bounds bounds)
    {
        float cellWidth = bounds.size.x / gridColumns;
        float cellHeight = bounds.size.z / gridRows;

        for (int j = 0; j <= gridColumns; j++)
        {
            float x = bounds.min.x + j * cellWidth;
            Vector3 topEdgeSite = new Vector3(x, bounds.center.y, bounds.max.z);
            Vector3 bottomEdgeSite = new Vector3(x, bounds.center.y, bounds.min.z);

            sites.Add(topEdgeSite);
            sites.Add(bottomEdgeSite);
        }

        for (int i = 0; i <= gridRows; i++)
        {
            float z = bounds.min.z + i * cellHeight;
            Vector3 leftEdgeSite = new Vector3(bounds.min.x, bounds.center.y, z);
            Vector3 rightEdgeSite = new Vector3(bounds.max.x, bounds.center.y, z);

            sites.Add(leftEdgeSite);
            sites.Add(rightEdgeSite);
        }
    }

    void GenerateVoronoiDiagram()
    {
        var points = sites.Select(site => new DefaultVertex { Position = new double[] { site.x, site.z } }).ToList();
        var triangulation = Triangulation.CreateDelaunay(points, Constants.DefaultPlaneDistanceTolerance);

        voronoiCells.Clear();
        foreach (var cell in triangulation.Cells) voronoiCells.Add(cell);

        CreateMeshesForCells();
    }

    void CreateMeshesForCells()
    {
        foreach (var cell in voronoiCells)
        {
            GameObject cellObject = voronoiCellMeshGenerator.CreateMeshForCell(cell);
            int layerIndex = applyExclusionArea && CellTouchesExclusionZone(cell) ? LayerMaskToLayer(exclusionZoneCellLayer) : LayerMaskToLayer(cellLayer);
            if (layerIndex >= 0)
            {
                cellObject.layer = layerIndex;
            }
        }
    }

    int LayerMaskToLayer(LayerMask layerMask)
    {
        int layerIndex = (int)Mathf.Log(layerMask.value, 2);
        return layerIndex >= 0 && layerIndex < 32 ? layerIndex : -1;
    }

    bool CellTouchesExclusionZone(DefaultTriangulationCell<DefaultVertex> cell)
    {
        Bounds bounds = GetComponent<MeshRenderer>().localBounds;
        float exclusionZoneMinZ = bounds.min.z + exclusionHeightFromBottom;

        foreach (var vertex in cell.Vertices)
        {
            Vector3 vertexPosition = new Vector3((float)vertex.Position[0], 0, (float)vertex.Position[1]);
            if (vertexPosition.z < exclusionZoneMinZ)
            {
                return true;
            }
        }
        return false;
    }


    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Transform meshTransform = GetComponent<Transform>();

        Gizmos.color = Color.blue;
        foreach (var site in sites)
        {
            Vector3 worldSitePosition = meshTransform.TransformPoint(site);
            Gizmos.DrawSphere(worldSitePosition, 0.08f);
        }

        Gizmos.color = Color.yellow;
        foreach (var cell in voronoiCells)
        {
            var vertices = cell.Vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 start = meshTransform.TransformPoint(new Vector3((float)vertices[i].Position[0], 0, (float)vertices[i].Position[1]));
                Vector3 end = meshTransform.TransformPoint(new Vector3((float)vertices[(i + 1) % vertices.Length].Position[0], 0, (float)vertices[(i + 1) % vertices.Length].Position[1]));

                Gizmos.DrawLine(start, end);
            }
        }

        if (!showGizmos || !applyExclusionArea) return;

        var renderer = GetComponent<MeshRenderer>();
        if (!renderer) return;

        Bounds bounds = renderer.bounds;
        Vector3 bottomCenter = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        Vector3 exclusionZoneHeightVector = transform.up * exclusionHeightFromBottom;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomCenter, bottomCenter + exclusionZoneHeightVector);
        Gizmos.DrawWireCube(bottomCenter + (exclusionZoneHeightVector / 2), new Vector3(bounds.size.x, bounds.size.y, bounds.size.z));
    }
}
