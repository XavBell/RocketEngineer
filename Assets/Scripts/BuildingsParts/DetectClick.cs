using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;



public class DetectClick : MonoBehaviour
{
    public GameObject input = null;
    public GameObject output = null;

    public BuildingManager buildingManager;
    public MasterManager MasterManager;

    public GameObject buildingUI;
    public GameObject fuelTankUI;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GM.GetComponent<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {

        CheckForRightClickOnBuilding();
        CheckForDestroy();

    }

    /// <summary>
    /// Checks for a right click on a building and performs the corresponding actions based on the building type.
    /// </summary>
    void CheckForRightClickOnBuilding()
    {
        if (Input.GetMouseButtonDown(1))
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
                        buildingUI.SetActive(false);
                        if (fuelTankUI.active == false)
                        {
                            fuelTankUI.SetActive(true);
                            buildingManager.hidePanels(fuelTankUI);
                            fuelTankUI.GetComponent<FuelTankMonitor>().target.text = "";
                            fuelTankUI.GetComponent<FuelTankMonitor>().container = current.GetComponent<container>();
                            fuelTankUI.GetComponent<FuelTankMonitor>().gasVent = current.GetComponent<gasVent>();
                            fuelTankUI.GetComponent<FuelTankMonitor>().cooler = current.GetComponent<cooler>();
                            PanelFadeIn(fuelTankUI);
                            return;
                        }
                        else
                        {
                            if(current.GetComponent<container>() == fuelTankUI.GetComponent<FuelTankMonitor>().container)
                            {
                                //Close panel
                                PanelFadeOut(fuelTankUI);
                                StartCoroutine(ActiveDeactive(0.1f, fuelTankUI, false));
                            }else{
                                //Update data for new tank
                                fuelTankUI.GetComponent<FuelTankMonitor>().target.text = "";
                                fuelTankUI.GetComponent<FuelTankMonitor>().container = current.GetComponent<container>();
                                fuelTankUI.GetComponent<FuelTankMonitor>().gasVent = current.GetComponent<gasVent>();
                                fuelTankUI.GetComponent<FuelTankMonitor>().cooler = current.GetComponent<cooler>();
                            }
                            return;
                        }
                    }

                    if (type == "VAB")
                    {
                        fuelTankUI.SetActive(false);
                        if (buildingUI.active == false)
                        {
                            buildingManager.hidePanels(buildingUI);
                            buildingUI.SetActive(true);
                            PanelFadeIn(buildingUI);
                            return;
                        }
                        else
                        {
                            PanelFadeOut(buildingUI);
                            StartCoroutine(ActiveDeactive(0.1f, buildingUI, false));
                            return;
                        }
                    }

                }

            }
        }

    }

    /// <summary>
    /// Checks if the building can be destroyed and handles the destruction logic.
    /// </summary>
    void CheckForDestroy()
    {
        if (buildingManager.CanDestroy == true)
        {
            if (Input.GetMouseButtonDown(0))
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

                        Destroy(current);
                        buildingManager.CanDestroy = false;

                    }

                }
            }
        }

    }

    private IEnumerator ActiveDeactive(float waitTime, GameObject panel, bool activated)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(activated);
    }

    public void PanelFadeIn(GameObject panel)
    {
        panel.transform.localScale = new Vector3(0, 0, 0);
        panel.transform.DOScale(1, 0.1f);
    }

    public void PanelFadeOut(GameObject panel)
    {
        panel.transform.DOScale(0, 0.1f);
        panel.transform.localScale = new Vector3(1, 1, 1);
    }
}
