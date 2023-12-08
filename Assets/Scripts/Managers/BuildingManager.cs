using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingManager : MonoBehaviour
{
    public GameObject partToConstruct;
    public GameObject customCursor;
    public GameObject earth;
    public GameObject moon;
    public GameObject pipe;
    public GameObject menu;

    public MasterManager MasterManager;
    public WorldSaveManager WorldSaveManager;

    public float planetRadius = 6371;

    public string localMode = "none";

    public int IDMax = 0;

    public List<GameObject> DynamicParts = new List<GameObject>();    
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GMM.GetComponent<MasterManager>();

        GameObject GWS = GameObject.FindGameObjectWithTag("WorldSaveManager");
        WorldSaveManager = GWS.GetComponent<WorldSaveManager>();

        customCursor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && partToConstruct != null)
        {
            if (partToConstruct != null && Cursor.visible == false && customCursor.GetComponent<CustomCursor>().constructionAllowed == true)
            {
                if(partToConstruct.GetComponent<buildingType>().type == "designer")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                    float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    position = (v.normalized*-(planetRadius + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                    position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                    current.transform.SetParent(earth.transform);
                    current.GetComponent<buildingType>().buildingID = IDMax+1;
                    IDMax += 1;
                }

                if(partToConstruct.GetComponent<buildingType>().type == "GSEtank")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                    float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    position = (v.normalized*-(planetRadius + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                    position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                    current.transform.SetParent(earth.transform);
                    current.GetComponent<buildingType>().buildingID = IDMax+1;
                    IDMax += 1;
                }

                if(partToConstruct.GetComponent<buildingType>().type == "launchPad")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                    float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    position = (v.normalized*-(planetRadius + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                    position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                    current.transform.SetParent(earth.transform);
                    current.GetComponent<buildingType>().buildingID = IDMax+1;
                    IDMax += 1;
                }

                if(partToConstruct.GetComponent<buildingType>().type == "pipe")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.eulerAngles = new Vector3(0, 0, customCursor.GetComponent<CustomCursor>().zRot);
                    current.GetComponent<buildingType>().buildingID = IDMax+1;
                    IDMax += 1;
                }

            }
            partToConstruct = null;
            Cursor.visible = true;
            customCursor.gameObject.SetActive(false);
        }

        if(Input.GetKey(KeyCode.C))
        {
            if(localMode == "none")
            {
                localMode = "connect";
                return;
            }
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            WorldSaveManager.saveTheWorld();
            Destroy(MasterManager.gameObject);
            SceneManager.LoadScene("Menu");
        }
    }

    public void Connect(GameObject output, GameObject input)
    {
        output.GetComponent<outputInputManager>().outputParentID = input.GetComponent<outputInputManager>().selfID;
        output.GetComponent<outputInputManager>().outputParent = input;
        input.GetComponent<outputInputManager>().inputParentID = output.GetComponent<outputInputManager>().selfID;
        input.GetComponent<outputInputManager>().inputParent = output;
        localMode = "none";
    }

    public void ConstructPart(GameObject part)
    {
        if(Cursor.visible == true)
        {
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = part.GetComponent<SpriteRenderer>().sprite;
            customCursor.GetComponent<SpriteRenderer>().size = part.GetComponent<SpriteRenderer>().size;
            customCursor.GetComponent<CustomCursor>().defaultColor = part.GetComponent<SpriteRenderer>().color;
            customCursor.GetComponent<SpriteRenderer>().color = customCursor.GetComponent<CustomCursor>().defaultColor;
            Cursor.visible = false;
            customCursor.GetComponent<CustomCursor>().type = part.GetComponent<buildingType>().type;
            partToConstruct = part;
        }
    }

    public void toggleBuildMenu()
    {
        if(menu.activeSelf == true)
        {
            menu.SetActive(false);
            return;
        }

        if(menu.activeSelf == false)
        {
            menu.SetActive(true);
            return;
        }
    }
}
