using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ScoreKeeper.cs;

public class SquareHole : MonoBehaviour
{
    // box collider of the square hole
    public BoxCollider2D hole;

    // RespawnPoint for player.
    public Vector3 RespawnPoint;

    public AudioSource android;

    private bool addonce;

    
    // Start is called before the first frame update
    void Start()
    {
        hole = GetComponent<BoxCollider2D>();
        RespawnPoint = new Vector3(-6, 0, 0);
        android = GetComponent<AudioSource>();
        addonce = true;
    }

    // returns true if a collision happens
    void Collision()
    {
        var p = FindObjectOfType<Player>();
        CircleCollider2D player = p.GetComponent<CircleCollider2D>();

        var d = FindObjectOfType<Death>();
        if (d)
        {
            CircleCollider2D death = d.GetComponent<CircleCollider2D>();
            if (hole.IsTouching(death))
            {
                p.is_win = false;
                if (addonce == true)
                {
                    ScoreKeeper.ScorePoints(-30);
                    addonce = false;
                }
                p.gs = Player.GameState.Over;
            }
        }

        // ReSharper disable once UnusedMember.Local
        if (hole.IsTouching(player))
        {
            p.transform.position = RespawnPoint;
            p.rb.velocity = new Vector3(0, 0, 0);
        }

        else
        {
            // Only work for "score" balls (tagged with "Respawn")
            var circles = GameObject.FindGameObjectsWithTag("Respawn"); //can't figure out how to create a new tag
            foreach (var circle in circles)
            {
                CircleCollider2D c = circle.GetComponent<CircleCollider2D>();

                if (hole.IsTouching(c))
                {
                    ScoreKeeper.ScorePoints(10);
                    Destroy(circle);
                    android.Play();
                }
            }
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        Collision();
    }
}
