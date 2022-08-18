using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DetectClick : MonoBehaviour
{
    public GameObject input = null;
    public GameObject output = null;

    public BuildingManager buildingManager;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if(buildingManager.mode == "connect"){
       if( Input.GetMouseButtonDown(0) )
        {
            RaycastHit2D raycastHit;
            Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 ray = new Vector2(cameraPos.x, cameraPos.y);
            raycastHit = Physics2D.Raycast(ray, -Vector2.up);
           
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
    }
        
        
    

    
}
