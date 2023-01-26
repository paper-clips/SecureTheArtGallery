using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// NOTE: MAKE SURE TO REMOVE ALL AFTER DONE USING LISTS!!!!!!!!!!

public class Graph
{
    public float x, y, z;
    //private List<Vector3> vertex;
    public List<Vector3> vertex;///
    public List<Vector2> edge;
    public List<Vector2> diagonal;
    public List<Vector3> verticesInOrder;///

    public int vertexSize() { return vertex.Count; }
    public int edgeSize() { return edge.Count; }
    public void toString() { Debug.Log("x: " + x + " | y: " + y + " | z: " + z); }  // Print the coordinates to the console
    public Vector3 getVertex(int i) { return vertex[i]; }
    public Vector2 getEdge(int i) { return edge[i]; }
    public List<Vector3> vertexList() { return vertex; }
    public List<Vector2> edgeList() { return edge; }
    public void removeEdges() { edge.Clear(); }

    // Create lists to store vertices and edges, separately
    public Graph()
    {
        vertex = new List<Vector3>();
        edge = new List<Vector2>();
        diagonal = new List<Vector2>();
    }

    // Add to vertex list (removes vertices)
    public void Vertex(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        int i = 0;
        Vector3 coordinate = new Vector3(x, y, z);
        bool isDuplicate = false;
        foreach (Vector3 v in vertex)
        {
            if (v.x == coordinate.x && v.y == coordinate.y && v.z == coordinate.z)
            {
                isDuplicate = true;
                vertex.RemoveAt(i);     // Remove vertices that aren't on corner
                break;
            }
            i++;
        }

        // Add vertex to list if it's not a duplicate
        if (isDuplicate == false)
            vertex.Add(coordinate);
    }

    // Add to vertex list (doesn't remove any vertices)
    public void Vertex(Vector3 vec)
    {
        this.x = vec.x;
        this.y = vec.y;
        this.z = vec.z;
        int i = 0;
        Vector3 coordinate = new Vector3(x, y, z);
        bool isDuplicate = false;
        foreach (Vector3 v in vertex)
        {
            if (v.x == coordinate.x && v.y == coordinate.y && v.z == coordinate.z)
            {
                isDuplicate = true;
                break;
            }
            i++;
        }

        // Add vertex to list if it's not a duplicate
        if (isDuplicate == false)
            vertex.Add(coordinate);
    }


    // Edge stores v1 and v2 number (not location)
    public void Edge(int v1, int v2)
    {
        Vector2 e = new Vector2(v1, v2);
        int isDuplicate = 0;

        // Check if not already added
        foreach (Vector2 t in edge)
        {
            if ((e.x == t.x && e.y == t.y) || (e.x == t.y && e.y == t.x))
            {
                isDuplicate = 1;
                break;
            }
        }

        if (isDuplicate == 0)
            edge.Add(e);
    }

    // Add edges in CCW order
    public void addEdges(MonotoneTriangulation mt)
    {
        // If have same z coordinate, add as edge on x-axis
        for (int i = 0; i < vertex.Count; i++)
        {
            for (int j = 0; j < vertex.Count; j++)
            {
                if (i != j)
                {
                    if (vertex[i].z == vertex[j].z)
                        Edge(i, j);
                }
            }
        }

        // Sort by y-axis (biggest to smallest)
        List<int> vSorted = new List<int>();
        for (int i = 0; i < vertex.Count; i++)
            vSorted.Add(i);

        // Sort by decreasing x-axis first then by decreasing x-axis if same x-axis
        vSorted = mt.Quicksort(this, vSorted, 0, vSorted.Count - 1);

        for (int i = 1; i < vSorted.Count; i++)
        {
            if (i % 2 != 0)
                Edge(vSorted[i - 1], vSorted[i]);
        }

    }

    public int getNeighbor(int vec, int prev)
    {
        foreach (Vector2 e in edge)
        {
            if (e.x == vec)
            {
                if (e.y != prev)
                    return (int)e.y;
            }
            else if (e.y == vec)
            {
                if (e.x != prev)
                    return (int)e.x;
            }
        }

        return -1;
    }

