using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Day Cycle Settings")]
    public int urchinsPerDay = 4;          
    [HideInInspector] public int urchinsInteractedToday = 0;
    public int currentDay = 1;
    [HideInInspector] public bool readyForBed = false; 

    public event System.Action<GameObject> OnUrchinSpawned;
    public event System.Action<GameObject> OnUrchinCollected;
    public event System.Action OnReadyForBed;
    public event System.Action OnDayEnded;

    [Header("Sky Sprites (Day Progression)")]
    public SpriteRenderer skyRenderer;
    public Sprite[] skySprites;

    [Header("Trash Spawning")]
    public GameObject trashPrefab;         
    public Sprite[] trashSprites;          
    public BoxCollider2D spawnArea;        
    public int baseTrashPerDay = 5;
    public int trashIncreasePerDay = 2;    
    private List<GameObject> activeTrash = new List<GameObject>();

    [Header("Kelp Objects")]
    public List<GameObject> kelpObjects;   

    [Header("Urchin Spawning")]
    public GameObject urchinPrefab;        
    public BoxCollider2D urchinSpawnArea;  
    public Minigame minigame;              
    public List<GameObject> activeUrchins = new List<GameObject>();

    [Header("Sleep Time")]
    public GameObject BlackScreen;
    public Animator SleepingAnimation;

    private void Start()
    {
        UpdateSkySprite();
        SpawnTrashForDay();
        SpawnUrchinsForDay();
        BlackScreen.SetActive(false);
    }
    public void AdvanceTime(GameObject collectedUrchin)
    {
        urchinsInteractedToday++;
        UpdateSkySprite();
        OnUrchinCollected?.Invoke(collectedUrchin);

        if (urchinsInteractedToday >= urchinsPerDay)
        {
            readyForBed = true;
            OnReadyForBed?.Invoke(); 
        }
    }

    public void GoToSleep()
    {
        if (!readyForBed) return;
        BlackScreen.SetActive(true);
    }

    private void UpdateSkySprite()
    {
        if (skyRenderer == null || skySprites == null || skySprites.Length < 3) return;

        if (urchinsInteractedToday == 1)
        {
            skyRenderer.sprite = skySprites[0];
            SleepingAnimation.SetBool("OnOrOff", true);
        }
        else if (urchinsInteractedToday == 2 || urchinsInteractedToday == 3)
            skyRenderer.sprite = skySprites[1];
        else if (urchinsInteractedToday >= 4)
            skyRenderer.sprite = skySprites[2];
    }

    public void EndDay()
    {
        ClearTrash();
        BlackScreen.SetActive(false);

        currentDay++;
        urchinsInteractedToday = 0;
        readyForBed = false;

        int kelpIndex = currentDay - 2;
        if (kelpIndex >= 0 && kelpIndex < kelpObjects.Count && kelpObjects[kelpIndex] != null)
        {
            kelpObjects[kelpIndex].SetActive(false);
        }

        SpawnUrchinsForDay();
        SpawnTrashForDay();

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
        ClearUrchins(); 

        if (urchinPrefab == null || urchinSpawnArea == null) return;

        Bounds bounds = urchinSpawnArea.bounds;

        for (int i = 0; i < urchinsPerDay; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            GameObject urchin = Instantiate(urchinPrefab, spawnPos, Quaternion.identity);

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