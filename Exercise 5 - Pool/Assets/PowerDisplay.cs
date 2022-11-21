using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class PowerDisplay : MonoBehaviour
{
    // SpriteRenderer representing the power of the shot, and its color.
    // SpriteRenderer sprite;
    // Color color;
    // Text displaying power. 

    public TMP_Text power_text;

    public Player player;

    private float power_val;

    // Start is called before the first frame update
    void Start()
    {
        //sprite = GetComponent<SpriteRenderer>();
        //sprite.color = Color.yellow;
        power_text = GetComponent<TMP_Text>();
        power_text.color = Color.yellow;

        player = FindObjectOfType<Player>();
        //player = go.GetComponent<Player>();
        power_val = player.power/10;
        power_text.text = Convert.ToString(power_val);
        
    }

    // Update is called once per frame 
    void Update()
    {
        power_val = Convert.ToInt32(player.power/10);
        power_text.text = $"Power: {Convert.ToString(power_val)}";
    }
}
