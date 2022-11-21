using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Spawner class will handle spawning Projectiles and Coins.
/// </summary>
public class Spawner : MonoBehaviour
{
    // Prefab of projectile
    public GameObject Prefab;

    // Seconds between spawns
    public float SpawnInterval = 1.0f;

    // Keeps track of time
    public float TimePassed = 0.0f;

    // Float storing time freeze length to add to TimePassed
    // Hard set to 2.0f
    public float freeze_time;

    // Bool that if true, only spawns if there is no other object of type prefab that currently exists.
    public bool spawn_once;

    // Radius to avoid spawning in
    public float SpawnRadius;

    // Player to grab from is_Freeze from
    public Player p;

    // is_Freeze bool grabbed from Player
    private bool is_Freeze;

    // only add the freeze_time once to TimePassed
    private bool freeze_time_added;

    // Dimensions of box to use for random range.

    float xmin = -11.25f;
    float xmax = 11.25f;

    float ymin = -6.2f;
    float ymax = 6.2f;

    // stop spawning
    bool alive;

    void Start()
    {
        is_Freeze = false;
        p = FindObjectOfType<Player>();
        //freeze_time = p.freeze_time;
        freeze_time = 2.0f;
        freeze_time_added = false;
        SpawnRadius = 5.0f;
        TimePassed = 0.5f;
        alive = true;
    }

    //Returns true if there is a Wall or Player within the position and radius.
    bool FreeSpace(Vector2 pos, float r)
    {
        Collider2D[] wallCircle = Physics2D.OverlapCircleAll(pos, (r - 2.0f));
        Collider2D[] playerCircle = Physics2D.OverlapCircleAll(pos, r);
        foreach (Collider2D c in wallCircle)
        {
            if ((c.tag == "Wall"))
            {
                return false;
            }
        }

        foreach (Collider2D c in playerCircle)
        {
            if ((c.tag == "Player"))
            {
                return false;
            }
        }
        return true;
    }

    void SpawnObject()
    {
        Vector3 SpawnArea; 

        //try 10 times
        for (int i = 0; i < 10; i++)
        {
            //Random spawning, but retry if you spawn near a wall or player
            SpawnArea = new Vector3(Random.Range(xmin, xmax), Random.Range(ymin, ymax), 0);
            if (FreeSpace(SpawnArea, SpawnRadius))
            {
                Instantiate(Prefab, SpawnArea, Quaternion.identity);
                return;
            }      
        }
        //aaaand just give up if you try too many times
        SpawnArea = new Vector3(5, 0, 0);
        Instantiate(Prefab, SpawnArea, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        alive = p.is_Alive;
        if (alive)
        { 
            is_Freeze = p.is_Freeze;
            //if game is not frozen
            if (!is_Freeze)
            {
                freeze_time_added = false;
                if (Time.time > TimePassed)
                {
                    TimePassed += SpawnInterval;

                    if (spawn_once)
                    {
                        if (!FindObjectOfType<Coin>())
                        {
                            SpawnObject();
                        }
                    }
                    else
                    {
                        SpawnObject();
                    }

                }
            }
            else // if game is frozen
            {
                if (!freeze_time_added)
                {
                    TimePassed += freeze_time;
                    freeze_time_added = true;
                }
            }
        }
    }
}
