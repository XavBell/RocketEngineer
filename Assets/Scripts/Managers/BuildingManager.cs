using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject partToConstruct;
    public GameObject customCursor;
    public GameObject earth;

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
                    current.transform.SetParent(earth.transform);
                }

                if(partToConstruct.GetComponent<buildingType>().type == "pipe")
                {
                    GameObject closest = null;
                    GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
                    float lookAngle;
                    Vector2 v;
                    foreach(GameObject building in buildings)
                    {
                        if(building.GetComponent<outputInputManager>())
                        {
                            DynamicParts.Add(building);
                        }   
                    }

                    Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector3 rotator = new Vector3(0f, 0f, customCursor.GetComponent<CustomCursor>().zRot);
                    Debug.Log(customCursor.GetComponent<CustomCursor>().zRot);
                    GameObject current = Instantiate(partToConstruct, position, Quaternion.identity);
                    current.transform.eulerAngles = rotator;

                    float bestDistance = Mathf.Infinity;
                    foreach(GameObject dynamicPart in DynamicParts)
                    {
                        float currentDistance = Vector2.Distance(current.transform.position, dynamicPart.transform.position);
                        if(currentDistance < bestDistance)
                        {
                            bestDistance = currentDistance;
                            closest = dynamicPart;
                        }
                    }

                    if(closest != null)
                    {
                        float inputOutputDistance = Mathf.Infinity;
                        float outputInputDistance = Mathf.Infinity;

                        float initialDistance = Vector2.Distance(earth.transform.position, current.transform.position);

                        if(closest.GetComponent<outputInputManager>().attachedInput == null && closest.GetComponent<outputInputManager>().input != null)
                        {
                            inputOutputDistance = Vector2.Distance(closest.GetComponent<outputInputManager>().input.transform.position, current.GetComponent<outputInputManager>().output.transform.position);
                        }

                        if(closest.GetComponent<outputInputManager>().attachedOutput == null && closest.GetComponent<outputInputManager>().output != null)
                        {
                            outputInputDistance = Vector2.Distance(closest.GetComponent<outputInputManager>().output.transform.position, current.GetComponent<outputInputManager>().input.transform.position);
                        }

                        if(inputOutputDistance < outputInputDistance)
                        {
                            
                            Vector2 difference = closest.GetComponent<outputInputManager>().input.transform.position - current.GetComponent<outputInputManager>().output.transform.position;
                            current.transform.position+= new Vector3(difference.x, difference.y, 0);
                            current.transform.eulerAngles = rotator;
                        }

                        if(inputOutputDistance > outputInputDistance)
                        {
                            Vector2 difference = closest.GetComponent<outputInputManager>().output.transform.position - current.GetComponent<outputInputManager>().input.transform.position;
                            current.transform.position+= new Vector3(difference.x, difference.y, 0);
                            current.transform.eulerAngles = rotator;
                        }
                    }

                    current.transform.SetParent(earth.transform);
                }

            }
            partToConstruct = null;
            Cursor.visible = true;
            customCursor.gameObject.SetActive(false);
        }
        
    }

    public void ConstructPart(GameObject part)
    {
        if(Cursor.visible == true)
        {
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = part.GetComponent<SpriteRenderer>().sprite;
            customCursor.GetComponent<SpriteRenderer>().size = part.GetComponent<SpriteRenderer>().size;
            customCursor.GetComponent<SpriteRenderer>().color = Color.green;
            Cursor.visible = false;
            customCursor.GetComponent<CustomCursor>().type = part.GetComponent<buildingType>().type;
            partToConstruct = part;
        }
    }
}
