using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioItem;
    public AudioClip audioRun;
    AudioSource audioSource;

    public float speed;
    float runSpeed;
    float hAxis;
    float vAxis;
    bool run;
    bool jDown;
    bool isJump;
    
    Vector3 moveVec;

    Animator anim;

    Rigidbody rigid;

    public int flower;
    public int maxFlower;

    public int hunter;
    public int maxHunter;

    public GameObject[] flowers;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        this.audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput() 
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButtonDown("Jump");
        run = Input.GetKey(KeyCode.LeftShift);
        if (run)
            runSpeed = 2f;
    
        else
            runSpeed = 1f;
    }

        void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * runSpeed * speed * Time.deltaTime;
       
        anim.SetBool("isWalking", moveVec != Vector3.zero);
        anim.SetBool("isRunning", run);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", true);
            rigid.AddForce(Vector3.up * 20, ForceMode.Impulse);
            isJump = true;
            PlaySound(1);
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type) {
                case Item.Type.Flower :
                    flowers[flower].SetActive(true);
                    flower += item.value;
                    if(flower > maxFlower)
                        flower = maxFlower;
                    break;
                
                case Item.Type.Hunter :
                    hunter += item.value;
                    if(hunter > maxHunter)
                        hunter = maxHunter;
                    break;
            }

            PlaySound(2);

            Destroy(other.gameObject);
        }      
    }

    void PlaySound(int action) {
        switch (action) {
            case 1:
                audioSource.clip = audioJump;
                break;

            case 2:
                audioSource.clip = audioItem;
                break;

            case 3:
                audioSource.clip = audioRun;
                break;
        }
        audioSource.Play();
    }
    
}

