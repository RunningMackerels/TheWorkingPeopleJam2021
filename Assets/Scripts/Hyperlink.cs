using UnityEngine;

public class Hyperlink : MonoBehaviour
{
    [SerializeField]
    private string linkToOpen;

    private void OnMouseUp()
    {
        Application.OpenURL(linkToOpen);
    }
}
