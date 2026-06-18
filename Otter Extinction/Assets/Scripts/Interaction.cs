using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameManager GM;
    private bool Interacting;
    public GameObject PressE;
    public Minigame minigame;
    public GameObject interactedObject;

    private void Start()
    {
        PressE.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Interacting && !minigame.MinigameObject.activeSelf)
        {
            interactedObject = gameObject;
            minigame.ResetMinigame();
            minigame.MinigameObject.SetActive(true);
        }
        if (minigame.finished == true)
        {
            GM.AdvanceTime();
            minigame.finished = false;
            Destroy(interactedObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressE.SetActive(true);
            Interacting = true;
            Debug.Log("Interacting");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressE.SetActive(true);
            Interacting = false;
            Debug.Log("Not Interacting");
        }
    }
}