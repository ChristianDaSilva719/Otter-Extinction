using UnityEngine;

// Put this on a trigger collider at the player's home/bed location.
public class HomeZone : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && gameManager.readyForBed)
        {
            gameManager.GoToSleep();
        }
    }
}