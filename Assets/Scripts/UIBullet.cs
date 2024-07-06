using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class UIBullet : MonoBehaviour
{
    public static UIBullet instance { get; private set; } // 单例模式

    public TextMeshProUGUI bulletText;
    public TextMeshProUGUI robotLeft;
    public TextMeshProUGUI lifeLeft;
    public Image Bullet;
    RubyController rubyController;
    UIHealthBar healthBarUI;
    // Start is called before the first frame update
    void Start()
    {
        rubyController = FindObjectOfType<RubyController>();
        healthBarUI = FindObjectOfType<UIHealthBar>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text = rubyController.bullet.ToString() + "/20"; // 显示子弹数量
        if ((11 - healthBarUI.fixedNum) >= 0)
        {
            robotLeft.text = (12 - healthBarUI.fixedNum).ToString() + "/12"; // 显示修复的机器人数量
        }
        else
        {
            robotLeft.text = "0/12";
        }

        if(rubyController.life >= 0 && !rubyController.isDead)
        {
            lifeLeft.text = rubyController.life.ToString() + "/3"; // 显示剩余生命
        }
        else
        {
            lifeLeft.text = "0/3";
        }
    }

    public void bulletUIvisibility(bool visible)
    {
        if (Bullet != null)
        {
            Bullet.gameObject.SetActive(visible);
        }
        else
        {
            Debug.LogError("Bullet Image is not assigned in the inspector.");
        }
    }
}
