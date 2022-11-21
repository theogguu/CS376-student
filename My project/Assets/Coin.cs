using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // CircleCollider2D detects collision with player
    public CircleCollider2D cc;

    // Point value of coin
    public int value;

    // Player to check for collision
    public Player p;


    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CircleCollider2D>();
        p = FindObjectOfType<Player>();
        value = 10;
    }

    // If the coin collides with player, destroy it and add points.
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj == p.GetComponent<BoxCollider2D>())
        {
            // Add points (value = 10)
            ScoreKeeper.ScorePoints(value);
            FindObjectOfType<coin_sound>().PlayPickup();
            //coin_sound.PlayPickup;

            Destroy(gameObject); 
        }
    }
}

