using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    static public Data S; //Singleton

    //player fields
    public float attackDamage = 0.4f;
    public float jumpForce = 7.5f;
    public float moveSpeed = 4.0f;
    public float maxHealth = 5.0f;

    //setting singleton
    private void Awake()
    {
        if (S == null)
        {
            S = this;
            DontDestroyOnLoad(S);
        }

        else if (S != this)
        {
            Destroy(gameObject);
        }
    }

}