    public void sortCCWOrder(MonotoneTriangulation mt)
    {
        List<int> vSorted = new List<int>();
        int maxVertex = findMaxVertexIndex();   // Find left topmost vertex
        vSorted.Add(maxVertex);

        int left = -1;
        int right = -1;
        int v1 = -1;
        int v2 = -1;

        // Find neighbors of topmost vertex
        foreach (Vector2 e in edge)
        {
            if (e.x == maxVertex)
            {
                if (v1 == -1)
                    v1 = (int)e.y;
                else if (v2 == -1)
                    v2 = (int)e.y;
            }
            else if (e.y == maxVertex)
            {
                if (v1 == -1)
                    v1 = (int)e.x;
                else if (v2 == -1)
                    v2 = (int)e.x;
            }

            if (v1 != -1 && v2 != -1)
            {
                break;
            }
        }

        if (getVertex(v1).z < getVertex(v2).z)
        {
            left = v1;
            right = v2;
        }
        else
        {
            left = v2;
            right = v1;
        }

        vSorted.Add(left);

        // Add other vertices
        int prevVertex = maxVertex;
        int leftN = left;
        for (int i = 1; i < vertex.Count; i++)
        {
            if (leftN == right) // Stop from looping over again
                break;
            int tempVertex = leftN;
            leftN = getNeighbor(leftN, prevVertex);
            vSorted.Add(leftN);
            prevVertex = tempVertex;
        }

        // Change vertex order
        List<Vector3> temp = new List<Vector3>();
        foreach (int v in vSorted)
            temp.Add(getVertex(v));
        vertex = temp;

        // Fix edges (add in CCW Order)
        removeEdges();
        addEdges(mt);
    }

    // Find the vertex with the max z coordinate (break tie by x coordinate)
    public int findMaxVertexIndex()
    {
        const int MIN = -999999999;
        int maxVector = -1;                             // Vertex index of max
        Vector3 max = new Vector3(MIN, MIN, MIN);       // Vertex coordinates of max
        for (int i = 0; i < vertex.Count; i++)
        {
            if (vertex[i].z > max.z)
            {
                max = vertex[i];
                maxVector = i;
            }
            else if (vertex[i].z == max.z)    // Break tie by x (but actually y) coordinate
            {
                if (vertex[i].x < max.x)      // Leftmost
                {
                    max = vertex[i];
                    maxVector = i;
                }
            }
        }

        return maxVector;
    }

    // Add diagonal between 2 edges (not location)
    public void Diagonal(int v1, int v2)
    {
        diagonal.Add(new Vector2(v1, v2));

        // Remove duplicates
        foreach (Vector2 d in diagonal)
        {
            if ((d.x == v1 && d.y == v2) || (d.x == v2 && d.y == v1))
            {
                diagonal.Remove(d);
                break;
            }
        }
    }

    // Print the vertices + edges of the graph
    public void printGraph()
    {
        Debug.Log("Vertices:");
        for (int i = 0; i < vertex.Count; i++)
            Debug.Log("v" + i + ": " + vertex[i]);

        Debug.Log("Edges:");
        for (int i = 0; i < edge.Count; i++)
            Debug.Log("e" + i + ": " + edge[i]);
    }

}


public class ColorGraph
{
    private List<Vector3> triangles;
    private List<Vector2> edges;
    private List<Vector3> vertices;
    Dictionary<int, char> color;
    private int red, green, blue, noColor;
    public GameObject colorParent;

    // Constructor takes triangles (stores 3 vertices (names))
    public ColorGraph(List<Vector3> vertices, List<Vector2> edges, List<Vector3> triangles, GameObject colorParent)
    {
        this.colorParent = colorParent;
        this.triangles = new List<Vector3>(triangles);
        this.edges = new List<Vector2>(edges);
        this.vertices = new List<Vector3>(vertices);
        red = 0;
        green = 0;
        blue = 0;
        noColor = 0;

        // r, g, b or just n (none)
        color = new Dictionary<int, char>();

        // Add all vertices to dictionary with no color
        for (int i = 0; i < vertices.Count; i++)
        {
            color.Add(i, 'n');
        }
    }

