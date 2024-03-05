using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    //data fields
    [SerializeField] float moveSpeed = 4.0f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float attackDamage = 0.4f;
    [SerializeField] float attackRange = 0.46f;
    [SerializeField] float maxHealth = 3f;
    [SerializeField] float currentHealth;


    private int lastAttack = 3;
    private float delayToIdle = 0.0f;
    private float timeSinceAttack = 0.0f;

    public Transform attackPoint;
    public BoxCollider2D boxColliderPlayer;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask enemyLayers;

    public GameObject regularProjectile;
    private float projectileSpeed = 5f;
    private float fireRate = 0.5f;
    private float canFire = -1f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool awake = true;

    public int ammo = 3;

    bool blocking = false;

    static public PlayerController S; //Singleton

    private void Awake()
    {
        S = this; //setting Singleton
    }

    //sets up game
    void Start()
    {
        //sets rigidbody, and health
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        boxColliderPlayer = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        maxHealth = Data.S.maxHealth;
        currentHealth = maxHealth;
        attackDamage = Data.S.attackDamage;
        moveSpeed = Data.S.moveSpeed;
        jumpForce = Data.S.jumpForce;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        timeSinceAttack += Time.deltaTime;
        animator.SetBool("Grounded", IsGrounded());
        Move(); //calls move method

        //if the player uses left click on their mouse, call the attack method
        if(Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f)
        {
            if(lastAttack == 3)
            {
                animator.SetTrigger("Attack1");
                lastAttack = 1;
            }

            else
            {
                animator.SetTrigger("Attack3");
                lastAttack = 3;
            }

            // Reset timer
            timeSinceAttack = 0.0f;
            Attack();
        }

        //blocking
        else if(Input.GetMouseButtonDown(1))
        {
            blocking = true;
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }

        else if(Input.GetMouseButtonUp(1))
        {
            blocking = false;
            animator.SetBool("IdleBlock", false);
        }

        else if(Input.GetKeyDown(KeyCode.Q) && Time.time > canFire)
        {
            animator.SetTrigger("Attack2");
            RangedAttack(regularProjectile);
        }
            
    }

    //moves the player
    public virtual void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (x > 0.01f)
        {
            transform.localScale = Vector3.one;
        }

        else if (x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            blocking = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }

        //Run
        else if (Mathf.Abs(x) > Mathf.Epsilon)
        {

            // Reset timer
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            delayToIdle -= Time.deltaTime;
            if (delayToIdle < 0)
                animator.SetInteger("AnimState", 0);
        }
    }

    //checks if the player is on the ground
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(boxColliderPlayer.bounds.center, boxColliderPlayer.bounds.size, 0f, Vector2.down, .1f, ground);
    }

    //attack method, attacks enemies within the radius
    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.tag == "Skeleton")
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            Debug.Log(enemy.tag + " was hit!");
        }
    }

    public void RangedAttack(GameObject projectilePrefab)
    {
        canFire = Time.time + fireRate;

        // Instantiate the projectile as a child of the player
        GameObject projGO = Instantiate<GameObject>(projectilePrefab, transform.position, Quaternion.identity, transform);
        Vector3 offSet;

        Rigidbody2D rigidB = projGO.GetComponent<Rigidbody2D>();

        if (transform.localScale.x < 0)
        {
            rigidB.velocity = Vector3.left * projectileSpeed;
            offSet = new Vector3(-1f, 0f, 0f);
        }
        else
        {
            rigidB.velocity = Vector3.right * projectileSpeed;
            offSet = new Vector3(1f, 0f, 0f);
        }

        projGO.transform.localPosition = offSet; // Set the local position relative to the player
    }


    //detects collision with player
    public void OnCollisionEnter2D(Collision2D c)
    {
        GameObject other = c.gameObject;

        if (other.tag == "Skeleton")
        {
            float dmg = 0.25f;
            if (!awake)
                dmg *= 3;

            TakeDamage(0.25f);
        }
    }

    //method that damages the enemy when hit
    public virtual void TakeDamage(float damage)
    {
        if (!blocking)
        {
            currentHealth -= damage;
            animator.SetTrigger("Hurt");
            Debug.Log("You took " + damage + " damage.");
        }

        if (currentHealth <= 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Invoke("Death", 3);
            animator.SetTrigger("Death");
        }
    }

    public void Death()
    {
        Destroy(Data.S.gameObject);
        SceneManager.LoadScene("Level 1"); //resets level when dead
    }

    public void SaveData()
    {
        Data.S.attackDamage = attackDamage;
        Data.S.jumpForce = jumpForce;
        Data.S.moveSpeed = moveSpeed;
        Data.S.maxHealth = maxHealth;
    }
}
