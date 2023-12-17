using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] float bulletSpeed;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
private Collider2D myCollider;

    


    private int health = 4;

     private  bool jumpbutton = false;
    private bool canShoot = true;
    private  bool attackButton = false;
 public GameObject bulletPrefab; 
 public GameObject winningPanal; 

 [SerializeField]
    public GameObject[] hearts = new GameObject[4];
   Mangment mangment = Mangment.returnManger();



    // Use this for initialization
    void Start ()
    {
        Application.targetFrameRate = 120;
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
         myCollider = GetComponent<Collider2D>();

    }
     private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health--;
             Destroy(collision.gameObject);
             hearts[health].SetActive(false);
        }
    }
    public void jump(){
        jumpbutton = true;
    }
    public void attack(){
        attackButton = true;
    }

    // Update is called once per frame
     void Update ()
    {
   
        m_timeSinceAttack += Time.deltaTime;

     
 

   

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

       

        //Death
        if (health <= 0)
        {
            m_animator.SetBool("noBlood", true);
            m_animator.SetTrigger("Death");
            winningPanal.SetActive(true);
             myCollider.enabled = false;
              Rigidbody2D rb = GetComponent<Rigidbody2D>(); 
        rb.constraints = ~RigidbodyConstraints2D.FreezePositionY;

            return;
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(canShoot && attackButton && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            canShoot = false;
            attackButton = false;
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);
            Vector3 spawnPosition;
            if (transform.position.x > 0){
            spawnPosition = transform.position + new Vector3(-1f, 1f, 0f);
           }else {
             spawnPosition = transform.position + new Vector3(1f, 1f, 0f);
           }
            GameObject bullet = Instantiate(bulletPrefab,spawnPosition , transform.rotation);
              bullet.tag = "Bullet"; 
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); // Assuming the bullet has a Rigidbody component
    
        // Move the bullet in a straight line (adjust speed as needed)
        rb.velocity = bullet.transform.right * bulletSpeed;

        // Destroy the bullet after 5 seconds
        Destroy(bullet, 5f);
     
            // Reset timer
            m_timeSinceAttack = 0.0f;
            attackButton = false;
             StartCoroutine(EnableShooting());
        }

        // Block
        else if (false)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (false)
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (false)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
            

        //Jump
        else if (jumpbutton && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
            jumpbutton = false;
        }

        //Run
        else if (false)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
IEnumerator EnableShooting()
{
    yield return new WaitForSeconds(0.5f);
    canShoot = true;
}

IEnumerator waitAlittel(float time)
{
    yield return new WaitForSeconds(time);
    Time.timeScale = 0f;
}
}
