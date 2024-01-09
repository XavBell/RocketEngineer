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
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit2D raycastHit;
            Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 ray = new Vector2(cameraPos.x, cameraPos.y);
            raycastHit = Physics2D.Raycast(ray, -Vector2.up);
            if (raycastHit.transform != null)
            {
                if (raycastHit.transform.gameObject.GetComponent<buildingType>())
                {

                    GameObject current = raycastHit.transform.gameObject;
                    string type = current.GetComponent<buildingType>().type;

                    if (type == "GSEtank")
                    {
                        if (current.GetComponent<buildingType>().UI.active == false)
                        {
                            current.GetComponent<buildingType>().UI.SetActive(true);
                            return;
                        }else{
                            current.GetComponent<buildingType>().UI.SetActive(false);
                        }
                    }

                }

            }
        }
    }
}
