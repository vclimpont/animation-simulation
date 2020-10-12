using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Point[] points;

    // Start is called before the first frame update
    void Start()
    {
        points = new Point[2];
    }

    public void Initialize(Point p1, Point p2)
    {
        points[0] = p1;
        points[1] = p2;
    }
}
