using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class infoController : MonoBehaviour
{
    [SerializeField]private GameObject customCursor;
    [SerializeField]private GameObject infoPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            if(customCursor != null)
            {
                if(customCursor.GetComponent<SpriteRenderer>().enabled == true)
                {
                    infoPanel.SetActive(true);
                }else{
                    infoPanel.SetActive(false);
                }
            }
        }

        if(SceneManager.GetActiveScene().name == "Building")
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if(customCursor != null)
            {
                if(gameManager.partToConstruct != null)
                {
                    infoPanel.SetActive(true);
                }else{
                    infoPanel.SetActive(false);
                }
            }
        }


        
    }
}
