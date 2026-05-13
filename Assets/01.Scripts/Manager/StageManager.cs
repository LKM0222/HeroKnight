using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StageInfo
{
    public List<SpawnPos> spawnPosList;

}

[Serializable]
public class SpawnPos
{
    public int spawnPosNum;
    public EnemyType enemyType;
    public int enemyCount;
    
}

public class StageManager : MonoSingleton<StageManager>
{
    // 스테이지 구성...
    [SerializeField] List<StageInfo> stageInfoList = new List<StageInfo>();

    [SerializeField] List<Enemy> remainEnemyList = new List<Enemy>();
    [SerializeField] List<Transform> spawnPosList = new List<Transform>();
    public Transform playerStartPos;

    [SerializeField] int nowStage = 0;

    Coroutine stageCoroutine = null;

    public UnityAction deadAction;
    public int RemainEnemyCount => remainEnemyList.Count;

    public bool isGameStart;

    public void GaemStart()
    {
        Init();
    }


    void Init()
    {
        if (stageCoroutine != null)
        {
            StopCoroutine(stageCoroutine);
        }

        stageCoroutine = StartCoroutine(StageCoroutine());
    }

    void SpawnEnemy(SpawnPos pos)
    {
        var enemy = EnemyPoolManager.Instance.Enemy_Dequeue(pos.enemyType);
        remainEnemyList.Add(enemy);

        enemy.transform.position = spawnPosList[pos.spawnPosNum].position;
    }

    IEnumerator StageCoroutine()
    {
        // 게임 시작 대기 (시작버튼 대기)

        yield return new WaitUntil(() => EnemyPoolManager.Instance != null && EnemyPoolManager.Instance.isInit);

        // 기존에 있던 Enemy 초기화
        remainEnemyList.ForEach(x => EnemyPoolManager.Instance.Enemy_Enqueue(x));
        remainEnemyList.Clear();
        
        // 스테이지 진행 시작
        while (nowStage < stageInfoList.Count)
        {
            // 스타트 UI 출력, 출력 후 N초 대기

            isGameStart = true;

            //Enemy 소환 / enemy 전부 처리될때까지 대기 / 다음 enemy 소환
            stageInfoList[nowStage].spawnPosList.ForEach(x => SpawnEnemy(x));

            yield return new WaitUntil(() => RemainEnemyCount <= 0);
            nowStage++;
            isGameStart = false;
            PlayerPrefs.SetInt("MaxStage", Math.Max(nowStage, PlayerPrefs.GetInt("MaxStage", 0)));
        }

        Debug.Log("Game Finish");
    }

    public void EnemyDead(Enemy enemy)
    {
        remainEnemyList.Remove(enemy);
    }

}
