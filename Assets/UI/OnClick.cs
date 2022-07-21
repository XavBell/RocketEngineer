using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class OnClick : MonoBehaviour
{
    public Button b1;
    public GameObject tankPrefab;
    public GameObject enginePrefab;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        Debug.Log(GameManager);
        if(GameManager == null)
        {
            Debug.Log("Hi");
        }

        if (GameManager != null)
        {
            GameManager.GetComponent<GameManager>().path = b1.GetComponentInChildren<TextMeshProUGUI>().text;
            string filePath = GameManager.GetComponent<GameManager>().filePath;
            if (filePath == "/engines/")
            {
                GameManager.GetComponent<GameManager>().ConstructPart(enginePrefab);
            }

            if (filePath == "/rockets/")
            {
                GameManager.GetComponent<GameManager>().load("/rockets/");
            }

            if (filePath == "/tanks/")
            {
                GameManager.GetComponent<GameManager>().ConstructPart(tankPrefab);
            }

        }
        
    }
}
