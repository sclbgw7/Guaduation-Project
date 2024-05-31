using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAndFaceAtTarget : MonoBehaviour
{
    [SerializeField]
    private Transform follow;
    [SerializeField]
    private Transform faceAt;
    [SerializeField]
    private float easeSpeed;
    [SerializeField]
    private float minAngularSpeed;

    // Update is called once per frame
    public void UpdateManually()
    {
        // if (follow != null)
        {
            transform.position = follow.position;
        }

        if (faceAt != null)
        {
            Quaternion preferedRotation = Quaternion.LookRotation(faceAt.position - transform.position);

            float theta1 = Quaternion.Angle(transform.rotation, preferedRotation) / 180.0f * Mathf.PI;
            // Debug.Log(theta1);
            
            float t = Mathf.Max(minAngularSpeed * Time.deltaTime / theta1, easeSpeed * Time.deltaTime);
            t = Mathf.Min(1.0f, t);

            transform.rotation = Quaternion.Lerp(transform.rotation, preferedRotation, t);
        }
    }

    public void SetFaceAt(Transform newFaceAt)
    {
        if (faceAt == null && newFaceAt != null) 
        {
            transform.rotation = Quaternion.LookRotation(newFaceAt.position - transform.position);
        }

        faceAt = newFaceAt;
    }
}
