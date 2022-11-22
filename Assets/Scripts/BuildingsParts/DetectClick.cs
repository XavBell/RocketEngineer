using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DetectClick : MonoBehaviour
{
    public GameObject input = null;
    public GameObject output = null;

    public BuildingManager buildingManager;
    public MasterManager MasterManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GM.GetComponent<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(buildingManager.localMode == "connect")
        {
            if(Input.GetMouseButtonDown(0) )
            {
                RaycastHit2D raycastHit;
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 ray = new Vector2(mousePos.x, mousePos.y);
                raycastHit = Physics2D.Raycast(ray, Vector2.zero);
           
                    if (raycastHit.transform != null)
                    {
                        if(raycastHit.transform.gameObject.GetComponent<buildingType>())
                        {
                            string type = raycastHit.transform.gameObject.GetComponent<buildingType>().type;
                            GameObject current = raycastHit.transform.gameObject;
                            if(output == null)
                            {
                                if(current.GetComponent<outputInputManager>().attachedOutput == null)
                                {
                                    output = current;
                                }
                                return;
                            }

                            if(output != null)
                            {
                                if(current.GetComponent<outputInputManager>().attachedInput == null)
                                {
                                    input = current;
                                    buildingManager.Connect(output, input);
                                    output = null;
                                    input = null;
                                }
                            }
                        }
                    }
             
            }
        }

        if(buildingManager.localMode == "none")
        {
            if(Input.GetMouseButtonDown(0))
            {
                RaycastHit2D raycastHit;
                Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 ray = new Vector2(cameraPos.x, cameraPos.y);
                raycastHit = Physics2D.Raycast(ray, Vector2.zero);
                Debug.Log(ray);
                if (raycastHit.transform != null)
                {
                    Debug.Log(raycastHit.transform.gameObject);
                    if(raycastHit.transform.gameObject.GetComponent<buildingType>())
                    {
                        
                        GameObject current = raycastHit.transform.gameObject;
                        string type = current.GetComponent<buildingType>().type;
                        if(type == "designer")
                        {
                            GameObject panel = current.GetComponent<designerManager>().Panel;
                            if(panel.active == false)
                            {
                                panel.active = true;
                            }
                     
                        }

                        if(type == "launchPad")
                        {
                            Debug.Log("Active");
                            GameObject panel = current.GetComponent<launchPadManager>().Panel;
                            if(panel.active == false)
                            {
                                panel.active = true;

                            }
                            current.GetComponent<outputInputManager>().log = true; 
                        }

                        if(type == "GSEtank")
                        {
                            current.GetComponent<outputInputManager>().log = true; 
                        }
                            
                    }

                    if(raycastHit.transform.gameObject.GetComponent<PlanetGravity>())
                    {
                        
                        GameObject current = raycastHit.transform.gameObject;
                        Debug.Log("clicked on rocket");
                        current.GetComponent<PlanetGravity>().possessed = true;
                        MasterManager.gameState = "Flight";
                        MasterManager.ActiveRocket = current;
                            
                    }
                      
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                RaycastHit2D raycastHit;
                Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 ray = new Vector2(cameraPos.x, cameraPos.y);
                raycastHit = Physics2D.Raycast(ray, -Vector2.up);

                if (raycastHit.transform != null)
                {
                    if(raycastHit.transform.gameObject.GetComponent<buildingType>())
                    {
                        
                        GameObject current = raycastHit.transform.gameObject;
                        string type = current.GetComponent<buildingType>().type;
                        if(type == "designer")
                        {
                            GameObject panel = current.GetComponent<designerManager>().Panel;
                            if(panel.active == true)
                            {
                                panel.active = false;
                            }
                            
                        }

                        if(type == "launchPad")
                        {
                            Debug.Log("Active");
                            GameObject panel = current.GetComponent<launchPadManager>().Panel;
                            if(panel.active == true)
                            {
                                panel.active = false;
                            }
                            current.GetComponent<outputInputManager>().log = false; 
                        }

                        if(type == "GSEtank")
                        {
                            current.GetComponent<outputInputManager>().log = false; 
                        }
                            
                    }

                    if(raycastHit.transform.gameObject.GetComponent<PlanetGravity>())
                    {
                        
                        GameObject current = raycastHit.transform.gameObject;
                        if(current = MasterManager.ActiveRocket)
                        {
                            current.GetComponent<PlanetGravity>().possessed = false;
                            MasterManager.gameState = "Building";
                            MasterManager.ActiveRocket = null;
                        }
                    }
                      
                }
            }
        }
    }
}
