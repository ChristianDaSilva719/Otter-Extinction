using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [Header("Arrow Settings")]
    [Tooltip("RectTransform of the arrow indicator (the small triangle).")]
    public RectTransform arrow;
    [Tooltip("Lowest Y position the arrow can reach.")]
    public float minY = -75f;
    [Tooltip("Highest Y position the arrow can reach.")]
    public float maxY = 80f;
    [Tooltip("How fast the arrow moves between min and max (units/sec).")]
    public float arrowSpeed = 60f;
    [Tooltip("How much arrowSpeed increases after each successful hit.")]
    public float speedIncreasePerHit = 10f;

    [Header("Green Zone Settings")]
    [Tooltip("Prefab for a single green square target.")]
    public GameObject greenZonePrefab;
    [Tooltip("Parent the zones spawn under - usually the teal/blue track.")]
    public RectTransform trackContainer;
    [Tooltip("Lowest Y a green zone can spawn at.")]
    public float zoneMinY = -65f;
    [Tooltip("Highest Y a green zone can spawn at.")]
    public float zoneMaxY = 70f;
    [Tooltip("How many green zones are active in the track at once.")]
    public int zoneCount = 3;
    [Tooltip("Vertical size of each zone, used for overlap checks.")]
    public float zoneHeight = 20f;
    [Tooltip("Extra leeway in pixels when checking if the arrow is 'on' a zone.")]
    public float hitTolerance = 5f;

    [Header("Urchin Settings")]
    [Tooltip("Image component showing the urchin sprite.")]
    public Image urchinImage;
    [Tooltip("Sprites in order, from undamaged to fully cracked open.")]
    public Sprite[] urchinStages;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Activate")]
    public GameObject MinigameObject;

    // Which Interaction (urchin) currently has this minigame open. Set by Interaction.cs right
    // before it activates the minigame, so completion only ever notifies the correct urchin.
    [HideInInspector] public Interaction activeInteraction;

    private int direction = 1;
    private readonly List<RectTransform> activeZones = new List<RectTransform>();
    private int urchinStageIndex = 0;
    private float baseArrowSpeed;
    public bool finished = false;

    void Start()
    {
        finished = false;
        baseArrowSpeed = arrowSpeed;
        SpawnZones();
        MinigameObject.SetActive(false);
        if (urchinImage != null && urchinStages.Length > 0)
        {
            urchinImage.sprite = urchinStages[0];
        }
    }

    void Update()
    {
        MoveArrow();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMiniGame();
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryHit();
        }
    }

    void MoveArrow()
    {
        if (arrow == null) return;

        Vector2 pos = arrow.anchoredPosition;
        pos.y += direction * arrowSpeed * Time.deltaTime;

        if (pos.y >= maxY)
        {
            pos.y = maxY;
            direction = -1;
        }
        else if (pos.y <= minY)
        {
            pos.y = minY;
            direction = 1;
        }

        arrow.anchoredPosition = pos;
    }

    void SpawnZones()
    {
        for (int i = 0; i < zoneCount; i++)
        {
            SpawnSingleZone();
        }
    }

    void SpawnSingleZone()
    {
        if (greenZonePrefab == null || trackContainer == null) return;

        GameObject zoneObj = Instantiate(greenZonePrefab, trackContainer);
        RectTransform zoneRect = zoneObj.GetComponent<RectTransform>();

        float randomY = Random.Range(zoneMinY, zoneMaxY);
        zoneRect.anchoredPosition = new Vector2(zoneRect.anchoredPosition.x, randomY);

        activeZones.Add(zoneRect);
    }

    void TryHit()
    {
        if (arrow == null || trackContainer == null) return;

        float arrowY = trackContainer.InverseTransformPoint(arrow.position).y;

        for (int i = activeZones.Count - 1; i >= 0; i--)
        {
            RectTransform zone = activeZones[i];
            if (zone == null)
            {
                activeZones.RemoveAt(i);
                continue;
            }

            float zoneY = zone.anchoredPosition.y;
            float halfHeight = (zoneHeight * 0.5f) + hitTolerance;

            if (Mathf.Abs(arrowY - zoneY) <= halfHeight)
            {
                OnSuccessfulHit(zone, i);
                return; // only register one hit per key press
            }
        }

        // Missed - shuffle the zones and reset the arrow so spamming E doesn't help
        OnMissedHit();
    }

    void OnMissedHit()
    {
        foreach (RectTransform zone in activeZones)
        {
            if (zone == null) continue;

            Vector2 zonePos = zone.anchoredPosition;
            zonePos.y = Random.Range(zoneMinY, zoneMaxY);
            zone.anchoredPosition = zonePos;
        }

        if (arrow != null)
        {
            Vector2 pos = arrow.anchoredPosition;
            pos.y = minY;
            arrow.anchoredPosition = pos;
        }
        direction = 1;
    }

    void OnSuccessfulHit(RectTransform zone, int index)
    {
        Destroy(zone.gameObject);
        activeZones.RemoveAt(index);

        AdvanceUrchinSprite();

        // Arrow gets faster with each successful hit
        arrowSpeed += speedIncreasePerHit;

        // No respawn - the pool of zones shrinks until empty
        if (activeZones.Count == 0)
        {
            OnMinigameComplete();
        }
    }

    void OnMinigameComplete()
    {
        finished = true;
        MinigameObject.SetActive(false);

        // Notify only the urchin that actually opened this minigame
        if (activeInteraction != null)
        {
            activeInteraction.OnMinigameFinished();
            activeInteraction = null;
        }
    }

    public void ResetMinigame()
    {
        finished = false;
        foreach (RectTransform zone in activeZones)
        {
            if (zone != null)
            {
                Destroy(zone.gameObject);
            }
        }
        activeZones.Clear();

        direction = 1;
        arrowSpeed = baseArrowSpeed;

        if (arrow != null)
        {
            Vector2 pos = arrow.anchoredPosition;
            pos.y = minY;
            arrow.anchoredPosition = pos;
        }

        urchinStageIndex = 0;
        if (urchinImage != null && urchinStages.Length > 0)
        {
            urchinImage.sprite = urchinStages[0];
        }

        SpawnZones();
    }

    void AdvanceUrchinSprite()
    {
        if (urchinImage == null || urchinStages.Length == 0) return;

        urchinStageIndex = Mathf.Min(urchinStageIndex + 1, urchinStages.Length - 1);
        urchinImage.sprite = urchinStages[urchinStageIndex];
    }

    public void CloseMiniGame()
    {
        MinigameObject.SetActive(false);
        ResetMinigame();
    }
}