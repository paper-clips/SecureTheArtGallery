using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroToGame : MonoBehaviour
{
    public bool wasStartClicked;
    public Camera cam;
    public GameObject startBtn;
    public GameObject instructionsBtn;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;
    public GameObject actualInstructions;
    bool isStopped;
    public GameObject paintingCover;
    public GameObject HideIntro;
    public GameObject InputText;
    public GameObject fallingPainting;

    // Start is called before the first frame update
    void Start()
    {
        wasStartClicked = false;
        cam1.enabled = false;
        cam2.enabled = false;
        cam3.enabled = false;
        cam4.enabled = false;
        isStopped = false;
        InputText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If user clicks left mouse button
        if (Input.GetMouseButtonUp(0) && isStopped == false)
        {
            Vector3 mousePosition = Input.mousePosition;
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.transform.name);
                // If press start, start the game
                if (hit.transform.name == "StartBtn")
                {
                    Debug.Log("Start Game");
                    cam.enabled = false;
                    cam2.enabled = true;
                    isStopped = true;
                    wasStartClicked = true;
                    InputText.SetActive(false);
                }
                // If press Instructions button, show instructions
                else if (hit.transform.name == "InstructionsBtn")
                {
                    Debug.Log("Instructions");
                    actualInstructions.transform.localPosition = new Vector3(actualInstructions.transform.localPosition.x, 0.016f, actualInstructions.transform.localPosition.z);
                }
                // If the instructions are shown, clicking on them again will make them dissapear
                else if (hit.transform.name == "InstructionsBody")
                {
                    Debug.Log("Remove Instructions");
                    actualInstructions.transform.localPosition = new Vector3(actualInstructions.transform.localPosition.x, -0.6f, actualInstructions.transform.localPosition.z);
                }
                else if (hit.transform.name == "PaintingCover")
                {
                    Debug.Log("Remove Painting");
                    paintingCover.transform.localPosition = new Vector3(1f, 0.035f, -0.00001f);
                    HideIntro.transform.localPosition = new Vector3(0, 0, -0.01f);
                    InputText.SetActive(true);
                    fallingPainting.transform.localPosition = new Vector3(-0.179f, -0.031f, -0.011f);
                }
            }
        }
    }
}