    // Color vertices based on recursion
    public void coloring(int n, int total, Vector3 triangle, bool isStart)
    {
        Debug.Log("Coloring: " + triangle);

        // No more triangles
        if (n == 0)
            return;

        // First triangle
        if (isStart == true)
        {
            Debug.Log("n: " + n);
            Debug.Log(triangles.Count);
            color[(int)triangle.x] = 'r';
            color[(int)triangle.y] = 'g';
            color[(int)triangle.z] = 'b';

            triangles.Remove(triangle);
            Vector3 n1 = neighbors(new Vector3(triangle.x, triangle.y), triangle);
            if (n1.x != -1)
                coloring(triangles.Count, total, n1, false);
            Vector3 n2 = neighbors(new Vector3(triangle.y, triangle.z), triangle);
            if (n2.x != -1)
                coloring(triangles.Count, total, n2, false);
            Vector3 n3 = neighbors(new Vector3(triangle.x, triangle.z), triangle);
            if (n3.x != -1)
                coloring(triangles.Count, total, n3, false);

            triangles.Remove(triangle);
        }
        else
        {
            Debug.Log("Size of n: " + n);
            colorVertex(triangle);
            triangles.Remove(triangle);

            Vector3 n1 = neighbors(new Vector3(triangle.x, triangle.y), triangle);
            if (n1.x != -1)
                coloring(triangles.Count, total, n1, false);
            Vector3 n2 = neighbors(new Vector3(triangle.y, triangle.z), triangle);
            if (n2.x != -1)
                coloring(triangles.Count, total, n2, false);
            Vector3 n3 = neighbors(new Vector3(triangle.x, triangle.z), triangle);
            if (n3.x != -1)
                coloring(triangles.Count, total, n3, false);
        }

        return;     // Remove?
    }

    public void colorVertex(Vector3 triangle)
    {
        int v1, v2, v3;
        char col = 'n';
        if (color[(int)triangle.x] == 'n')
        {
            v1 = color[(int)triangle.x];
            v2 = color[(int)triangle.y];
            v3 = color[(int)triangle.z];
        }
        else if (color[(int)triangle.y] == 'n')//
        {
            v1 = color[(int)triangle.y];
            v2 = color[(int)triangle.x];
            v3 = color[(int)triangle.z];
        }
        else if (color[(int)triangle.z] == 'n')
        {
            v1 = color[(int)triangle.z];
            v2 = color[(int)triangle.y];
            v3 = color[(int)triangle.x];
        }
        else
        {
            Debug.Log("OH NO PROBLEM!");
            v1 = color[(int)triangle.x];
            v2 = color[(int)triangle.y];
            v3 = color[(int)triangle.z];
        }

        // Color the other side of the triangles (based on the given 2 vertices)
        if ((v2 == 'r' && v3 == 'g') || (v2 == 'g' && v3 == 'r'))
            col = 'b';
        else if ((v2 == 'g' && v3 == 'b') || (v2 == 'b' && v3 == 'g'))
            col = 'r';
        else if ((v2 == 'r' && v3 == 'b') || (v2 == 'b' && v3 == 'r'))
            col = 'g';

        // Change color of vertex
        if (v1 == color[(int)triangle.x])
            color[(int)triangle.x] = col;
        else if (v1 == color[(int)triangle.y])
            color[(int)triangle.y] = col;
        else if (v1 == color[(int)triangle.z])
            color[(int)triangle.z] = col;
    }

    // Count number of each color
    public void countColors()
    {
        // Count number of colors for each vertex
        foreach (char k in color.Values)
        {
            if (k == 'r')
                red++;
            else if (k == 'g')
                green++;
            else if (k == 'b')
                blue++;
            else if (k == 'n')
                noColor++;
        }

        Debug.Log("Red: (" + red + ") Green: (" + green + ") Blue: (" + blue + ") No color: (" + noColor + ")");
    }

