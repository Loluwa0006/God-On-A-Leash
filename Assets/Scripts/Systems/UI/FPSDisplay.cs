using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text fpsText;

    [SerializeField] int updateRate = 5;

    int currentTick;

    private void Start()
    {
        if (updateRate < 1) updateRate = 1;
    }
    private void Update()
    {
        currentTick++;
        if (currentTick == updateRate)
        {
            currentTick = 0;
            fpsText.text = "FPS: " + (1.0f / Time.deltaTime).ToString("F0");
        }
    }
}
