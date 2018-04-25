using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Vertex {
    public int meshIndex;
    public Vector3 pos;
    private List<Segment> connectedSegments = new List<Segment>();
    public ReadOnlyCollection<Segment> ConnectedSegments { get { return connectedSegments.AsReadOnly(); } }
    private List<Triangle> connectedTriangles = new List<Triangle>();
    public ReadOnlyCollection<Triangle> ConnectedTriangles { get { return connectedTriangles.AsReadOnly(); } }
    public Vertex(Vector3 position, int index)
    {
        pos = position;
        meshIndex = index;
    }
    public bool Contains(Segment segment)
    {
        foreach (var s in connectedSegments)
        {
            if (s == segment)
                return true;
        }
        return false;
    }
    public bool Contains(Triangle triangle)
    {
        foreach (var t in connectedTriangles)
        {
            if (t == triangle)
                return true;
        }
        return false;
    }
    public bool Equals(Vertex vertex)
    {
        if (pos == vertex.pos)
            return true;
        return false;
    }
    public void ConnectSegment(Segment segment)
    {
        if (!this.Contains(segment))
            connectedSegments.Add(segment);
    }
    public void ConnectTriangle(Triangle triangle)
    {
        if (!this.Contains(triangle))
            connectedTriangles.Add(triangle);
    }
}