    // Get minimum number of cameras based on minimum number of colors
    public List<int> getMinCameraVertices()
    {
        List<int> minVertices = new List<int>();
        char col = 'n';
        if (red <= green && red <= blue)
            col = 'r';
        else if (green <= red && green <= blue)
            col = 'g';
        else if (blue <= green && blue <= red)
            col = 'b';

        for (int i = 0; i < vertices.Count; i++)
        {
            if (color[i] == col)
                minVertices.Add(i);
        }

        return minVertices;
    }

    // Get neighbor triangle (3 possible neighbors)
    public Vector3 neighbors(Vector2 edge, Vector3 currTriangle)
    {
        Vector3 neighbor = new Vector3(-1, -1, -1);
        foreach (Vector3 t in triangles)
        {
            // Triangle neighbors
            Vector2 t1_e1 = new Vector2(t.x, t.y);
            Vector2 t1_e2 = new Vector2(t.y, t.z);
            Vector2 t1_e3 = new Vector2(t.x, t.z);

            // Make sure not same triangle as the one currently testing
            if (!(t.x == currTriangle.x && t.y == currTriangle.y && t.z == currTriangle.z))
            {
                // Add neighboring triangles
                if ((t1_e1.x == edge.x && t1_e1.y == edge.y) || (t1_e1.x == edge.y && t1_e1.y == edge.x))
                {
                    neighbor = t;
                    break;
                }
                else if ((t1_e2.x == edge.x && t1_e2.y == edge.y) || (t1_e2.x == edge.y && t1_e2.y == edge.x))
                {
                    neighbor = t;
                    break;
                }
                else if ((t1_e3.x == edge.x && t1_e3.y == edge.y) || (t1_e3.x == edge.y && t1_e3.y == edge.x))
                {
                    neighbor = t;
                    break;
                }
            }
        }

        // Return the neighbor triangle
        return neighbor;
    }

    // Print the colors of the vertices + show color
    public void printColors(Graph g)
    {
        int i = 0;
        Debug.Log("Colors:");
        foreach (char k in color.Values)
        {
            /*
            ////////////////////////////
            GameObject v = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (k == 'r')
            {
                v.GetComponent<Renderer>().material.color = Color.red;
                v.name = "Red";
                v.tag = "Color";
                v.transform.SetParent(colorParent.transform, false);
            }
            else if (k == 'g')
            {
                v.GetComponent<Renderer>().material.color = Color.green;
                v.name = "Green";
                v.tag = "Color";
                v.transform.SetParent(colorParent.transform, false);
            }
            else if (k == 'b')
            {
                v.GetComponent<Renderer>().material.color = Color.blue;
                v.name = "Blue";
                v.tag = "Color";
                v.transform.SetParent(colorParent.transform, false);
            }

            Vector3 pos = g.getVertex(i);
            v.transform.localPosition = new Vector3(pos.x, pos.y + 15, pos.z);
            v.transform.localScale = new Vector3(0.2f, 25f, 0.2f);
            ////////////////////////////
            ///*/

            Debug.Log("v" + i + ": " + k);
            i++;
        }
    }
}

class InfinityLoopException : System.Exception
{
    public InfinityLoopException()
    {
        Debug.Log(">>>Infinity Loop Detected<<<");
    }
}

public class EarBasedTriangulation
{
    private List<int> vs;
    private List<Vector3> vertices;     // Expects them in order 0->n (topmost->rightmost)  
    private Graph g;
    private List<Vector3> triangles;

    public List<Vector3> getTriangles() { return triangles; }
    public int trianglesSize() { return triangles.Count; }

    // Add vertices in order
    public EarBasedTriangulation(Graph g, List<Vector3> vertices)
    {
        this.g = g;
        this.vertices = vertices;
        triangles = new List<Vector3>();
        vs = new List<int>();
        for (int i = 0; i < vertices.Count; i++)
            vs.Add(i);
    }

