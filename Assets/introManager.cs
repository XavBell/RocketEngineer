using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class introManager : MonoBehaviour
{
    float waitTime = 1f;
    float lastTime = 0f;
    public bool tutorialActive = false;

    public bool partOnePassed = false;
    int partOne = 0;
    IEnumerator savedPartOne;
    public TutorialHelper tutorialHelper;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene" && tutorialActive == false)
        {
            if(tutorialHelper == null)
            {
                tutorialHelper = FindObjectOfType<TutorialHelper>();
                tutorialHelper.gameObject.SetActive(false);
            }
            Destroy(this.gameObject);
        }

        if(SceneManager.GetActiveScene().name == "SampleScene" && tutorialActive == true)
        {
            if(tutorialHelper == null)
            {
                tutorialHelper = FindObjectOfType<TutorialHelper>();
            }
            if(partOnePassed == false)
            {
                if(lastTime + waitTime < Time.time)
                {
                    lastTime = Time.time;
                    savedPartOne = tutorialPartOne();
                    StartCoroutine(savedPartOne);
                }
            }

        }
    }

    public IEnumerator tutorialPartOne()
    {
        if(partOne == 0)
        {
            tutorialHelper.text.text = "Welcome to Rocket Engineer!";
            partOne++;
            yield break;
        }
        if(partOne == 1)
        {
            tutorialHelper.text.text = "This is the tutorial, I will guide you through the basics of the game.";
            partOne++;
            yield break;
        }
        if(partOne == 2)
        {
            tutorialHelper.text.text = "Let’s start by placing the first buildings that will allow you to design, tests and launch your rockets! ";
            partOne++;
            yield break;
        }
        if(partOne == 3)
        {
            tutorialHelper.text.text = "Open the build menu at the bottom left";
            partOne++;
            yield break;
        }
        if(partOne == 4)
        {
            if(tutorialHelper.buildPressed == true)
            {
                tutorialHelper.text.text = "Great! Now build the command center! This building will allow you to access basic tasks.";
                tutorialHelper.buildPressed = false;
                partOne++;
                yield break;
            }
            yield break;
        }
        if(partOne == 5)
        {
            if(tutorialHelper.commandCenterPressed == true)
            {
                tutorialHelper.text.text = "Great!";
                tutorialHelper.commandCenterPressed = false;
                partOne++;
                yield break;
            }
            yield break;
        }
        yield return null;

    }

    
}
