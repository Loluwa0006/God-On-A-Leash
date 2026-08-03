using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RaceCheckpoint : MonoBehaviour
{
    public event Action<RaceCheckpoint> checkpointReached;

    [SerializeField] Material checkpointMaterial;
    [SerializeField] MeshRenderer checkpointModel;
    
   

    public bool CheckpointDisabled { get; set; } = false;


    private void Start()
    {
        //clone the material so that there is a per instance copy of it
        if (checkpointModel == null) checkpointModel = GetComponent<MeshRenderer>();
        checkpointMaterial = new (checkpointMaterial);
        GetComponent<MeshRenderer>().material = checkpointMaterial;
        checkpointMaterial.color = Color.red;
    }

    public void Hide()
    {
        if (checkpointModel != null) checkpointModel.enabled = false;
        CheckpointDisabled = true;
    }

    public void Show()
    {
        if (checkpointModel != null) checkpointModel.enabled = true;
        CheckpointDisabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (CheckpointDisabled) return;
        if (other.CompareTag("Player"))
        {
            checkpointReached.Invoke(this);
            checkpointMaterial.color = Color.green;
            CheckpointDisabled = true;
        }
    }
}
