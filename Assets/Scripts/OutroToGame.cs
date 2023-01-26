using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutroToGame : MonoBehaviour
{
    public Camera cam;
    public GameObject SavedPainting;
    public GameObject LostPainting;
    int numPaintingsLeft;
    List<GameObject> paintings;
    List<Vector3> positions;
    int totalArtPieces;
    public TextMeshPro JobStatus;
    bool didGameEnd;
    public Camera cam2;
    bool isStopping;
    public GameObject StartPlane;
    public GameObject Thief;
    public Camera cam1;
    public Camera cam3;
    public Camera cam4;
    string seedCodeStr;
    public TextMeshPro SeedCodeText;
    public List<int> seedCode;
    public GameObject startPlane;
    public TextMeshPro JobMessage;

    // Start is called before the first frame update
    void Start()
    {
        isStopping = false;
        seedCode = new List<int>();

    }

    // Update is called once per frame
    void Update()
    {
        didGameEnd = cam2.GetComponent<CameraScript>().didGameEnd;
        seedCode = startPlane.GetComponent<FindVertices>().randomSeed;
        // RUn once, at end of game
        if (didGameEnd == true && isStopping == false)
        {
            Debug.Log("OVER HERR!!!!!");
            // Disable other cameras
            cam1.enabled = false;
            cam2.enabled = false;
            cam3.enabled = false;
            cam4.enabled = false;
            cam.enabled = true;

            numPaintingsLeft = Thief.GetComponent<ThiefAnimation>().artPiecesLeft;   // Change later
            totalArtPieces = StartPlane.GetComponent<FindVertices>().totalArtPieces;

            paintings = new List<GameObject>();
            positions = new List<Vector3>();

            // Positions of the paintings
            positions.Add(new Vector3(-0.3f, 0f, 0f));
            positions.Add(new Vector3(-0.15f, 0f, 0f));
            positions.Add(new Vector3(-0f, 0f, 0f));
            positions.Add(new Vector3(0.15f, 0f, 0f));
            positions.Add(new Vector3(0.3f, 0f, 0f));
            positions.Add(new Vector3(0.45f, 0f, 0f));
            int posCount = 0;

            // Calculates number of stars that the user earned
            int numStars = (numPaintingsLeft * 5) / totalArtPieces;
            Debug.Log(numStars);

            if (numStars > 0)
            {
                // First (not stolen) painting
                paintings.Add(SavedPainting);
                paintings[0].transform.SetParent(this.transform, false);
                paintings[0].transform.localPosition = positions[posCount];
                posCount++;

                // Add other (not stolen) paintings to outro screen
                if (numStars > 1)
                {
                    for (int i = 1; i < numStars; i++)
                    {
                        paintings.Add(GameObject.Instantiate(SavedPainting));
                        paintings[i].transform.SetParent(this.transform, false);
                        paintings[i].transform.localPosition = positions[posCount];
                        posCount++;
                    }
                }
            }
            // Add rest of paintings (stolen)
            for (int i = numStars; i < 5; i++)
            {
                paintings.Add(GameObject.Instantiate(LostPainting));
                paintings[i].transform.SetParent(this.transform, false);
                paintings[i].transform.localPosition = positions[posCount];
                posCount++;
            }

            // Display job status
            if (numStars == 0)
            {
                JobStatus.text = "FIRED";
                JobStatus.color = Color.red;
                getSeed();
                SeedCodeText.text = SeedCodeText.text + seedCodeStr;
                JobMessage.text = numPaintingsLeft + "/" + totalArtPieces + " art pieces secured\n" + "Please find a job elsewhere";
            }
            else if (numStars == 5)
            {
                JobStatus.text = "PROMOTED";
                JobStatus.color = Color.black;
                getSeed();
                SeedCodeText.text = SeedCodeText.text + seedCodeStr;
                JobMessage.text = numPaintingsLeft + "/" + totalArtPieces + " art pieces secured\n" + "Great job!";
            }
            else
            {
                JobStatus.text = numPaintingsLeft + "/" + totalArtPieces + " art pieces secured\n";
                getSeed();
                SeedCodeText.text = SeedCodeText.text + seedCodeStr;
                JobMessage.text = "Management will be hearing about this incident";
            }
            isStopping = true;
        }
    }

    public void getSeed()
    {
        //foreach(int s in seedCode)
        for (int i = 0; i < 10; i++)
        {
            seedCodeStr = seedCodeStr + seedCode[i].ToString();
        }
    }
}
