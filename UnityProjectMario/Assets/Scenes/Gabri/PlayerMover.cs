using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;



public class PlayerMover : MonoBehaviour {

    float tickTimer = 2f;
    float timer = 0f;
    public int hits = 2;

    Coins coins;
    GameManager gm;
    Rigidbody2D rb;
    [Header("Move info")]
    public float speed = 7f;
    bool wallhangPressed = false;
    public bool jumpPressed = false;
    bool momentumPressed = false;
    bool facingRight = true;
    bool grounded;
    bool canMove = true;
    bool onWall = false;
    bool hitbrick = false;
    public  bool buttSlumpPressed = false;
    bool crouchPressed = false;
    public float deathFromFallingY = -10;
    bool dead;
    [SerializeField] Vector2 wallJumpDirection;
    [Range(1, 10)] public float jumpVelocity;
    SpriteRenderer bigSprite;
    SpriteRenderer smallSprite;
    SpriteRenderer FireSprite;
    private Vector3 respawnPosition;
    bool rushPressed = false;



    [Header("change status")]
    public GameObject MarioMini;
    public GameObject MarioBig;
    public GameObject FireMan;
    public GameObject fireball;
    public Transform Firestart;
     public Animator animMario_1;
    public float fireDistance = 1f;
   



    [Header("Collision info")]
    public LayerMask Platform;
    public LayerMask Wall;
    public LayerMask Brick;
    public Transform groundCheckPoint;
   // public Transform brickCheckPoint;
    public static bool MarioIsSmall = false;
    public Transform wallGrabPoint;
    public float grabDistance = 1f;
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;



    [Header("Audio info")]
    [SerializeField] private AudioSource Maintheme;
    [SerializeField] private AudioSource FireballSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource walljump;
    [SerializeField] private AudioSource loose;
    [SerializeField] private AudioSource smallCo;
    [SerializeField] private AudioSource bigCo;
    [SerializeField] private AudioSource victory;
    [SerializeField] private AudioSource die;





    // enum animState { Idle, Run, Jump, Death,SmallMario,NormalMario }; TODO BIG Mario Small Mario

    void Awake() {
        respawnPosition = transform.position;
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        bigSprite = transform.Find("Mario_1").GetComponent<SpriteRenderer>();
        smallSprite = transform.Find("MarioMini").GetComponent<SpriteRenderer>();
        FireSprite = transform.Find("FireMan").GetComponent<SpriteRenderer>();
      
    }

    private void FixedUpdate() {
        if (jumpPressed) {
            Jump();
          jumpPressed = false;
           

        }

        if (buttSlumpPressed) {
            Buttslump();
           // buttSlumpPressed = false;
        }
       
        if (wallhangPressed) {
            WallHanging();
            wallhangPressed = false;
            
        }
        if (rushPressed) {
            Rush();

            rushPressed = false;
            animMario_1.SetBool("Run", false);

        }
        if (crouchPressed) {
            Crouch();
            crouchPressed = false;
            canMove = true;
        }
        if (canMove) {
            Move();
        }
        
        if (hits <= 0 || transform.position.y < this.deathFromFallingY) {
           
        
                Die();
               
            
            transform.position = respawnPosition;


        }

    }


    void Update() {



        if (Input.GetKey(KeyCode.E)) {
            rushPressed = true;
            animMario_1.SetBool("Run", true);
        } else {
            speed = 7;
           
        }
        SetMarioState();

        CollisionCheck();
        //Abl to shoot
        if (hits == 3) {
            if (Input.GetKeyDown(KeyCode.E)) {
                if (fireList.Count < 2) {
                    Shoot();
                    FireballSound.Play();
                }
            }
        }

       /* if (!grounded) {
            jumpPressed = true;
        } else if (grounded){ jumpPressed = !jumpPressed; }*/

        if (Input.GetKeyDown(KeyCode.X)) { if (grounded == false) { buttSlumpPressed = true; } }
        if (Input.GetKeyDown(KeyCode.Space)) {
            animMario_1.SetBool("Jump", true);
            jumpPressed = true;

        }
       
     /*   if (Input.GetKey(KeyCode.X)) {
            canMove = false;
            crouchPressed = true;
        } else { speed = 7; }*/

       

        //CHECK TO BE ABLE TO WALLJUMP
        if (onWall == true && grounded == false) {
            if (facingRight && Input.GetAxis("Horizontal") > 0 || (!facingRight && Input.GetAxis("Horizontal") < 0)) {

                wallhangPressed = true;
                animMario_1.SetBool("WallHang", true);
                animMario_1.SetBool("Jump", false);
            }
        }
        //tO MOVE AFTER WALLJUMP
        if (grounded) {
           
        canMove = true;
        buttSlumpPressed = false;
            animMario_1.SetBool("Jump", false);
            animMario_1.SetBool("WallHang", false);



        }
        if (!grounded && !onWall) {
            animMario_1.SetBool("Jump", true);
          //  animMario_1.SetBool("WallHang", false);

        }


        if (gm.lives == 0) {
            Maintheme.Stop();
           die.Play();
            Invoke("CallGameOver", 3);
        }


       
        if (Input.GetAxis("Horizontal") < -0.1f) {
            facingRight = false;
        } else if (Input.GetAxis("Horizontal") > 0.1f) {
            facingRight = true;
        }
        
        animMario_1.SetFloat("Speed", MathF.Abs(Input.GetAxis("Horizontal")));
        Firestart.localPosition = Vector3.right * fireDistance * (facingRight ? 1 : -1);
        wallGrabPoint.localPosition = Vector3.right * grabDistance * (facingRight ? 1 : -1);


        bigSprite.flipX = !facingRight;
        smallSprite.flipX = !facingRight;
        FireSprite.flipX = !facingRight;


    }
    public void CallGameOver() {
        FindObjectOfType<GameOverscript>().GameOver();
    }

