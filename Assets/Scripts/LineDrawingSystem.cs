using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawingSystem : MonoBehaviour
{
    public Material material;
    public Vector3[] vertices;
    public float width;
    public float constraintAngle;

    private LineRenderer line;
    private List<Vector3> listVertices;
    private float[] distBtwVertices;
    private Vector3 anchor;

    // Start is called before the first frame update
    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();
        listVertices = new List<Vector3>();
        distBtwVertices = new float[vertices.Length - 1];
        anchor = vertices[0];
        SetupList();
        SetupDistances();
        SetupLine();

        Debug.Log(GetNewPositionConstrained(new Vector3(0, 0, 0), new Vector3(-2, 0, 0), new Vector3(0, -2, 0), 2));
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            MoveToTarget(GetMousePosition());
           // MoveBackward();
        }
    }

    void SetupDistances()
    {
        for(int i = 0; i < distBtwVertices.Length; i++)
        {
            float d = Vector3.Distance(vertices[i], vertices[i + 1]);
            distBtwVertices[i] = d;
        }
    }

    void SetupList()
    {
        for(int i = 0; i < vertices.Length; i++)
        {
            listVertices.Add(vertices[i]);
        }
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

    void MoveToTarget(Vector3 target)
    {
        for (int i = vertices.Length - 1; i >= 0; i--)
        {
            Vector3 newPosition;
            if(i == vertices.Length - 1)
            {
                newPosition = target;
            }
            else
            {
                float crtDstBtwPoints = Vector3.Distance(line.GetPosition(i + 1), line.GetPosition(i));
                float dstBtwPoints = distBtwVertices[i];
                float dstToMove = crtDstBtwPoints - dstBtwPoints;

                Vector3 dirToMove = (line.GetPosition(i + 1) - line.GetPosition(i)).normalized;
                newPosition = line.GetPosition(i) + dirToMove * dstToMove;
            }
            ChangeVerticePositionAt(i, newPosition);
            //if (i > 0)
            //{
            //    Vector3 newPositionConstrained = GetNewPositionConstrained(line.GetPosition(i - 1), line.GetPosition(i), newPosition, distBtwVertices[i - 1]);
            //    ChangeVerticePositionAt(i, newPositionConstrained);
            //}
            //else
            //{
            //    ChangeVerticePositionAt(i, newPosition);
            //}
        }
    }

    void MoveBackward()
    {
        ChangeVerticePositionAt(0, anchor);

        for (int i = 1; i < vertices.Length; i++)
        {
            float crtDstBtwPoints = Vector3.Distance(line.GetPosition(i - 1), line.GetPosition(i));
            float dstBtwPoints = distBtwVertices[i - 1];
            float dstToMove = crtDstBtwPoints - dstBtwPoints;

            Vector3 dirToMove = (line.GetPosition(i - 1) - line.GetPosition(i)).normalized;
            Vector3 newPosition = line.GetPosition(i) + dirToMove * dstToMove;

            ChangeVerticePositionAt(i, newPosition);
            //Vector3 newPositionConstrained = GetNewPositionConstrained(line.GetPosition(i - 1), line.GetPosition(i), newPosition, distBtwVertices[i - 1]);
            //ChangeVerticePositionAt(i, newPositionConstrained);
        }
    }

    void ChangeVerticePositionAt(int i, Vector3 newPosition)
    {
        line.SetPosition(i, newPosition);
    }

    Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);
        return mousePosition;
    }

    Vector3 ClampPositionTo(Vector3 prevVerticePosition, Vector3 previousPosition, Vector3 newPosition, float r)
    {
        float radAngle = Mathf.Deg2Rad * constraintAngle;
        float cosR = r * Mathf.Cos(radAngle);
        float sinR = r * Mathf.Sin(radAngle);

        if (previousPosition.x < prevVerticePosition.x)
        {
            cosR *= -1;
        }
        if(newPosition.y < prevVerticePosition.y)
        {
            sinR *= -1;
        }

        float x = prevVerticePosition.y + cosR;
        float y = prevVerticePosition.y + sinR;

        return new Vector3(x, y, 0);
    }

    Vector3 GetNewPositionConstrained(Vector3 prevVerticePosition, Vector3 prevPosition, Vector3 newPosition, float r)
    {
        float cosTheta = Vector3.Dot((prevPosition - prevVerticePosition).normalized, (newPosition - prevVerticePosition).normalized);
        float cosThetaMax = Mathf.Cos(constraintAngle * Mathf.Deg2Rad);

        if(cosTheta < cosThetaMax)
        {
            return ClampPositionTo(prevVerticePosition, prevPosition, newPosition, r);
        }
        else
        {
            return newPosition;
        }
    }
}
