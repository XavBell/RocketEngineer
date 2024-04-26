using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;



public class DetectClick : MonoBehaviour
{
    public GameObject input = null;
    public GameObject output = null;

    public BuildingManager buildingManager;
    public MasterManager MasterManager;

    public GameObject buildingUI;
    public GameObject fuelTankUI;
    public GameObject operationUI;
    public FuelConnectorManager fuelConnectorManager;
    public TMP_Dropdown operationDropdown;
    public GameObject designUI;

    public GameObject testPlanet;
    public GameObject debugObject;
    public bool test = false; //For testing purposes

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

        if(test)
        {
            checkForLeftClick();
        }

    }

    void checkForLeftClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit;
            Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 ray = camPos;
            Vector2 destination = new Vector2(testPlanet.transform.position.x, testPlanet.transform.position.y);
            raycastHit = Physics2D.Raycast(ray, destination - ray);
            if(raycastHit.transform != null)
            {
                Debug.Log(raycastHit.point);
                GameObject GO = Instantiate(debugObject, raycastHit.point, Quaternion.identity);
            }
        }
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
            Vector2 ray = cameraPos;

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
                        if (!fuelTankUI.activeSelf)
                        {
                            fuelTankUI.SetActive(true);
                            buildingManager.hidePanels(fuelTankUI);
                            fuelTankUI.GetComponent<FuelTankMonitor>().target.text = "";
                            fuelTankUI.GetComponent<FuelTankMonitor>().container = current.GetComponent<container>();
                            fuelTankUI.GetComponent<FuelTankMonitor>().gasVent = current.GetComponent<gasVent>();
                            fuelTankUI.GetComponent<FuelTankMonitor>().cooler = current.GetComponent<cooler>();
                            if (fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<gasVent>() != null)
                            {
                                fuelTankUI.GetComponent<FuelTankMonitor>().ventToggle.isOn = fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<gasVent>().open;
                            }
                            if (fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<cooler>() != null)
                            {
                                fuelTankUI.GetComponent<FuelTankMonitor>().coolerToggle.isOn = fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<cooler>().active;
                            }
                            PanelFadeIn(fuelTankUI);
                            return;
                        }
                        else
                        {
                            if (current.GetComponent<container>() == fuelTankUI.GetComponent<FuelTankMonitor>().container)
                            {
                                //Close panel
                                PanelFadeOut(fuelTankUI);
                                StartCoroutine(ActiveDeactive(0.1f, fuelTankUI, false));
                            }
                            else
                            {
                                //Update data for new tank
                                fuelTankUI.GetComponent<FuelTankMonitor>().target.text = "";
                                fuelTankUI.GetComponent<FuelTankMonitor>().container = current.GetComponent<container>();
                                fuelTankUI.GetComponent<FuelTankMonitor>().gasVent = current.GetComponent<gasVent>();
                                fuelTankUI.GetComponent<FuelTankMonitor>().cooler = current.GetComponent<cooler>();
                                if (fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<gasVent>() != null)
                                {
                                    fuelTankUI.GetComponent<FuelTankMonitor>().ventToggle.isOn = fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<gasVent>().open;
                                }
                                if (fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<cooler>() != null)
                                {
                                    fuelTankUI.GetComponent<FuelTankMonitor>().coolerToggle.isOn = fuelTankUI.GetComponent<FuelTankMonitor>().container.GetComponent<cooler>().active;
                                }
                            }
                            return;
                        }
                    }

                    if (type == "VAB")
                    {
                        fuelTankUI.SetActive(false);
                        if (!buildingUI.activeSelf)
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

                    if (type == "launchPad" || type == "staticFireStand" || type == "standTank")
                    {
                        if (!operationUI.activeSelf)
                        {
                            buildingManager.hidePanels(operationUI);
                            operationUI.SetActive(true);
                            PanelFadeIn(operationUI);
                            if (type == "launchPad")
                            {
                                operationDropdown.value = 0;
                            }
                            if (type == "staticFireStand")
                            {
                                operationDropdown.value = 1;
                            }
                            if (type == "standTank")
                            {
                                operationDropdown.value = 2;
                            }
                            return;
                        }
                        else
                        {
                            PanelFadeOut(operationUI);
                            StartCoroutine(ActiveDeactive(0.1f, operationUI, false));
                            return;
                        }
                    }

                    if (type == "designer")
                    {
                        if (!designUI.activeSelf)
                        {
                            buildingManager.hidePanels(designUI);
                            designUI.SetActive(true);
                            PanelFadeIn(designUI);
                            return;
                        }
                        else
                        {
                            PanelFadeOut(designUI);
                            StartCoroutine(ActiveDeactive(0.1f, designUI, false));
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
        if (buildingManager.CanDestroy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D raycastHit;
                Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 ray = cameraPos;
                raycastHit = Physics2D.Raycast(ray, -Vector2.up);
                if (raycastHit.transform != null)
                {
                    if (raycastHit.transform.gameObject.GetComponent<buildingType>())
                    {

                        GameObject current = raycastHit.transform.gameObject;

                        DestroyImmediate(current);
                        buildingManager.CanDestroy = false;
                        fuelConnectorManager.ShowConnection();

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
