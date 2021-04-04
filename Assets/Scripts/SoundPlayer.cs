using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SoundPlayer : MonoBehaviour
{
    private enum Clips
    {
        Move,
        Stopped,
        Reversed,
        OneLine,
        TwoLines,
        ThreeLines,
        FourLines,
        Size
    }
    
    private static SoundPlayer _instance = null;

    public static SoundPlayer Instance => _instance;

    [SerializeField] 
    private AudioClip[] clips;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        _instance = this;
    }


    public void PlayHorizontalMove(Tetrimo t)
    {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        AudioSource.PlayClipAtPoint(clips[(int)Clips.Move], Camera.main.transform.position);
    }

    public void PlayStopped(Tetrimo t)
    {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        AudioSource.PlayClipAtPoint(clips[(int)Clips.Stopped], Camera.main.transform.position);
    }

    public void PlayReversed()
    {
        Debug.Assert(Camera.main != null, "Camera.main != null");
        AudioSource.PlayClipAtPoint(clips[(int)Clips.Reversed], Camera.main.transform.position);
    }

    public void PlayNumberLines(int n)
    {
        n = n - 1;
        Debug.Assert(Camera.main != null, "Camera.main != null");
        AudioSource.PlayClipAtPoint(clips[(int)Clips.OneLine + n], Camera.main.transform.position);
    }
}
