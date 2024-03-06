using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //data fields
    [SerializeField] float moveSpeed = 4.0f;
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float attackDamage = 0.4f;
    [SerializeField] float attackRange = 0.46f;
    [SerializeField] float maxHealth = 5f;
    [SerializeField] float currentHealth;

    //for hearts UI
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public TextMeshProUGUI ammoText;
    public Image keyImage;

    public GameObject playerUI;

    //two backgrounds
    public GameObject awakeBackground;
    public GameObject asleepBackground;

    //for attacking
    private int lastAttack = 3;
    private float delayToIdle = 0.0f;
    private float timeSinceAttack = 0.0f;

    //colliders and masks
    public Transform attackPoint;
    public BoxCollider2D boxColliderPlayer;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask enemyLayers;

    public Projectile projPrefab;
    public Transform projOffset;

    public Vector2 boxSize;
    public float castingDistance;

    private Rigidbody2D rb;
    private Animator animator;
    public bool wakefullness = true;
    public bool hasKey = false;

    public int ammo = 3;

    public bool blocking = false; //check if blocking

    static public PlayerController S; 

    private void Awake()
    {
        S = this; 
    }

    

    //boolean with getter and setter, used to handle changes to wakefullness bool
    public bool Wakefullness
    {
        get //getter
        {
            return wakefullness;
        }   
        set //setter
        {
            if (wakefullness != value)
            {
                wakefullness = value;
                HandleAwakeStatusChange(value); //called to handle changes
            }
        }
    }

    //handling awake/asleep cycle changes
    private void HandleAwakeStatusChange(bool newAwakeStatus)
    {
        if (newAwakeStatus == false)
        {
            //when asleep, change backgrounds and slow player
            rb.GetComponent<SpriteRenderer>().color = Color.gray;
            moveSpeed = 2.0f;
            asleepBackground.SetActive(true);
            awakeBackground.SetActive(false);

            //for further development add more changes such as smaller viewing distance, etc.

        }
        else
        {
            //when awake, revert changes
            rb.GetComponent<SpriteRenderer>().color = Color.white;
            moveSpeed = 4.0f;
            asleepBackground.SetActive(false);
            awakeBackground.SetActive(true);

            //for further development, reverse any extra changes added. 
        }
    }

    //sets up game
    void Start()
    {
        keyImage.enabled = false;
        asleepBackground.SetActive(false);
        awakeBackground.SetActive(true);
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

        Health();
    }

    public virtual void Update()
    {
        timeSinceAttack += Time.deltaTime;
        animator.SetBool("Grounded", IsGrounded());
        Move(); //calls move method

        //if the player uses left click on their mouse, call the attack method
        if(Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f)
        {
            blocking = false; //cannot block while attacking

            //if else to change different attack animations
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

            //resets timer and calls attack method
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

        //for release of block button
        else if(Input.GetMouseButtonUp(1))
        {
            blocking = false;
            animator.SetBool("IdleBlock", false);
        }

        //shooting ranged weapon
        else if(Input.GetKeyDown(KeyCode.Q) && ammo > 0)
        {
            blocking = false;
            animator.SetTrigger("Attack2");
            Projectile proj = Instantiate(projPrefab, projOffset.position, transform.rotation);         
            ammo--;
            ammoText.text = "x" + ammo;
        } 
    }

    //moves the player
    public virtual void Move()
    {
        if(Time.timeScale == 1)
        {
            float x = Input.GetAxisRaw("Horizontal");
            transform.position += new Vector3(x, 0, 0) * Time.deltaTime * moveSpeed;

            //turning
            if(!Mathf.Approximately(0,x))
                transform.rotation = x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            //jumping
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                blocking = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("Jump");
            }

            //running
            else if (Mathf.Abs(x) > Mathf.Epsilon)
            {

                delayToIdle = 0.05f;
                animator.SetInteger("AnimState", 1);
            }

            //idle
            else
            {
                //prevents flickering transitions
                delayToIdle -= Time.deltaTime;
                if (delayToIdle < 0)
                    animator.SetInteger("AnimState", 0);
            }
        }
    }

    //checks if the player is on the ground
    public bool IsGrounded()
    {
        //using box collider and casting
        Vector2 size = new Vector2(boxColliderPlayer.bounds.size.x * 0.99f, boxColliderPlayer.bounds.size.y * 0.5f);

        return Physics2D.BoxCast(boxColliderPlayer.bounds.center, size, 0f, Vector2.down, .5f, ground);
    }

    //private void OnDrawGizmos()
    //{
        
    //}

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

    //detects collision with player
    public void OnCollisionEnter2D(Collision2D c)
    {
        GameObject other = c.gameObject;

        //enemy collision
        if (other.tag == "Skeleton")
        {
            float dmg = 1f;
            if (!wakefullness)
                dmg *= 2;

            TakeDamage(dmg);
        }


        //various pickup collisions


        else if(other.tag == "Heart")
        {
            if (currentHealth < maxHealth)
            {
                currentHealth++;
                Health();
            }
            Destroy(other);
        }

        else if (other.tag == "Ammo")
        {
            ammo++;
            ammoText.text = "x" + ammo;
            Destroy(other);
        }

        else if(other.tag == "Wake")
        {
            WakeBar.S.WakeUp(0.5f);
            Destroy(other);
        }

        else if(other.tag == "Key")
        {
            keyImage.enabled = true;
            hasKey = true;
            Destroy(other);
        }

        else if(other.tag == "Lock")
        {
            if (hasKey)
            {
                hasKey = false;
                keyImage.enabled = false;
                Destroy(other);
                Debug.Log("You Win!");
                Destroy(Data.S);
                SceneManager.LoadScene(0);
            }
            else
                Debug.Log("Need a key!");
        }
    }

    //method that adjusts health when you get hit by enemy
    public virtual void TakeDamage(float damage)
    {
        if (!blocking)
        {
            currentHealth -= damage;
            animator.SetTrigger("Hurt");
            Debug.Log("You took " + damage + " damage.");
        }
        Health();

        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Invoke("Death", 1);
            Time.timeScale = 0f;
        }
    }

    //method to handle dying
    public void Death()
    {
        Destroy(Data.S.gameObject);
        SceneManager.LoadScene("Level 1"); //resets level when dead
    }

    //updates health UI
    public void Health()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    //saves data
    public void SaveData()
    {
        Data.S.attackDamage = attackDamage;
        Data.S.jumpForce = jumpForce;
        Data.S.moveSpeed = moveSpeed;
        Data.S.maxHealth = maxHealth;
    }
}
