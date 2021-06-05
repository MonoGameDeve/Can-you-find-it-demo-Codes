using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] AudioClip jumpsfx;
    [SerializeField] AudioClip diesfx;

    Rigidbody2D myRigidBody2D;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    Animator myAnimator;

    bool playerIsAlive = true;
    float gravityScaleAtStart;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent < Rigidbody2D >();
        myBodyCollider = GetComponent < CapsuleCollider2D >();
        myFeetCollider = GetComponent < BoxCollider2D >();
        myAnimator = GetComponent < Animator >();
        gravityScaleAtStart = myRigidBody2D.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerIsAlive) { return; }
        Run();
        ClimbRope();
        FlipPlayer();
        Jump();
        Die();
    }

    private void Run()
    {
        float movementInHorizontal = Input.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(movementInHorizontal * runSpeed , myRigidBody2D.velocity.y);
        myRigidBody2D.velocity = playerVelocity;

        bool playerHorizontalSpeed = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("RunHd", playerHorizontalSpeed);
    }

    private void ClimbRope()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Rope")))
        {
            myAnimator.SetBool("ClimbHd", false);
            myRigidBody2D.gravityScale = gravityScaleAtStart;
            return;
        }
        float movementInVertical = Input.GetAxis("Vertical");
        Vector2 climbRopeVelocity = new Vector2(myRigidBody2D.velocity.x, movementInVertical * climbSpeed);
        myRigidBody2D.velocity = climbRopeVelocity;
        myRigidBody2D.gravityScale = 0f;

        bool playerVerticalSpeed = Mathf.Abs(myRigidBody2D.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("ClimbHd", playerVerticalSpeed);

    }

    private void Jump()
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){ return; }
        if(Input.GetButtonDown("Jump"))
        {
            AudioSource.PlayClipAtPoint(jumpsfx, Camera.main.transform.position);
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody2D.velocity += jumpVelocity;
        }
    }

    public void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards", "Lava")))
        {
            playerIsAlive = false;
            myAnimator.SetTrigger("DieHd");
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
            AudioSource.PlayClipAtPoint(diesfx, Camera.main.transform.position);
        }
    }

    private void FlipPlayer()
    {
        bool playerHorizontalSpeed = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;
        if(playerHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody2D.velocity.x), 1f);
        }
    }
}
