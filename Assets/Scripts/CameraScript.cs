using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class CameraInfo
{
    public int cameraNum;
    public Vector3 position;
    public Quaternion rotation;
    float xRotation = 0f;
    float yRotation = 0f;
    public Transform cam;
    public Camera currentCam;
    public Vector3 startingPos;
    public float zoom;
    List<GameObject> walls;
    Vector3 plantPos;
    GameObject plant;

    //public CameraInfo(int cameraNum, Vector3 position, Transform cam, Text text, Camera currentCam)
    public CameraInfo(int cameraNum, Vector3 position, Transform cam, Camera currentCam, List<GameObject> walls, GameObject plant)
    {
        this.cameraNum = cameraNum;
        this.position = position;
        this.cam = cam;
        this.currentCam = currentCam;
        zoom = currentCam.fieldOfView;
        this.walls = walls;
        this.position = new Vector3(position.x, position.y - 0.5f, position.z);
        this.plant = plant;
        findNeighborWalls();  // Makes sure camera view only shows wall (cannot see behind wall)
    }

    public void performRotation()
    {
        float keyX = Input.GetAxis("Vertical") * Time.deltaTime * 30;
        float keyY = -Input.GetAxis("Horizontal") * Time.deltaTime * 30;

        xRotation -= keyX;
        yRotation -= keyY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);      // Prevent it from rotating all the way

        cam.transform.localRotation = Quaternion.Euler(startingPos.x + xRotation, startingPos.y + yRotation, 0f);       // ISSUES
    }

    // Makes sure camera first looks at closest art piece, instead on in a random direction
    public Vector3 firstView(int i, List<GameObject> artPiecesList)
    {
        Vector3 relativePos = artPiecesList[i].transform.position - cam.transform.position;
        cam.transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        rotation = cam.transform.rotation;
        Vector3 rot = cam.transform.eulerAngles;
        cam.transform.rotation = rotation;
        return rot;
    }

    public void findNeighborWalls()
    {
        GameObject leftNeighbor = new GameObject();
        GameObject rightNeighbor = new GameObject();
        for (int i = 1; i < walls.Count; i++)
        {
            
            if (walls[i].transform.localPosition.z == position.z && walls[i-1].transform.localPosition.x == position.x)
            {
                if (position.x == walls[i - 1].transform.localPosition.x && position.z == walls[i].transform.localPosition.z)
                {
                    Debug.Log("1-Wall neighbors: " + walls[i].transform.localPosition + " " + walls[i - 1].transform.localPosition);
                    leftNeighbor = walls[i];
                    rightNeighbor = walls[i-1];
                    break;
                }
                
            }
            else if(walls[i].transform.localPosition.x == position.x && walls[i - 1].transform.localPosition.z == position.z)
            {
                if (position.z == walls[i - 1].transform.localPosition.z && position.x == walls[i].transform.localPosition.x)
                {
                    Debug.Log("2-Wall neighbors: " + walls[i].transform.localPosition + " " + walls[i - 1].transform.localPosition);
                    leftNeighbor = walls[i];
                    rightNeighbor = walls[i - 1];
                    break;
                }
            }
        }

        // Last and first
        if (walls[walls.Count-1].transform.localPosition.z == position.z && walls[0].transform.localPosition.x == position.x)
        {
            leftNeighbor = walls[0];
            rightNeighbor = walls[walls.Count - 1];
        }
        else if (walls[walls.Count - 1].transform.localPosition.x == position.x && walls[0].transform.localPosition.z == position.z)
        {
            leftNeighbor = walls[0];
            rightNeighbor = walls[walls.Count - 1];
        }

        Vector3 left = leftNeighbor.transform.localPosition;
        Vector3 middle = position;
        Vector3 right = rightNeighbor.transform.localPosition;

        if (isConvex(left, middle, right))
        {
            // Add plant 
            plant = GameObject.Instantiate(plant);
            plant.name = "plant" + cameraNum;

            if (left.z == middle.z && right.x == middle.x && left.z < right.z)
            {
                position = new Vector3(position.x + 0.8f, position.y, position.z + 0.8f);
                plant.transform.localPosition = new Vector3(position.x + 1.4f, 0f, position.z + 1.4f);
            }
            else if (left.z == middle.z && right.x == middle.x && left.z > right.z)
            {
                position = new Vector3(position.x - 0.8f, position.y, position.z - 0.8f);
                plant.transform.localPosition = new Vector3(position.x - 1.4f, 0f, position.z - 1.4f);
            }
            else if (left.x == middle.x && right.z == middle.z && left.z < right.z)
            {
                position = new Vector3(position.x + 0.8f, position.y, position.z - 0.8f);
                plant.transform.localPosition = new Vector3(position.x + 1.4f, 0f, position.z - 1.4f);
            }
            else if (left.x == middle.x && right.z == middle.z && left.z > right.z)
            {
                position = new Vector3(position.x - 0.8f, position.y, position.z + 0.8f);
                plant.transform.localPosition = new Vector3(position.x - 1.4f, 0f, position.z + 1.4f);
            }



        }
        else if (isConcave(left, middle, right))
        {
            if (left.z == middle.z && right.x == middle.x && left.z > right.z)
            {
                position = new Vector3(position.x - 0.8f, position.y, position.z + 0.8f);
            }
            else if (left.z == middle.z && right.x == middle.x && left.z < right.z)
            {
                position = new Vector3(position.x + 0.8f, position.y, position.z - 0.8f);
            }
            else if (left.x == middle.x && right.z == middle.z && left.z > right.z)
            {
                position = new Vector3(position.x - 0.8f, position.y, position.z - 0.8f);
            }
            else if (left.x == middle.x && right.z == middle.z && left.z < right.z)
            {
                position = new Vector3(position.x + 0.8f, position.y, position.z + 0.8f);
            }
        }
    }

    public bool isConcave(Vector3 l, Vector3 m, Vector3 r)
    {
        // dot(A, B) > 0
        Vector3 A = new Vector3(l.x-m.x, l.y-m.y, l.z-m.z);
        Vector3 B = new Vector3(m.x - r.x, m.y - r.y, m.z - r.z);

        if (A.x * B.z - A.z * B.x > 0)
        {
            Debug.Log("Concave");
            return true;
        }
        else
        {
            Debug.Log("Not Concave");
            return false;
        }
    }

    public bool isConvex(Vector3 l, Vector3 m, Vector3 r)
    {
        // dot(A, B) < 0
        Vector3 A = new Vector3(l.x - m.x, l.y - m.y, l.z - m.z);
        Vector3 B = new Vector3(m.x - r.x, m.y - r.y, m.z - r.z);

        if (A.x * B.z - A.z * B.x < 0)
        {
            Debug.Log("Convex");
            return true;
        }
        else
        {
            Debug.Log("Not Convex");
            return false;
        }
    }

    // When switch between cameras, set the default zoom
    public void setDefaultZoom()
    {
        currentCam.fieldOfView = zoom;
    }

    // Zoom in with camera
    public void zoomIn()
    {
        if (zoom < 40)
            zoom = 40;
        else
            zoom -= 0.2f;
        currentCam.fieldOfView = zoom;
    }

    // Soom out with camera
    public void zoomOut()
    {
        if (zoom > 80)
            zoom = 80;
        else
            zoom += 0.2f;
        currentCam.fieldOfView = zoom;
    }

    // Find the closest art piece to the camera
    public int findClosestArtPiece(List<GameObject> artPiecesList, List<CameraInfo> cameraList)
    {
        int closestArtNum = -1;
        float closestArtPiece = 99999999999f;
        for (int i = 0; i < artPiecesList.Count; i++)
        {
            float distance = Vector3.Distance(cameraList[cameraNum].position, artPiecesList[i].transform.localPosition);
            if (distance < closestArtPiece)
            {
                closestArtPiece = distance;
                closestArtNum = i;
            }
        }
        return closestArtNum;
    }
}

