using UnityEngine;
using TMPro;

public class RebindButton : MonoBehaviour
{
    public TMP_InputField inputField;

    public void StartRebind()
    {
        // Let ControlsManager handle everything — saves previousKey and listens for input
        ControlsManager.Instance.StartListening(inputField);
    }
}