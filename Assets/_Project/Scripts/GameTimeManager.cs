using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    [Header("Time")]
    public int day = 1;
    public float timeOfDay = 8f; // 0-24
    public float timeScale = 60f; // 1 real sec = 1 herní minuta

    void Update()
    {
        timeOfDay += (Time.deltaTime * timeScale) / 60f;

        if (timeOfDay >= 24f)
        {
            timeOfDay = 0f;
            day++;
            Debug.Log("Nový den: " + day);
        }
    }

    public bool IsNight()
    {
        return timeOfDay >= 20f || timeOfDay < 6f;
    }
}