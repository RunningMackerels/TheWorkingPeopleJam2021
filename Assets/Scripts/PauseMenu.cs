using System;
using UnityEngine;

[RequireComponent(typeof(PauseMenu))]
public class PauseMenu : MonoBehaviour
{

    public static PauseMenu Instance => _instance;
    private static PauseMenu _instance = null;

    private Canvas _canvas = default;
    private bool _visible = false;
    
    #region Events
    public static event Action ButtonResume;
    public static event Action ButtonRestart;
    public static event Action ButtonQuit;
    #endregion

    void VerifySingleton()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        _instance = this;
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = _visible;
    }

    private void Awake()
    {
        VerifySingleton();
    }

    public void Show()
    {
        Toggle();
    }

    private void Toggle()
    {
        _visible = !_visible;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnButtonResume();
        }
        _canvas.enabled = _visible;
    }

    public bool IsVisible() => _visible;

    public void OnButtonResume()
    {
        ButtonResume?.Invoke();
        Toggle();
    }

    public void OnButtonRestart()
    {
        ButtonRestart?.Invoke();
        Toggle();        
    }
    
    public void OnButtonQuit()
    {
        ButtonQuit?.Invoke();
        Toggle();
    }
}

