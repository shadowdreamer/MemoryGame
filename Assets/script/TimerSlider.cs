using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance { get; private set; } // ����

    [Header("UI Components")]
    public Slider slider; // ���õ� UI Slider
    public TextMeshProUGUI timeText; // ��ʾ����ʱ�ķ����ʽ

    [Header("Audio Settings")]
    public AudioSource audioSource; // ������Ч
    public AudioClip tickSound; // ÿ�벥�ŵ���ʾ��
    public AudioClip finishSound; // ����ʱ��������ʾ��

    [Header("Animation Settings")]
    public Animator sliderAnimator; // Slider ����
    public string animationTrigger = "Pulse"; // ��������������

    [Header("Timer Settings")]
    [Tooltip("����ʱ��ʱ�����룩")]
    private float countdownTime = 60f; // �ܵ���ʱ���룩
    private float currentTime; // ��ǰʣ��ʱ��
    private bool isCountingDown = false; // �Ƿ����ڵ���ʱ

    public Action OnCountdownEnd; // ����ʱ������ί��

    private void Awake()
    {
        // ����ģʽ��ȷ��ֻ��һ��ʵ��
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
        InitializeTimer(countdownTime); // ��ʼ����ʱ��
    }

    private void Update()
    {
        if (isCountingDown && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            // ����ÿ����ʾ��
            if (tickSound != null && Mathf.FloorToInt(currentTime) != Mathf.FloorToInt(currentTime + Time.deltaTime))
            {
                audioSource?.PlayOneShot(tickSound);
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
                isCountingDown = false;

                // ���Ž�����Ч
                audioSource?.PlayOneShot(finishSound);

                // ��������ʱ�����¼�
                OnCountdownEnd?.Invoke();
            }

            UpdateUI();
        }
    }

    /// <summary>
    /// ��ʼ����ʱ��
    /// </summary>
    /// <param name="totalTime">��ʱ�䣨�룩</param>
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
    /// ��ʼ����ʱ
    /// </summary>
    public void StartCountdown()
    {
        isCountingDown = true;
    }

    /// <summary>
    /// ��ͣ����ʱ
    /// </summary>
    public void PauseCountdown()
    {
        isCountingDown = false;
    }

    /// <summary>
    /// ���õ���ʱ
    /// </summary>
    public void ResetCountdown()
    {
        isCountingDown = false;
        currentTime = countdownTime;
        UpdateUI();
    }
    /// <summary>
    /// ����ʣ��ʱ��
    /// </summary>
    public void IncreaceCountdown(float sec)
    {
        currentTime += sec;
        if (currentTime + sec > countdownTime)
            currentTime = countdownTime;
        UpdateUI();
    }
    /// <summary>
    /// ����ʣ��ʱ������ֵ
    /// </summary>
    public void IncreaceAllTime(float sec)
    {
        currentTime += sec;
        countdownTime += sec;
        slider.maxValue = countdownTime;
        UpdateUI();
    }
    /// <summary>
    /// ���µ���ʱ�� UI
    /// </summary>
    private void UpdateUI()
    {
        if (slider != null)
        {
            slider.value = currentTime;

            // ��������Ч������������
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
