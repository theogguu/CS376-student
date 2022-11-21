using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class GameoverText : MonoBehaviour
{
    // SpriteRenderer representing the power of the shot, and its color.
    // SpriteRenderer sprite;
    // Color color;
    // Text displaying power. 

    public TMP_Text text;

    public Player player;

    private float power_val;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.color = Color.white;
        player = FindObjectOfType<Player>();

    }

    // Update is called once per frame 
    void Update()
    {
        if (player.gs == Player.GameState.Over)
        {
            if (player.is_win == true)
            {
                text.text = "Good job! You win!";
            }

            else
            {
                text.text = "Game over! You lose!";
            }
        }  
    }
}
