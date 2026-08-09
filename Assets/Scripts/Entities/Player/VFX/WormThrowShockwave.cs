using UnityEngine;

public class WormThrowShockwave : MonoBehaviour
{
    [SerializeField] Animator shockwaveAnimator;
    [SerializeField] Transform shockwaveTransform;
    [SerializeField] Rigidbody rb;


    Vector3 positionOffset;

    private void Start()
    {
        //Use world space
      //  positionOffset = shockwaveTransform.transform.localPosition;
       // shockwaveAnimator.transform.SetParent(null);
        if (shockwaveAnimator == null) shockwaveAnimator = GetComponent<Animator>();
        
    }
    public void OnWormThrown()
    {
        if (rb == null) return;
        shockwaveAnimator.Play("PlayShockwave", 0, 0f);
      //  shockwaveAnimator.transform.position = rb.position + positionOffset;
        Debug.Log("worm thrown");
    }
}
