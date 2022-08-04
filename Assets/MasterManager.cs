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
    public string FolderName;

    public TMP_InputField savePath;

    public GameObject AlertText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void newGame()
    {
        string folder = savePath.text.ToString();
        Debug.Log("Folder");
        if(!Directory.Exists(Application.persistentDataPath + "/" + folder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + folder);
            FolderName = folder;
        }
        else if(Directory.Exists(Application.persistentDataPath + "/" + folder))
        {
            StartCoroutine(Text());
        }
    }

    IEnumerator Text()  //  <-  its a standalone method
    {
	    AlertText.active = true;
        yield return new WaitForSeconds(1);
        AlertText.active = false;
    }
}
