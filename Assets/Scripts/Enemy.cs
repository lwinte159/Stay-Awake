using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public float maxHealth = 1f;
    public float currentHealth;

    [HideInInspector]
    public Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        rb.GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ColorReset", 1);

        if (currentHealth <= 0)
            Destroy(gameObject);
           
    }
    void ColorReset()
    {
        rb.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
