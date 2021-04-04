using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLevel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] 
    private TMP_Text txtLevel;

    // Update is called once per frame
    void Update()
    {
        txtLevel.text = GameState.Instance.Level.ToString();
    }
}
