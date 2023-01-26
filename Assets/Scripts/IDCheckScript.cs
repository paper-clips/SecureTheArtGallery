using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IDCheckScript : MonoBehaviour
{
    public GameObject IDScannerMain;
    public GameObject ScrapPaper;
    public GameObject IDCard;
    public GameObject button0Off;
    public GameObject button0On;
    public GameObject button1Off;
    public GameObject button1On;
    public GameObject button2Off;
    public GameObject button2On;
    public GameObject button3Off;
    public GameObject button3On;
    public GameObject button4Off;
    public GameObject button4On;
    public GameObject button5Off;
    public GameObject button5On;
    public GameObject button6Off;
    public GameObject button6On;
    public GameObject button7Off;
    public GameObject button7On;
    public GameObject button8Off;
    public GameObject button8On;
    public GameObject button9Off;
    public GameObject button9On;
    public Camera cam;
    public GameObject Code;
    bool isKeyPressed;
    bool isScrapPaperHeld;
    public GameObject ScrapPaperBody;
    public GameObject IDCardBody;
    bool isIDCardHeld;
    Vector3 IDCardPos;
    Vector3 ScrapPaperPos;
    Vector3 IDCardBodyPos;
    Vector3 ScrapPaperBodyPos;
    public TextMeshPro CodeOnScreen;
    Queue<char> UserCode;
    int codeCount;
    public TextMeshPro RandCode;
    public int randomCode;
    bool canTakeNewCodeInput;
    public GameObject CorrectIDScanner;
    public GameObject IncorrectIDScanner;
    bool isRoomGenerated;
    bool didGameEnd;
    public GameObject startPlane;
    public Camera cam2;
    public bool wasGameLost;
    public bool wasGameWon;
    bool isIDScannerTriggered;
    bool wasRopeRaised;
    bool areAllArtPiecesStolen;
    bool isReset;
    public GameObject Thief;
    bool isLightOff;
    public GameObject spotLight;
    bool isStep2;
    bool isPaused;
    float countDown;

    // Start is called before the first frame update
    void Start()
    {
        isKeyPressed = false;
        isScrapPaperHeld = false;
        isIDCardHeld = false;
        IDCardPos = IDCard.transform.position;
        IDCardBodyPos = IDCardBody.transform.position;
        ScrapPaperPos = ScrapPaper.transform.position;
        ScrapPaperBodyPos = ScrapPaperBody.transform.position;
        UserCode = new Queue<char>();
        for (int i = 0; i < 4; i++)
            UserCode.Enqueue(' ');
        codeCount = 0;
        randomCode = Random.Range(1000, 10000);
        RandCode.text = randomCode.ToString();
        canTakeNewCodeInput = true;
        isRoomGenerated = false;
        didGameEnd = false;
        wasGameWon = false;
        wasGameWon = false;
        isIDScannerTriggered = false;
        isReset = false;
        wasRopeRaised = false;
        isIDScannerTriggered = false;
        isPaused = false;
        countDown = 1.3f;
    }

    // Update is called once per frame
    void Update()
    {
        isRoomGenerated = startPlane.GetComponent<FindVertices>().isRoomGenerated;
        didGameEnd = cam2.GetComponent<CameraScript>().didGameEnd;

        isIDScannerTriggered = cam2.GetComponent<CameraScript>().isIDScannerTriggered;

        if (isPaused == false)
        {
            if (isRoomGenerated == true && didGameEnd == false && isIDScannerTriggered == true)
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (isKeyPressed == false)
                    {
                        Vector3 tempPos = button0Off.transform.position;
                        button0Off.transform.position = button0On.transform.position;
                        button0On.transform.position = tempPos;
                        isKeyPressed = true;
                    }
                    else
                    {
                        Vector3 tempPos = button0Off.transform.position;
                        button0Off.transform.position = button0On.transform.position;
                        button0On.transform.position = tempPos;
                        isKeyPressed = false;
                    }
                }

                // Change the position of the scrap paper as it's being held
                if (isScrapPaperHeld == true)
                {
                    Vector3 mousePosition = Input.mousePosition;
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.name == "ScrapPaperBody")
                        {
                            Debug.Log(hit.transform.name);
                            ScrapPaper.transform.position = new Vector3(hit.point.x, ScrapPaperPos.y + 0.1f, hit.point.z);
                            ScrapPaperBody.transform.position = new Vector3(hit.point.x, ScrapPaperBodyPos.y, hit.point.z);
                            ScrapPaperPos = new Vector3(ScrapPaper.transform.position.x, ScrapPaperPos.y, ScrapPaper.transform.position.z);
                            ScrapPaperBodyPos = new Vector3(ScrapPaperBody.transform.position.x, ScrapPaperBodyPos.y, ScrapPaperBody.transform.position.z);
                        }
                    }
                }
                // Change the position of the ID card as it's being held
                if (isIDCardHeld == true)
                {
                    Vector3 mousePosition = Input.mousePosition;
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.name == "IDCardBody")
                        {
                            Debug.Log(hit.transform.name);
                            IDCard.transform.position = new Vector3(hit.point.x, IDCardPos.y + 0.1f, hit.point.z);
                            IDCardBody.transform.position = new Vector3(hit.point.x, IDCardBodyPos.y, hit.point.z);
                            IDCardPos = new Vector3(IDCard.transform.position.x, IDCardPos.y, IDCard.transform.position.z);
                            IDCardBodyPos = new Vector3(IDCardBody.transform.position.x, IDCardBodyPos.y, IDCardBody.transform.position.z);
                        }
                    }
                }

                // If user lifts left mouse button, drop scrapbook paper/ID card to current position
                if (Input.GetMouseButtonUp(0))
                {
                    isScrapPaperHeld = false;
                    isIDCardHeld = false;
                    IDCard.transform.position = new Vector3(IDCardPos.x, IDCardPos.y, IDCardPos.z);
                    IDCardBody.transform.position = new Vector3(IDCardBodyPos.x, IDCardBodyPos.y, IDCardBodyPos.z);
                    ScrapPaper.transform.position = new Vector3(ScrapPaperPos.x, ScrapPaperPos.y, ScrapPaperPos.z);
                    ScrapPaperBody.transform.position = new Vector3(ScrapPaperBodyPos.x, ScrapPaperBodyPos.y, ScrapPaperBodyPos.z);
                    Vector3 randPos = new Vector3(18, 0, -200);
                    button0On.transform.position = randPos;
                    button1On.transform.position = randPos;
                    button2On.transform.position = randPos;
                    button3On.transform.position = randPos;
                    button4On.transform.position = randPos;
                    button5On.transform.position = randPos;
                    button6On.transform.position = randPos;
                    button7On.transform.position = randPos;
                    button8On.transform.position = randPos;
                    button9On.transform.position = randPos;

                    // Prevent ID scanner from taking more than 4 inputs
                    if (codeCount <= 4)
                    {
                        IncorrectIDScanner.transform.localPosition = new Vector3(0.5f, -1f, 0f);
                        Debug.Log("HERE1" + codeCount);
                        string tempCode = "";
                        foreach (char c in UserCode)
                            tempCode = tempCode + c.ToString();
                        CodeOnScreen.text = tempCode;
                        if (codeCount == 4)
                        {
                            canTakeNewCodeInput = false;
                            codeCount++;
                        }
                    }
                    else
                    {
                        Debug.Log("HERE");
                        if (isCardOnScanner())
                        {
                            // Check if user code is correct
                            if (isUserCodeCorrect() == true)
                            {
                                //wasGameWon = true;
                                isPaused = true;
                            }
                            else
                            {
                                //wasGameLost = true;
                                isPaused = true;
                            }
                        }
                        else
                            canTakeNewCodeInput = false;
                    }
                }

                // If user presses down with left mouse button
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        // Check what the user clicked on
                        Debug.Log(hit.transform.name);
                        if (hit.transform.name == "ScrapPaperBody")
                        {
                            isScrapPaperHeld = true;
                        }
                        else if (hit.transform.name == "IDCardBody")
                        {
                            isIDCardHeld = true;
                        }
                        else if (hit.transform.name == "Button0" && canTakeNewCodeInput == true)
                        {
                            button0On.transform.position = button0Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('0');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button1" && canTakeNewCodeInput == true)
                        {
                            button1On.transform.position = button1Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('1');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button2" && canTakeNewCodeInput == true)
                        {
                            button2On.transform.position = button2Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('2');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button3" && canTakeNewCodeInput == true)
                        {
                            button3On.transform.position = button3Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('3');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button4" && canTakeNewCodeInput == true)
                        {
                            button4On.transform.position = button4Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('4');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button5" && canTakeNewCodeInput == true)
                        {
                            button5On.transform.position = button5Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('5');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button6" && canTakeNewCodeInput == true)
                        {
                            button6On.transform.position = button6Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('6');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button7" && canTakeNewCodeInput == true)
                        {
                            button7On.transform.position = button7Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('7');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button8" && canTakeNewCodeInput == true)
                        {
                            button8On.transform.position = button8Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('8');
                            codeCount++;
                        }
                        else if (hit.transform.name == "Button9" && canTakeNewCodeInput == true)
                        {
                            button9On.transform.position = button9Off.transform.position;
                            UserCode.Dequeue();
                            UserCode.Enqueue('9');
                            codeCount++;
                        }
                    }
                }
            }

            isRoomGenerated = startPlane.GetComponent<FindVertices>().isRoomGenerated;
            didGameEnd = cam2.GetComponent<CameraScript>().didGameEnd;
            wasRopeRaised = Thief.GetComponent<ThiefAnimation>().wasRopeRaised;
            areAllArtPiecesStolen = Thief.GetComponent<ThiefAnimation>().areAllArtPiecesStolen;

            // Reset everything after first playthrough
            if (isRoomGenerated == true && didGameEnd == false)
            {
                isStep2 = Thief.GetComponent<ThiefAnimation>().isStep2;
                if (((isStep2 == true && wasGameLost == true) || wasGameWon == true))
                {
                    Debug.Log("Resetting minigame2");
                    resetEverything();
                }
            }
            if (isRoomGenerated == true && didGameEnd == false)
            {
                isStep2 = Thief.GetComponent<ThiefAnimation>().isStep2;
                if (isIDScannerTriggered == false && wasGameLost == true && isStep2 == true)
                {
                    Debug.Log("Falsing game");
                    wasGameLost = false;

                }
                else if (isIDScannerTriggered == false)
                {
                    wasGameWon = false;
                }
            }
        }
        else
        {
            if (countDown > 0)
            {
                countDown = countDown - (1 * Time.deltaTime);
            }
            else 
            {
                if (isUserCodeCorrect() == true)
                {
                    if (isReset == false)
                    {
                        wasGameWon = true;
                        isPaused = false;


                        isReset = true;
                    }
                }
                else
                {
                    if (isReset == false)
                    {
                        wasGameLost = true;
                        isPaused = false;


                        isReset = true;
                    }
                }
            }
        }
    }

    public void resetEverything()
    {
        Debug.Log("RESETTING EVERYTHING");
        ScrapPaper.transform.localPosition = new Vector3(-0.18f, 0.1f, 5.09f);
        IDCard.transform.localPosition = new Vector3(-0.19f, 0.1f, -5.19f);

        IDCardPos = IDCard.transform.position;
        IDCardBodyPos = IDCardBody.transform.position;
        ScrapPaperPos = ScrapPaper.transform.position;
        ScrapPaperBodyPos = ScrapPaperBody.transform.position;

        randomCode = Random.Range(1000, 10000);
        RandCode.text = randomCode.ToString();
        isPaused = false;
        countDown = 1.3f;

        //Reset green/red ID scanner
        CorrectIDScanner.transform.localPosition = new Vector3(0.361f, -1.097f, -0.001f);
        IncorrectIDScanner.transform.localPosition = new Vector3(-0.375f, -1.104f, -0.001f);

        // Reset ID scanner code
        UserCode.Dequeue();
        UserCode.Dequeue();
        UserCode.Dequeue();
        char lastInput = UserCode.Dequeue();
        UserCode.Enqueue(' ');
        UserCode.Enqueue(' ');
        UserCode.Enqueue(' ');
        UserCode.Enqueue(' ');//
        string tempCode = "";
        foreach (char c in UserCode)
            tempCode = tempCode + c.ToString();
        CodeOnScreen.text = tempCode;
        canTakeNewCodeInput = true;
        codeCount = 0;

        isReset = false;

    }

    // Check if the ID card is on the scanner
    public bool isCardOnScanner()
    {
        Debug.Log(IDCard.transform.localPosition);
        if (IDCard.transform.localPosition.x <= 0.12f && IDCard.transform.localPosition.x >= -1.2f && IDCard.transform.localPosition.z <= -1.2f && IDCard.transform.localPosition.z >= -2.3f)
        {
            Debug.Log("Card is on Scanner");
            return true;
        }
        return false;
    }
    
    // Check if the user code is correct
    public bool isUserCodeCorrect()
    {
        string code = "";
        foreach (char u in UserCode)
            code = code + u.ToString();

        Debug.Log("Random code: " + randomCode.ToString() + " User code: " + code);

        if (randomCode.ToString() == code)
        {
            Debug.Log("Code is correct | ran: " + randomCode.ToString() + " code: " + code);
            CorrectIDScanner.transform.localPosition = new Vector3(0, 0, -0.001f);
            return true;
        }
        else
        {
            Debug.Log("Code is NOT correct | ran: " + randomCode.ToString() + " code: " + code);
            IncorrectIDScanner.transform.localPosition = new Vector3(0, 0, -0.001f);
            return false;
        }
    }
}
