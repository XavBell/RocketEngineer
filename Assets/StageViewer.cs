using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StageViewer : MonoBehaviour
{
    public GameObject rocket;
    public TMP_Dropdown stageDropdown;
    public GameObject EngineUI;
    public GameObject DecouplerUI;
    public GameObject TankUI;
    public GameObject Panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fullReset(bool forcedCall)
    {
        stageDropdown.value = 0;
        updateStagesView(true);
        updateInfoPerStage(true);
    }
    public void updateStagesView(bool forcedCall)
    {
        if(rocket != null)
        {
            List<string> options = new List<string>();
            foreach(Transform child in Panel.transform)
            {
                if(child.gameObject.GetComponent<EngineUIModule>() != null || child.gameObject.GetComponent<TankUIModule>() != null || child.gameObject.GetComponent<DecouplerUIModule>() != null)
                {
                    DestroyImmediate(child.gameObject);
                }

            }
            stageDropdown.ClearOptions();
            int i = 0;
            foreach(Stages stage in rocket.GetComponent<Rocket>().Stages)
            {
                options.Add($"Stage {i}");
                i++;
            }
            stageDropdown.value = 0;
            stageDropdown.AddOptions(options);
        }
    }

    public void updateInfoPerStage(bool forcedCall)
    {
        foreach(Transform child in Panel.transform)
        {
            if(child.gameObject.GetComponent<EngineUIModule>() != null || child.gameObject.GetComponent<EngineUIModule>() != null || child.gameObject.GetComponent<DecouplerUIModule>() != null)
            {
                DestroyImmediate(child.gameObject);
            }

        }
        int value = 0;
        if(forcedCall == false)
        {
            value = stageDropdown.value;
        }

        if(forcedCall == true)
        {
            value = 0;
            stageDropdown.value = 0;
        }

        foreach(RocketPart part in rocket.GetComponent<Rocket>().Stages[value].Parts)
        {
            if(part._partType == "engine")
            {
                GameObject engineUI = Instantiate(EngineUI, Panel.transform);
                engineUI.GetComponent<EngineUIModule>().engine = part.GetComponent<Engine>();
            }

            if(part._partType == "decoupler")
            {
                GameObject decouplerUI = Instantiate(DecouplerUI, Panel.transform);
                decouplerUI.GetComponent<DecouplerUIModule>().decoupler = part.GetComponent<Decoupler>();
            }

            if(part._partType == "tank")
            {
                GameObject tankUI = Instantiate(TankUI, Panel.transform);
                tankUI.GetComponent<TankUIModule>().tank = part.GetComponent<outputInputManager>();
            }
        }
    }

    public void launch()
    {
        rocket.GetComponent<PlanetGravity>().possessed = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.gameState = "Flight";
        masterManager.ActiveRocket = rocket;
    }

    public void Stop()
    {
        rocket.GetComponent<PlanetGravity>().possessed = false;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.gameState = "Building";
        CameraControl camera = FindObjectOfType<CameraControl>();
        launchsiteManager launchsiteManager = FindObjectOfType<launchsiteManager>();
        camera.transform.position = launchsiteManager.commandCenter.transform.position;
        masterManager.ActiveRocket = null;
    }
}
