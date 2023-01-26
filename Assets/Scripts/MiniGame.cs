using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniGame : MonoBehaviour
{
    public GameObject vase0;
    public GameObject vase1;
    public GameObject vase2;
    public GameObject stand0;
    public GameObject stand1;
    public GameObject stand2;
    public GameObject case0;
    public GameObject case1;
    public GameObject case2;
    string caseDirection;
    bool isCaseStopped;
    int currVase;
    GameObject currCase;
    List<GameObject> caseList;
    bool isVase1Correct;
    bool isVase2Correct;
    bool isVase3Correct;
    bool isMiniGameTriggered;
    bool isRoomGenerated;
    public Camera cam2;
    public GameObject startPlane;
    bool didGameEnd;
    public bool wasGameWon;
    public bool wasGameLost;
    bool wasRopeRaised;
    public GameObject Thief;
    public bool isReset;
    bool areAllArtPiecesStolen;
    public bool isGamePlaying;
    List<float> speedList;
    bool isPaused;
    float countDown;
    public TextMeshPro MiniGameResults;

    // Start is called before the first frame update
    void Start()
    {
        caseDirection = "down";
        isCaseStopped = false;
        currVase = 0;
        caseList = new List<GameObject>();
        caseList.Add(case0);
        caseList.Add(case1);
        caseList.Add(case2);
        currCase = caseList[0];
        isMiniGameTriggered = false;
        isRoomGenerated = false;
        didGameEnd = false;
        wasGameWon = false;
        wasGameLost = false;
        wasRopeRaised = false;
        isReset = false;
        areAllArtPiecesStolen = false;
        speedList = new List<float>();
        speedList.Add(2.2f);
        speedList.Add(2.4f);
        speedList.Add(2.6f);
        isPaused = false;
        countDown = 1;
        MiniGameResults.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        isRoomGenerated = startPlane.GetComponent<FindVertices>().isRoomGenerated;
        didGameEnd = cam2.GetComponent<CameraScript>().didGameEnd;
        isMiniGameTriggered = cam2.GetComponent<CameraScript>().isMiniGameTriggered;
        wasRopeRaised = Thief.GetComponent<ThiefAnimation>().wasRopeRaised;
        areAllArtPiecesStolen = Thief.GetComponent<ThiefAnimation>().areAllArtPiecesStolen;

        if (isPaused == false)
        {
            // Reset everything after first playthrough
            if (isRoomGenerated == true && didGameEnd == false)
            {
                if ((wasRopeRaised == true || wasGameWon == true) && isReset == true)
                {
                    Debug.Log("Resetting minigame");
                    resetPositions();
                    currVase = 0;
                    currCase = caseList[0];
                    isCaseStopped = false;
                    wasGameLost = false;
                    wasGameWon = false;
                    isReset = false;
                }
            }

            if (isRoomGenerated == true && didGameEnd == false && areAllArtPiecesStolen == false)
            {
                // Change to space key later
                if (Input.GetKeyDown(KeyCode.Space) && isMiniGameTriggered == true)
                {
                    // Change the current vase (go though each of them in order)
                    if (currVase < 3)
                    {
                        Debug.Log("H: " + currVase);
                        // Check whether the user paused the case on the correct position + some error acceptance
                        bool isCorrect = isCasePlacementCorrect(currCase);
                        if (currVase == 0)
                            isVase1Correct = isCorrect;
                        else if (currVase == 1)
                            isVase2Correct = isCorrect;
                        else
                            isVase3Correct = isCorrect;

                        if (currVase != 2)
                        {
                            // Next vase
                            currVase++;
                            currCase = caseList[currVase];
                        }
                        else    // Check if cases are placed on vases correctly
                        {
                            Debug.Log("Is Case 1 correct? " + isVase1Correct);
                            Debug.Log("Is Case 2 correct? " + isVase2Correct);
                            Debug.Log("Is Case 3 correct? " + isVase3Correct);

                            // If user won game or if user lost game
                            if (isVase1Correct == true && isVase2Correct == true && isVase3Correct == true)
                            {
                                isPaused = true;
                            }
                            else
                            {
                                isPaused = true;
                            }

                            isCaseStopped = true;
                            currVase++;
                        }
                    }
                }

                // Move the cases up/down
                if (isCaseStopped == false && isMiniGameTriggered == true)
                {
                    if (currCase.transform.localPosition.x < 0)
                        caseDirection = "up";
                    else if (currCase.transform.localPosition.x > 3)
                        caseDirection = "down";
                    if (caseDirection == "up")
                        currCase.transform.localPosition = new Vector3(currCase.transform.localPosition.x + (speedList[currVase] * Time.deltaTime), currCase.transform.localPosition.y, currCase.transform.localPosition.z);
                    else
                        currCase.transform.localPosition = new Vector3(currCase.transform.localPosition.x - (speedList[currVase] * Time.deltaTime), currCase.transform.localPosition.y, currCase.transform.localPosition.z);
                    isGamePlaying = true;
                }
            }
        }
        else
        {
            if (countDown > 0)
            {
                countDown = countDown - (1 * Time.deltaTime);
                if (isVase1Correct == true && isVase2Correct == true && isVase3Correct == true)
                {
                    MiniGameResults.text = "Correct";
                    MiniGameResults.color = Color.green;
                }
                else
                {
                    MiniGameResults.text = "Incorrect";
                    MiniGameResults.color = Color.red;
                }
            }
            else
            {
                // If user won game or if user lost game
                if (isVase1Correct == true && isVase2Correct == true && isVase3Correct == true)
                {
                    wasGameWon = true;
                    isGamePlaying = false;
                    isReset = true;
                    isPaused = false;
                }
                else
                {
                    wasGameLost = true;
                    isGamePlaying = false;
                    isReset = true;
                    isPaused = false;
                }
            }
        }
    }

    // Check if the case is covering the art piece
    public bool isCasePlacementCorrect(GameObject currCase)
    {
        if (currCase.transform.localPosition.x >= 0 && currCase.transform.localPosition.x <= 0.1)
            return true;
        else
            return false;
    }

    // Reset the positions of each case
    public void resetPositions()
    {
        foreach(GameObject c in caseList)
            c.transform.localPosition = new Vector3(3f, c.transform.localPosition.y, c.transform.localPosition.z);
        countDown = 1;
        isPaused = false;
        MiniGameResults.text = "";
    }
}
