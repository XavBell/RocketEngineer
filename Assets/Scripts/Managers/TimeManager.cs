using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;

public class TimeManager : MonoBehaviour
{
    MasterManager masterManager = null;
    //TimeScale
    public float scaler = 1;
    public double time = 0;
    public float deltaTime = 0;
    public float normalDeltaTime;
    public TMP_Text date;

    public StageViewer stage;
    public bool bypass = false;

    //Buttons with number being the different time scales
    [SerializeField]
    GameObject button1 = null;
    [SerializeField]

    GameObject button5 = null;
    [SerializeField]

    GameObject button10 = null;
    [SerializeField]

    GameObject button50 = null;
    [SerializeField]

    GameObject button100 = null;
    [SerializeField]

    GameObject button500 = null;
    [SerializeField]

    GameObject button1000 = null;
    [SerializeField]

    GameObject button5000 = null;
    [SerializeField]

    GameObject button10000 = null;

    //List contains all buttons, set in editor
    [SerializeField]

    List<GameObject> buttons = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        calculateDeltaTime();
        if (bypass == false)
        {
            time = Time.time;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateDeltaTime();
        time += deltaTime;
        calculateDate();
        setVisibleButtons();
    }

    void calculateDeltaTime()
    {
        deltaTime = Time.fixedDeltaTime * scaler;
    }

    public void setScaler(float desiredScaler)
    {
        scaler = desiredScaler;
    }

    public void calculateDate()
    {

        double year = time / (60 * 60 * 24 * 365);
        double remainingSec = time % (60 * 60 * 24 * 365);
        double month = remainingSec / (60 * 60 * 24 * 30);
        remainingSec = remainingSec % (60 * 60 * 24 * 30);
        double days = remainingSec / (60 * 60 * 24);
        remainingSec = remainingSec % (60 * 60 * 24);
        double hours = remainingSec / (60 * 60);
        remainingSec = remainingSec % (60 * 60);
        double minutes = remainingSec / 60;
        remainingSec = remainingSec % 60;

        date.text = Math.Truncate(year) + "Y:" + Math.Truncate(month) + "M:" + Math.Truncate(days) + "D:" + Math.Truncate(hours) + "H:" + Math.Truncate(minutes) + "m:" + Math.Truncate(remainingSec) + "s";
    }

    public void setVisibleButtons()
    {
        if (masterManager.ActiveRocket != null)
        {
            PlanetGravity pg = masterManager.ActiveRocket.GetComponent<PlanetGravity>();
            if (masterManager.ActiveRocket.GetComponent<Rocket>().throttle > 0)
            {
                foreach (GameObject button in buttons)
                {
                    button.SetActive(false);
                }
                button1.SetActive(true);
                return;
            }else if((pg.getPlanet().transform.position - masterManager.ActiveRocket.transform.position).magnitude < pg.getPlanetRadius() + pg.getAtmoAlt())
            {
                foreach (GameObject button in buttons)
                {
                    button.SetActive(false);
                }
                button1.SetActive(true);
                if(scaler > 1)
                {
                    scaler = 1;
                }
                return;
            }
            else
            {
                foreach (GameObject button in buttons)
                {
                    button.SetActive(true);
                }
                return;
            }
        }
        else
        {
            foreach (GameObject button in buttons)
            {
                button.SetActive(true);
            }
            return;
        }
    }


}
