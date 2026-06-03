using UnityEngine;
using UnityEngine.Rendering;

public class Interaction : MonoBehaviour
{
    public GameManager GM;


    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if (Input.GetKeyDown("e"))
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
}
