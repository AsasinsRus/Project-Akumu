using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UIStatisticManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI enemiesLeft;
    [SerializeField] private TextMeshProUGUI wave;

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
    }

    public void AddScore(int score)
    {
        this.score.text = (Convert.ToUInt32(this.score.text) + score).ToString();
    }

    public void SetEnemiesLeft(int enemiesLeft)
    {
        this.enemiesLeft.text = enemiesLeft.ToString();
    }

    public void AddEnemiesLeft(int enemiesLeft)
    {
        this.enemiesLeft.text = (Convert.ToUInt32(this.enemiesLeft.text) + enemiesLeft).ToString();
    }

    public void SetWave(int wave)
    {
        this.wave.text = wave.ToString();
    }

    public void AddWave(int wave)
    {
        this.wave.text = (Convert.ToUInt32(this.wave.text) + wave).ToString();
    }
}
