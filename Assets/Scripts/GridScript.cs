using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public Material GridMaterial;
    public GameObject Cover;
    List<Material> materialList;
    bool isStopping;
    bool didGameStart;
    bool didGameEnd;
    public GameObject StartScreenScript;
    public GameObject CameraScript;
    public GameObject StartPlane;
    bool isRoomGenerated;
    List<GameObject> hiddenObjects;

    // Start is called before the first frame update
    void Start()
    {
        materialList = new List<Material>();
        isStopping = false;
        didGameStart = false;
        didGameEnd = false;
        isRoomGenerated = false;
        hiddenObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        didGameStart = StartScreenScript.GetComponent<IntroToGame>().wasStartClicked;
        didGameEnd = CameraScript.GetComponent<CameraScript>().didGameEnd;
        isRoomGenerated = StartPlane.GetComponent<FindVertices>().isRoomGenerated;

        if (isRoomGenerated == true && didGameStart == true && isStopping == false)
        {
            // Save materials
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Grid"))
            {
                materialList.Add(obj.GetComponent<Renderer>().material);
            }

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hide"))
            {
                hiddenObjects.Add(obj);
            }

            isStopping = true;
        }

        if (isRoomGenerated == true && didGameStart == true && didGameEnd == false)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))      // Turn materials into grid
            {
                // Add cover
                Cover.transform.localScale = new Vector3(30, 1, 30);
                Cover.transform.position = new Vector3(-5.8f, 20.2f, 122.1f);


                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Grid"))
                {
                    obj.GetComponent<Renderer>().material = GridMaterial;
                }
                foreach (GameObject obj in hiddenObjects)
                {
                    obj.SetActive(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))   // Turn grid material back to original material
            {
                // Add cover
                Cover.transform.localScale = new Vector3(0, 0, 0);
                Cover.transform.position = new Vector3(-5.8f, 20.2f, 122.1f);

                int index = 0;
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Grid"))
                {
                    obj.GetComponent<Renderer>().material = materialList[index];
                    index++;
                }
                foreach (GameObject obj in hiddenObjects)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
