using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Set the projectile's speed
    public float speed = 5f;

    // Set the projectile's direction
    public Vector2 direction = Vector2.right;

    // Update is called once per frame
    void Update()
    {
        // Move the projectile in the specified direction
        transform.Translate(direction * speed * Time.deltaTime);
    }

    // Detect collision with other objects
    public void OnCollisionEnter2D(Collision2D c)
    {
        GameObject other = c.gameObject;

        if (other.tag == "Skeleton")
        {
            other.GetComponent<Enemy>().TakeDamage(0.2f);
            Destroy(gameObject);
        }

        // Destroy the projectile when colliding with anything tagged with "Ground"
        if (other.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
