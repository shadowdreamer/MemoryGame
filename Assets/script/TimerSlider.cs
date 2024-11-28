using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance { get; private set; } // 单例

    [Header("UI Components")]
    public Slider slider; // 引用到 UI Slider
    public TextMeshProUGUI timeText; // 显示倒计时的分秒格式

    [Header("Audio Settings")]
    public AudioSource audioSource; // 播放音效
    public AudioClip tickSound; // 每秒播放的提示音
    public AudioClip finishSound; // 倒计时结束的提示音

    [Header("Animation Settings")]
    public Animator sliderAnimator; // Slider 动画
    public string animationTrigger = "Pulse"; // 动画触发器名称

    [Header("Timer Settings")]
    [Tooltip("倒计时总时长（秒）")]
    private float countdownTime = 60f; // 总倒计时（秒）
    private float currentTime; // 当前剩余时间
    private bool isCountingDown = false; // 是否正在倒计时

    public Action OnCountdownEnd; // 倒计时结束的委托

    private void Awake()
    {
        // 单例模式，确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeTimer(countdownTime); // 初始化计时器
    }

    private void Update()
    {
        if (isCountingDown && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // 播放每秒提示音
            if (tickSound != null && Mathf.FloorToInt(currentTime) != Mathf.FloorToInt(currentTime + Time.deltaTime))
            {
                audioSource?.PlayOneShot(tickSound);
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
                isCountingDown = false;

                // 播放结束音效
                audioSource?.PlayOneShot(finishSound);

                // 触发倒计时结束事件
                OnCountdownEnd?.Invoke();
            }

            UpdateUI();
        }
    }

    /// <summary>
    /// 初始化计时器
    /// </summary>
    /// <param name="totalTime">总时间（秒）</param>
    public void InitializeTimer(float totalTime)
    {
        countdownTime = totalTime;
        currentTime = totalTime;

        if (slider != null)
        {
            slider.maxValue = countdownTime;
            slider.value = countdownTime;
        }

        UpdateUI();
    }

    /// <summary>
    /// 开始倒计时
    /// </summary>
    public void StartCountdown()
    {
        isCountingDown = true;
    }

    /// <summary>
    /// 暂停倒计时
    /// </summary>
    public void PauseCountdown()
    {
        isCountingDown = false;
    }

    /// <summary>
    /// 重置倒计时
    /// </summary>
    public void ResetCountdown()
    {
        isCountingDown = false;
        currentTime = countdownTime;
        UpdateUI();
    }
    /// <summary>
    /// 增加剩余时间
    /// </summary>
    public void IncreaceCountdown(float sec)
    {
        currentTime += sec;
        if (currentTime + sec > countdownTime)
            currentTime = countdownTime;
        UpdateUI();
    }
    /// <summary>
    /// 增加剩余时间和最大值
    /// </summary>
    public void IncreaceAllTime(float sec)
    {
        currentTime += sec;
        countdownTime += sec;
        slider.maxValue = countdownTime;
        UpdateUI();
    }
    /// <summary>
    /// 更新倒计时的 UI
    /// </summary>
    private void UpdateUI()
    {
        if (slider != null)
        {
            slider.value = currentTime;

            // 触发动画效果（如脉动）
            if (sliderAnimator != null && currentTime <= countdownTime * 0.1f)
            {
                sliderAnimator.SetTrigger(animationTrigger);
            }
        }

        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            int C_minutes = Mathf.FloorToInt(countdownTime / 60);
            int c_seconds = Mathf.FloorToInt(countdownTime % 60);
            timeText.text = string.Format("{0:00}:{1:00} / {2:00}:{3:00} ", minutes, seconds, C_minutes, c_seconds);
        }
    }
}
