using UnityEngine;
using TMPro;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance;

    [Header("Keybind Text")]
    public TMP_Text forwardField;
    public TMP_Text backwardField;
    public TMP_Text leftField;
    public TMP_Text rightField;
    public TMP_Text jumpField;
    public TMP_Text sprintField;
    public TMP_Text slideField;
    public TMP_Text interactField;

    private TMP_Text currentField;
    private string previousKey;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartListening(TMP_Text field)
    {
        currentField = field;
        previousKey = field.text;
        field.text = "...";
    }

    private void Update()
    {
        if (currentField == null)
            return;

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                if (key == KeyCode.Mouse0 ||
                    key == KeyCode.Mouse1 ||
                    key == KeyCode.Mouse2)
                    return;

                string newKey = key.ToString();

                AssignKey(currentField, newKey);

                currentField = null;
                break;
            }
        }
    }

    private void AssignKey(TMP_Text changedField, string newKey)
    {
        TMP_Text[] allFields =
        {
            forwardField,
            backwardField,
            leftField,
            rightField,
            jumpField,
            sprintField,
            slideField,
            interactField
        };

        foreach (TMP_Text field in allFields)
        {
            if (field == changedField)
                continue;

            if (field.text.ToUpper() == newKey.ToUpper())
            {
                field.text = previousKey;
                break;
            }
        }

        changedField.text = newKey;
    }

    public static bool GetKey(string keyName)
    {
        try
        {
            KeyCode code =
                (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName, true);

            return Input.GetKey(code);
        }
        catch
        {
            return false;
        }
    }

    public static bool GetKeyDown(string keyName)
    {
        try
        {
            KeyCode code =
                (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName, true);

            return Input.GetKeyDown(code);
        }
        catch
        {
            return false;
        }
    }
}