    // Sometimes has issues (freezes Unity)
    // Could be due to n-- never reaching and becoming an infinite loop
    public void triangulation()
    {
        int index = 0;
        int n = vertices.Count;

        while (n > 3)
        {
            int l = vs[(index + n + 1) % n];
            int m = vs[(index + n) % n];
            int r = vs[(index + n - 1) % n];
            Debug.Log("l=" + l + " m=" + m + " r=" + r);

            // Loop issue: never n-- if none of them are interior
            if (isInterior((index + n + 1) % n, (index + n) % n, (index + n - 1) % n, n))
            {
                vs.Remove(m);
                Debug.Log("Removing v" + m + " | Diagonal: (" + l + ", " + r + ") ");
                triangles.Add(new Vector3(l, m, r));
                n--;
                index--;
            }

            index++;
            if (index > 300)    // Remove later
            {
                throw new InfinityLoopException();
            }
        }

        // Add last triangle
        if (triangles.Count > 0)
            triangles.Add(new Vector3(vs[0], vs[1], vs[2]));
    }

    // Add diagonal + info to scene
    public void createDiagonals(int v1, int v2)
    {
        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.SetPosition(0, g.getVertex(v1));
        line.SetPosition(1, g.getVertex(v2));
        line.tag = "Diagonal";
        line.name = "d (" + v1 + ", " + v2 + ")";
    }

    public bool isInterior(int vi, int v2, int vj, int n)
    {
        // Cross product: x1 * y2 - y1 * x2
        int viB = vs[(vi + n - 1) % n];
        int viA = vs[(vi + n + 1) % n]; // or v2?

        Vector2 e_i1 = new Vector2(vs[vi], vs[v2]);   // Edge immediately before vi
        Vector2 e_i2 = new Vector2(vs[v2], vs[vj]);   // Edge immediately after vi

        Debug.Log("e_i1: " + e_i1 + " e_i2: " + e_i2);

        e_i1 = new Vector2(g.getVertex((int)e_i1.y).x - g.getVertex((int)e_i1.x).x, g.getVertex((int)e_i1.y).z - g.getVertex((int)e_i1.x).z);
        e_i2 = new Vector2(g.getVertex((int)e_i2.y).x - g.getVertex((int)e_i2.x).x, g.getVertex((int)e_i2.y).z - g.getVertex((int)e_i2.x).z);

        Vector2 diagonal = new Vector2(g.getVertex(vs[vj]).x - g.getVertex(vs[vi]).x, g.getVertex(vs[vj]).z - g.getVertex(vs[vi]).z);


        if (e_i1.x * e_i2.y - e_i1.y * e_i2.x > 0) // Convex
        {
            Debug.Log("Convex1");

            if ((e_i1.x * diagonal.y - e_i1.y * diagonal.x <= 0) || (e_i2.x * diagonal.y - e_i2.y * diagonal.x <= 0))
            {
                Debug.Log("Exterior at m (convex) / v2: " + vs[v2]);
                return false;
            }
            else
            {
                if ((e_i1.x * diagonal.y - e_i1.y * diagonal.x <= 0) && (e_i2.x * diagonal.y - e_i2.y * diagonal.x <= 0))
                {
                    Debug.Log("Exterior at m (reflex) / v2: " + vs[v2]);
                    return false;
                }
            }
        }

        // Switch vi and vj?
        foreach (Vector2 e in g.edge)
        {
            if (vs[vi] == e.x || vs[vi] == e.y  || vs[vj] == e.x || vs[vj] == e.y)
            {
                // Debug.Log("CONTONUING");
                continue;
            }
            Debug.Log("vi: " + vi + " vj: " + vj + " v2: " + v2 + " e.x: " + e.x + " e.y: " + e.y);

            if (isIntersecting(g.getVertex((int)e.y), g.getVertex((int)e.x), g.getVertex(vs[vi]), g.getVertex(vs[vj])))
            {
                Debug.Log("vi: " + vi + " vj: " + vj + " v2: " + v2 + " e.x: " + e.x + " e.y: " + e.y);
                Debug.Log("INTERSECTING!");
                return false;
            }
        }
        
        //createDiagonals(vs[vi], vs[vj]);

        return true;

    }

    class IntersectionException : System.Exception
    {
        public IntersectionException()
        {
            Debug.Log(">>>Intersection not part of algorithm<<<");
        }
    }

