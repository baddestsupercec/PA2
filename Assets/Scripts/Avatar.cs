using UnityEngine;

public class Avatar : MonoBehaviour
{
    public Transform targetTransform;

    private void Update()
    {
        if (targetTransform != null)
        {
            // Match position
            transform.position = targetTransform.position;

            // Match rotation
            transform.rotation = targetTransform.rotation;

            // Match scale
            transform.localScale = targetTransform.localScale;
        }
    }
}
