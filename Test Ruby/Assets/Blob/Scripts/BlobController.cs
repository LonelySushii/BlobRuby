using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

public class BlobController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;

    public GameObject projectilePrefab;
    public GameObject interactGameObject;
    public GameObject portalGameObject;
    public GameObject area1Col;
    public GameObject area2Col;
    public GameObject area3Col;
    public GameObject area4Col;
    public GameObject[] abilityicons;

    public AudioSource musicAud;
    public AudioClip eatSound;
    public AudioClip throwSound;
    public AudioClip hitSound;
    private AudioSource audioSource;

    public int Health { get { return currentHealth; } }
    private int currentHealth;
    private int currentemyeaten;
    private int currBlobAnimIndex;

    public float timeInvincible = 2.0f;
    private bool isInvincible;
    private bool isMoving;
    private float invincibleTimer;

    private Rigidbody2D rigidbody2d;
    private float horizontal;
    private float vertical;

    public PlayableDirector outroTimeline;
    public Animator postProcessAnim;
    public RuntimeAnimatorController[] blobCharacterAnim;
    public Animator cameraanim;
    public Animator fadeBG;
    private Animator blobAnim;
    private Vector2 lookDirection = new Vector2(1, 0);

    private Collider2D _col2D;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        blobAnim = GetComponent<Animator>();

        currentHealth = maxHealth;
        isMoving = true;
        currBlobAnimIndex = 1;
        audioSource = GetComponent<AudioSource>();

        fadeBG.Play("Fade_In");
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

        if (isMoving)
        {
            blobAnim.SetFloat("Look X", lookDirection.x);
            blobAnim.SetFloat("Look Y", lookDirection.y);
            blobAnim.SetFloat("Speed", move.magnitude);
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Launch();
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
        //    if (hit.collider != null)
        //    {
        //        NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
        //        if (character != null)
        //        {
        //            character.DisplayDialog();
        //        }
        //    }
        //}

        if (_col2D != null && Input.GetKeyDown(KeyCode.X))
        {
            if (_col2D.GetComponent<NonPlayerCharacter>() != null)
                _col2D.GetComponent<NonPlayerCharacter>().DisplayDialog();
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            Destroy(collision.gameObject);
            blobAnim.SetTrigger("Eating");
            if (currentemyeaten < abilityicons.Length)
                abilityicons[currentemyeaten].SetActive(true);

            currentemyeaten++;
            Playsound(eatSound);

            if (currentemyeaten == 1)
                area1Col.SetActive(false);

            if (currentemyeaten == 2)
            {
                area2Col.SetActive(false);
                musicAud.mute = false;
            }

            if (currentemyeaten == 3)
                area3Col.SetActive(false);

            if (currentemyeaten == 4)
                area4Col.SetActive(false);

            if (currentemyeaten == 5)
            {
                portalGameObject.SetActive(true);
                outroTimeline.Play();
            }

            cameraanim.Play("camerazoom");
            Debug.Log("Eating");
        }

        if (collision.CompareTag("NPC") && currentemyeaten >= 3)
        {
            interactGameObject.SetActive(true);
            _col2D = collision;
        }

        if (collision.CompareTag("Finish"))
            StartCoroutine(EndGameDelay());
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("NPC"))
        {
            interactGameObject.SetActive(false);
            _col2D = null;
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

        blobAnim.SetTrigger("Launch");

        Playsound(throwSound);
    }

    public void Playsound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void FreezeRuby()
    {
        isMoving = false;
        blobAnim.SetFloat("Look X", 0);
        blobAnim.SetFloat("Look Y", 0);
        blobAnim.SetFloat("Speed", 0);
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
        blobAnim.runtimeAnimatorController = blobCharacterAnim[currBlobAnimIndex];
        Debug.Log("Switching Anim Controllers");
        //}

        currBlobAnimIndex++;
        Debug.Log("Updating currBlobAnimIndex");
    }

    IEnumerator EndGameDelay()
    {
        fadeBG.Play("Fade_Out");
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel(0);
    }
}