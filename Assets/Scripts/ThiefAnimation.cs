using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAnimation : MonoBehaviour
{
    public Light spotLight;
    public GameObject startPlane;
    bool isRoomGenerated;
    public GameObject rope;
    bool isRopeLowered;
    List<GameObject> artPiecesList;
    public int artPiece;
    List<Light> lightList;
    bool isStopping;
    public GameObject startSpotlight;
    bool areLightsSet;
    public Animator thiefAnimator;
    bool isThiefAtPos;
    public GameObject hand;
    bool removeArtPiece;
    bool isStep1;
    public bool isStep2;
    bool isStep3;
    bool isStep4;
    bool isStep5;
    bool isMiniGameTriggered;
    bool isLightOff;
    bool isThiefGone;
    bool isArtPieceRightSize;
    public int artPiecesLeft;
    public Camera cam2;
    bool didGameEnd;
    bool wasGameLost;
    public GameObject miniGame1;
    public bool wasRopeRaised;
    bool isReseting;
    public bool areAllArtPiecesStolen;
    bool wasCodeIncorrect;
    public GameObject IDScanner;
    public int numThiefEncounters;
    public GameObject hole;

    // Start is called before the first frame update
    void Start()
    {
        isRopeLowered = false;
        transform.localPosition = new Vector3(0, 25, 0);
        rope.transform.position = new Vector3(0, 30, 0);
        hole.transform.position = new Vector3(0, 30, 0);
        isStopping = false;
        isThiefAtPos = false;
        removeArtPiece = false;
        isStep1 = false;
        isStep2 = false;
        isStep3 = false;
        isStep4 = false;
        isStep5 = false;
        isThiefGone = false;
        didGameEnd = false;
        wasGameLost = false;
        wasRopeRaised = false;
        isReseting = false;
        areAllArtPiecesStolen = false;
        wasCodeIncorrect = false;
        numThiefEncounters = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure other scripts are performed
        isRoomGenerated = startPlane.GetComponent<FindVertices>().isRoomGenerated;
        areLightsSet = startSpotlight.GetComponent<LightingS>().areLightsSet;
        isMiniGameTriggered = startSpotlight.GetComponent<LightingS>().isMiniGameTriggered;
        isLightOff = startSpotlight.GetComponent<LightingS>().isLightOff;
        didGameEnd = cam2.GetComponent<CameraScript>().didGameEnd;

        // Run only once
        if (isRoomGenerated == true && isStopping == false && areLightsSet == true && didGameEnd == false)
        {
            artPiecesList = startPlane.GetComponent<FindVertices>().artPiecesList;
            lightList = startSpotlight.GetComponent<LightingS>().lightList;
            Debug.Log("light count: " + lightList.Count);

            // Select a random art piece
            artPiece = Random.Range(0, artPiecesList.Count);

            artPiecesLeft = artPiecesList.Count;

            isArtPieceRightSize = true;
            isStopping = true;
            isStep1 = true;
        }

        wasGameLost = miniGame1.GetComponent<MiniGame>().wasGameLost;

        if (wasGameLost == true)
        {
            isReseting = false;
        }

        wasCodeIncorrect = IDScanner.GetComponent<IDCheckScript>().wasGameLost;
        // Activate thief animation
        if (isRoomGenerated == true && didGameEnd == false && (wasGameLost == true /*|| wasThiefNotSpotted == true*/))
        {

            Debug.Log("THERE WITH THIEf: mini:" + isMiniGameTriggered + " isLightoff: " + isLightOff);
            if (isMiniGameTriggered == true && isLightOff == false && wasRopeRaised == true)
            {
                Debug.Log("Game triggered");
                if (artPiecesList.Count != 0)
                {
                    Debug.Log("Step1");
                    isStep1 = true;
                    isThiefAtPos = false;
                    isArtPieceRightSize = true;
                    wasRopeRaised = false;

                    numThiefEncounters++;

                    // Select a random art piece
                    artPiece = Random.Range(0, artPiecesList.Count);
                }
            }
        }

        
        if (isRoomGenerated == true && didGameEnd == false && (wasCodeIncorrect == true /*|| wasThiefNotSpotted == true*/))
        {
            if (isLightOff == false && wasRopeRaised == true)
            {
                Debug.Log("Game triggered");
                if (artPiecesList.Count != 0)
                {
                    Debug.Log("Step1");
                    isStep1 = true;
                    isThiefAtPos = false;
                    isArtPieceRightSize = true;
                    wasRopeRaised = false;

                    numThiefEncounters++;//

                    // Select a random art piece
                    artPiece = Random.Range(0, artPiecesList.Count);
                }
            }
        }
        

        // If all art pieces are stolen
        if (isRoomGenerated == true && didGameEnd == false && areLightsSet == true)
        {
            if (artPiecesList.Count == 0 && isLightOff == false)
                areAllArtPiecesStolen = true;
        }


        if (isStep1 == true)
        {
            // After lights turn off, start lowering the rope and set thief in position
            if (lightList[artPiece].intensity == 0)
            {
                rope.transform.position = new Vector3(artPiecesList[artPiece].transform.localPosition.x, rope.transform.localPosition.y, artPiecesList[artPiece].transform.localPosition.z + 3f);
                hole.transform.position = new Vector3(artPiecesList[artPiece].transform.localPosition.x, 20.2f, artPiecesList[artPiece].transform.localPosition.z + 3f);
                // Lower the rope
                if (rope.transform.localPosition.y > 15 && isRopeLowered == false)
                {
                    Vector3 ropePos = new Vector3(rope.transform.localPosition.x, rope.transform.localPosition.y - (4f * Time.deltaTime), rope.transform.localPosition.z);

                    rope.transform.localPosition = ropePos;
                }
                else
                {
                    isRopeLowered = true;

                    // Set thief at position
                    if (isThiefAtPos == false)
                    {
                        Vector3 thiefPos = new Vector3(artPiecesList[artPiece].transform.localPosition.x, artPiecesList[artPiece].transform.localPosition.y + 15f, artPiecesList[artPiece].transform.localPosition.z + 3.7f);
                        transform.LookAt(artPiecesList[artPiece].transform.localPosition);
                        transform.eulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                        transform.localPosition = thiefPos;
                        isThiefAtPos = true;

                        isStep1 = false;
                        isStep2 = true;
                    }
                }
            }
        }

        if (isStep2 == true)
        {
            // Lower the thief
            thiefAnimator.SetBool("areLightsOff", true);
            if (transform.localPosition.y > 0)
            {
                transform.LookAt(artPiecesList[artPiece].transform.localPosition);
                transform.eulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (6f * Time.deltaTime), transform.localPosition.z);
            }
            else
            {
                isStep2 = false;
                isStep3 = true;
            }
        }

        if (isStep3 == true)
        {
            // Make the art piece size small/dissapear
            if (thiefAnimator.GetCurrentAnimatorStateInfo(0).IsName("PutAwayObject"))
            {
                if (isArtPieceRightSize == true)
                {
                    artPiecesList[artPiece].transform.SetParent(hand.transform, false);
                    artPiecesList[artPiece].transform.localPosition = hand.transform.localPosition;
                    artPiecesList[artPiece].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    isArtPieceRightSize = false;
                }

                if (artPiecesList[artPiece].transform.localScale.x > 0)
                {
                    Vector3 artSize = new Vector3(artPiecesList[artPiece].transform.localScale.x - (0.45f * Time.deltaTime), artPiecesList[artPiece].transform.localScale.y - (0.45f * Time.deltaTime), artPiecesList[artPiece].transform.localScale.z - (0.45f * Time.deltaTime));
                    artPiecesList[artPiece].transform.localScale = artSize;
                }
                else
                {
                    isStep3 = false;
                    isStep4 = true;
                }
            }
        }

        if (isStep4 == true)
        {
            // Stop animation from repeating
            if (thiefAnimator.GetCurrentAnimatorStateInfo(0).IsName("ClimbUp"))
            {
                artPiecesList[artPiece].transform.localScale = new Vector3(0f, 0f, 0f);
                thiefAnimator.SetBool("areLightsOff", false);

                removeArtPiece = true;

                isStep4 = false;
                isStep5 = true;
            }
        }

        if (isStep5 == true)
        {
            // Raise the thief and remove art pieces
            if (removeArtPiece == true)
            {
                Destroy(artPiecesList[artPiece], 2);
                artPiecesList.RemoveAt(artPiece);
                artPiecesLeft--;
                removeArtPiece = false;
            }

            if (transform.localPosition.y < 20)
            {
                Debug.Log("WAITING FOR ROPE");
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + (6f * Time.deltaTime), transform.localPosition.z);
            }
            else
            {
                // Raise the rope
                if (rope.transform.localPosition.y < 30)
                {
                    Debug.Log("Rope is going up");
                    Vector3 ropePos = new Vector3(rope.transform.localPosition.x, rope.transform.localPosition.y + (5f * Time.deltaTime), rope.transform.localPosition.z);

                    rope.transform.localPosition = ropePos;
                }
                else
                {
                    hole.transform.position = new Vector3(0f, 30f, 0f);
                    wasRopeRaised = true;
                    isRopeLowered = false;
                    isStep5 = false;
                    isThiefGone = true;
                }
            }
        }
        
    }
}
