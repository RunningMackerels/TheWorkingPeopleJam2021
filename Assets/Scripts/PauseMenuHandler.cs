using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuHandler : MonoBehaviour
{

    private LoadSceneButton _lsb = null;

    private void Awake()
    {
        _lsb = GetComponent<LoadSceneButton>();
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        PauseMenu.ButtonQuit += OnButtonQuit;
        PauseMenu.ButtonRestart += OnButtonRestart;
    }


    private void OnButtonRestart()
    {
        _lsb.LoadScene("Main");
    }

    private void OnButtonQuit()
    {
        _lsb.LoadScene("Start");
    }
}
