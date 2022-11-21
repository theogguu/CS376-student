using UnityEngine;

/// <summary>
/// Control the player on screen
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// Prefab for the orbs we will shoot
    /// </summary>
    public GameObject OrbPrefab;

    /// <summary>
    /// How fast our engines can accelerate us
    /// </summary>
    public float EnginePower = 1;
    
    /// <summary>
    /// How fast we turn in place
    /// </summary>
    public float RotateSpeed = 1;

    /// <summary>
    /// How fast we should shoot our orbs
    /// </summary>
    public float OrbVelocity = 10;

    /// <summary>
    /// RigidBody2D Player Body
    /// </summary>
    public Rigidbody2D Body;

    /// Initialize Body
    void Start()
    {
        Body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Handle moving and firing.
    /// Called by Uniity every 1/50th of a second, regardless of the graphics card's frame rate
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void FixedUpdate()
    {
        Manoeuvre();
        MaybeFire();
    }

    /// <summary>
    /// Fire if the player is pushing the button for the Fire axis
    /// Unlike the Enemies, the player has no cooldown, so they shoot a whole blob of orbs
    /// </summary>
    void MaybeFire()
    {   
        
        if (Input.GetAxis("Fire") == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                FireOrb();
            }
            
        }
        
        else if (Input.GetAxis("Fire") > 0)
        {
            FireOrb();
        }
    }

    /// <summary>
    /// Fire one orb.  The orb should be placed one unit "in front" of the player.
    /// transform.right will give us a vector in the direction the player is facing.
    /// It should move in the same direction (transform.right), but at speed OrbVelocity.
    /// </summary>
    private void FireOrb()
    {
        Vector3 Vec = new Vector3(Body.position.x + transform.right.x + 1, Body.position.y + transform.right.y);


        var Orb = Instantiate(OrbPrefab, Vec, Quaternion.identity);

        Rigidbody2D OrbBody = Orb.GetComponent<Rigidbody2D>();

        //OrbBody.AddForce(new Vector2(10,10));

        OrbBody.velocity = new Vector2(OrbVelocity*transform.right.x, OrbVelocity*transform.right.y);   
    }

    /// <summary>
    /// Accelerate and rotate as directed by the player
    /// Apply a force in the direction (Horizontal, Vertical) with magnitude EnginePower
    /// Note that this is in *world* coordinates, so the direction of our thrust doesn't change as we rotate
    /// Set our angularVelocity to the Rotate axis time RotateSpeed
    /// </summary>
    void Manoeuvre()
    {
        Vector2 F = new Vector2(Input.GetAxis("Horizontal")*EnginePower, Input.GetAxis("Vertical")*EnginePower);

        Body.AddForce(F);
        Body.angularVelocity = Input.GetAxis("Rotate")*RotateSpeed;
    }

    /// <summary>
    /// If this is called, we got knocked off screen.  Deduct a point!
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void OnBecameInvisible()
    {
        ScoreKeeper.ScorePoints(-1);
    }
}
