﻿using System.Collections;
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

    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            MoveToTarget(GetMousePosition());
            MoveBackward();
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

            if(i < vertices.Length - 2)
            {
                newPosition = GetNewPositionConstrained(line.GetPosition(i + 1), line.GetPosition(i + 2), newPosition, distBtwVertices[i]);
            }

            ChangeVerticePositionAt(i, newPosition);
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

            if(i > 1)
            {
                newPosition = GetNewPositionConstrained(line.GetPosition(i - 1), line.GetPosition(i - 2), newPosition, distBtwVertices[i - 1]);
            }

            ChangeVerticePositionAt(i, newPosition);
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

    Vector3 ClampPositionTo(Vector3 verticePositionM1, Vector3 alignedPosition, Vector3 newPosition, float r)
    {
        Vector3 alignedPositionZero = alignedPosition - verticePositionM1;
        float epsilon = Mathf.Deg2Rad * constraintAngle;
        float phi = Mathf.Atan2(alignedPositionZero.y, alignedPositionZero.x);
        Debug.Log(phi * Mathf.Rad2Deg);

        float xp = verticePositionM1.x + r * Mathf.Cos(phi - epsilon);
        float yp = verticePositionM1.y + r * Mathf.Sin(phi - epsilon);
        Vector3 vp = new Vector3(xp, yp, 0);

        float xs = verticePositionM1.x + r * Mathf.Cos(phi + epsilon);
        float ys = verticePositionM1.y + r * Mathf.Sin(phi + epsilon);
        Vector3 vs = new Vector3(xs, ys, 0);


        if(Vector3.Distance(newPosition, vp) < Vector3.Distance(newPosition, vs))
        {
            return vp;
        }
        else
        {
            return vs;
        }
    }

    Vector3 GetNewPositionConstrained(Vector3 verticePositionM1, Vector3 verticePositionM2, Vector3 newPosition, float r)
    {
        Vector3 prevDir = (verticePositionM1 - verticePositionM2).normalized;
        Vector3 crtDir = (newPosition - verticePositionM1).normalized;
        Vector3 alignedPosition = verticePositionM1 + prevDir * r;
        float cosTheta = Vector3.Dot(prevDir, crtDir);
        float cosThetaMax = Mathf.Cos(constraintAngle * Mathf.Deg2Rad);

        if(cosTheta < cosThetaMax)
        {
            return ClampPositionTo(verticePositionM1, alignedPosition, newPosition, r);
        }
        else
        {
            return newPosition;
        }
    }
}
