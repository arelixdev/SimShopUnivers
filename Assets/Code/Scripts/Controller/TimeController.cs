using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    [Header("Réglages de l'heure")]
    [SerializeField] private string startTime = "08:30";
    [SerializeField] private string endTime = "22:00";


    [Header("Vitesse du temps")]
    [SerializeField] private float timeScale = 60f;

    [Header("Etat du temp")]
    public bool isRunning = false;

    [SerializeField] private TextMeshProUGUI timeText;

    private DateTime currentTime;
    private DateTime targetEndTime;

    public event Action<DateTime> OnTimeUpdated;
    public event Action OnTimeFinished;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        currentTime = DateTime.Parse(startTime);
        targetEndTime = DateTime.Parse(endTime);
        
        timeText.text = GetFormattedTime();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime = currentTime.AddSeconds(Time.deltaTime * timeScale);

        if (currentTime >= targetEndTime)
        {
            currentTime = targetEndTime;
            isRunning = false;
            OnTimeFinished?.Invoke();
        }

        timeText.text = GetFormattedTime();

        OnTimeUpdated?.Invoke(currentTime);
    }

    public string GetFormattedTime()
    {
        return currentTime.ToString("HH:mm");
    }

    public void ResetTime()
    {
        currentTime = DateTime.Parse(startTime);
    }

}
