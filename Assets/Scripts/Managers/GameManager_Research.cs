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

    public void basic()
    { 
        masterManager.nodeUnlocked.Add("basic");

        masterManager.nozzleUnlocked.Add("NozzleSmall");
        masterManager.turbineUnlocked.Add("TurbineSmall");
        masterManager.pumpUnlocked.Add("PumpSmall");

        masterManager.tankMaterialUnlocked.Add("stainlessSteel");
        masterManager.maxTankBuildSizeX = 1;
        masterManager.maxTankBuildSizeY = 1;

        masterManager.maxRocketBuildSizeX = 5;
        masterManager.maxRocketBuildSizeY = 10;
    }
}
