using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EnemyType { Goblin, EvilWizard }

public class EnemyPoolManager : MonoBehaviour
{
    [SerializeField] Goblin goblinPrefab;
    [SerializeField] EvilWizard evilWizardPrefab;

    [SerializeField] Queue<Goblin> goblinPool = new Queue<Goblin>();
    [SerializeField] Queue<EvilWizard> evilWizardPool = new Queue<EvilWizard>();

    [SerializeField] int initEnemyCount;

    void Start()
    {
        for (int i = 0; i < initEnemyCount; i++)
        {
            goblinPool.Enqueue(Instantiate(goblinPrefab, this.transform));
            evilWizardPool.Enqueue(Instantiate(evilWizardPrefab, this.transform));
        }
    }

    public Enemy Enemy_Dequeue(EnemyType type)
    {
        Enemy resultEnemy = null;

        switch (type)
        {
            case EnemyType.Goblin:
                {
                    resultEnemy = goblinPool.Dequeue();
                }
                break;
            case EnemyType.EvilWizard:
                {
                    resultEnemy = evilWizardPool.Dequeue();
                }
                break;
        }

        resultEnemy.gameObject.SetActive(true);

        return resultEnemy;
    }

    public void Enemy_Enqueue(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);

        switch (enemy.enemyType)
        {
            case EnemyType.Goblin:
                {
                    goblinPool.Enqueue(enemy as Goblin);
                }
                break;

            case EnemyType.EvilWizard:
                {
                    evilWizardPool.Enqueue(enemy as EvilWizard);
                }
                break;
        }
    }

}
