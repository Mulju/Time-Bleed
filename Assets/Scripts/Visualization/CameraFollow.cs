using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public float lerpTime = 1.5f;

    private bool isLerping = false;
    private Vector3 startPos;
    private Vector3 endPos;

    private void Update()
    {
        if (!target.GetComponent<PlayerEntity>().isAlive)
        {
            isLerping = true;
            startPos = transform.position;
            endPos = target.position + Vector3.up * 5f; // Change 5f to desired height above player
        }

        if (isLerping)
        {
            float t = Mathf.Clamp01(Time.deltaTime * lerpTime);
            transform.position = Vector3.Lerp(startPos, endPos, t);

            if (transform.position == endPos)
            {
                isLerping = false;
            }
        }
    }
}