    public void CollisionCheck() {
        grounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, Platform);
        onWall = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, Wall);
       // hitbrick = Physics2D.OverlapCircle(brickCheckPoint.position, .2f, Brick);
    }
    public void Move() {

        float HorizotalMov = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(HorizotalMov * speed, rb.velocity.y);


    }

    public void Big() {
        MarioMini.SetActive(false);
        MarioBig.SetActive(true);
        FireMan.SetActive(false);
        MarioIsSmall = false;

    }
    public void Small() {
        MarioMini.SetActive(true);
        MarioBig.SetActive(false);
        FireMan.SetActive(false);
        MarioIsSmall = true;
    }
    public void FireMario() {
        MarioBig.SetActive(false);
        MarioMini.SetActive(false);
        FireMan.SetActive(true);
        MarioIsSmall = false;
    }

    public List<GameObject> fireList = new List<GameObject>();


    public void Shoot() {
        var fireB = Instantiate(fireball, Firestart.position, Firestart.rotation);
        var fireballmov = fireB.GetComponent<Fireball>();
        fireballmov.pm = this;
        fireballmov.velocity = new Vector2(fireballmov.velocity.x * (facingRight ? 1 : -1), fireballmov.velocity.y);
        fireList.Add(fireB);

    }

    public void Rush() {
        speed = 11f;
        print("rush");

    }

    public void WallHanging() {

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.8f);

    }

    public void Jump() {


        if (wallhangPressed) {
            {

                rb.velocity = new Vector2(-Input.GetAxisRaw("Horizontal") * wallJumpDirection.x, wallJumpDirection.y);
                walljump.Play();
                canMove = false;

            }
        } else if (grounded) {


            rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            grounded = false;

            jumpSound.Play();
        }


    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            hits--;
            StartCoroutine(Invulnerability());
        }
        if (collision.gameObject.tag == "smallcoin") {
            smallCo.Play();
        }
        if (collision.gameObject.tag == "bigCoin") {
            bigCo.Play();
        }
        if (collision.tag == "Checkpoint") {
            respawnPosition = transform.position;
        }
        if (collision.tag == "EndPoint") {
            Maintheme.Stop();
            victory.Play();
            Invoke("CallGameOver", 7f);
            //FindObjectOfType<GameOverscript>().GameOver();
        }
    }

    public void Die() {

        //  animator.Play("death");
        // GetComponentInChildren<Collider2D>().enabled = false;


        gm.lives-=1;
        hits = 1;
         gm.UpdateLivesText();
    }
    public void Buttslump() {


        rb.velocity = Vector2.down * speed*2;

    }
      
   
    public void Crouch() {
        speed = 0  ;
        //playanimation
        print("crouch");

    }

    public void SetMarioState() {


        if (hits == 1) {
            Small();
        }

        if (hits == 2) {
            Big();
        }
        if (hits == 3) {
            FireMario();
        }


    }
    private IEnumerator Invulnerability() {

        Physics2D.IgnoreLayerCollision(3, 7, true);
        for (int i = 0; i < numberOfFlashes; i++) {

            bigSprite.color = new Color(1, 0, 0, 0.5f);
            smallSprite.color = new Color(1, 0, 0, 0.5f);
            FireSprite.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes) * 2);
            bigSprite.color = Color.white;
            smallSprite.color = Color.white;
            FireSprite.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes) * 2);
        }

        Physics2D.IgnoreLayerCollision(3, 7, false);
    }


    public void StompJump() {
        rb.AddForce(Vector2.up * jumpVelocity / 2, ForceMode2D.Impulse);
    }

}






