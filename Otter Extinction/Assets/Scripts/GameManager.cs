using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    // Day Cycle Variables
    public int intPerDay = 0;
    public float amountOfInterations;
    public Volume volume;
    public float additionPerDay;

    public void Update()
    {
        additionPerDay = 1f / amountOfInterations;
    }
}
