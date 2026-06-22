using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Day Cycle Settings")]
    public int urchinsPerDay = 4;          // 4 urchins = full day = sleep
    [HideInInspector] public int urchinsInteractedToday = 0;
    public int currentDay = 1;
    [HideInInspector] public bool readyForBed = false; // true once all of today's urchins are collected

    [Header("Game Length")]
    public int totalDays = 5;
    public GameObject endingObject;        // SetActive(true) once totalDays is reached — hook your credits sequence to this
    [HideInInspector] public bool gameEnded = false;

    // ArrowManager (and anything else) can subscribe to these instead of polling every frame
    public event System.Action<GameObject> OnUrchinSpawned;
    public event System.Action<GameObject> OnUrchinCollected;
    public event System.Action OnReadyForBed;
    public event System.Action OnDayEnded;

    [Header("Sky Sprites (Day Progression)")]
    public SpriteRenderer skyRenderer;
    // skySprites[0] = morning (0 or 1 urchins)
    // skySprites[1] = after 2nd AND 3rd urchin
    // skySprites[2] = after 4th urchin (end of day)
    public Sprite[] skySprites;

    [Header("Trash Spawning")]
    public GameObject trashPrefab;         // prefab with a SpriteRenderer on it
    public Sprite[] trashSprites;          // your 9 trash sprites
    public BoxCollider2D spawnArea;        // drag a BoxCollider2D over your map bounds (set isTrigger = true)
    public int baseTrashPerDay = 5;
    public int trashIncreasePerDay = 2;    // how many more pieces of trash spawn each new day
    private List<GameObject> activeTrash = new List<GameObject>();

    [Header("Kelp Objects (one disabled per day after Day 1)")]
    public List<GameObject> kelpObjects;   // size 4, in the order you want them to disappear

    [Header("Objects To Reveal (one enabled per day after Day 1)")]
    public List<GameObject> objectsToEnablePerDay; // size 4, same day-2/3/4/5 ordering as kelpObjects above

    [Header("Otters Sleeping (one disabled per day)")]
    public List<GameObject> otterSleepingObjects;  // 4 otters in order — index 0 disabled on day 2, etc.
    [Tooltip("The last otter that stays active on the final day — gets a new sprite and animation.")]
    public UnityEngine.UI.Image lastOtterImage;
    public Sprite lastOtterSprite;                 // the sprite to swap to on day 4

    [Header("Images (one disabled per day)")]
    public List<GameObject> imageObjects;          // 4 Image objects in order — index 0 disabled on day 2, etc.

    [Header("Water - Day 3 Sprite Change")]
    public SpriteRenderer waterSpriteRenderer;
    public Sprite waterNewSprite;
    public List<UnityEngine.UI.Image> waterUIImages;
    public Sprite waterUINewSprite;                // all waterUIImages swap to this single sprite on day 3

    [Header("Urchin Spawning")]
    public GameObject urchinPrefab;        // prefab needs the Interaction script + its own PressE child
    public BoxCollider2D urchinSpawnArea;  // drag a BoxCollider2D over where urchins can spawn (isTrigger = true)
    public Minigame minigame;              // the single Minigame object in your scene, assigned to each spawned urchin
    private List<GameObject> activeUrchins = new List<GameObject>();

    [Header("SleepingTime")]
    public GameObject BlackScreen;

    private void Start()
    {
        UpdateSkySprite();
        SpawnTrashForDay();
        SpawnUrchinsForDay();
    }

    // Called by Interaction.cs when a minigame finishes — pass in the urchin that was just collected
    public void AdvanceTime(GameObject collectedUrchin)
    {
        if (gameEnded) return;

        urchinsInteractedToday++;
        UpdateSkySprite();
        OnUrchinCollected?.Invoke(collectedUrchin);

        if (urchinsInteractedToday >= urchinsPerDay)
        {
            readyForBed = true;
            OnReadyForBed?.Invoke(); // ArrowManager shows the "go home" arrow on this
        }
    }

    // Call this from a Home trigger zone once the player walks home with readyForBed == true
    public void GoToSleep()
    {
        if (!readyForBed) return;
        BlackScreen.SetActive(true);
    }

    private void UpdateSkySprite()
    {
        if (skyRenderer == null || skySprites == null || skySprites.Length < 3) return;

        if (urchinsInteractedToday == 0)
            skyRenderer.sprite = skySprites[0];
        else if (urchinsInteractedToday == 1)
            skyRenderer.sprite = skySprites[0];
        else if (urchinsInteractedToday == 2 || urchinsInteractedToday == 3)
            skyRenderer.sprite = skySprites[1];
        else if (urchinsInteractedToday >= 4)
            skyRenderer.sprite = skySprites[2];
    }

    public void EndDay()
    {
        if (gameEnded) return; // ignore any extra calls once the game has already ended

        ClearTrash();

        currentDay++;
        urchinsInteractedToday = 0;
        readyForBed = false;
        UpdateSkySprite(); // reset sky to morning sprite immediately

        if (currentDay > totalDays)
        {
            TriggerEnding();
            return;
        }

        // Day 2 affects index 0, Day 3 affects index 1, etc. — same ordering for all lists below.
        int dayIndex = currentDay - 2;

        if (dayIndex >= 0 && dayIndex < kelpObjects.Count && kelpObjects[dayIndex] != null)
            kelpObjects[dayIndex].SetActive(false);

        if (dayIndex >= 0 && dayIndex < objectsToEnablePerDay.Count && objectsToEnablePerDay[dayIndex] != null)
            objectsToEnablePerDay[dayIndex].SetActive(true);

        if (dayIndex >= 0 && dayIndex < otterSleepingObjects.Count && otterSleepingObjects[dayIndex] != null)
            otterSleepingObjects[dayIndex].SetActive(false);

        if (dayIndex >= 0 && dayIndex < imageObjects.Count && imageObjects[dayIndex] != null)
            imageObjects[dayIndex].SetActive(false);

        // On day 4, swap the surviving otter's sprite — animation is handled in AnimationFunctions
        if (currentDay == 4)
        {
            if (lastOtterImage != null && lastOtterSprite != null)
                lastOtterImage.sprite = lastOtterSprite;
        }

        // On day 3, change the water sprites
        if (currentDay == 3)
        {
            if (waterSpriteRenderer != null && waterNewSprite != null)
                waterSpriteRenderer.sprite = waterNewSprite;

            foreach (var img in waterUIImages)
            {
                if (img != null && waterUINewSprite != null)
                    img.sprite = waterUINewSprite;
            }
        }

        SpawnUrchinsForDay();
        SpawnTrashForDay();

        OnDayEnded?.Invoke(); // ArrowManager hides the "go home" arrow on this
    }

    private void TriggerEnding()
    {
        gameEnded = true;
        ClearUrchins(); // nothing left wandering around once the credits start

        if (endingObject != null)
            endingObject.SetActive(true);

        OnDayEnded?.Invoke();
    }

    private void SpawnTrashForDay()
    {
        if (trashPrefab == null || spawnArea == null || trashSprites.Length == 0) return;

        int trashCount = baseTrashPerDay + (currentDay - 1) * trashIncreasePerDay;
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < trashCount; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            GameObject trash = Instantiate(trashPrefab, spawnPos, Quaternion.identity);
            SpriteRenderer sr = trash.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sprite = trashSprites[Random.Range(0, trashSprites.Length)];

            activeTrash.Add(trash);
        }
    }

    private void ClearTrash()
    {
        foreach (var trash in activeTrash)
        {
            if (trash != null)
                Destroy(trash);
        }
        activeTrash.Clear();
    }

    private void SpawnUrchinsForDay()
    {
        ClearUrchins(); // safety net in case any from the previous day were left un-interacted

        if (urchinPrefab == null || urchinSpawnArea == null) return;

        Bounds bounds = urchinSpawnArea.bounds;

        for (int i = 0; i < urchinsPerDay; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            GameObject urchin = Instantiate(urchinPrefab, spawnPos, Quaternion.identity);

            // Prefabs can't store references to scene objects, so wire them up here
            Interaction interaction = urchin.GetComponent<Interaction>();
            if (interaction != null)
            {
                interaction.GM = this;
                interaction.minigame = minigame;
            }

            activeUrchins.Add(urchin);
            OnUrchinSpawned?.Invoke(urchin);
        }
    }

    private void ClearUrchins()
    {
        foreach (var urchin in activeUrchins)
        {
            if (urchin != null)
                Destroy(urchin);
        }
        activeUrchins.Clear();
    }
}