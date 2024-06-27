using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class ModuleLoader : MonoBehaviour
{
    public string resourcesName;
    public GameObject location;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadModule()
    {
        GameObject module = Instantiate(Resources.Load("Prefabs/Modules/CapsuleModules/" + resourcesName)) as GameObject;
        float x_scale = FindObjectOfType<CapsuleComponent>().transform.localScale.x;
        module.transform.localScale = module.transform.localScale * x_scale;
        if(location.transform.childCount > 0)
        {
            Destroy(location.transform.GetChild(0).gameObject);
        }
        module.transform.SetParent(location.transform);
        module.transform.eulerAngles = location.transform.eulerAngles;
        module.transform.position = location.transform.position - module.GetComponent<ModuleComponent>().attachModule.transform.position;
        location.GetComponent<CapsuleModuleComponent>().moduleName = resourcesName;
    }
}
