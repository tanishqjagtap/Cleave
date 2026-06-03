using UnityEngine;
using TMPro;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance;

    public TMP_InputField forwardField;
    public TMP_InputField backwardField;
    public TMP_InputField leftField;
    public TMP_InputField rightField;
    public TMP_InputField jumpField;
    public TMP_InputField slideField;

    private TMP_InputField listeningField = null;
    private string previousKey = ""; // store key BEFORE we show "..."

    private void Awake()
    {
        Instance = this;
    }

    public void StartListening(TMP_InputField field)
    {
        listeningField = field;
        previousKey = field.text; // save current key before overwriting
        field.text = "...";
    }

    private void Update()
    {
        if (listeningField == null) return;

        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key == KeyCode.Escape || key == KeyCode.Mouse0 ||
                        key == KeyCode.Mouse1 || key == KeyCode.Mouse2) break;

                    AssignKey(listeningField, key.ToString());
                    listeningField = null;
                    break;
                }
            }
        }
    }

    public void AssignKey(TMP_InputField changedField, string newKey)
    {
        TMP_InputField[] allFields =
        {
            forwardField, backwardField, leftField,
            rightField, jumpField, slideField
        };

        // If another field already has this key, give it our previous key
        foreach (TMP_InputField field in allFields)
        {
            if (field != changedField && field.text == newKey)
            {
                field.text = previousKey; // swap with the saved previous key
                break;
            }
        }

        changedField.text = newKey;
    }
}