using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public AudioSource BGM;
    public Slider volumeSlider; // 音量滑块
    public void StartMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // 加载下一个场景
        if(Time.timeScale == (0))
        {
            Time.timeScale = (1); // 恢复游戏
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // 退出游戏
    }
    // Start is called before the first frame update
    void Start()
    {
        // 加载保存的音量
        if (PlayerPrefs.HasKey("MusicVolume_title"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume_title");
            BGM.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnVolumeChange()
    {
        BGM.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("MusicVolume_title", volumeSlider.value); // 保存音量
        PlayerPrefs.Save();
    }
}
