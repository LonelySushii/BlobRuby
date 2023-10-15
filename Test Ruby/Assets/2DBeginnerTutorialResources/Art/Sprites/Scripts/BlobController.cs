using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;

    public GameObject projectilePrefab;
    public GameObject area1Col;
    public GameObject area2Col;
    //public GameObject postprocessing;
    public GameObject[] abilityicons;
    public Animator postProcessAnim;
    public RuntimeAnimatorController[] blobCharacterAnim;

    public AudioClip throwSound;
    public AudioClip hitSound;

    public int Health { get { return currentHealth; } }
    private int currentHealth;
    private int currentemyeaten;
    [SerializeField] private int currBlobAnimIndex;

    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private bool isMoving;
    private float invincibleTimer;

    private Rigidbody2D rigidbody2d;
    private float horizontal;
    private float vertical;

    public Animator cameraanim;
    private Animator rubyAnim;
    private Vector2 lookDirection = new Vector2(1, 0);

    private AudioSource audioSource;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        rubyAnim = GetComponent<Animator>();

        currentHealth = maxHealth;
        isMoving = true;
        currBlobAnimIndex = 1;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        rubyAnim.SetFloat("Look X", lookDirection.x);
        rubyAnim.SetFloat("Look Y", lookDirection.y);
        rubyAnim.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 position = rigidbody2d.position;
            position.x += speed * horizontal * Time.deltaTime;
            position.y += speed * vertical * Time.deltaTime;
            rigidbody2d.MovePosition(position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            Destroy(collision.gameObject);
            rubyAnim.SetTrigger("Eating");
            if (currentemyeaten < abilityicons.Length)
                abilityicons[currentemyeaten].SetActive(true);

            currentemyeaten++;

            if (currentemyeaten == 1)
                area1Col.SetActive(false);

            if (currentemyeaten == 2)
                area2Col.SetActive(false);

            cameraanim.Play("camerazoom");
            Debug.Log("Eating");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            Playsound(hitSound);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        rubyAnim.SetTrigger("Launch");

        Playsound(throwSound);
    }

    public void Playsound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void FreezeRuby()
    {
        isMoving = false;
    }

    public void UnFreezeRuby()
    {
        isMoving = true;
    }

    public void ColourizePostProcess()
    {
        if (currentemyeaten == 1)
            postProcessAnim.Play("PostProcessAnim");
    }

    public void SwitchCharacter()
    {
        //if (currBlobAnimIndex < blobCharacterAnim.Length)
        //{
        rubyAnim.runtimeAnimatorController = blobCharacterAnim[currBlobAnimIndex];
        Debug.Log("Switching Anim Controllers");
        //}

        currBlobAnimIndex++;
        Debug.Log("Updating currBlobAnimIndex");
    }
}