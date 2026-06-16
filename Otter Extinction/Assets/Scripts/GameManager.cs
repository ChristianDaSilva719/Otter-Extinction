using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    // Day Cycle Variables
    public int intPerDay = 0;
    public float amountOfInterations;
    public Volume volume;

    [Header("Day/Night Settings")]
    public float lerpSpeed = 2f;

    [HideInInspector]
    public float additionPerDay;

    private float targetWeight;

    private void Start()
    {
        targetWeight = volume.weight;
    }

    private void Update()
    {
        additionPerDay = 1f / amountOfInterations;

        // Smoothly move towards the target weight
        volume.weight = Mathf.Lerp(
            volume.weight,
            targetWeight,
            lerpSpeed * Time.deltaTime
        );

        // Snap when very close
        if (Mathf.Abs(volume.weight - targetWeight) < 0.001f)
        {
            volume.weight = targetWeight;
        }
    }

    public void AdvanceTime()
    {
        intPerDay++;

        if (intPerDay <= amountOfInterations)
        {
            targetWeight += additionPerDay;
            targetWeight = Mathf.Clamp01(targetWeight);
        }
    }
}