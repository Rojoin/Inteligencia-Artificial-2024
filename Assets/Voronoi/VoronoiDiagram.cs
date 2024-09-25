using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// [ExecuteAlways]
public class VoronoiDiagram : MonoBehaviour
{
    public bool createSegments;
    public bool drawPolis;

    [SerializeField] private List<Vector2> intersections = new List<Vector2>();
    [Space(15), SerializeField] private List<PoligonsVoronoi> polis = new List<PoligonsVoronoi>();
    [SerializeField] private List<Node<Vector2>> transformPoints = new List<Node<Vector2>>();
    [SerializeField] private List<SegmentLimit> segmentLimit = new List<SegmentLimit>();
    public GrapfView graph;

    public List<PoligonsVoronoi> GetPoly => polis;

    public IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        transformPoints = graph.graph.mines;
    }

    // public void AddNewItem (Transform item)
    // {
    //     transformPoints.Add(item);
    //     CreateSegments();
    // }
    //
    // public void RemoveItem (Transform item)
    // {
    //     transformPoints.Remove(item);
    //     CreateSegments();
    // }

    private void Update()
    {
        if (createSegments)
        {
            createSegments = false;
            CreateSegments();
        }
    }
[ContextMenu("Create Segments")]
    private void CreateSegments()
    {
        if (transformPoints == null)
            return;
        if (transformPoints.Count < 1)
            return;

        Segment.amountSegments = 0;
        polis.Clear();
        intersections.Clear();
        for (int i = 0; i < transformPoints.Count; i++)
        {
            PoligonsVoronoi poli = new PoligonsVoronoi(transformPoints[i], intersections);
            polis.Add(poli);
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
                Segment segment = new Segment(transformPoints[i].GetCoordinate(), transformPoints[j].GetCoordinate());
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
            foreach (PoligonsVoronoi poli in polis)
            {
                poli.DrawPoli(drawPolis);
            }
        }
    }
#endif
}