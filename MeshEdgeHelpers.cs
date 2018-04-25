using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshEdgeHelpers {
    public struct Edge
    {
        public int v1;
        public int v2;
        public int ID;
        public int triangleIndex;
        public Edge(int aV1, int aV2, int id, int triID)
        {
            v1 = aV1;
            v2 = aV2;
            ID = id;
            triangleIndex = triID;
        }
        public void SetID(int id)
        {
            ID = id;
        }
    }

    public struct Triangle
    {
        public int v1, v2, v3, triID;
        public Edge e1, e2, e3;
        public Triangle(int vertex1, int vertex2, int vertex3,Edge[] edges, int ID)
        {
            v1 = vertex1;
            v2 = vertex2;
            v3 = vertex3;
            //TODO remove edges for triangles. They are useless
            //e1 = new Edge(v1, v2, 0, ID);
            //e2 = new Edge(v1, v3, 0, ID);
            //e3 = new Edge(v2, v3, 0, ID);
            e1 = edges[0];
            e2 = edges[1];
            e3 = edges[2];
            triID = ID;
        }
        public bool Contains(Edge edge)
        {
           // Debug.Log( string.Format( "e1: {0} , e2: {1} , e3 {2}", e1.ID, e2.ID, e3.ID));
            if (edge.ID == e1.ID ||
                edge.ID == e2.ID ||
                edge.ID == e3.ID)
                return true;

            return false;
        }
        public bool Contains(int vertex)
        {
            if (vertex == v1 || vertex == v2 || vertex == v3)
                return true;

            return false;
        }
    }


    public static List<Edge> GetEdges(int[] aIndices)
    {
        List<Edge> result = new List<Edge>();
        
        for (int i = 0; i < aIndices.Length; i += 3)
        {
            int v1 = aIndices[i];
            int v2 = aIndices[i + 1];
            int v3 = aIndices[i + 2];
            int id1 = i;
            int id2 = i + 1;
            int id3 = i + 2;
            for (int n = 0; n < result.Count; n++)
            {
                if ((result[n].v1 == v1 || result[n].v1 == v2) &&
                    (result[n].v2 == v1 || result[n].v2 == v2))
                {
                    id1 = result[n].ID;
                    continue;
                }
                    
                if ((result[n].v1 == v2 || result[n].v1 == v3) &&
                    (result[n].v2 == v2 || result[n].v2 == v3))
                {
                    id2 = result[n].ID;
                    continue;
                }
                if ((result[n].v1 == v3 || result[n].v1 == v1) &&
                    (result[n].v2 == v3 || result[n].v2 == v1))
                {
                    id3 = result[n].ID;
                    continue;
                }
            }
            result.Add(new Edge(v1, v2, id1, i));
            result.Add(new Edge(v2, v3, id2, i));
            result.Add(new Edge(v3, v1, id3, i));
        }

        SetupEdgeId(result);
        return result;
    }
    /*
    public static List<Triangle> GetTriangles(int[] indices)
    {
        List<Triangle> result = new List<Triangle>();
        for (int i = 0; i < indices.Length; i += 3)
        {
            int v1 = indices[i];
            int v2 = indices[i + 1];
            int v3 = indices[i + 2];
            result.Add(new Triangle(v1, v2, v3, i));
        }
        return result;
    }
    */
    public static List<Triangle> SetupTriangles(int[] indices, List<Edge> edges)
    {
        List<Triangle> result = new List<Triangle>();
        for (int i = 0; i < indices.Length; i += 3)
        {
            int v1 = indices[i];
            int v2 = indices[i + 1];
            int v3 = indices[i + 2];
            Edge[] e = new Edge[3];
            e[0] = edges[i]; e[1] = edges[i + 1]; e[2] = edges[i + 2];
            result.Add(new Triangle(v1, v2, v3, e, i));
        }
        return result;
    }

    public static Edge GetEdgeFromVerts(Mesh mesh, int v1, int v2)
    {
        List<Edge> edges = GetEdges(mesh.triangles);
        for (int i = 0; i < edges.Count; i++)
        {
            if((edges[i].v1 == v1 || edges[i].v2 == v1) && (edges[i].v1 == v2 || edges[i].v2 == v2))
            {
                return edges[i];
            }
        }
        Edge e = new Edge(v1,v2, 0, 0); return e;
    }
    /*
    public static Triangle GetTriFromEdge(Mesh mesh, Edge edge)
    {
        Triangle tri = new Triangle();
        List<Triangle> triangles = GetTriangles(mesh.triangles);
        for (int i = 0; i < triangles.Count; i++)
        {
            if (edge.triangleIndex == triangles[i].triID)
                return triangles[i];
        }
        Debug.Log("no tris found");
        return tri;
    }
    */
    public static Triangle GetTriFromEdge(List<Triangle> triangles, Edge edge)
    {
        Triangle tri = new Triangle();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (edge.triangleIndex == triangles[i].triID)
                return triangles[i];
        }
        Debug.Log("no tris found");
        return tri;
    }

    public static List<Triangle> FindConnectedTriangles(List<Triangle> triangles, Edge currentEdge)
    {
        List<Triangle> result = new List<Triangle>();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i].Contains(currentEdge))
            {
                result.Add(triangles[i]);
            }
        }
        Debug.Log("Edge "+currentEdge.ID + " has " + result.Count + " connected triangles");
        return result;
    }
    
    public static List<Triangle> FindConnectedTriangles(List<Triangle> triangles, Triangle triangle)
    {
        List<Triangle> result = new List<Triangle>();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangle.Contains(triangles[i].e1) ||
                triangle.Contains(triangles[i].e2) ||
                triangle.Contains(triangles[i].e3))
            {
                result.Add(triangles[i]);
            }
        }
        return result;
    }

    public static Edge FindNextEdgeInRing(List<Triangle> triangles, Edge currentEdge)
    {
        Triangle t1 = new Triangle();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i].Contains(currentEdge))
            {
                t1 = triangles[i];
            }
        }
        Edge e = new Edge();
        return e;

    }

    public static List<Edge> SortEdges(this List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = 0; i < result.Count - 2; i++)
        {
            Edge E = result[i];
            for (int n = i + 1; n < result.Count; n++)
            {
                Edge a = result[n];
                if (E.v2 == a.v1)
                {
                    // in this case they are already in order so just continoue with the next one
                    if (n == i + 1)
                        break;
                    // if we found a match, swap them with the next one after "i"
                    result[n] = result[i + 1];
                    result[i + 1] = a;
                    break;
                }
            }
        }
        return result;
    }

    public static List<Edge> FindBoundary(List<Edge> aEdges)
    {
        List<Edge> result = new List<Edge>(aEdges);
        for (int i = result.Count - 1; i > 0; i--)
        {
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                {
                    // shared edge so remove both
                    result.RemoveAt(i);
                    result.RemoveAt(n);
                    i--;
                    break;
                }
            }
        }
        return result;
    }

    public static List<Edge> SetupEdgeId(List<Edge> aEdges)
    {
        List<Edge> result = aEdges;
        int id = 0;
        for (int i = result.Count - 1; i > 0; i--)
        {
            result[i].SetID(i);
            for (int n = i - 1; n >= 0; n--)
            {
                if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                {
                    // shared edge so remove both
                    //result.RemoveAt(i);
                    result[n].SetID(i);
                    i--;
                    break;
                }
            }
        }
        return result;
    }

}
