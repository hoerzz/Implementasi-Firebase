using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Audio;
    public AudioClip ClickCoint,failUpgrade,upgrade,unclocked;
    public static SoundManager smInstance;

    private void Awake()
    {
        if(smInstance != null && smInstance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        smInstance = this;
        DontDestroyOnLoad(this);
    }
}
