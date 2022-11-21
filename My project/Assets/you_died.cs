using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class you_died : MonoBehaviour
{
    // SpriteRenderer representing the power of the shot, and its color.
    // SpriteRenderer sprite;
    // Color color;
    // Text displaying power. 

    public TMP_Text text;

    public Player player;

    private float power_val;

    public AudioSource a;
    public AudioClip death;
    public bool played;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.color = Color.red;
        a = GetComponent<AudioSource>();
        player = FindObjectOfType<Player>();
        played = false;
    }

    // Update is called once per frame 
    void Update()
    {
        if ((player.is_win == false) && (player.is_Alive == false) && (!played))
        {           
            a.PlayOneShot(death);
            text.text = "YOU DIED";
            played = true;
        }

        if ((player.is_win == true) && (!played))
        {
            text.color = new Color(255, 236, 47);
            text.text = "Victory!";
            played = true;
        }
    }
}
