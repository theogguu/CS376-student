using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Rigidbody2D of player
    public Rigidbody2D rb;

    // BoxCollider2D
    public BoxCollider2D bc;

    // Move Speed 
    public float moveSpeed;

    // Bool if time is frozen or not
    public bool is_Freeze;

    // is the player alive?
    public bool is_Alive;

    // Freeze time.
    public float freeze_time; 

    // Controls the movement of the player.
    public Vector2 movement;

    public bool is_win;

    

    // Audio when player freezes time or picks up coin
    public AudioClip the_world;
    public AudioSource a;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        moveSpeed = 10.0f;
        is_Freeze = false;
        movement = new Vector2(0, 0);
        a = GetComponent<AudioSource>();
        freeze_time = 3.0f;
        is_Alive = true;
        is_win = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Alive)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            if (is_Freeze == false)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    freeze_time = 2.0f; // freeze time
                    is_Freeze = true;
                    a.PlayOneShot(the_world);
                    ScoreKeeper.ScorePoints(-2);
                }
            }
            else
            {
                if (freeze_time < 0.0f)
                {
                    is_Freeze = false;
                }
                freeze_time -= Time.deltaTime;
            }

            /*
            if (collided_with_coin)
            {
                //a.PlayOneShot(pickup);
                collided_with_coin = false;
            }*/
        }
    }

    // Called multiple times per frame
    void FixedUpdate()
    {
        if (is_Alive)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

}
