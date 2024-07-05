using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 5.0f;
    public int nowLove = 1;
    public int maxLove = 6;
    private LoveGaugeManager loveGaugeManager;
    public float jumpPw = 5.0f;
    bool goJump = false;
    public LayerMask groundLayer;
    private SpriteRenderer spriteRenderer;
    private SceneController sceneController;
    private LoveItemManager loveItemManager;
    bool isButtonInput = false;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        loveGaugeManager = FindObjectOfType<LoveGaugeManager>();
        gameManager = FindObjectOfType<GameManager>();
        UpdateLoveGauge();
    }

    void Update()
    {
        

        if(isButtonInput==false){
            //変数のaxisHに変動がない場合はキー入力を受け取る
            axisH = Input.GetAxisRaw("Horizontal");
        }

        Move();

        if (Input.GetButton("Jump"))
        {
            Jump();
    }
    }

    void FixedUpdate()
    {
        if (!gameManager.IsGamePaused()){
        if (goJump)
        {
            Vector2 jumpVec = new Vector2(0, jumpPw);
            rbody.velocity = new Vector2(rbody.velocity.x, 0);
            rbody.AddForce(jumpVec, ForceMode2D.Impulse);
            goJump = false;
        }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LoveItem"))
        {
            loveItemManager = collision.GetComponent<LoveItemManager>();
            IncreaseLove(loveItemManager.loveUp);
            Destroy(collision.gameObject);
        }
    }

    public bool onGroundCheck(GameObject obj)
    {
        return Physics2D.CircleCast(transform.position, 0.2f, Vector2.down, 0.0f, groundLayer);
    }

    public void Move()
    {
        if (gameManager.IsGamePaused()){
            axisH = 0;
        }
        if (axisH > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (axisH < 0)
        {
            spriteRenderer.flipX = true;
        }

        if (onGroundCheck(gameObject) || axisH != 0)
        {
            rbody.velocity = new Vector2(speed * axisH, rbody.velocity.y);
        }
    }

    public void MoveLeft()
    {
        isButtonInput = true;
        axisH = -1.0f;
    }

    public void MoveRight()
    {
        isButtonInput = true;
        axisH = 1.0f;
    }

    public void StopMoving()
    {
        isButtonInput = false;
        axisH = 0;
    }

    public void Jump()
    {
        if (onGroundCheck(gameObject) && !gameManager.IsGamePaused())
        {
            goJump = true;
        }
    }

    public void IncreaseLove(int addLove)
    {
        for (int i = 0; i < addLove; i++)
        {
            nowLove++;
            UpdateLoveGauge();
            if (nowLove >= maxLove)
            {
                Debug.Log("BIG Love!hoge!!!!!!!!");
                gameManager.GoHome();
                return;
            }
        }
    }

    public void IncreaseMaxLove()
    {
        if (maxLove < loveGaugeManager.hearts.Length)
        {
            maxLove++;
            UpdateLoveGauge();
        }
    }

    private void UpdateLoveGauge()
    {
        if (loveGaugeManager != null)
        {
            loveGaugeManager.UpdateHearts(nowLove, maxLove);
        }
    }
}