using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //fields for enemy health
    [HideInInspector]
    public float maxHealth = 1f;
    public float currentHealth;

    [HideInInspector]
    public Rigidbody2D rb;


    void Start()
    {
        //setting health and rigidbody
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    //set damage when hit
    public void TakeDamage(float damage)
    {
        //take away health, indicate hit with turning red
        currentHealth -= damage;
        rb.GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ColorReset", 1); //resets colour

        //if out of health, destroy enemy and add points to wakefullness bar
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            WakeBar.S.WakeUp(0.25f);
        }
    }

    //resets the colour
    void ColorReset()
    {
        rb.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
