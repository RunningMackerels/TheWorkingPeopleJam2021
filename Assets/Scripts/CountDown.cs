using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] 
    private TMP_Text text;
    
    [SerializeField] 
    private int number = 3;

    [SerializeField] 
    private AudioClip goClip = default;
    
    private AudioSource _as = default;
    public void Awake()
    {
        _as = GetComponent<AudioSource>();
        text.text = number.ToString();
    }

    public void NextOrPlay()
    {
        number--;
        if (number < 0)
        {
            gameObject.SetActive(false);      
            GameState.Instance.Go();

            return;
        }
        if (number == 0)
        {
            _as.clip = goClip;
            text.text = "GO!";
            return;
        }

        text.text = number.ToString();
    }
}
