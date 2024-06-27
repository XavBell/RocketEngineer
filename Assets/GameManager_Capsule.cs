using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;
using TMPro;

public class GameManager_Capsule : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject CreatorPanel;
    public TMP_InputField savePath;
    public savePath savePathRef = new savePath();
    public MasterManager masterManager;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backToBuild()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMain()
    {
        MainPanel.SetActive(true);
        CreatorPanel.SetActive(false);
    }

    public void EnterCreator()
    {
        MainPanel.SetActive(false);
        CreatorPanel.SetActive(true);
    }

    public void ToggleInterior()
    {
        CapsuleComponent capsule = FindObjectOfType<CapsuleComponent>();
        if(capsule != null)
        {
            if(capsule.GetComponent<SpriteRenderer>().sprite == capsule.interiorSprite)
            {
                capsule.GetComponent<SpriteRenderer>().sprite = capsule.exteriorSprite;
                capsule.internalEditor.SetActive(false);
                capsule.externalEditor.SetActive(true);
                CapsuleModuleComponent[] capsuleModuleComponents = capsule.GetComponentsInChildren<CapsuleModuleComponent>();
                foreach(CapsuleModuleComponent capsuleModuleComponent in capsuleModuleComponents)
                {
                    if(!capsuleModuleComponent.interior)
                    {
                        SpriteRenderer[] spriteRenderers = capsuleModuleComponent.GetComponentsInChildren<SpriteRenderer>();
                        foreach(SpriteRenderer spriteRenderer in spriteRenderers)
                        {
                            spriteRenderer.enabled = true;
                        }
                    }else{
                        SpriteRenderer[] spriteRenderers = capsuleModuleComponent.GetComponentsInChildren<SpriteRenderer>();
                        foreach(SpriteRenderer spriteRenderer in spriteRenderers)
                        {
                            spriteRenderer.enabled = false;
                        }
                    }
                }
            }
            else
            {
                capsule.GetComponent<SpriteRenderer>().sprite = capsule.interiorSprite;
                capsule.internalEditor.SetActive(true);
                capsule.externalEditor.SetActive(false);
                CapsuleModuleComponent[] capsuleModuleComponents = capsule.GetComponentsInChildren<CapsuleModuleComponent>();
                foreach(CapsuleModuleComponent capsuleModuleComponent in capsuleModuleComponents)
                {
                    SpriteRenderer[] spriteRenderers = capsuleModuleComponent.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                    {
                        if(capsuleModuleComponent.interior)
                        {
                            spriteRenderer.enabled = true;
                        }else{
                            spriteRenderer.enabled = false;
                        }
                    }
                }
            }
        }
    }

    public void save()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.capsuleFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.capsuleFolder);
        }

        string saveName = "/" + savePath.text;

        if (!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.capsuleFolder + saveName + ".json"))
        {
            saveCapsule saveObject = new saveCapsule();
            saveObject.capsuleName = saveName;
            CapsuleComponent capsule = FindObjectOfType<CapsuleComponent>();
            saveObject.modules = new List<string>();
            saveObject.modulePositionsX = new List<float>();
            saveObject.modulePositionsY = new List<float>();
            saveObject.moduleRotationsY = new List<float>();
            saveObject.moduleRotationsZ = new List<float>();
            foreach(CapsuleModuleComponent module in capsule.modules)
            {
                saveObject.modules.Add(module.moduleName);
                saveObject.modulePositionsX.Add(module.transform.localPosition.x);
                saveObject.modulePositionsY.Add(module.transform.localPosition.y);
                saveObject.moduleRotationsY.Add(module.transform.eulerAngles.y);
                saveObject.moduleRotationsZ.Add(module.transform.eulerAngles.z);
            }
            var jsonString = JsonConvert.SerializeObject(saveObject);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.capsuleFolder + saveName + ".json", jsonString);
        }
    }
}
