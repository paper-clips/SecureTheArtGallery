using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FindVertices : MonoBehaviour
{
    public GameObject vertexParent;
    public GameObject edgeParent;
    public GameObject planeParent;
    public GameObject colorParent;
    public GameObject cameraParent;
    public GameObject artPieceParent;
    public GameObject cam0;
    public List<Vector3> cameraPosition = new List<Vector3>();
    public int cameraNumber;
    public int totalCameras;
    public bool isRoomGenerated;
    public List<GameObject> artPiecesList = new List<GameObject>();
    public Material floorMaterial;
    public Material wallMaterial;
    public GameObject vase1;
    public List<GameObject> walls;
    public GameObject startCeilingPlane;
    bool isStopped;
    public GameObject startScreen;
    public int totalArtPieces;
    public GameObject window;
    public List<int> randomSeed;
    bool isInput;
    bool isStopping;
    int randCounter;
    public TMP_InputField seedCodeText;
    public GameObject openingScreenScript;
    bool wasStartClicked;
    bool isGo;
    bool isFunctStopped;
    public GameObject wallWithWindowLR;
    public GameObject wallWithWindowFB;
    public GameObject wallWithWindowLR_painting1;
    public GameObject wallWithWindowLR_painting5;
    public GameObject wallWithWindowLR_painting2;
    public GameObject wallWithWindowLR_painting6;
    public GameObject wallWithWindowLR_painting3;
    public GameObject wallWithWindowLR_painting7;
    public GameObject wallWithWindowLR_painting4;
    public GameObject wallWithWindowLR_painting8;

    const int SIZE = 10;     // Number of planes
    // Note: Debug.Log("Hello", gameObject); // This will highlight the object in the scene if you click on it in the debug window

    // Start is called before the first frame update
    void Start()
    {
        isInput = false;
        isStopping = false;
        randCounter = 0;
        isStopped = false;
        isRoomGenerated = false;
        randomSeed = new List<int>();
        wasStartClicked = false;
        isGo = false;
        isFunctStopped = false;
    }

    // Quicksort algorithm from the textbook "Introduction to Algorithms," Third Edition, pg. 171
    public List<Vector2> Quicksort(List<Vector2> A, int p, int r)
    {
        if (p < r)
        {
            int q = Partition(A, p, r);
            Quicksort(A, p, q - 1);
            Quicksort(A, q + 1, r);
        }
        return A;
    }

    // Part of the Quicksort algorithm, also from the textbook
    // Rearanges the subarray A[p..r] in place
    public int Partition(List<Vector2> A, int p, int r)
    {
        Vector2 x = A[r];
        Vector2 temp;

        int i = p - 1;

        for (int j = p; j <= r - 1; j++)
        {
            if (A[j].y >= x.y)      // Sort by max area
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

    // Update is called once per frame
    void Update()
    {
        wasStartClicked = openingScreenScript.GetComponent<IntroToGame>().wasStartClicked;
        if (!String.IsNullOrWhiteSpace(seedCodeText.text) && wasStartClicked == true && isFunctStopped == false)
        {
            isInput = true;

            for(int i = 0; i < 10; i++)
            {
                randomSeed.Add(int.Parse(seedCodeText.text[i].ToString()));
            }

            isFunctStopped = true;
        }
        else if (String.IsNullOrWhiteSpace(seedCodeText.text) && wasStartClicked == true && isFunctStopped == false)
        {
            isInput = false;

            isFunctStopped = true;
        }

        bool isGameStarting = startScreen.GetComponent<IntroToGame>().wasStartClicked;      // Get variable from other script
        if (isGameStarting == true && isStopped == false)
        {
            isStopped = true;
            isRoomGenerated = true;
        }

        if (isGameStarting == true && isStopping == false)
        {
            Debug.Log("Generating Room...");

            // Store the middle position of the plane (scale: 1 x 1 x 1)
            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;

            // Create objects from classes
            Graph g = new Graph();

            // Used to add objects on screen
            AddUnityComponents auc = new AddUnityComponents(vertexParent, edgeParent, planeParent, cameraParent, artPieceParent, floorMaterial, wallMaterial, window);
            int pSize = 10; // Size of plane (only do 10 for now)

            // Starting plane location: (0, 0, 0), Scale: 10
            g.Vertex(x - (pSize / 2), y, z - (pSize / 2));  // Lower left   (-5, 0, -5)
            g.Vertex(x + (pSize / 2), y, z - (pSize / 2));  // Lower right   (5, 0, -5)
            g.Vertex(x + (pSize / 2), y, z + (pSize / 2));  // Upper right   (5, 0, 5)
            g.Vertex(x - (pSize / 2), y, z + (pSize / 2));  // Upper left  (-5, 0, 5)

            Vector3 prevPlane = new Vector3(0, 0, 0);       // Store previous plane location 
            int planeNum = 1;
            int prevDirection = -1;
            int rand;

            int minRand = 2;
            int maxRand = 6;
            // Randomly generate room (using planes)
            for (int i = 1; i < SIZE; i++)
            {
                // First layer
                if (prevDirection == -1)
                {
                    if (isInput == true)
                    {
                        rand = randomSeed[randCounter];
                        randCounter++;
                    }
                    else
                        rand = Random.Range(1, 3); // Pick option 1 or 2 (1=Left, 2=Right for now)
                    randomSeed.Add(rand);
                    if (rand == 1)      // Left
                    {
                        if (isInput == true)
                        {
                            rand = randomSeed[randCounter];
                            randCounter++;
                        }
                        else
                            rand = Random.Range(minRand, maxRand);
                        randomSeed.Add(rand);

                        int currPlaneNum = planeNum;
                        for (int j = currPlaneNum; j < currPlaneNum + rand; j++)
                        {
                            prevPlane = auc.addPlane(2, j, prevPlane, pSize);

                            // Add vertices
                            Vector3 c = prevPlane;
                            g.Vertex(c.x - (pSize / 2), c.y, c.z - (pSize / 2)); // Lower left
                            g.Vertex(c.x + (pSize / 2), c.y, c.z - (pSize / 2)); // Lower right
                            g.Vertex(c.x - (pSize / 2), c.y, c.z + (pSize / 2)); // Upper left
                            g.Vertex(c.x + (pSize / 2), c.y, c.z + (pSize / 2)); // Upper right

                            planeNum++;
                        }
                        prevDirection = 2;  // Keep track of which direction these planes were added
                    }
                    else    // Right
                    {
                        if (isInput == true)
                        {
                            rand = randomSeed[randCounter];
                            randCounter++;
                        }
                        else
                            rand = Random.Range(minRand, maxRand);
                        randomSeed.Add(rand);

                        int currPlaneNum = planeNum;
                        for (int j = currPlaneNum; j < currPlaneNum + rand; j++)
                        {
                            prevPlane = auc.addPlane(0, j, prevPlane, pSize);
                            // Add vertices
                            Vector3 c = prevPlane;
                            g.Vertex(c.x - (pSize / 2), c.y, c.z - (pSize / 2)); // Lower left
                            g.Vertex(c.x + (pSize / 2), c.y, c.z - (pSize / 2)); // Lower right
                            g.Vertex(c.x - (pSize / 2), c.y, c.z + (pSize / 2)); // Upper left
                            g.Vertex(c.x + (pSize / 2), c.y, c.z + (pSize / 2)); // Upper right
                            planeNum++;
                        }
                        prevDirection = 0;  // Keep track of which direction these planes were added
                    }

                }

                // If previous was left, add one above and _ amount of spaces to the right
                else if (prevDirection == 0)
                {
                    if (isInput == true)
                    {
                        rand = randomSeed[randCounter];
                        randCounter++;
                    }
                    else
                        rand = Random.Range(minRand, maxRand);
                    randomSeed.Add(rand);
                    // Add one above
                    prevPlane = auc.addPlane(1, planeNum, prevPlane, pSize);
                    // Add vertices
                    Vector3 c = prevPlane;
                    g.Vertex(c.x - (pSize / 2), c.y, c.z - (pSize / 2)); // Lower left
                    g.Vertex(c.x + (pSize / 2), c.y, c.z - (pSize / 2)); // Lower right
                    g.Vertex(c.x - (pSize / 2), c.y, c.z + (pSize / 2)); // Upper left
                    g.Vertex(c.x + (pSize / 2), c.y, c.z + (pSize / 2)); // Upper right
                    planeNum++;

                    int currPlaneNum = planeNum;
                    for (int j = currPlaneNum; j < currPlaneNum + rand; j++)
                    {
                        prevPlane = auc.addPlane(2, j, prevPlane, pSize);
                        // Add vertices
                        c = prevPlane;
                        g.Vertex(c.x - (pSize / 2), c.y, c.z - (pSize / 2)); // Lower left
                        g.Vertex(c.x + (pSize / 2), c.y, c.z - (pSize / 2)); // Lower right
                        g.Vertex(c.x - (pSize / 2), c.y, c.z + (pSize / 2)); // Upper left
                        g.Vertex(c.x + (pSize / 2), c.y, c.z + (pSize / 2)); // Upper right
                        planeNum++;
                    }
                    prevDirection = 2;
                }
                else if (prevDirection == 2)    // Right
                {
                    if (isInput == true)
                    {
                        rand = randomSeed[randCounter];
                        randCounter++;
                    }
                    else
                        rand = Random.Range(minRand, maxRand);
                    randomSeed.Add(rand);
                    // Add one above
                    prevPlane = auc.addPlane(1, planeNum, prevPlane, pSize);
                    // Add vertices
                    Vector3 c = prevPlane;
                    g.Vertex(c.x - (pSize / 2), c.y, c.z - (pSize / 2)); // Lower left
                    g.Vertex(c.x + (pSize / 2), c.y, c.z - (pSize / 2)); // Lower right
                    g.Vertex(c.x - (pSize / 2), c.y, c.z + (pSize / 2)); // Upper left
                    g.Vertex(c.x + (pSize / 2), c.y, c.z + (pSize / 2)); // Upper right
                    planeNum++;

                    int currPlaneNum = planeNum;
                    for (int j = currPlaneNum; j < currPlaneNum + rand; j++)
                    {
                        prevPlane = auc.addPlane(0, j, prevPlane, pSize);
                        // Add vertices
                        c = prevPlane;
                        g.Vertex(c.x - (pSize / 2), c.y, c.z - (pSize / 2)); // Lower left
                        g.Vertex(c.x + (pSize / 2), c.y, c.z - (pSize / 2)); // Lower right
                        g.Vertex(c.x - (pSize / 2), c.y, c.z + (pSize / 2)); // Upper left
                        g.Vertex(c.x + (pSize / 2), c.y, c.z + (pSize / 2)); // Upper right
                        planeNum++;
                    }
                    prevDirection = 0;
                }
            }


            int planeCounter = 1;
            // Add ceiling planes
            foreach (GameObject c in auc.planes)
            {
                GameObject tempPlane = GameObject.Instantiate(startCeilingPlane);
                tempPlane.transform.localPosition = new Vector3(c.transform.position.x, tempPlane.transform.position.y, c.transform.position.z);
                tempPlane.name = "TopPlane" + planeCounter;
                tempPlane.transform.SetParent(planeParent.transform, false);
                planeCounter++;
            }

            MonotoneTriangulation mt = new MonotoneTriangulation(g.vertexList());
            g.addEdges(mt);         // Add edges based on vertices
            g.sortCCWOrder(mt);     // Sort vertices in CCW Order

            // Add spheres to scene (vertices)
            //for (int i = 0; i < g.vertexSize(); i++)
            //    auc.createVertices(g, i);

            /*
            // Add cubes to scene (edges)
            for (int i = 0; i < g.edgeSize(); i++)
                auc.createEdges(g, i, pSize);
            */

            // Add paintings to walls
            int paintingCounter = 0;
            List<GameObject> wallWithArt = new List<GameObject>();
            wallWithArt.Add(wallWithWindowLR_painting1);
            wallWithArt.Add(wallWithWindowLR_painting2);
            wallWithArt.Add(wallWithWindowLR_painting3);
            wallWithArt.Add(wallWithWindowLR_painting4);
            wallWithArt.Add(wallWithWindowLR_painting5);
            wallWithArt.Add(wallWithWindowLR_painting6);
            wallWithArt.Add(wallWithWindowLR_painting7);
            wallWithArt.Add(wallWithWindowLR_painting8);
            for (int i = 1; i <= g.vertex.Count; i++)
            {
                Debug.Log("painting counter --------------------- ");
                if (i == g.vertex.Count)
                    paintingCounter = auc.createEdges(g, g.vertex[i - 1], g.vertex[0], i - 1, pSize, wallWithWindowLR, wallWithWindowFB, paintingCounter, wallWithArt[paintingCounter]);
                else
                    paintingCounter = auc.createEdges(g, g.vertex[i], g.vertex[i - 1], i - 1, pSize, wallWithWindowLR, wallWithWindowFB, paintingCounter, wallWithArt[paintingCounter]);
            }

            walls = auc.walls;

            // Perform earbased triangulations
            EarBasedTriangulation ebt = new EarBasedTriangulation(g, g.vertexList());
            ebt.triangulation();
            List<Vector3> tri = ebt.getTriangles();
            // Print triangulations
            Debug.Log("Triangulation:\n");
            foreach (Vector3 t in tri)
                Debug.Log(t);

            // New graph for the triangles
            Graph triGraph = new Graph();
            foreach (Vector3 t in tri)
            {
                // Add each edge of triangle to graph (will remove duplicates)
                triGraph.Edge((int)t.x, (int)t.y);
                triGraph.Edge((int)t.x, (int)t.z);
                triGraph.Edge((int)t.y, (int)t.z);

                // Add each vertex of triangle to graph (will remove duplicates)
                Vector3 v1 = g.getVertex((int)t.x);
                triGraph.Vertex(v1);
                Vector3 v2 = g.getVertex((int)t.y);
                triGraph.Vertex(v2);
                Vector3 v3 = g.getVertex((int)t.z);
                triGraph.Vertex(v3);
            }

            Debug.Log("NEW GRAPH:");
            triGraph.printGraph();

            // Coloring the graph
            ColorGraph cg = new ColorGraph(triGraph.vertex, triGraph.edge, tri, colorParent);
            cg.coloring(ebt.trianglesSize(), ebt.trianglesSize(), tri[ebt.trianglesSize() - 1], true);
            cg.printColors(g);  // Remove g?
            cg.countColors();
            List<int> minCameras = cg.getMinCameraVertices();

            // Add cameras
            for (int i = 0; i < minCameras.Count; i++)
            {
                auc.addCameras(g, minCameras[i], i, pSize);
                cam0.transform.localPosition = new Vector3(g.getVertex(minCameras[i]).x, g.getVertex(minCameras[i]).y + 20, g.getVertex(minCameras[i]).z);
                cameraPosition.Add(new Vector3(g.getVertex(minCameras[i]).x, g.getVertex(minCameras[i]).y + 20, g.getVertex(minCameras[i]).z));
                totalCameras++;
            }

            // Find the areas of each of the triangles
            List<Vector2> areaList = new List<Vector2>();
            for (int i = 0; i < tri.Count; i++)
            {
                Vector3 A = g.getVertex((int)tri[i].x);
                Vector3 B = g.getVertex((int)tri[i].y);
                Vector3 C = g.getVertex((int)tri[i].z);

                float area = Math.Abs(A.x * (B.z - C.z) + B.x * (C.z - A.z) + C.x * (A.z - B.z)) / 2;
                areaList.Add(new Vector2(i, area));

            }

            // Sort based on which triangles had the biggest area
            areaList = Quicksort(areaList, 0, areaList.Count - 1);

            // Find max area triangles and add art piece in the center of the triangles
            for (int i = 0; i <= minCameras.Count; i++)
            {
                // Creates actual stand that holds the art piece
                GameObject artPiece = auc.createArtPiece(g, tri, areaList[i]);

                // Add vases
                GameObject cv1 = GameObject.Instantiate(vase1);
                Vector3 pos = new Vector3(artPiece.transform.position.x, artPiece.transform.position.y + 2, artPiece.transform.position.z);
                cv1.transform.position = pos;
                cv1.transform.localScale = new Vector3(2, 2, 2);
                cv1.transform.SetParent(artPieceParent.transform, false);
                artPiecesList.Add(cv1);
                totalArtPieces++;
            }

            isStopping = true;
        }
    }
}
    
