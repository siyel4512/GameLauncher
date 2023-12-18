using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource[] audioSourcess;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AudioSwitch(bool TurnOn)
    {
        for (int i = 0; i < audioSourcess.Length; i++)
        {
            audioSourcess[i].mute = TurnOn;
        }
    }
}
