using UnityEngine;
using TMPro;

public class RebindButton : MonoBehaviour
{
    public TMP_Text keyText;

    public void StartRebind()
    {
        ControlsManager.Instance.StartListening(keyText);
    }
}