public class Timers
{
    int totalCameras;
    float totalSeconds;
    Transform cam;
    TextMeshProUGUI gameTimerText;
    int currentCameraNum;
    bool didGameEnd;

    public Timers(int totalCameras, TextMeshProUGUI gameTimerText, Transform cam, bool didGameEnd)
    {
        this.totalCameras = totalCameras;
        this.cam = cam;
        this.gameTimerText = gameTimerText;
        totalSeconds = 60 * (totalCameras);
        this.didGameEnd = didGameEnd;
    }

    public void getCameraNum(int cameraNum)
    {
        currentCameraNum = cameraNum;
    }

    // Counts down the timer for the game (based on minutes = totalCameras)
    public bool gameCountdown(bool areAllArtPiecesStolen)
    {
        int seconds;
        int minutes;

        if (areAllArtPiecesStolen == true)
        {
            didGameEnd = true;
            gameTimerText.text = "";
            return didGameEnd;
        }

        // Timer ran out
        if (totalSeconds <= 0)
        {
            seconds = (int)0;
            minutes = (int)0;
            didGameEnd = true;
            gameTimerText.text = "";
            return didGameEnd;
        }
        else
        {
            totalSeconds -= Time.deltaTime;      // Decrease by second (per frame)
            seconds = (int)totalSeconds % 60;
            minutes = (int)totalSeconds / 60;

            // Add timer text + camera text to screen
            Vector3 pos = new Vector3(cam.transform.localPosition.x + 2, cam.transform.localPosition.y, cam.transform.localPosition.z);
            string cameraText = "CAMERA " + currentCameraNum;
            string shiftText = "SHIFT ENDS IN " + minutes.ToString("D1") + ":" + seconds.ToString("D2") + " HOURS";
            gameTimerText.text = cameraText + "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n" + shiftText;
            return didGameEnd;
        }

    }
}


