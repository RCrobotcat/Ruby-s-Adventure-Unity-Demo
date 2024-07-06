using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public static UIHealthBar instance { get; private set; } // 单例模式

    public Image mask;
    float originalSize;

    public bool hasTask; // 是否有任务
    // public bool ifCompleteTask; // 是否完成任务, 默认为0
    public int fixedNum = 0; // 修复的机器人数量

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }
}
