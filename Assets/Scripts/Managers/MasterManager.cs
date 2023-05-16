using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class MasterManager : MonoBehaviour
{
    public savePath savePathRef = new savePath();
    public string FolderName;

    public TMP_InputField savePath;

    public GameObject AlertText;

    public GameObject scrollBox;

    public GameObject buttonPrefab;

    public GameObject loadButton;

    public string worldPath;

    public string gameState = "Building";

    public GameObject currentBuildingBody;

    public GameObject ActiveRocket;

    public GameObject MainCanvas;
    public GameObject ScrollCanvas;



    public float MenuThrust = 200;
    public GameObject MenuRocket;
    public bool rocketActive = false;
   

    // Start is called before the first frame update
    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder);
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder);
        var fileInfo = info.GetDirectories();
        foreach (var file in fileInfo)
        {
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            GameObject child = button.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scrollBox.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
        }
        
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBuildingBody == null)
        {
            GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
            foreach(GameObject go in planets)
            {
                if(go.GetComponent<TypeScript>().type == "earth")
                {
                    currentBuildingBody = go;
                }
            }
        }

        if(SceneManager.GetActiveScene().name == "Menu")
        {
            thrust();
        }
    }

    public void newGame()
    {
        string folder = savePath.text.ToString();
        if(!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder);
            FolderName = folder;
            saveWorld saveWorld = new saveWorld();
            var jsonString = JsonConvert.SerializeObject(saveWorld);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder + "/" + FolderName + ".json", jsonString);
            worldPath = Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder + "/" + FolderName + ".json";
            SceneManager.LoadScene("SampleScene");
        }
        else if(Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + "/" + folder))
        {
            StartCoroutine(Text());
        }
    }

    public void load()
    {
        if(FolderName != null)
        {
            worldPath = Application.persistentDataPath + savePathRef.worldsFolder + "/" + FolderName + "/" + FolderName + ".json";
            UnityEngine.Debug.Log(worldPath);
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void play()
    {
        rocketActive = true;
        MainCanvas.SetActive(false);
        ScrollCanvas.SetActive(true);
        
    }

    void thrust()
    {
        if(rocketActive == true)
        {
            Vector3 Thrust = MenuRocket.transform.up * MenuThrust;
            MenuRocket.GetComponent<Rigidbody2D>().AddForce(Thrust);
        }
    }

    IEnumerator Text() 
    {
	    AlertText.SetActive(true);
        yield return new WaitForSeconds(1);
        AlertText.SetActive(false);
    }
}
