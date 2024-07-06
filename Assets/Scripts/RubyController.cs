using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 5.0f;

    public int maxHealth = 5;
    public float timeInvincible = 1.0f; // 无敌时间2秒, 无敌时间结束后，Ruby 将再次受到伤害

    public int maxBullet = 20; // 最大子弹数量

    public int bullet { get { return currentBullet; } }
    int currentBullet; // 当前子弹数量

    public int health { get { return currentHealth; } }
    int currentHealth;

    public int maxLife = 3;
    public int life { get { return currentLife; } }
    int currentLife;
    public bool isDead;

    bool isInvincible; // defalut false
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public ParticleSystem PickEffect;

    public AudioSource audioSourceFootStep; // 脚步声音
    public AudioSource audioSourceOtherClip; // 其他声音
    public AudioClip hitClip;
    public AudioClip footStep;
    public AudioClip respawnSound;

    private Vector3 respawnPosition; // 重生位置

    public NonPlayerCharacter nearbyNPC;
    public float tipDistance = 1.5f; // 玩家需要接近NPC的距离
    public bool pressF = false;
    public bool pressF_success = false; // 是否任务完成后按下F键

    public float displayTime = 5.0f;
    float timerDisplay;
    public GameObject tipBox; // 提示框
    public TextMeshProUGUI tipText; // 提示框文本
    public GameObject success; // 菜单列表
    public GameObject die; // 死亡界面
    public AudioSource BGM;
    // 在第一次帧更新之前调用 Start
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentBullet = 5;
        currentLife = maxLife;
        PickEffect.Stop();

        audioSourceFootStep = GetComponents<AudioSource>()[0];
        audioSourceOtherClip = GetComponents<AudioSource>()[1];
        respawnPosition = transform.position; // 记录重生位置

        UIBullet.instance.bulletUIvisibility(false);
    }

    // 每帧调用一次 Update
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f)) // 当玩家输入的某个轴向的值不为 0 时，Ruby 将朝着该方向移动
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
            if (!audioSourceFootStep.isPlaying)
            {
                audioSourceFootStep.clip = footStep;
                audioSourceFootStep.Play();
            }
        }
        else
        {
            audioSourceFootStep.Stop();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false; // 取消无敌状态
        }

        if (Input.GetKeyDown(KeyCode.E)) // 按下 E 键时，Ruby 将发射一个飞弹
        {
            if (UIHealthBar.instance.fixedNum >= 12 && pressF_success)
            {
                success.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // 显示鼠标
                Time.timeScale = (0); // 暂停游戏
            }
            else
            {
                Launch();
            }
        }

        if (currentLife <= 0)
        {
            displayDie(); // 显示死亡界面
            currentLife = maxLife;
        }

        // 检测玩家与NPC的距离
        if (nearbyNPC != null)
        {
            float distance = Vector3.Distance(transform.position, nearbyNPC.transform.position);
            if (distance <= tipDistance && !pressF)
            {
                nearbyNPC.DisplayTip();  // 如果玩家足够接近，显示提示框
            }
            else
            {
                nearbyNPC.HideTip();  // 如果玩家离开，隐藏提示框
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) // 按下 F 键时，Ruby 将与 NPC 对话
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                // Debug.Log("Raycast has hit the object " + hit.collider.gameObject);
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    displayTip();
                    pressF = true;
                }
            }
        }

        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                tipBox.SetActive(false);
            }
        }
    }

        void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void displayTip()
    {
        timerDisplay = displayTime;
        tipBox.SetActive(true);
        if (UIHealthBar.instance.fixedNum >= 12 && pressF_success)
        {
            // 任务完成
            tipText.text = "Press \"E\" To Restart or Quit";
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            animator.SetTrigger("Hit");
            PlaySound(hitClip);
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if(amount > 0)
        {
            PickEffect.Play();
        }
        /*
         钳制功能 (Clamping) 可确保第一个参数（此处为 currentHealth + amount）绝不会小于第二个参数（此处为 0），
         也绝不会大于第三个参数 (maxHealth)。因此，Ruby 的生命值将始终保持在 0 与 maxHealth 之间。
         */
        // Debug.Log(currentHealth + "/" + maxHealth);

        if(currentHealth <= 0)
        {
            ChangeLife(-1);
            if(currentLife > 0)
            {
                Respawn();
            }
        }

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth); // 更新生命值条
    }

    public void ChangeBullet(int amount)
    {
        currentBullet = Mathf.Clamp(currentBullet + amount, 0, maxBullet);
        if(amount > 0)
        {
            PickEffect.Play();
        }
    }

    public void ChangeLife(int amount)
    {
        currentLife = Mathf.Clamp(currentLife + amount, 0, maxLife);
    }

    void Launch()
    {
        if(!UIHealthBar.instance.hasTask || currentBullet == 0)
        {
            return; // 如果没有任务或者子弹数量为 0，Ruby 将无法发射飞弹
        }
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>(); // 获取 Projectile 组件
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        ChangeBullet(-1);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSourceOtherClip.PlayOneShot(clip);
    }

    public void displayDie()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // 显示鼠标
        isDead = true;
        die.SetActive(true);
        Time.timeScale = (0); // 暂停游戏
        BGM.Pause();
    }

    private void Respawn() // 重生
    {
        ChangeHealth(maxHealth);
        transform.position = respawnPosition;
        PlaySound(respawnSound);
    }
}
