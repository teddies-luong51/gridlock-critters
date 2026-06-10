using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Persists level progression to PlayerPrefs as JSON and computes resume target.</summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveKey = "gridlock_save_v1";

    private SaveData _data;

    /// <summary>Currently loaded save data (loaded on Awake).</summary>
    public SaveData Data => _data;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _data = Load();
    }

    /// <summary>Serializes the current save data to PlayerPrefs as JSON.</summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(_data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    /// <summary>Deserializes save data from PlayerPrefs, returning a default instance if none exists.</summary>
    public SaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return new SaveData();
        }

        string json = PlayerPrefs.GetString(SaveKey);
        SaveData loaded = JsonUtility.FromJson<SaveData>(json);
        return loaded ?? new SaveData();
    }

    /// <summary>Returns true if the given level index is recorded as completed.</summary>
    public bool HasCompleted(int levelIndex)
    {
        return _data.completedLevels != null && _data.completedLevels.Contains(levelIndex);
    }

    /// <summary>Marks a level completed (deduped), updates lastPlayedLevel, and persists.</summary>
    public void MarkCompleted(int levelIndex)
    {
        List<int> levels = _data.completedLevels != null
            ? new List<int>(_data.completedLevels)
            : new List<int>();

        if (!levels.Contains(levelIndex))
        {
            levels.Add(levelIndex);
        }

        _data.completedLevels = levels.ToArray();
        _data.lastPlayedLevel = levelIndex;
        Save();
    }

    /// <summary>Records the level the player just entered and persists.</summary>
    public void SetLastPlayedLevel(int levelIndex)
    {
        _data.lastPlayedLevel = levelIndex;
        Save();
    }

    /// <summary>Adds elapsed gameplay seconds to the cumulative play time and persists.</summary>
    public void AddPlayTime(float seconds)
    {
        _data.totalPlayTime += seconds;
        Save();
    }

    /// <summary>Returns the lowest level index NOT in completedLevels (the next thing to play).</summary>
    public int GetResumeLevel()
    {
        for (int i = 0; i < LevelProgressManager.TotalLevels; i++)
        {
            if (!HasCompleted(i))
            {
                return i;
            }
        }
        // All levels complete — clamp to last level.
        return LevelProgressManager.TotalLevels - 1;
    }
}

/// <summary>Serializable container for all persisted progression state.</summary>
[System.Serializable]
public class SaveData
{
    public int[] completedLevels = new int[0];
    public int lastPlayedLevel = 0;
    public float totalPlayTime = 0f;
}
