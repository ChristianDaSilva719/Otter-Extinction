using System.Collections.Generic;
using UnityEngine;


public class ArrowManager : MonoBehaviour
{
    public GameManager gameManager;
    public Transform player;
    public GameObject arrowPrefab; 
    public Transform home;         
    public float radius = 2f;

    private Dictionary<GameObject, GameObject> urchinArrows = new Dictionary<GameObject, GameObject>();
    private GameObject homeArrow;

    private void OnEnable()
    {
        gameManager.OnUrchinSpawned += HandleUrchinSpawned;
        gameManager.OnUrchinCollected += HandleUrchinCollected;
        gameManager.OnReadyForBed += HandleReadyForBed;
        gameManager.OnDayEnded += HandleDayEnded;
    }

    private void OnDisable()
    {
        gameManager.OnUrchinSpawned -= HandleUrchinSpawned;
        gameManager.OnUrchinCollected -= HandleUrchinCollected;
        gameManager.OnReadyForBed -= HandleReadyForBed;
        gameManager.OnDayEnded -= HandleDayEnded;
    }

    private void Start()
    {
        homeArrow = Instantiate(arrowPrefab);
        ArrowIndicator indicator = homeArrow.GetComponent<ArrowIndicator>();
        if (indicator != null)
        {
            indicator.player = player;
            indicator.target = home;
            indicator.radius = radius;
        }
        homeArrow.SetActive(false);
    }

    private void HandleUrchinSpawned(GameObject urchin)
    {
        GameObject arrow = Instantiate(arrowPrefab);
        ArrowIndicator indicator = arrow.GetComponent<ArrowIndicator>();
        if (indicator != null)
        {
            indicator.player = player;
            indicator.target = urchin.transform;
            indicator.radius = radius;
        }
        urchinArrows[urchin] = arrow;
    }

    private void HandleUrchinCollected(GameObject urchin)
    {
        if (urchinArrows.TryGetValue(urchin, out GameObject arrow))
        {
            if (arrow != null) Destroy(arrow);
            urchinArrows.Remove(urchin);
        }
    }

    private void HandleReadyForBed()
    {
        if (homeArrow != null)
            homeArrow.SetActive(true);
    }

    private void HandleDayEnded()
    {
        if (homeArrow != null)
            homeArrow.SetActive(false);
    }
}