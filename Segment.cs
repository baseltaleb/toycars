using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Segment {
    public int drawId;
    public Vertex v1 { get; private set; }
    public Vertex v2 { get; private set; }
    public Triangle t1 { get; private set; }
    public Triangle t2 { get; private set; }
    private List<Segment> connectedSegments = new List<Segment>();
    public ReadOnlyCollection<Segment> ConnectedSegments { get { return connectedSegments.AsReadOnly(); } }
    public bool isShared;
    public Segment(){}
    public Segment(Vertex V1, Vertex V2)
    {
        v1 = V1;
        v2 = V2;
        //Check if my verts have other connected segments and connect these segments to me
        if (v1.ConnectedSegments.Count > 0)
            foreach (var s in v1.ConnectedSegments)
            {
                ConnectSegment(s);
            }
        if(v2.ConnectedSegments.Count > 0)
            foreach (var s in v2.ConnectedSegments)
            {
                ConnectSegment(s);
            }
        //Tell my verts they are connected to me
        v1.ConnectSegment(this);
        v2.ConnectSegment(this);
    }

    public bool Contains(Vertex vertex)
    {
        if (v1 == vertex || v2 == vertex)
            return true;
        return false;
    }
    public bool Contains(Triangle triangle)
    {
        if(t1 == triangle || t2 == triangle)
        {
                return true;
        }
        return false;
    }
    public bool Equals(Segment segment)
    {
        if (v1 == segment.v1 || v1 == segment.v2)
            if (v2 == segment.v1 || v2 == segment.v2)
                return true;
        return false;
    }
    public void ConnectSegment(Segment segment)
    {
        if(!connectedSegments.Contains(segment) && segment != this)
            connectedSegments.Add(segment);
    }
    public void ConnectTriangle(Triangle triangle)
    {
        if (t1 == null)
            t1 = triangle;
        else if (t2 == null)
        {
            t2 = triangle;
            //Tell the triangles they are connected to each other
            t1.ConnectTrianlge(t2);
            t2.ConnectTrianlge(t1);
            isShared = true;
        }
        else
            Debug.LogError("Segment cannot have more than 2 connected triangles");
    }
}
