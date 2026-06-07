using System.Collections;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    [Header("移动平台属性设置 ")]
    [SerializeField] private Transform p1, p2;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float waitTime = 1f;

    private Transform _player;
    private bool _playerOnPlatform = false;

    void Start()
    {
        StartCoroutine(platformMoving());
    }

    private IEnumerator platformMoving()
    {
        while (true)
        {
            // p2
            yield return MoveToPoint(p1.position, p2.position);
            yield return new WaitForSeconds(waitTime);

            // p1
            yield return MoveToPoint(p2.position, p1.position);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // 
    private IEnumerator MoveToPoint(Vector3 start, Vector3 end)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        transform.position = end;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_playerOnPlatform)
        {
            _player = collision.transform;
            _playerOnPlatform = true;
            _player.SetParent(transform); 
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerOnPlatform = false;
            _player.SetParent(null);
        }
    }
}