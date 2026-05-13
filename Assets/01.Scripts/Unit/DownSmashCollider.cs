using System.Collections.Generic;
using UnityEngine;

public class DownSmashCollider : MonoBehaviour
{
    [SerializeField] BoxCollider2D downSmashCollider;

    Player player;

    List<Enemy> enemyList = new List<Enemy>();

    public void Init(Player _player, float size)
    {
        this.player = _player;
        downSmashCollider.size = new Vector2(size, 1);
        this.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy == null) return;

            if (!enemyList.Contains(enemy))
            {
                enemy.EnemyKnockback(player);
                enemyList.Add(enemy);
            }
        }
    }

    public void EnemyHit(float damage)
    {
        foreach (var enemy in enemyList)
        {
            enemy.Hit(damage);
        }

        GameManager.Instance.player.CamImpulse(damage);
        enemyList.Clear();
    }
}
