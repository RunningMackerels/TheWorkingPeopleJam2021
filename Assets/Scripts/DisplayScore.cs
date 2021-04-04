using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;

    private void Update()
    {
        text.SetText(GameState.Instance.Score.ToString());
    }
}
