using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_Capsule : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject CreatorPanel;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
