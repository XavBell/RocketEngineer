using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class availableModuleManager : MonoBehaviour
{
    public List<GameObject> availableModules;
    public GameObject attachLocation;
    public GameObject moduleButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddParts()
    {
        GameObject container = FindObjectOfType<CapsuleDesignerModuleContainer>().container;
        foreach(Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(GameObject module in availableModules)
        {
            GameObject moduleButton1  = Instantiate(moduleButton);
            Button button = moduleButton1.GetComponentInChildren<Button>();
            button.transform.SetParent(container.transform, false);
            button.GetComponentInChildren<TMP_Text>().text = module.GetComponent<ModuleComponent>().displayName;
            button.GetComponent<ModuleLoader>().resourcesName = module.GetComponent<ModuleComponent>().resourcesName;
            button.GetComponent<ModuleLoader>().location = attachLocation;
        }
    }
}
