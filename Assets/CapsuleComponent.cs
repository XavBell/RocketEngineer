using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class CapsuleComponent : MonoBehaviour
{
    public Sprite interiorSprite;
    public Sprite exteriorSprite;
    public GameObject internalEditor;
    public GameObject externalEditor;
    public List<CapsuleModuleComponent> modules;
    public bool moduleSet = false;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "CapsuleDesign")
        {
            internalEditor.SetActive(false);
            externalEditor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "CapsuleDesign")
        {
            if (moduleSet == false)
            {
                foreach (CapsuleModuleComponent module in modules)
                {
                    if (module.interior)
                    {
                        module.gameObject.SetActive(false);
                    }
                    else
                    {
                        module.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
