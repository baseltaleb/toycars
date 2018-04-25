using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditableMesh {
    public List<Vertex> vertices = new List<Vertex>();
    public List<Segment> segments = new List<Segment>();
    public List<Triangle> triangles = new List<Triangle>();
    public List<MeshEdgeHelpers.Triangle> triangls;
    public List<MeshEdgeHelpers.Edge> edges;
    private Mesh rawMesh;
    public EditableMesh (Mesh mesh)
    {
        rawMesh = mesh;
        edges = MeshEdgeHelpers.GetEdges(rawMesh.triangles);
        triangls = MeshEdgeHelpers.SetupTriangles(rawMesh.triangles, edges);
        BuildMesh();
    }

    private void BuildMesh()
    {
        if (!rawMesh)
            return;
        for (int i = 0; i < rawMesh.triangles.Length; i += 3)
        {
            Vertex[] v = new Vertex[] {
                new Vertex(rawMesh.vertices[rawMesh.triangles[i]], i),
                new Vertex(rawMesh.vertices[rawMesh.triangles[i + 1]], i + 1),
                new Vertex(rawMesh.vertices[rawMesh.triangles[i + 2]], i + 2)
            };
            //Check for redundant verts
            for (int q = 0; q < v.Length; q++)
            {
                bool exists = false;
                foreach (var item in vertices)
                {
                    if (item.Equals(v[q]))
                    {
                        v[q] = item;
                        exists = true;
                    }
                }
                if (!exists)
                    vertices.Add(v[q]);
            }

            Segment[] s = new Segment[]
            {
                new Segment(v[0], v[1]),
                new Segment(v[0], v[2]),
                new Segment(v[1], v[2])
            };
            //Check for redundant segments
            for (int x = 0; x < s.Length; x++)
            {
                bool exists = false;
                foreach (var seg in segments)
                {
                    if (s[x].Equals(seg))
                    {
                        s[x] = seg;
                        exists = true;
                    }
                }
                if (!exists)
                    segments.Add(s[x]);
            }
            /*
            //Build relations
            v[0].ConnectSegment(s[0]); v[0].ConnectSegment(s[1]);
            v[1].ConnectSegment(s[0]); v[1].ConnectSegment(s[2]);
            v[2].ConnectSegment(s[1]); v[2].ConnectSegment(s[2]);
            */
            /*
            //Check previously added triangles for redundant verts and segments
            for (int t = 0; t < triangles.Count; t++)
            {
                if (triangles.Count == 0)
                    break;
                //Check for redundant verts
                foreach (var tVert in triangles[t].verts)
                {
                    for (int x = 0; x < v.Length; x++)
                    {
                        if (v[x].Equals(tVert))
                        {
                            v[x] = tVert;
                            Debug.Log("redundant verte");
                        }
                    }
                }
                //Check for shared segments
                foreach (var seg in triangles[t].segments)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        if (s[y].Equals(seg))
                        {
                            //Shared segment
                            Debug.Log("shared seg");
                            seg.isShared = true;
                            s[y] = seg;
                            triangle.ConnectTrianlge(triangles[t]);
                            triangles[t].ConnectTrianlge(triangle);
                            continue;
                        }
                        //Look for connected segs
                        if (seg.Contains(v[y]))
                        {
                            seg.ConnectSegment(s[y]);
                            s[y].ConnectSegment(seg);
                        }
                    }
                }
            }
            */
            /*
            //Add segments to list
            for (int z = 0; z < s.Length; z++)
            {
                bool exists = false;
                foreach (var item in segments)
                {
                    if (item == s[z])
                    {
                        exists = true;
                        Debug.Log("seg not added");
                    }
                }
                if (!exists)
                    segments.Add(s[z]);
            }
            */
            Triangle triangle = new Triangle(v, s);
            triangles.Add(triangle);
        }

        //Debug.Log("vertex: " + vertices.Count);
        //Debug.Log("segments: "+ segments.Count);
        //Debug.Log("triangles: " + triangles.Count);
    }
    public Segment GetSegmentFromVerts(Vertex v1, Vertex v2)
    {
        foreach (var s in segments)
        {
            if (s.Contains(v1) && s.Contains(v2))
                return s;
        }
        return null;
    }
    public Triangle GetTriangleFromSegment(Segment segment)
    {
        foreach (var t in triangles)
        {
            if (t.Contains(segment))
                return t;
        }
        return null;
    }
    public Triangle GetTriangleFromVerts(Vertex v1, Vertex v2)
    {
        foreach (var t in triangles)
        {
            if (t.Contains(v1) && t.Contains(v2))
                return t;
        }
        return null;
    }
    public Segment GetNextSegmentInRing(Segment segment)
    {
        Triangle currentTriangle = GetTriangleFromSegment(segment);
        Triangle nextTriangle = null;
        foreach (var s in currentTriangle.segments)
        {
            if(s.isShared)
            {
                if (s != segment && s.t1 == currentTriangle)
                    nextTriangle = s.t2;
                else if (s != segment && s.t2 == currentTriangle)
                    nextTriangle = s.t1;
                else
                {
                    //Debug.Log("No connected triangles found; This shouldn't happen");
                    return null;
                }
                foreach (var seg in nextTriangle.segments)
                {
                    if (seg.isShared && seg != s)
                        return seg;
                }
            }
        }
        Debug.Log("Cannot find next segment in ring");
        return null;
    }
    public List<Segment> GetAllSegmentsInRing(Segment startingSegment)
    {
        List<Segment> result = new List<Segment>();
        Segment s = startingSegment;
        while (true)
        {
            s = GetNextSegmentInRing(s);
            if (s != null)
                result.Add(s);
            else
                break;
        }
        return result;
    }
}
