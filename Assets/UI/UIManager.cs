using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [SerializeField] private RectTransform canvas;
    [SerializeField] private Camera UICamera;


    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text grazeText;
    [SerializeField] private TMP_Text missText;
    [SerializeField] private TMP_Text itemScoreText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Image healthBar;

    [SerializeField] private GetScoreText ScoreText;

    [SerializeField] private SpellCardView spellCardView;

    private ulong score = 0;
    private ulong itemScore = 10000;
    private uint graze;

    public void ShowScoreText(ulong score, Color color, Vector3 position)
    {
        var text = Instantiate(ScoreText, position, Quaternion.identity, canvas);
        text.SetText($"{score}", color);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0)
        {
            healthBar.fillAmount = 0;
            return;
        }

        float fillPercent = currentHealth / maxHealth;
        fillPercent = Mathf.Clamp01(fillPercent);
        healthBar.fillAmount = fillPercent;
    }

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
        Score += 500;
        if (Graze % 10 == 0)
        {
            ItemScore += 10;
        }
    }

    public void AddScore(ulong value)
    {
        Score += value;
    }

    public void AddItemScore(ulong value)
    {
        ItemScore += value;
    }

    public void SetCountdown(float time)
    {
        int showTime = Mathf.FloorToInt(time);
        Color color = Color.white;
        if (showTime >= 0)
        {
            if (showTime <= 10)
            {
                color = Color.red;
            }
            countdownText.text = $"{showTime}";
            countdownText.color = color;
        }
        else
        {
            countdownText.text = "";
        }
    }


    public void EnterSpellCard(string name, ulong bonus)
    {

        spellCardView.gameObject.SetActive(true);
        spellCardView.SetName(name);
        spellCardView.SetBonus(bonus);

    }

    public void ExitSpellCard()
    {
        spellCardView.gameObject.SetActive(false);
    }

    public void SetBonus(ulong bonus)
    {
        spellCardView.SetBonus(bonus);
    }

    void Awake()
    {
        Instance = this;
        Graze = 0;
        Score = 0;
        ItemScore = 10000;
        SetCountdown(-1f);
        UpdateHealthBar(0f, -1f);
        spellCardView.gameObject.SetActive(false);
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
