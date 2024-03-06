using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //speed
    public float speed = 5f;

    void Update()
    {
        //moves projectile in proper direction
        transform.position += transform.right * Time.deltaTime * speed;
    }

    //collsions
    public void OnCollisionEnter2D(Collision2D c)
    {
        GameObject other = c.gameObject;

        //if you collide with enemy, do damage, projectile gets destroyed
        if (other.tag == "Skeleton")
        {
            other.GetComponent<Enemy>().TakeDamage(0.25f);
            Destroy(gameObject);
        }

        //if it hits the ground/walls, gets destroyed
        else if(other.tag == "Ground")
            Destroy(gameObject);
    }
}
