using UnityEngine;

public class TargetBox : MonoBehaviour
{
    /// <summary>
    /// Targets that move past this point score automatically.
    /// </summary>
    public static float OffScreen;
    private bool notCounted = true;

    internal void Start() {
        OffScreen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-100, 0, 0)).x;
    }

    internal void Update()
    {
        if (transform.position.x > OffScreen)
            Scored();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
            Scored();

    }

    private void Scored()
    {
        if (notCounted)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
            ScoreKeeper.AddToScore(GetComponent<Rigidbody2D>().mass);
            notCounted = false;
        }
    }
}
