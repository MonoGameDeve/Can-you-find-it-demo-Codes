# Can-you-find-it-demo-Codes

Chest Code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    Animator myChestAnimation;
    BoxCollider2D frame;

    [SerializeField] int treasureToAdd = 10;
    [SerializeField] AudioClip treasureChestOpensfx;

    private void Start()
    {
        myChestAnimation = GetComponent<Animator>();
        frame = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        myChestAnimation.SetTrigger("OpenChest");
        FindObjectOfType<GameSession>().AddToTreasure(treasureToAdd);
        AudioSource.PlayClipAtPoint(treasureChestOpensfx, Camera.main.transform.position);
        Destroy(frame);
    }
}

Player Code:
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

Game Session Code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int treasure = 0;
    [SerializeField] int relic = 0;
    [SerializeField] float ShowDeathDelay = 2f;
    [SerializeField] float delayWin = 1f;
    [SerializeField] Text treasureText;
    [SerializeField] Text relicText;

    private void Awake()
    {
        var numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions < 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        treasureText.text = treasure.ToString();
        relicText.text = relic.ToString();
    }

    public void AddToRelic(int RelicToAdd)
    {
        relic += RelicToAdd;
        relicText.text = relic.ToString();
    }

    public void AddToTreasure(int TreasureToAdd)
    {
        treasure += TreasureToAdd;
        treasureText.text = treasure.ToString();
    }

    public void ProcessPlayerDeath()
    {
        StartCoroutine(ShowDeath());
        //SceneManager.LoadScene("You Died");
    }

    IEnumerator ShowDeath()
    {
        yield return new WaitForSecondsRealtime(ShowDeathDelay);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("You Died");
    }

    public void ProcessPlayerWin()
    {
        StartCoroutine(DelayWin());
    }

    IEnumerator DelayWin()
    {
        yield return new WaitForSecondsRealtime(delayWin);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("You Win");
    }
}

Relic Code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Relic : MonoBehaviour
{
    [SerializeField] int relicToAdd = 1;
    [SerializeField] AudioClip winsfx;

    void OnTriggerEnter2D(Collider2D collision)
    {
        FindObjectOfType<GameSession>().AddToRelic(relicToAdd);
        Destroy(gameObject);
        FindObjectOfType<GameSession>().ProcessPlayerWin();
        AudioSource.PlayClipAtPoint(winsfx, Camera.main.transform.position);
    }
}

Sound Code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioClip soundsfx;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioSource.PlayClipAtPoint(soundsfx, Camera.main.transform.position);
    }
}

Pause Menu Code:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
