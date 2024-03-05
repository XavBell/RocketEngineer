using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHelper : MonoBehaviour
{
    public TMP_Text text;
    //Part One
    public Button buildButton;
    public bool buildPressed = false;
    public Button commandCenterButton;
    public bool commandCenterPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        buildButton.onClick.AddListener(delegate { buildPressed = true; });
        commandCenterButton.onClick.AddListener(delegate { commandCenterPressed = true; });
    }
}
