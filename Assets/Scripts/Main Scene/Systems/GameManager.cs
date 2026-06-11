using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform maya;
    public Transform lena;

    private void Awake()
    {
        Checkpoint.maya = maya;
        Checkpoint.lena = lena;
    }
}