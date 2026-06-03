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
    public TMP_InputField sprintField;
    public TMP_InputField slideField;

    private TMP_InputField listeningField = null;
    private string previousKey = "";

    private void Awake()
    {
        Instance = this;
    }

    public void StartListening(TMP_InputField field)
    {
        listeningField = field;
        previousKey = field.text;
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
            rightField, jumpField, sprintField, slideField
        };

        foreach (TMP_InputField field in allFields)
        {
            if (field != changedField &&
                string.Equals(field.text, newKey, System.StringComparison.OrdinalIgnoreCase))
            {
                field.text = previousKey;
                break;
            }
        }

        changedField.text = newKey;
    }

    // Convert KeyCode name to the format Input.GetKey(string) understands
    public static bool GetKey(string keyCodeName)
    {
        try
        {
            KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeName, true);
            return Input.GetKey(code);
        }
        catch { return false; }
    }

    public static bool GetKeyDown(string keyCodeName)
    {
        try
        {
            KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyCodeName, true);
            return Input.GetKeyDown(code);
        }
        catch { return false; }
    }
}