    // Check whether edge intersects with doaginal
    public bool isIntersecting(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {
        double a = q1.x - p1.x;
        double b = q1.z - p1.z;

        double c = q2.x - p2.x;
        double d = q2.z - p2.z;

        double e = p2.x - p1.x;
        double f = p2.z - p1.z;

        if (a * d - b * c == 0)
        {
            // Parallel
            if (a * f - b * e != 0)
                return false;

            double t_p = (p2.x - p1.x) / a;
            double t_q = (q2.x - p1.x) / a;
            double l = Math.Max(Math.Min(t_p, t_q), 0);
            double r = Math.Min(Math.Max(t_p, t_q), 1);

            if (l > r)
                return false;
            else if (l == r)
                return true;
            else
                return true;
        }
        else
        {
            double t1 = -((c * f - d * e) / (a * d - b * c));
            double t2 = -((a * f - b * e) / (a * d - b * c));

            if ((0 <= t1 && t1 <= 1) && (0 <= t2 && t2 <= 1))
            {
                Debug.Log("+++ I-3 | t1: " + t1 + " t2: " + t2);
                    return true;
            }
            else
                return false;
        }
    }
}

public class AddUnityComponents
{
    private List<GameObject> vertices;
    public List<GameObject> walls;
    private List<GameObject> cameras;
    public List<GameObject> planes;
    private List<GameObject> artPieces;

    private GameObject planeParent;
    private GameObject vertexParent;
    private GameObject edgeParent;
    private GameObject cameraParent;
    private GameObject artPieceParent;
    private Material floorMaterial;
    private Material wallMaterial;
    private GameObject window;

    public AddUnityComponents(GameObject vertexParent, GameObject edgeParent, GameObject planeParent, GameObject cameraParent, GameObject artPieceParent, Material floorMaterial, Material wallMaterial, GameObject window)
    {
        vertices = new List<GameObject>();
        walls = new List<GameObject>();
        cameras = new List<GameObject>();
        artPieces = new List<GameObject>();
        planes = new List<GameObject>();

        this.planeParent = planeParent;
        this.vertexParent = vertexParent;
        this.edgeParent = edgeParent;
        this.cameraParent = cameraParent;
        this.artPieceParent = artPieceParent;
        this.floorMaterial = floorMaterial;
        this.wallMaterial = wallMaterial;
        this.window = window;
    }

    // Add plane on left, right, or above current plane
    public Vector3 addPlane(int r, int i, Vector3 prevPlane, int pSize)
    {
        // Direction where new plane will be added
        string direction = "";
        switch (r)
        {
            case 0:
                direction = "Left";
                break;
            case 1:
                direction = "Top";
                break;
            case 2:
                direction = "Right";
                break;
        }

        // Add new plane + set name + add to folder
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(pSize / 10, pSize / 10, pSize / 10);
        plane.name = "Plane" + i;
        plane.transform.SetParent(planeParent.transform, false);
        plane.GetComponent<MeshRenderer>().material = floorMaterial;
        plane.tag = "Grid";

        Vector3 location = new Vector3(0, 0, 0);

        // Add new plane to specified direction
        if (direction == "Left")
        {
            Vector3 c = new Vector3(prevPlane.x - pSize, prevPlane.y, prevPlane.z);
            plane.transform.localPosition = new Vector3(c.x, c.y, c.z);
            location = plane.transform.localPosition;
        }
        else if (direction == "Right")
        {
            Vector3 c = new Vector3(prevPlane.x + pSize, prevPlane.y, prevPlane.z);
            plane.transform.localPosition = new Vector3(c.x, c.y, c.z);
            location = plane.transform.localPosition;
        }
        else if (direction == "Top")
        {
            Vector3 c = new Vector3(prevPlane.x, prevPlane.y, prevPlane.z + pSize);
            plane.transform.localPosition = new Vector3(c.x, c.y, c.z);
            location = plane.transform.localPosition;
        }

        planes.Add(plane);

        return location;
    }

    // Add vertex + info to scene
    public void createVertices(Graph g, int i)
    {
        GameObject v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        v.GetComponent<Renderer>().material.color = Color.red;
        v.name = "v" + i;
        v.tag = "Vertex";
        Vector3 pos = g.getVertex(i);
        //Debug.Log(pos.x + " " + pos.y + " " + pos.z);
        v.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
        v.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        v.transform.SetParent(vertexParent.transform, false);

        //vertices.Add(v);
    }

