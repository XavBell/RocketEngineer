using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OnClick : MonoBehaviour
{
    public Button b1;
    public GameObject tankPrefab;
    public GameObject enginePrefab;
    public savePath savePathRef = new savePath();
    public string filePath;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name.ToString() == "Menu")
        {
           GameObject MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
           if(b1.GetComponentInChildren<TextMeshProUGUI>().text.ToString() != MasterManager.GetComponent<MasterManager>().FolderName)
           {
                b1.interactable = true;
           }
        }

        if(SceneManager.GetActiveScene().name.ToString() == "Building")
        {
           GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
           if("/" + b1.GetComponentInChildren<TextMeshProUGUI>().text != GameManager.GetComponent<GameManager>().path || GameManager.GetComponent<GameManager>().partPath != filePath)
           {
                b1.interactable = true;
           }
        }
    }

    public void clicked()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject MasterManager = GameObject.FindGameObjectWithTag("MasterManager");

        if (GameManager != null)
        {
            GameManager.GetComponent<GameManager>().path = "/"+ b1.GetComponentInChildren<TextMeshProUGUI>().text;
            if (filePath == savePathRef.engineFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                GameManager.GetComponent<GameManager>().PrefabToConstruct = GameManager.GetComponent<GameManager>().Engine;
                b1.interactable = false;
            }

            if (filePath == savePathRef.rocketFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                b1.interactable = false;
            }

            if (filePath == savePathRef.tankFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                GameManager.GetComponent<GameManager>().PrefabToConstruct = GameManager.GetComponent<GameManager>().Tank;
                b1.interactable = false;
            }

        }

        if(SceneManager.GetActiveScene().name.ToString() == "Menu")
        {
            MasterManager.GetComponent<MasterManager>().FolderName = b1.GetComponentInChildren<TextMeshProUGUI>().text;
            b1.interactable = false;
        }
        
    }
}
