using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] float projectileSpeed;
    [SerializeField] float dmg;
    [SerializeField] Vector2 moveDir;
    [SerializeField] float lifeTime;

    Coroutine lifeTimeCoroutine = null;

    void OnEnable()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }

        lifeTimeCoroutine = StartCoroutine(LifeTimeCoroutine());
    }

    void Update()
    {
        transform.Translate(moveDir * Time.deltaTime * projectileSpeed);
    }

    public void Init(Vector3 start, Vector2 moveDir, float dmg)
    {
        this.dmg = dmg;
        this.moveDir = new Vector2(moveDir.x, 0f).normalized;
        this.transform.position = start; // 월드 좌표로 실행

        this.gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(7))
        {
            Debug.Log($"Hit Player");
            collision.GetComponent<Player>().Hit(dmg);
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator LifeTimeCoroutine()
    {
        float time = 0;

        while (time < lifeTime)
        {
            var deltaTime = Time.deltaTime;
            time += deltaTime;
            yield return new WaitForSeconds(deltaTime);
        }

        gameObject.SetActive(false);
    }
}