    /*
    // Add edge + info to scene
    public void createEdges(Graph g, int i, int pSize)
    {
        GameObject e = GameObject.CreatePrimitive(PrimitiveType.Cube);
        e.GetComponent<Renderer>().material.color = Color.white;
        e.transform.SetParent(edgeParent.transform, false);
        e.GetComponent<MeshRenderer>().material = wallMaterial;
        e.tag = "Edge";
        e.name = "e" + i;

        Vector2 edgeNum = g.getEdge(i);
        Vector3 v1 = g.getVertex((int)edgeNum.x);
        Vector3 v2 = g.getVertex((int)edgeNum.y);

        if (v1.x == v2.x)
        {
            float dif = Math.Abs(v1.z - v2.z);
            e.transform.localScale = new Vector3(0.5f, 20f, dif + 0.5f);  // Vertical

            if (v1.z < v2.z)
                e.transform.localPosition = new Vector3(v1.x, v1.y + 10, v1.z + (dif / 2));
            else
                e.transform.localPosition = new Vector3(v1.x, v1.y + 10, v1.z - (dif / 2));
        }
        else
        {
            float dif = Math.Abs(v1.x - v2.x);
            e.transform.localScale = new Vector3(dif + 0.5f, 20f, 0.5f);  // Vertical

            if (v1.x < v2.x)
                e.transform.localPosition = new Vector3(v1.x + (dif / 2), v1.y + 10, v1.z);
            else
                e.transform.localPosition = new Vector3(v1.x - (dif / 2), v1.y + 10, v1.z);
        }

        walls.Add(e);

    }
    */

    // Add edge + info to scene
    public int createEdges(Graph g, Vector3 v1, Vector3 v2, int i, int pSize, GameObject wallWithWindowLR, GameObject wallWithWindowFB, int pCounter, GameObject wallWithPaintingLR)
    {
        GameObject e;

        if (v1.x == v2.x)
        {
            if (i % 5 == 0)
                e = GameObject.Instantiate(wallWithWindowLR);
            else if (i % 4 == 0 && pCounter < 5)
            {
                e = GameObject.Instantiate(wallWithPaintingLR);
                pCounter = pCounter + 1;
            }
            else 
                e = GameObject.CreatePrimitive(PrimitiveType.Cube);

            float dif = Math.Abs(v1.z - v2.z);
            e.transform.localScale = new Vector3(0.5f, 20f, dif + 0.5f);  // Vertical

            if (v1.z < v2.z)
                e.transform.localPosition = new Vector3(v1.x, v1.y + 10, v1.z + (dif / 2));
            else
                e.transform.localPosition = new Vector3(v1.x, v1.y + 10, v1.z - (dif / 2));
        }
        else
        {
            if (i % 5 == 0)
                e = GameObject.Instantiate(wallWithWindowFB);
            //else if (i % 5 == 0 && pCounter < 7)
            //{
            //    e = GameObject.Instantiate(wallWithPaintingFB);
            //    pCounter = pCounter + 2;
            //}
            else
                e = GameObject.CreatePrimitive(PrimitiveType.Cube);
            float dif = Math.Abs(v1.x - v2.x);
            e.transform.localScale = new Vector3(dif + 0.5f, 20f, 0.5f);  // Vertical

            if (v1.x < v2.x)
                e.transform.localPosition = new Vector3(v1.x + (dif / 2), v1.y + 10, v1.z);
            else
                e.transform.localPosition = new Vector3(v1.x - (dif / 2), v1.y + 10, v1.z);
        }
        e.GetComponent<Renderer>().material.color = Color.white;
        e.transform.SetParent(edgeParent.transform, false);
        e.GetComponent<MeshRenderer>().material = wallMaterial;
        e.tag = "Grid";
        e.name = "e" + i;

        Debug.Log("painting counter: " + pCounter);
        walls.Add(e);
        return pCounter;
    }

