using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Triangle {
    public Vertex[] verts;
    public Segment[] segments;
    private List<Triangle> connectedTriangles = new List<Triangle>();
    public ReadOnlyCollection<Triangle> ConnectedTriangles { get { return connectedTriangles.AsReadOnly(); } }
    public Triangle(Vertex[] Verts, Segment[] Segments)
    {
        verts = Verts;
        segments = Segments;
        //Tell my segments and verts they are connected to me
        for (int i = 0; i < Verts.Length; i++)
        {
            verts[i].ConnectTriangle(this);
            segments[i].ConnectTriangle(this);
        }
    }
    public bool Contains(Segment segment)
    {
        foreach (var seg in segments)
        {
            if (seg == segment)
                return true;
        }
        return false;
    }
     
    public bool Contains(Vertex vertex)
    {
        foreach (var vert in verts)
        {
            if (vert == vertex)
                return true;
        }
        return false;
    }
    public bool Equals(Triangle triangle)
    {
        //TODO finish this
        return false;
    }
    public void ConnectTrianlge(Triangle triangle)
    {
        if (!connectedTriangles.Contains(triangle) && triangle != this)
            connectedTriangles.Add(triangle);
    }
    public Vector3[] GetVertsPositions()
    {
        Vector3[] v = new Vector3[verts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            v[i] = verts[i].pos;
        }
        return v;
    }
}
