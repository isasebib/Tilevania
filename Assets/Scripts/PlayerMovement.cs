using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rigidBody;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 4f;
    [SerializeField] float climbSpeed = 140f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    int collectedCoins = 0;

    [SerializeField] AudioClip coinSFX;

    [SerializeField] float deathDelayTime = 1f;
    [SerializeField] float FinishDelayTime = 1f;
    float gravity;
    bool isAlive = true;
    Animator myAnimator;
    CapsuleCollider2D capsuleCollider;
    BoxCollider2D boxCollider;
    bool canJump = true;
    [SerializeField] TextMeshProUGUI victoryText;
    [SerializeField] float victoryDelayTime = 5f;
    [SerializeField] float victoryTextDelay = 1.5f;
    bool finished;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] Transform coinsParent;

    int totalCoins;



    void Start()
    {
        if (victoryText != null) 
        {
            victoryText.gameObject.SetActive(false);
            victoryText.text = "VICTORY";
        }


        totalCoins = coinsParent.childCount;
        UpdateCoinText();

        rigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        GetGravity();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = collectedCoins + "/" + totalCoins;
    }

    void Update()
    {
        if(!isAlive) {return;}
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if(!isAlive) {return;}
        moveInput = value.Get<Vector2>();
        
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rigidBody.linearVelocity.y);
        rigidBody.linearVelocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidBody.linearVelocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);    
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rigidBody.linearVelocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidBody.linearVelocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {

        if (value.isPressed && boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && canJump)
        {
            rigidBody.linearVelocity += new Vector2(0f, jumpSpeed);
        }
    }

    void ClimbLadder()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(rigidBody.linearVelocity.y) > Mathf.Epsilon;
        
        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")) )
        {
            myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
            Vector2 climbVelocity = new Vector2(rigidBody.linearVelocity.x, moveInput.y * climbSpeed);
            rigidBody.linearVelocity = climbVelocity;
            rigidBody.gravityScale = 0;
            canJump = false;
        }

        else
        {
            myAnimator.SetBool("isClimbing", false);
            rigidBody.gravityScale = gravity;
            canJump = true;

        }


    }

    void GetGravity()
    {
        gravity = rigidBody.gravityScale;
    }

    void Die()
    {
        if(capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemy" ,"Hazards", "Water")) || boxCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards", "Water")))
        {
            isAlive = false;
            rigidBody.linearVelocity = new Vector2 (0, 0);
            myAnimator.SetTrigger("Dying"); 
            capsuleCollider.enabled = false;
            boxCollider.enabled = false;
            rigidBody.gravityScale = 1f;
            Invoke(nameof(ReloadScene), deathDelayTime);
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.layer == LayerMask.NameToLayer("Coin"))
        {
            if (!collision.gameObject.activeSelf) return;
            
            AudioSource.PlayClipAtPoint(coinSFX, transform.position);
            collision.gameObject.SetActive(false);
            collectedCoins++;
            Destroy(collision.gameObject);
            UpdateCoinText();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Door") && !finished)
        {
            finished = true;
            myAnimator.SetTrigger("Finished");
            StartCoroutine(LoadNextScene());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hazards"))
        {
            Die();
        }
    }

    void OnAttack(InputValue value)
    {
        if(!isAlive) {return;}
        Instantiate(bullet, gun.position, transform.rotation);
    }

    public void ReloadScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }


    IEnumerator LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        bool isLastLevel = nextIndex == SceneManager.sceneCountInBuildSettings;

        if (isLastLevel)
        {
            yield return new WaitForSecondsRealtime(victoryTextDelay);

            if (victoryText != null)
                victoryText.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(victoryDelayTime);
            SceneManager.LoadScene(0);
        }
        else
        {
            yield return new WaitForSecondsRealtime(FinishDelayTime);
            SceneManager.LoadScene(nextIndex);
        }
    }



}