    // Not actual cameras, placeholder spheres
    public void addCameras(Graph g, int cNum, int i, int pSize)
    {
        GameObject v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        v.GetComponent<Renderer>().material.color = Color.black;
        v.name = "c" + i;
        v.tag = "Hide";
        Vector3 pos = g.getVertex(cNum);
        v.transform.localPosition = new Vector3(pos.x, pos.y + 20f, pos.z); // Changed from 20.3f
        v.transform.localScale = new Vector3(1f, 1f, 1f);
        v.transform.SetParent(cameraParent.transform, false);

        //cameras.Add(v);
    }

    public GameObject createArtPiece(Graph g, List<Vector3> tri, Vector2 triangleArea)
    {
        Vector3 v1 = g.getVertex((int)tri[(int)triangleArea.x].x);
        Vector3 v2 = g.getVertex((int)tri[(int)triangleArea.x].y);
        Vector3 v3 = g.getVertex((int)tri[(int)triangleArea.x].z);
        Vector3 center = new Vector3((v1.x + v2.x + v3.x) / 3, (v1.y + v2.y + v3.y) / 3, (v1.z + v2.z + v3.z) / 3);

        GameObject v = GameObject.CreatePrimitive(PrimitiveType.Cube);
        v.GetComponent<Renderer>().material.color = Color.white;
        v.name = "a" + triangleArea.x;
        v.tag = "ArtPiece";
        v.transform.localPosition = new Vector3(center.x, center.y + 2, center.z);
        v.transform.localScale = new Vector3(2f, 4f, 2f);
        v.transform.SetParent(artPieceParent.transform, false);

        return v;
    }
}

public class MonotoneTriangulation
{
    private List<Vector3> vertices;

    // Constructor takes list of vertices
    public MonotoneTriangulation(List<Vector3> vertices) { this.vertices = vertices; }

    // Prints all of the vertices + coordinates
    public void printVertices()
    {
        for (int i = 0; i < vertices.Count; i++)
            Debug.Log("v" + i + ": (" + vertices[i].x + ", " + vertices[i].y + ", " + vertices[i].z + ") ");
    }

    // Find the vertex with the max z coordinate (break tie by x coordinate)
    public int findMaxVertexIndex()
    {
        const int MIN = -999999999;
        int maxVector = -1;                             // Vertex index of max
        Vector3 max = new Vector3(MIN, MIN, MIN);       // Vertex coordinates of max
        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i].z > max.z)
            {
                max = vertices[i];
                maxVector = i;
            }
            else if (vertices[i].z == max.z)    // Break tie by x (but actually y) coordinate
            {
                if (vertices[i].x < max.x)      // Leftmost
                {
                    max = vertices[i];
                    maxVector = i;
                }
            }
        }

        return maxVector;
    }

    // Quicksort algorithm from the textbook "Introduction to Algorithms," Third Edition, pg. 171
    public List<int> Quicksort(Graph g, List<int> A, int p, int r)
    {
        if (p < r)
        {
            int q = Partition(g, A, p, r);
            Quicksort(g, A, p, q - 1);
            Quicksort(g, A, q + 1, r);
        }
        return A;
    }

    // Part of the Quicksort algorithm, also from the textbook
    // Rearanges the subarray A[p..r] in place
    public int Partition(Graph g, List<int> A, int p, int r)
    {
        Vector3 x = g.getVertex((int)A[r]);
        int temp;

        int i = p - 1;

        for (int j = p; j <= r - 1; j++)
        {
            if (g.getVertex(A[j]).x > x.x)      // Sort by x-vertices first (max-min)
            {
                i = i + 1;

                // Swap A[i] with A[j]
                temp = A[i];
                A[i] = A[j];
                A[j] = temp;
            }

            else if (g.getVertex(A[j]).x == x.x && g.getVertex(A[j]).z > x.z)   // Sort by z-axis (max-min) if same x-axis
            {
                i = i + 1;

                // Swap A[i] with A[j]
                temp = A[i];
                A[i] = A[j];
                A[j] = temp;
            }

        }

        // Swap A[i+1] with A[r]
        temp = A[i + 1];
        A[i + 1] = A[r];
        A[r] = temp;

        return i + 1;
    }
}