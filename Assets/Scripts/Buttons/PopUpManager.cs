using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PopUpManager : MonoBehaviour
{
    public GameObject refObj;
    public GameObject GameManager;
    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void dismiss()
    {
        if(SceneManager.GetActiveScene().name == "Building")
        {
            GameManager.GetComponent<GameManager>().panel.SetActive(true);
        }

        if(SceneManager.GetActiveScene().name == "EngineDesign")
        {
            GameManager.GetComponent<GameManager_Engine>().panel.SetActive(true);
        }

        if(SceneManager.GetActiveScene().name == "TankDesign")
        {
            GameManager.GetComponent<GameManager_Tank>().panel.SetActive(true);
        }

        Destroy(refObj);   
    }
}
