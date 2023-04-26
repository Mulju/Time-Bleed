using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    public float lerpTime = 3f;

    public bool isLerping = false;
    private Vector3 startPos;
    private Vector3 endPos;

    private void Update()
    {
        if (target == null) return;


        if (!target.GetComponent<PlayerEntity>().isAlive)
        {
            transform.LookAt(target.transform);
            startPos = transform.position;
            endPos = target.position + Vector3.up * 10f;
            float t = Mathf.Clamp01(Time.deltaTime * lerpTime);
            transform.position = Vector3.Lerp(startPos, endPos, t);
        }
    }
}