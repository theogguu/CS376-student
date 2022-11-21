using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Player 
    private Player player;

    // Player transform
    private Transform pt;

    // Wall gameobject
    private GameObject[] wall_list;

    // Rigidbody2D and BoxCollider2D of projectile
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    // Acceleration value for projectile
    private float power;

    // Vector from projectile to player
    private Vector2 PlayerVec;

    // Unit vector in the direction of the player relative to projectile
    private Vector2 PlayerDir;

    // Velocity at the moment of freeze
    private Vector2 freeze_velocity;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        pt = player.transform;
        rb = GetComponent<Rigidbody2D>();
        freeze_velocity = new Vector2(0.0f, 0.0f);
        bc = GetComponent<BoxCollider2D>();
        power = 4.5f;
        

        // Unit vector of player's initial direction
        PlayerVec = pt.position - transform.position;
        PlayerDir = PlayerVec.normalized;

        // Make the projectile face the player
        transform.right = pt.position - transform.position;

        wall_list = GameObject.FindGameObjectsWithTag("Wall");
    }

    void FixedUpdate()
    {
        foreach (var wall in wall_list)
        {
            if (bc.IsTouching(wall.GetComponent<BoxCollider2D>()))
            {
                Destroy(gameObject);
            }


            if (player.is_Freeze == false)
            {
                if (rb.velocity == new Vector2(0.0f, 0.0f))
                {
                    rb.velocity = freeze_velocity;
                }
                freeze_velocity = new Vector2(0.0f, 0.0f);
                rb.AddForce(PlayerDir * power);
            }

            else
            {
                if (freeze_velocity == new Vector2(0.0f, 0.0f))
                {
                    freeze_velocity = rb.velocity;
                }
                rb.velocity = new Vector2(0.0f, 0.0f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        
        if (obj == player.GetComponent<BoxCollider2D>())
        { 
            player.is_Alive = false;
            player.is_Freeze = true;
            Destroy(gameObject);
        }

    }
}
