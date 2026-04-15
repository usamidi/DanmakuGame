using UnityEngine;
using TMPro;

public class GrazeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text grazeText;

    public void SetGraze(uint value)
    {
        grazeText.text = $"Graze: {value}";
    }
}
