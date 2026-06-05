using UnityEngine;
using UnityEngine.Rendering;

public class Interaction : MonoBehaviour
{
    public GameManager GM;
    private bool Interacting;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Interacting == true)
            {
                GM.intPerDay += 1;
                if (GM.intPerDay != GM.amountOfInterations + 1)
                {
                    GM.volume.weight += GM.additionPerDay;
                }
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Interacting = true;
            Debug.Log("Interacting");
        }
        else
        {
            Interacting = false;
            Debug.Log("Not Interacting");
        }
    }
}
