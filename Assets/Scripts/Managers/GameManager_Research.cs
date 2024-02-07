using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager_Research : MonoBehaviour
{
    MasterManager masterManager;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back()
    { 
        SceneManager.LoadScene("SampleScene");
    }

    public void research(Node node)
    { 
        masterManager.nodeUnlocked.Add(node.NodeName);

        foreach(Nozzle nozzle in node.unlockedNozzle)
        {
            masterManager.nozzleUnlocked.Add(nozzle);
        }

        foreach(Turbine turbine in node.unlockedTurbine)
        {
            masterManager.turbineUnlocked.Add(turbine);
        }

        foreach(Pump pump in node.unlockedPump)
        {
            masterManager.pumpUnlocked.Add(pump);
        }

        masterManager.tankMaterialUnlocked.Add("stainlessSteel");
        masterManager.maxTankBuildSizeX = 1;
        masterManager.maxTankBuildSizeY = 1;

        masterManager.maxRocketBuildSizeX = 5;
        masterManager.maxRocketBuildSizeY = 10;

        researchButton[] researchButtons = FindObjectsOfType<researchButton>();
        foreach(researchButton researchButton in researchButtons)
        {
            researchButton.setVisibility();
        }
    }
}
