using System;
using UnityEngine;

public class RaceCheckpoint : MonoBehaviour
{
    public event Action<RaceCheckpoint> checkpointReached;

    [SerializeField] Material checkpointMaterial;
   

    public bool CheckpointDisabled { get; set; } = false;


    private void Start()
    {
        //clone the material so that there is a per instance copy of it
        checkpointMaterial = new (checkpointMaterial);
        GetComponent<MeshRenderer>().material = checkpointMaterial;
        checkpointMaterial.color = Color.red;
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
