using UnityEngine;

public class Bomb : MonoBehaviour {
    public float ThresholdForce = 2;
    public GameObject ExplosionPrefab;

    //Destroys Gameobject
    void Destruct()
    {
        Destroy(this);
    }

    //Makes Bomb blow up
    void Boom()
    {
        GetComponent<PointEffector2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = false;

        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform.parent);

        Invoke("Destruct", 0.1f);
    }

    ///An OnCollisionEnter2D() method that calls Boom() when an object hits the box with a velocity of 
    ///at least ThresholdForce.You can get the velocity of the collision from the argument to
    ///OnCollisionEnter2D.The idea is that you only want the box to explode if it’s hit hard enough.
    ///We’re using the velocity as a proxy for how hard the object is hit

    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.relativeVelocity.magnitude >= ThresholdForce)
        {
            Boom();
        }
    }
}