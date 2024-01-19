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
                        if (current.GetComponent<buildingType>().UI.active == false)
                        {
                            current.GetComponent<buildingType>().UI.SetActive(true);
                            PanelFadeIn(current.GetComponent<buildingType>().UI);
                            return;
                        }
                        else
                        {
                            PanelFadeOut(current.GetComponent<buildingType>().UI);
                            StartCoroutine(ActiveDeactive(0.1f, current.GetComponent<buildingType>().UI, false));
                            return;
                        }
                    }

                    if (type == "VAB")
                    {
                        if (current.GetComponent<buildingType>().UI.active == false)
                        {
                            current.GetComponent<buildingType>().UI.SetActive(true);
                            PanelFadeIn(current.GetComponent<buildingType>().UI);
                            return;
                        }
                        else
                        {
                            PanelFadeOut(current.GetComponent<buildingType>().UI);
                            StartCoroutine(ActiveDeactive(0.1f, current.GetComponent<buildingType>().UI, false));
                            return;
                        }
                    }

                }

            }
        }

    }

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
