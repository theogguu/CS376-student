using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    // Rigidbody2D of ball.
    public Rigidbody2D rb;

    // CircleCollider2D of ball.
    public CircleCollider2D cc;

    // Rotate speed.
    public float deg_per_sec;

    // Power of shot (multiply to force applied).
    public float power;
    public float power_scale;
    public float min_power;
    public float max_power;

    // Audio when player ball gets launched.
    public AudioSource boom;

    // Reference to the main camera.
    Camera cam;

    // State of the game, initialize to running
    public enum GameState {Running, Over}
    public GameState gs;

    // If player wins or loses.
    public bool is_win;

    // Position of a GameObject in screen coords
    Vector2 ScreenPos(GameObject o)
    {
        return cam.WorldToScreenPoint(o.transform.position);
    }

    // START: intialize components before the first game update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        deg_per_sec = 180f;
        power = 0f;
        power_scale = 10f;
        min_power = 0f;
        max_power = 1000f;
        cam = Camera.main;
        boom = GetComponent<AudioSource>();
        is_win = true;
    }

    // Changes BallState if spacebar is pressed.
    void ChangePhase()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.right * power);
            ScoreKeeper.ScorePoints(-1);
            boom.Play();
        }
    }

    // 

    // Handles BallControl.
    void BallControl()
    {
        // Aim the ball with up and down arrows.
        if (Input.GetKey(KeyCode.UpArrow) || (Input.GetKey(KeyCode.DownArrow)))
        {
            //transform.Rotate(new Vector3(0, deg_per_sec, 0) * Time.deltaTime);
            rb.angularVelocity = Input.GetAxis("Vertical") * deg_per_sec;
        }

        else // Reset angular velocity to 0 if not pressing up or down.
        {
            /*if (bs != BallState.Firing)
            {
                rb.angularVelocity = 0;
            }*/
            rb.angularVelocity = 0;
        }

        // Control the power with left and right arrows.
        if (Input.GetKey(KeyCode.LeftArrow) || (Input.GetKey(KeyCode.RightArrow)))
        {
            power += Input.GetAxis("Horizontal") * power_scale;
            if (power > max_power)
            {
                power = max_power;
            }

            if (power < min_power)
            {
                power = min_power;
            }
        }
    }

    void PlayerWins()
    {
        if (GameObject.FindGameObjectsWithTag("Respawn").Length == 0)
        {
            gs = GameState.Over;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gs == GameState.Running)
        {
            BallControl();
            ChangePhase();
            PlayerWins();
        }

        else
        {
            //Kill game and set all velocities to 0
            var d = FindObjectOfType<Death>();
            if (d)
            {
                d.GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
            }
            var circles = GameObject.FindGameObjectsWithTag("Respawn");
            foreach (var circle in circles)
            {
                circle.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
                //CircleCollider2D c = circle.GetComponent<CircleCollider2D>();
            }
            rb.velocity = new Vector3(0, 0, 0);
            rb.angularVelocity = 0;
        }

        
    }
}
