using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class designerManager : MonoBehaviour
{
    GameObject Panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterEngineDesigner()
    {
        SceneManager.LoadScene("EngineDesign");
    }

    public void EnterTankDesign()
    {
        SceneManager.LoadScene("TankDesign");
    }

    public void EnterRocketDesign()
    {
        SceneManager.LoadScene("Building");
    }
}
