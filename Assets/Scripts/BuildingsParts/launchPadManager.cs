using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class launchPadManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject Panel;
    public GameObject scroll;
    public savePath savePathRef = new savePath();
    public MasterManager MasterManager;
    public bool Spawned = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager = GMM.GetComponent<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Spawned == false)
        {
            retrieveRocketSaved();
            Spawned = true;
        }

    }

    public void retrieveRocketSaved()
    {
        //Panel.active = true;
        Debug.Log("Spawing");
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
            return;
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
        var fileInfo = info.GetFiles();
        if(fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            GameObject rocket = Instantiate(buttonPrefab) as GameObject;
            GameObject child = rocket.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scroll.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            child.GetComponentInChildren<OnClick>().filePath = savePathRef.rocketFolder;
            child.GetComponentInChildren<OnClick>().launchPad = this.gameObject;

        }
        //Panel.active = false;
    }
}
