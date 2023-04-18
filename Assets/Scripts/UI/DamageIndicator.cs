using System.Collections;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private GameObject _player = null;
    private Vector3 _bulletDirection;
    private UnityEngine.UI.Image _image;

    public void SetDamageIndicator(GameObject player, Vector3 bulletDirection)
    {
        _player = player;
        _bulletDirection = bulletDirection * -1;
        _image = GetComponentInChildren<UnityEngine.UI.Image>();

        StartCoroutine(DestroyDamageIndicator());
    }

    private void Update()
    {
        if (_player == null) return;

        Vector3 playerForward = _player.transform.forward;
        float angle = Vector3.Angle(playerForward, _bulletDirection);
        Vector3 cross = Vector3.Cross(playerForward, _bulletDirection);

        if (cross.y < 0)
            angle = 360 - angle;

        Quaternion rotation = Quaternion.Euler(0, 180, angle);

        Debug.Log(angle);
        transform.localRotation = rotation;
    }

    IEnumerator DestroyDamageIndicator(int fadeTime = 3)
    {
        yield return new WaitForSeconds(1);

        Color originalColor = _image.color;

        for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
        {
            _image.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t / fadeTime));
            yield return null;
        }
        Destroy(gameObject);
    }
}