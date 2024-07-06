using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuList : MonoBehaviour
{
    public GameObject menuList; // 菜单列表

    [SerializeField] private bool menuKey = true;
    [SerializeField] private AudioSource BGM;
    [SerializeField] private Slider volumeSlider; // 音量滑块

    RubyController rubyController;

    // Start is called before the first frame update
    void Start()
    {
        rubyController = FindObjectOfType<RubyController>();
        menuKey = !menuList.activeSelf; // 初始状态下菜单列表是关闭的

        // 加载保存的音量
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
            BGM.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuKey)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None; // 显示鼠标
                menuList.SetActive(true);
                menuKey = false;
                Time.timeScale = (0); // 暂停游戏
                BGM.Pause();
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked; // 隐藏鼠标
                menuList.SetActive(false);
                menuKey = true;
                Time.timeScale = (1); // 恢复游戏
                BGM.Play();
            }
        }
    }

    public void OnVolumeChange()
    {
        BGM.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value); // MusicVolume 是键，volumeSlider.value 是值
        PlayerPrefs.Save(); // 确保更改被保存
    }

    public void Return()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // 隐藏鼠标
        menuList.SetActive(false);
        menuKey = true;
        Time.timeScale = (1); // 恢复游戏
        BGM.Play();
    }

    public void Restart() // 重新开始
    {
        SceneManager.LoadScene(1);
        if(rubyController.pressF_success == true)
        {
            rubyController.pressF_success = false;
        }
        if(rubyController.isDead == true)
        {
            rubyController.isDead = false;
        }
        Time.timeScale = (1); // 恢复游戏
    }

    public void MainMenu() // 返回主菜单
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // 加载上一个场景
    }

    public void Quit()
    {
        Application.Quit();
    }
}
