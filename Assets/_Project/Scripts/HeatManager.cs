using UnityEngine;

public class HeatManager : MonoBehaviour
{
    [Header("Heat Settings")]
    public float currentHeat = 0f;
    public float maxHeat = 100f;

    [Tooltip("Kolik heatu ubere 1 herní den (pasivně)")]
    public float passiveDailyHeatReduction = 2f;

    [Header("Reference")]
    public GameTimeManager gameTime;

    private int lastProcessedDay = 1;

    void Start()
    {
        if (gameTime == null)
            gameTime = FindObjectOfType<GameTimeManager>();

        if (gameTime != null)
            lastProcessedDay = gameTime.day;
    }

    void Update()
    {
        if (gameTime == null) return;

        if (gameTime.day > lastProcessedDay)
        {
            int days = gameTime.day - lastProcessedDay;
            ReduceHeat(passiveDailyHeatReduction * days);
            lastProcessedDay = gameTime.day;
        }
    }

    public void AddHeat(float amount)
    {
        currentHeat = Mathf.Clamp(currentHeat + amount, 0f, maxHeat);
        Debug.Log($"[HEAT] +{amount} => {currentHeat:0.0}/{maxHeat}");
    }

    public void ReduceHeat(float amount)
    {
        currentHeat = Mathf.Clamp(currentHeat - amount, 0f, maxHeat);
        Debug.Log($"[HEAT] -{amount} => {currentHeat:0.0}/{maxHeat}");
    }

    public float GetHeatNormalized()
    {
        return maxHeat <= 0f ? 0f : currentHeat / maxHeat;
    }
}