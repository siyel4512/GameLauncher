using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] audioSourcess;

    public void AudioSwitch(bool TurnOn)
    {
        for (int i = 0; i < audioSourcess.Length; i++)
        {
            //audioSourcess[i].mute = TurnOn;
            audioSourcess[i].enabled = TurnOn;
        }
    }
}
