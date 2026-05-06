using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text grazeText;
    [SerializeField] private TMP_Text missText;
    [SerializeField] private TMP_Text itemScoreText;

    [SerializeField] private GameObject GetScoreText;

    private ulong score = 0;
    private ulong itemScore = 10000;
    private uint graze;

    public uint Graze
    {
        get => graze;
        private set
        {
            grazeText.text = $"Graze: {value}";
            graze = value;
        }
    }

    public ulong Score
    {
        get => score;
        private set
        {
            scoreText.text = $"Score: {value}";
            score = value;
        }

    }

    public ulong ItemScore
    {
        get => itemScore;
        private set
        {
            itemScoreText.text = $"Max Score: {value}";
            itemScore = value;
        }
    }


    public uint life;
    public uint bomb;

    public void SetMiss(uint value)
    {
        missText.text = $"Miss: {value}";
    }

    public void OnGraze()
    {
        Graze++;
        if (Graze % 10 == 0)
        {
            ItemScore += 10;
        }
    }

    public void AddScore(ulong value)
    {
        Score += value;
    }

    void Awake()
    {
        Instance = this;
        Graze = 0;
        Score = 0;
        ItemScore = 10000;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
