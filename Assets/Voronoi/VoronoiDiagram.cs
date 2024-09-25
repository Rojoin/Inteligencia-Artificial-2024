using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class VoronoiDiagram : MonoBehaviour
{
    public bool createSegments;
    public bool drawPolis;

    [SerializeField] private List<Vector2> intersections = new List<Vector2>();
    [Space(15), SerializeField]
    private List<ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2>> polis =
        new List<ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2>>();
    [SerializeField] private List<Transform> transformPoints = new List<Transform>();
    [SerializeField] private List<SegmentLimit> segmentLimit = new List<SegmentLimit>();
    [SerializeField]
    private Dictionary<ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2>, Color> polyColors =
        new Dictionary<ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2>, Color>();

    public List<ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2>> GetPoly => polis;

    public void AddNewItem(Transform item)
    {
        transformPoints.Add(item);
        CreateSegments();
    }

    public void RemoveItem(Transform item)
    {
        transformPoints.Remove(item);
        CreateSegments();
    }

    private void Update()
    {
        if (createSegments)
        {
            createSegments = false;
            CreateSegments();
        }
    }

    private void CreateSegments()
    {
        //Todo: Change transfroms to nodes
        if (transformPoints == null)
            return;
        if (transformPoints.Count < 1)
            return;

        SegmentVec2<Vector2>.amountSegments = 0;
        polis.Clear();
        intersections.Clear();
          polyColors.Clear();
         
        for (int i = 0; i < transformPoints.Count; i++)
        {
            ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2> poli =
                new ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2>(transformPoints[i].position, intersections);
            polis.Add(poli);
            poli.colorGizmos.r = Random.Range(0,1.0f);
            poli.colorGizmos.g = Random.Range(0,1.0f);
            poli.colorGizmos.b = Random.Range(0,1.0f);
            poli.colorGizmos.a = 0.3f;
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].AddSegmentsWithLimits(segmentLimit);
        }

        for (int i = 0; i < transformPoints.Count; i++)
        {
            for (int j = 0; j < transformPoints.Count; j++)
            {
                if (i == j)
                    continue;
                SegmentVec2<Vector2> segment =
                    new SegmentVec2<Vector2>(transformPoints[i].position, transformPoints[j].position);
                polis[i].AddSegment(segment);
            }
        }

        for (int i = 0; i < polis.Count; i++)
        {
            polis[i].SetIntersections();
        }

        SetWeightPoligons();
    }

    private void SetWeightPoligons()
    {
        // int allWeight = 0;
        // for (int i = 0; i < NodeGenerator.GetMap.Length; i++)
        // {
        //     if (IsNodeOutsideLimits(NodeGenerator.GetMap[i]))
        //         continue;
        //
        //     allWeight += NodeGenerator.GetMap[i].weight;
        //
        //     for (int j = 0; j < polis.Count; j++)
        //     {
        //         if (polis[j].IsInside(NodeGenerator.GetMap[i].position))
        //         {
        //             polis[j].weight += NodeGenerator.GetMap[i].weight;
        //             break;
        //         }
        //     }
        // }

        // for (int i = 0; i < polis.Count; i++)
        //     Debug.Log("Weight " + i + ": " + polis[i].weight);
        //
        // Debug.Log("Total Weight Polis: " + allWeight);
    }

    // bool IsNodeOutsideLimits (Node node)
    // {
    //     Vector3 origin = segmentLimit[0].Origin;
    //     Vector3 final = segmentLimit[2].Origin;
    //     Vector3 point = node.position;
    //
    //     return !(point.x > origin.x &&
    //              point.z > origin.z &&
    //              point.x < final.x &&
    //              point.z < final.z);
    // }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        DrawPolis(drawPolis);
    }

    private void DrawPolis(bool drawPolis)
    {
        if (polis != null)
        {
            foreach (ThiessenPolygon2D<SegmentVec2<Vector2>, Vector2> poli in polis)
            {
                if (poli.drawPoli || drawPolis)
                {
                    poli.DrawPoly();
                }
            }
        }
    }
#endif
}