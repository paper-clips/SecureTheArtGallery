using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LightingS : MonoBehaviour
{
    public bool isMiniGameTriggered;
    public bool isLightOff;
    List<GameObject> artPiecesList;
    public GameObject startPlane;
    public Light spotLight;
    public List<Light> lightList;
    bool isStopping;
    bool isRoomGenerated;
    public GameObject lightParent;
    public bool areLightsSet;
    public TextMeshProUGUI LightIndicator;
    public TextMeshProUGUI alert;
    bool isAlertOn;
    float alertCounter;
    bool didGameEnd;
    public Camera cam2;
    bool wasGameLost;
    public GameObject miniGame1;
    bool turnOffLights;
    bool wasRopeRaised;
    public GameObject Thief;
    bool wasGameWon;
    public bool isLightOn;
    bool isGamePlaying;
    bool isGameOver;
    public GameObject IDScanner;
    bool wasCodeIncorrect;
    bool isIDScannerTriggered;
    //bool wasThiefNotSpotted;

    // Start is called before the first frame update
    void Start()
    {
        isMiniGameTriggered = false;
        lightList = new List<Light>();
        isLightOff = false;
        isStopping = false;
        areLightsSet = false;
        LightIndicator.text = "";
        alert.text = "";
        isAlertOn = false;
        alertCounter = 0;
        didGameEnd = false;
        wasGameLost = false;
        turnOffLights = false;
        wasRopeRaised = false;
        wasGameWon = false;
        isLightOn = false;
        wasCodeIncorrect = false;
        isIDScannerTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        wasGameLost = miniGame1.GetComponent<MiniGame>().wasGameLost;
        wasGameWon = miniGame1.GetComponent<MiniGame>().wasGameWon;
        isRoomGenerated = startPlane.GetComponent<FindVertices>().isRoomGenerated;
        didGameEnd = cam2.GetComponent<CameraScript>().didGameEnd;
        wasRopeRaised = Thief.GetComponent<ThiefAnimation>().wasRopeRaised;

        // Runs once, adds all lights
        if (didGameEnd == false && isRoomGenerated == true)
        {
            if (isStopping == false)
            {
                Debug.Log("Adding lights");
                addLights();
                LightIndicator.text = "Lights: On";
                isStopping = true;
            }
        }

        if (didGameEnd == true)
        {
            alert.text = "";
            LightIndicator.text = "";
        }

        if (didGameEnd == false && isRoomGenerated == true)
        {
            isMiniGameTriggered = cam2.GetComponent<CameraScript>().isMiniGameTriggered;
            isGamePlaying = miniGame1.GetComponent<MiniGame>().isGamePlaying;
            isIDScannerTriggered = cam2.GetComponent<CameraScript>().isIDScannerTriggered;

            // Hides UI when playing game
            if (isGamePlaying == true || isIDScannerTriggered == true)
                LightIndicator.text = "";
            else
            {
                // Shows lights on indicator once mini game has ended
                if (isLightOff == false)
                    LightIndicator.text = "Lights: On";
            }
            isGameOver = miniGame1.GetComponent<MiniGame>().isReset;
            wasCodeIncorrect = IDScanner.GetComponent<IDCheckScript>().wasGameLost;

            // Activates alert if game was lost
            if (wasGameLost == true || wasCodeIncorrect == true /*|| wasThiefNotSpotted == true*/)
            {
                Debug.Log("LOST! wasGameLost: " + wasGameLost + " wasCodeIncorrect: " + wasCodeIncorrect);
                isAlertOn = true;
            }
            else
            {
                isAlertOn = false;
                alert.text = "";
            }

            // Show alert on screen every 2 seconds
            if (isAlertOn == true)
            {
                if ((int)alertCounter % 2 == 0)
                    alert.text = "";
                else
                    alert.text = "Alert";

                alertCounter = alertCounter + (2 * Time.deltaTime);
            }

            // Turn off lights
            if (isLightOff == false && (wasGameLost == true || wasCodeIncorrect == true /*|| wasThiefNotSpotted == true*/))
            {
                // Slowly turn off lights
                if (lightList[0].intensity > 0)
                {
                    foreach (Light l in lightList)
                        l.intensity -= 2.5f * Time.deltaTime;
                }
                else
                {
                    LightIndicator.text = "Lights: Off";
                    isLightOn = false;
                    isLightOff = true;
                }
            }
            else if (isLightOff == true && wasRopeRaised == true)        // Turn on lights
            {
                // Slowly turn on lights
                if (lightList[0].intensity < 20)
                {
                    foreach (Light l in lightList)
                        l.intensity += 2.5f * Time.deltaTime;
                }
                else
                {
                    LightIndicator.text = "Lights: On";
                    isLightOn = true;
                    isLightOff = false;
                }
            }
        }
        else
        {
            LightIndicator.text = "";
            alert.text = "";
        }

    }

    // Add spotlights above art pieces
    public void addLights()
    {
        artPiecesList = startPlane.GetComponent<FindVertices>().artPiecesList;
        Debug.Log("Number of art pieces: " + artPiecesList.Count);

        // Spotlight 1
        Light tempLight = spotLight;
        GameObject a = artPiecesList[0];
        Vector3 tempPos = new Vector3(a.transform.position.x, a.transform.position.y + 20, a.transform.position.z);
        tempLight.transform.position = tempPos;
        tempLight.transform.SetParent(lightParent.transform, false);
        tempLight.name = "l0";
        lightList.Add(tempLight);

        // Other spotlights
        for(int i = 1; i < artPiecesList.Count; i++)
        {
            a = artPiecesList[i];
            tempLight = Light.Instantiate(spotLight);
            tempPos = new Vector3(a.transform.position.x, a.transform.position.y + 20, a.transform.position.z);
            tempLight.transform.position = tempPos;
            tempLight.transform.SetParent(lightParent.transform, false);
            tempLight.name = "l" + i;
            lightList.Add(tempLight);
        }
        areLightsSet = true;
    }
}
