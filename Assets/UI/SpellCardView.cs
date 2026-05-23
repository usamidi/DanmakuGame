using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text bonusText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject bossSprite;
    [SerializeField] private Animator spellAnimator;

    public SpellCardView SetBonus(ulong bonus)
    {
        bonusText.text = $"Bonus: {bonus}";
        return this;
    }

    public SpellCardView SetName(string name)
    {
        nameText.text = name;
        return this;
    }

    public void OnEnable()
    {
        spellAnimator.SetTrigger("PlayIn");
    }



    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