public class CameraScript : MonoBehaviour
{
    public Transform cam;
    public GameObject startPlane;
    public int cameraNumber;
    int totalCameras;
    List<Vector3> cameraPosition;
    bool isRoomGenerated;
    bool isStopping;
    public Camera cam0;
    List<CameraInfo> cameraList;
    public List<GameObject> artPiecesList;
    public List<bool> isFirstTimeView;
    Timers timers; 
    public TextMeshProUGUI gameTimerText;
    List<GameObject> walls;
    public bool didGameEnd;
    bool isStopped;
    public bool isMiniGameTriggered;
    public Camera cam1;
    bool wasGameWon;
    public GameObject miniGame;
    bool wasGameLost;
    public GameObject spotLight;
    bool isLightOff;
    bool areAllArtPiecesStolen;
    public GameObject Thief;
    public bool didUserClickOnThief;
    public int numThiefVisit;
    bool isRopeRaised;
    public Camera cam2;
    public bool isIDScannerTriggered;
    public GameObject IDScanner;
    bool wasCodeCorrect;
    bool wasCodeIncorrect;
    public bool isReset;
    int numThiefEncounters;
    bool isFirstEncounter;
    public GameObject plant;
    int minTime;
    int maxTime;
    float countTimer;

    // Start is called before the first frame update
    void Start()
    {
        didGameEnd = false;
        isStopping = false;
        cameraList = new List<CameraInfo>();
        isFirstTimeView = new List<bool>();
        isStopped = false;
        isMiniGameTriggered = false;
        wasGameWon = false;
        wasGameLost = false;
        isLightOff = false;
        areAllArtPiecesStolen = false;
        didUserClickOnThief = false;
        numThiefVisit = 0;
        isRopeRaised = false;
        isIDScannerTriggered = false;
        wasCodeCorrect = false;
        wasCodeIncorrect = false;
        isReset = false;
        isFirstEncounter = false;
        minTime = 20;
        maxTime = 35;
        countTimer = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        // Get info from other script about cameras (needs to be updated only once after the other script has been performed)
        isRoomGenerated = startPlane.GetComponent<FindVertices>().isRoomGenerated;      // Get variable from other script
        if (isStopping == false && isRoomGenerated == true)
        {
            Debug.Log("GENERATED!!");
            // Save camera info from other script
            cameraPosition = startPlane.GetComponent<FindVertices>().cameraPosition;
            totalCameras = startPlane.GetComponent<FindVertices>().totalCameras;
            cameraNumber = startPlane.GetComponent<FindVertices>().cameraNumber;
            artPiecesList = startPlane.GetComponent<FindVertices>().artPiecesList;
            walls = startPlane.GetComponent<FindVertices>().walls;

            // Add camera info to list
            for(int i = 0; i < cameraPosition.Count; i++)
            {
                CameraInfo ci = new CameraInfo(i, cameraPosition[i], cam, cam0, walls, plant);
                cameraList.Add(ci);
            }

            Debug.Log("Camera: " + cameraNumber);       // Current camera number
            transform.localPosition = cameraList[cameraNumber].position;        // Set current camera position

            // Keep track whether it's first time viewing from this camera
            foreach (CameraInfo c in cameraList)
                isFirstTimeView.Add(true);

            // Used to keep track of timer data
            timers = new Timers(totalCameras, gameTimerText, cam, didGameEnd);

            isStopping = true;   // to Stop it from constantly updating it
        }

        areAllArtPiecesStolen = Thief.GetComponent<ThiefAnimation>().areAllArtPiecesStolen;
        if (isRoomGenerated == true && didGameEnd == false && areAllArtPiecesStolen == false)
        {
            if (cam0.enabled == true && isLightOff == false && didGameEnd == false)
            {
                if (countTimer > 0)
                {
                    countTimer = countTimer - (1 * Time.deltaTime);
                }
                else
                {
                    // Change camera view to minigame
                    cam0.enabled = false;
                    cam1.enabled = true;
                    gameTimerText.text = "";
                    isMiniGameTriggered = true;
                    isFirstEncounter = true;
                }
            }
            else
            {
                countTimer = Random.Range(minTime, maxTime);
            }
        }

        wasGameWon = miniGame.GetComponent<MiniGame>().wasGameWon;
        wasGameLost = miniGame.GetComponent<MiniGame>().wasGameLost;

        // Change camera view back to art gallery
        if ((wasGameWon == true || wasGameLost == true) && didGameEnd == false)
        {
            cam0.enabled = true;
            cam1.enabled = false;
            isMiniGameTriggered = false;
        }

        if (isRoomGenerated == true && isStopped == false && isMiniGameTriggered == false && isIDScannerTriggered == false)
        {
            didGameEnd = timers.gameCountdown(areAllArtPiecesStolen);     // Shift countdown, returns if game ended when countdown ends

            if (didGameEnd == true)
                isStopped = true;
            else
            {
                // If user presses right key, go to next camera
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    cameraNumber = (cameraNumber + totalCameras + 1) % totalCameras;
                    Debug.Log("Camera: " + cameraNumber);
                    transform.localPosition = cameraList[cameraNumber].position;
                    timers.getCameraNum(cameraNumber);
                    cameraList[cameraNumber].setDefaultZoom();
                }   // If user presses left key, go to previous camera
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    cameraNumber = (cameraNumber + totalCameras - 1) % totalCameras;
                    Debug.Log("Camera: " + cameraNumber);
                    transform.localPosition = cameraList[cameraNumber].position;
                    timers.getCameraNum(cameraNumber);
                    cameraList[cameraNumber].setDefaultZoom();
                }

                // Rotation
                if (isFirstTimeView[cameraNumber] == true)  // First time looking through this camera
                {
                    int artObject = cameraList[cameraNumber].findClosestArtPiece(artPiecesList, cameraList);
                    Vector3 rot = cameraList[cameraNumber].firstView(artObject, artPiecesList);
                    cameraList[cameraNumber].startingPos = rot;     // Set Vector rotation of when to look at first
                    transform.rotation = Quaternion.Euler(rot);
                    isFirstTimeView[cameraNumber] = false;
                }
                else
                    cameraList[cameraNumber].performRotation();     // Rotation

                // Zoom in/out
                if (Input.GetKey(KeyCode.Q))
                    cameraList[cameraNumber].zoomOut();
                else if (Input.GetKey(KeyCode.E))
                    cameraList[cameraNumber].zoomIn();

                // If user spots thief
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    RaycastHit hit;
                    Ray ray = cam0.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        Debug.Log(hit.transform.name);
                        if (hit.transform.name == "ArtThiefBody")
                        {
                            Debug.Log("THIEF SPOTTED");
                            if (isFirstEncounter == true)
                            {
                                didUserClickOnThief = true;
                                isFirstEncounter = false;
                            }
                        }
                    }
                }
                /*
                // When user doesn't click on thief
                if (isFirstEncounter == true && didUserClickOnThief == false && isRopeRaised == true && isLightOff == false)
                {
                    Debug.Log("NO THIEF CLICKED");
                    wasThiefNotSpotted = true;
                    isFirstEncounter = false;
                    countTimer = Random.Range(minTime, maxTime);
                }
                else if (isFirstEncounter == false && isRopeRaised == true && isLightOff == true)
                {
                    wasThiefNotSpotted = false;
                }
                */

            }
        }

        // Minigame
        isRopeRaised = Thief.GetComponent<ThiefAnimation>().wasRopeRaised;
        isLightOff = spotLight.GetComponent<LightingS>().isLightOff;
        if (didUserClickOnThief == true && isLightOff == false)
            isIDScannerTriggered = true;

        numThiefEncounters = Thief.GetComponent<ThiefAnimation>().numThiefEncounters;
        if (isLightOff == false && didUserClickOnThief == true && didGameEnd == false)
        {
            // Change camera view to minigame
            cam0.enabled = false;
            cam2.enabled = true;
            isIDScannerTriggered = true;
        }

        if (isIDScannerTriggered == true)
            gameTimerText.text = "";


        wasCodeCorrect = IDScanner.GetComponent<IDCheckScript>().wasGameWon;
        wasCodeIncorrect = IDScanner.GetComponent<IDCheckScript>().wasGameLost;
        if (wasCodeCorrect == true && didGameEnd == false)
        {
            Debug.Log(":)");
            // Win: Turn on lights + no thief
            cam0.enabled = true;
            cam2.enabled = false;
            didUserClickOnThief = false;
            isIDScannerTriggered = false;

        }
        else if (wasCodeIncorrect == true && didGameEnd == false)
        {
            // Lose: Thief animation
            cam0.enabled = true;
            cam2.enabled = false;
            didUserClickOnThief = false;
            isIDScannerTriggered = false;
        }

    }
}
