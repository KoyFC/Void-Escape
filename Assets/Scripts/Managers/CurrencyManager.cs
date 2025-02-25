using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance = null;

    public int Credits { get; private set; } = 0;

    public event Action<int> OnCreditsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCredits(int amount)
    {
        Credits = amount;
        OnCreditsChanged?.Invoke(Credits);
    }

    public void AddCredits(int amount)
    {
        Credits += amount;
        OnCreditsChanged?.Invoke(Credits);
    }

    public void RemoveCredits(int amount)
    {
        Credits -= amount;
        OnCreditsChanged?.Invoke(Credits);
    }
}