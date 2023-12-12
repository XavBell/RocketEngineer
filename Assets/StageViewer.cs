using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageViewer : MonoBehaviour
{
    public GameObject rocket;
    public TMP_Dropdown stageDropdown;
    public GameObject EngineUI;
    public GameObject DecouplerUI;
    public GameObject Panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateStagesView()
    {
        if(rocket != null)
        {
            List<string> options = new List<string>();
            int i = 0;
            foreach(Stages stage in rocket.GetComponent<Rocket>().Stages)
            {
                options.Add($"Stage {i}");
                i++;
            }
            stageDropdown.AddOptions(options);
            updateInfoPerStage();
        }
    }

    public void updateInfoPerStage()
    {
        int value = stageDropdown.value;
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
        }
    }
}
