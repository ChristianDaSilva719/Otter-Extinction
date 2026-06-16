using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameManager GM;
    private bool Interacting;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Interacting)
        {
            GM.AdvanceTime();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Interacting = true;
            Debug.Log("Interacting");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Interacting = false;
            Debug.Log("Not Interacting");
        }
    }
}