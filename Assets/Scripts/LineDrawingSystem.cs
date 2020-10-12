using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawingSystem : MonoBehaviour
{
    public Material material;
    public Vector3[] vertices;
    public float width;

    private LineRenderer line;
    private List<Vector3> listVertices;

    // Start is called before the first frame update
    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        SetupLine();
    }

    void SetupLine()
    {
        line.sortingLayerName = "Line";
        line.sortingOrder = 5;

        SetupVertices();

        line.startWidth = width;
        line.endWidth = width;
        line.useWorldSpace = true;
        line.material = material;
    }

    void SetupVertices()
    {
        line.positionCount = vertices.Length;
        line.SetPositions(vertices);
    }
}
