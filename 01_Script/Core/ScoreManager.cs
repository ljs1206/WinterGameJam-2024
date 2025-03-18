using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    public event Action<int> OnScoreChange;
    public int CurrentScore { get; private set; } = 0;
    public float CurrentTime { get; private set; } = 0;

    private void Update()
    {
        CurrentTime += Time.deltaTime;
    }

    public void ResetSocre(){
        CurrentScore = 0;
    }

    public void AddScore(int score)
    {
        CurrentScore += score;
        OnScoreChange?.Invoke(CurrentScore);
    }

}
