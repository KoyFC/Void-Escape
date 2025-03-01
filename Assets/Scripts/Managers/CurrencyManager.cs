using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance = null;

    [SerializeField] int m_StartingCredits = 120;
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

        ResetCredits();
    }

    public void ResetCredits()
    {
        SetCredits(m_StartingCredits);
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