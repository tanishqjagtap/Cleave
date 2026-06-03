using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    public TMP_InputField inputField;

    private bool waitingForKey = false;

    public void StartRebind()
    {
        inputField.text = "...";
        waitingForKey = true;
    }

    void Update()
    {
        if (!waitingForKey) return;

        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            foreach (var key in Keyboard.current.allKeys)
            {
                if (key.wasPressedThisFrame)
                {
                    string newKey = key.displayName;

                    ControlsManager.Instance.AssignKey(
                        inputField,
                        newKey
                    );

                    waitingForKey = false;

                    Debug.Log("New Key: " + newKey);

                    break;
                }
            }
        }
    }
}