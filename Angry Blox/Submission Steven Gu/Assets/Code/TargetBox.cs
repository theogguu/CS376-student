using UnityEngine;

public class TargetBox : MonoBehaviour
{
    /// <summary>
    /// Targets that move past this point score automatically.
    /// </summary>
    public static float OffScreen;

    //Keeps track if the targetbox has hit the ground previously or not
    public bool is_dead = false;


    internal void Start() {
        OffScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-100, 0, 0)).x;
    }

    internal void Update()
    {
        if (transform.position.x > OffScreen)
            Scored();
    }

    void OnCollisionEnter2D()
        //Write an OnCollisionEnter2D method to call the Scored() procedure when the box hits the 
        //ground(the ground object is tagged with the tag “Ground”).
    {
        var ground = GameObject.FindWithTag("Ground");
        Collider2D ground_collider = ground.GetComponent<Collider2D>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        //if (transform.position.y <= ground.transform.position.y)
        if ((collider.IsTouching(ground_collider)))
        {
            Scored();
        }
    }

    private void Scored()
    {
        if (this.is_dead == false)
        {
            this.is_dead = true;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.green;

            Rigidbody2D body = GetComponent<Rigidbody2D>();

            //assumption: mass is a float           
            ScoreKeeper.AddToScore(body.mass);
        }
    }
}
