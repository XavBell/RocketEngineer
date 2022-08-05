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
    }

    public void clicked()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
        Debug.Log(GameManager);
        if(GameManager == null)
        {
            Debug.Log("Hi");
        }

        if (GameManager != null)
        {
            GameManager.GetComponent<GameManager>().path = "/"+ b1.GetComponentInChildren<TextMeshProUGUI>().text;
            string filePath = GameManager.GetComponent<GameManager>().filePath;
            if (filePath == savePathRef.engineFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                b1.interactable = false;
            }

            if (filePath == savePathRef.rocketFolder)
            {
                //GameManager.GetComponent<GameManager>().load(savePathRef.rocketFolder);
                b1.interactable = false;
            }

            if (filePath == savePathRef.tankFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
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
