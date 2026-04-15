using UnityEngine;
using TMPro;

public class MissUI : MonoBehaviour
{
    [SerializeField] private TMP_Text missText;

    public void SetMiss(uint value)
    {
        missText.text = $"Miss: {value}";
    }
}
