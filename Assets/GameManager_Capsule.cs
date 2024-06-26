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
}
