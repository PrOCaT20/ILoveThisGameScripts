using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 8.5f, jumpForce = 15f;
    public Transform feet;

    private Rigidbody2D rb;
    private AudioSource JumpSound;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public bool canMove = true;
    private Pause pause;
    public GameObject playerParts;

    public Vector2 RespawnPoint = new Vector2(-5.69f, -3.656f);
    public bool dieOnFall = false;

    private Sprite fallingSprite, defaultSprite;

    private float _coyoteEndTime;
    public float coyoteTime = 0.15f;
    bool canJump = true;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DeadZone"))
        {
            if (!dieOnFall)
            {
                PlayerPrefs.SetInt("DeathsCount", PlayerPrefs.GetInt("DeathsCount") + 1);
                transform.position = RespawnPoint;
                rb.velocity = Vector2.zero;
            }
            else
            {
                PlayerPrefs.SetInt("DeathsCount", PlayerPrefs.GetInt("DeathsCount") + 1);
                GameManager.Instance.ReloadLevel();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NextLevelZone"))
        {
            int levelNumber;
            if (int.TryParse(SceneManager.GetActiveScene().name, out levelNumber) && levelNumber == PlayerPrefs.GetInt("LastCompletedLevel"))
                PlayerPrefs.SetInt("LastCompletedLevel", levelNumber + 1);
            PlayerPrefs.SetInt("lvlTip" + int.TryParse(SceneManager.GetActiveScene().name, out levelNumber), 1);

            SceneManager.LoadScene((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());
        }
    }

    public void PauseGame() => pause.PauseGame();

    void Start()
    {
        JumpSound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        pause = FindObjectOfType<Pause>();

        fallingSprite = Resources.Load<Sprite>("Sprites/fallingSprite");
        defaultSprite = Resources.Load<Sprite>("Sprites/defaultSprite");
    }

    void Update()
    {
        if (canMove)
        {
            isGrounded = Physics2D.OverlapBox(feet.position, new Vector2(-0.4f, 0.05f), 0f, LayerMask.GetMask("Ground"));

            animator.SetBool("isGrounded", isGrounded);
            //float move = Input.GetKey("d") ? 1f : Input.GetKey("a") ? -1f : 0f;      old player movement v=0->v=1
            float move = Mathf.MoveTowards(rb.velocity.x, Input.GetAxisRaw("Horizontal") * speed, 180f * Time.deltaTime);
            rb.velocity = new Vector2(move, rb.velocity.y);

            if (!isGrounded)
            {
                animator.SetFloat("Speed", 0f);
                spriteRenderer.sprite = fallingSprite;
            }
            else
            {
                animator.SetFloat("Speed", Mathf.Abs(move));
                spriteRenderer.sprite = Mathf.Abs(move) > 0 ? defaultSprite : fallingSprite;
            }

            if (move != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(move) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            if (isGrounded && Input.GetButtonDown("Jump") && canJump)
            {
                ExecuteJump();
                canJump = false;
                Invoke("ResetJumpFlag", 0.17f);
            }

            if (isGrounded)
            {
                _coyoteEndTime = Time.time + coyoteTime;
            }
            else if (Time.time < _coyoteEndTime && Input.GetButtonDown("Jump") && canJump)
            {
                ExecuteJump();
                canJump = false;
                Invoke("ResetJumpFlag", 0.17f);
            }
        }
    }

    void ResetJumpFlag() => canJump = true;

    private void ExecuteJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        JumpSound.Play();
        PlayerPrefs.SetInt("JumpsCount", PlayerPrefs.GetInt("JumpsCount") + 1);
    }

    public void DestroyPlayer()
    {
        if (playerParts != null)
        {
            playerParts.SetActive(true);
            spriteRenderer.enabled = false;
            enabled = false;
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        spriteRenderer.sprite = defaultSprite;
        animator.enabled = false;
        rb.velocity = Vector3.zero;
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.enabled = true;
    }
}
