using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 8.5f, jumpForce = 15f; // speed: velocitat jugador. jumpForce: força de salt/altura
    public Transform feet; // les coordenades dels peus del jugador. Serveix per no poder saltar infinitament i escanejar si esta a terra el jugador

    private Rigidbody2D rb; // Rigidbogy: Component que se li afegeix a un Objecte que té fisiques
    private AudioSource JumpSound; 
    private bool isGrounded;
    private SpriteRenderer spriteRenderer; // SpriteRenderer: Component que se li afegeix a un Objecte perque sigui visible
    private Animator animator; // animator: Component que se li afegeix a un Objecte que te animacions
    public bool canMove = true;  

    public Vector2 RespawnPoint = new Vector2(-5.69f, -3.656f); // coordenades per respawn
    public bool dieOnFall = false; // si cau del mapa, reinicia el nivell si dieOnFall = true

    private Sprite fallingSprite, defaultSprite; // Imatges del jugador quan esta a l'aire i quan esta quiet

    private float _coyoteEndTime; 
    public float coyoteTime = 0.15f;
    bool canJump = true;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DeadZone")) // si surt d'una zona que es diu DeadZone, ...
        {
            if (!dieOnFall) // si die on fall es false, ...
            {
                PlayerPrefs.SetInt("DeathsCount", PlayerPrefs.GetInt("DeathsCount") + 1); // augmenta el comptador de les morts una unitat
                transform.position = RespawnPoint; // respawn
                rb.velocity = Vector2.zero; // reinicia la velocitat del objecte
            }
            else // si dieOnfall es true, ...
            {
                PlayerPrefs.SetInt("DeathsCount", PlayerPrefs.GetInt("DeathsCount") + 1); // augmenta el comptador de les morts una unitat
                GameManager.Instance.ReloadLevel(); // Crida la fucnio de reinici del nivell al GameManager
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("NextLevelZone")) // si entra a una zone que es diu NextLevelZone, Carga el seguent nivell i guarda a la memoria l'ultim nivell completat
        {
            int levelNumber;
            if (int.TryParse(SceneManager.GetActiveScene().name, out levelNumber) && levelNumber == PlayerPrefs.GetInt("LastCompletedLevel"))
                PlayerPrefs.SetInt("LastCompletedLevel", levelNumber + 1);
            PlayerPrefs.SetInt("lvlTip" + int.TryParse(SceneManager.GetActiveScene().name, out levelNumber), 1);

            SceneManager.LoadScene((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());
        }
    }

    void Start() // Fes aixo al primer frame una vegada (quan inicies el joc)
    {
        JumpSound = GetComponent<AudioSource>(); 
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();            // Trobar components del jugador
        animator = GetComponent<Animator>();

        fallingSprite = Resources.Load<Sprite>("Sprites/fallingSprite"); // assigna els sprites a la variable desde els arxius del joc
        defaultSprite = Resources.Load<Sprite>("Sprites/defaultSprite");
    }

    void Update() // fes aixo una vegada cada frame (tot el rato)
    {
        if (canMove) // si li és permes moure's
        {
            isGrounded = Physics2D.OverlapBox(feet.position, new Vector2(-0.4f, 0.05f), 0f, LayerMask.GetMask("Ground")); // mira si toca el terra

            animator.SetBool("isGrounded", isGrounded); // strimejar la velocitat al animator perque pugui saber si el jugador es mou
            float move = Mathf.MoveTowards(rb.velocity.x, Input.GetAxisRaw("Horizontal") * speed, 180f * Time.deltaTime); // calcular velocitat del jugador
            rb.velocity = new Vector2(move, rb.velocity.y); // aplicar la velocitat sobre el jugador utilitzant variable move 


// animacio de caure o estar quiet. "start"
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
    // animacio de caure o estar quiet. "end"

            if (isGrounded && Input.GetButtonDown("Jump") && canJump) // si es clica tecla de saltar
            {
                ExecuteJump(); // saltar
                canJump = false; // pot saltar false
                Invoke("ResetJumpFlag", 0.17f); // espera per la seguent oportunitat per saltar
            }

            if (isGrounded) // coyote time. No se com explicar-ho, pero és una cosa que "millora" el salt
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
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); // emputxa jugador adalt
        JumpSound.Play(); // so de saltar
        PlayerPrefs.SetInt("JumpsCount", PlayerPrefs.GetInt("JumpsCount") + 1); // augmenta el comptador dels salts una unitat
    }

    public void DisableMovement() // inmobilitzar jugador
    {
        canMove = false;
        spriteRenderer.sprite = defaultSprite;
        animator.enabled = false;
        rb.velocity = Vector3.zero;
    }

    public void EnableMovement() // fer que li sigui possible moure's
    {
        canMove = true;
        animator.enabled = true;
    }
}
