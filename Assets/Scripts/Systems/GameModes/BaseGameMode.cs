using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BaseGameMode : MonoBehaviour
{

    private void Start()
    {
        InitializeMode();
    }

    public virtual void InitializeMode()
    {

    }
}

