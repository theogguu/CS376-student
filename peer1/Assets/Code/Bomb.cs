using UnityEngine;

public class Bomb : MonoBehaviour {
    public float ThresholdForce = 2;
    public GameObject ExplosionPrefab;

    void Destruct()
    {
        Destroy(this);
    }

    void Boom()
    {
        GetComponent<PointEffector2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = false;
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform.parent);
        Invoke("Destruct", .1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude >= ThresholdForce)
            Boom();
    }
}
