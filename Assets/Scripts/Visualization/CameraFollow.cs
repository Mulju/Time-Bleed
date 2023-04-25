using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public float lerpTime = 3f;

    private bool isLerping = false;
    private Vector3 startPos;
    private Vector3 endPos;

    private void Update()
    {
        if (target == null) return;

        transform.LookAt(target.transform);

        if (!target.GetComponent<PlayerEntity>().isAlive)
        {
            isLerping = true;
            startPos = transform.position;
            endPos = target.position + Vector3.up * 10f;
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