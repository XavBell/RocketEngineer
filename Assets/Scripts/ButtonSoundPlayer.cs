using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioSource source;
    public AudioClip click;
    public AudioClip hover;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playClick()
    {
        source.PlayOneShot(click);
    }

    public void playHover()
    {
        source.PlayOneShot(hover);
    }
}
