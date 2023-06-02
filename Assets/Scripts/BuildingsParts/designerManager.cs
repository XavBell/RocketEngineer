using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class designerManager : MonoBehaviour
{
    public GameObject Panel;
    WorldSaveManager WorldSaveManager;
    // Start is called before the first frame update
    void Start()
    {
        GameObject SaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        WorldSaveManager = SaveManager.GetComponent<WorldSaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterEngineDesigner()
    {
        SceneManager.LoadScene("EngineDesign");
        WorldSaveManager.saveTheWorld();
    }

    public void EnterTankDesign()
    {
        SceneManager.LoadScene("TankDesign");
        WorldSaveManager.saveTheWorld();
    }

    public void EnterRocketDesign()
    {
        SceneManager.LoadScene("Building");
        WorldSaveManager.saveTheWorld();
    }
}
