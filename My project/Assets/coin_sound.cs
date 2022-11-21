using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin_sound : MonoBehaviour
{
    public AudioSource a;
    public AudioClip pickup;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<AudioSource>();
    }

    public void PlayPickup()
    {
        a.PlayOneShot(pickup);
    }

     
    
}
