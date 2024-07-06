using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 2.0f;

    Rigidbody2D rigidbody2d;
    float timer;
    int direction = 1;

    Animator animator;

    bool broken = true;

    public ParticleSystem smokeEffect;
    public ParticleSystem BurstEffect;

    public AudioClip fixedClip;
    public AudioClip EnemyHitClip_1;
    public AudioClip EnemyHitClip_2;

    // 在第一次帧更新之前调用 Start
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        BurstEffect.Stop();
    }

    void Update()
    {
        if (!broken)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2d.MovePosition(position);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 3);

        if (player != null)
        {
            player.ChangeHealth(-1);
            if(randomNumber == 1)
            {
                player.PlaySound(EnemyHitClip_1);
            }
            else
            {
                player.PlaySound(EnemyHitClip_2);
            }
        }
    }

    //使用 public 的原因是我们希望像飞弹脚本一样在其他地方调用这个函数
    public void Fix()
    {
        broken = false;
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        BurstEffect.Play();
        RubyController controller = FindObjectOfType<RubyController>();
        controller.PlaySound(fixedClip);
        UIHealthBar.instance.fixedNum++; // 修复的机器人数量加一
    }
}