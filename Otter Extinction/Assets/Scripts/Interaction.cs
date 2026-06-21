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
            minigame.activeInteraction = this; // tell the minigame which urchin opened it
            minigame.ResetMinigame();
            minigame.MinigameObject.SetActive(true);
        }
    }

    // Called directly by Minigame.cs when it finishes — only ever fires for the urchin that actually opened it
    public void OnMinigameFinished()
    {
        GM.AdvanceTime(interactedObject);
        Destroy(interactedObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressE.SetActive(true);
            Interacting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PressE.SetActive(false); // fixed: this was setting it to true before, so the prompt never hid
            Interacting = false;
        }
    }
}