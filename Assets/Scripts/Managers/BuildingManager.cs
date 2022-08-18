using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject partToConstruct;
    public GameObject customCursor;
    public GameObject earth;
    public GameObject pipe;

    public string mode = "none";

    public List<GameObject> DynamicParts = new List<GameObject>();    
    // Start is called before the first frame update
    void Start()
    {
        
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
                    position = (v.normalized*-(127420f + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                    position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                    current.transform.SetParent(earth.transform);
                }

                if(partToConstruct.GetComponent<buildingType>().type == "GSEtank")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                    float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    position = (v.normalized*-(127420f + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                    position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                    //current.transform.SetParent(earth.transform);
                }

                if(partToConstruct.GetComponent<buildingType>().type == "launchPad")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 v = new Vector2(earth.transform.position.x, earth.transform.position.y) - position;
                    float lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    position = (v.normalized*-(127420f + partToConstruct.GetComponent<BoxCollider2D>().size.y/2));
                    position+= new Vector2(earth.transform.position.x, earth.transform.position.y);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.Euler(0f, 0f, lookAngle));
                    //current.transform.SetParent(earth.transform);
                }

                if(partToConstruct.GetComponent<buildingType>().type == "pipe")
                {
                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.identity);
                    //current.transform.SetParent(earth.transform);
                }

            }
            partToConstruct = null;
            Cursor.visible = true;
            customCursor.gameObject.SetActive(false);
        }

        if(Input.GetKey(KeyCode.C))
        {
            if(mode == "none")
            {
                mode = "connect";
                Debug.Log(mode);
                return;
            }
        }
    }

    public void Connect(GameObject output, GameObject input)
    {
        output.GetComponent<outputInputManager>().attachedOutput = input.GetComponent<outputInputManager>().input;
        input.GetComponent<outputInputManager>().attachedInput = output.GetComponent<outputInputManager>().output;
        Debug.Log(input.transform.position);
        Vector2 position = (output.GetComponent<outputInputManager>().output.transform.position + input.GetComponent<outputInputManager>().input.transform.position)/2;
        GameObject current = Instantiate(pipe, position, Quaternion.identity);
        float zRot = 0;
        if(input.GetComponent<outputInputManager>().input.transform.position.y > output.GetComponent<outputInputManager>().output.transform.position.y)
        {
            zRot = Vector2.Angle(input.GetComponent<outputInputManager>().input.transform.position, output.GetComponent<outputInputManager>().output.transform.position - current.transform.position)*-1;
        }

        if(input.GetComponent<outputInputManager>().input.transform.position.y < output.GetComponent<outputInputManager>().output.transform.position.y)
        {
            zRot = Vector2.Angle(output.GetComponent<outputInputManager>().output.transform.position- current.transform.position, input.GetComponent<outputInputManager>().input.transform.position);
        }

        Debug.Log(zRot);
        current.transform.rotation = Quaternion.Euler(new Vector3(0, 0, zRot+180));

        mode = "none";
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
}
