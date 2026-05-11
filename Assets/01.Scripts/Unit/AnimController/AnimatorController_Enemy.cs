using System.Collections;
using UnityEngine;

public class AnimatorController_Enemy : AnimatorController
{
    protected bool isAttack = false;
    [SerializeField] protected float deathWaitTime;

    protected IEnumerator DeathCoroutine()
    {
        owner.gameObject.layer = LayerMask.NameToLayer("DeathEnemy");
        GameManager.Instance.SetComboUI();

        yield return new WaitForSeconds(deathWaitTime);

        // 나중에 pooling으로 바꿔줄거임
        owner.gameObject.SetActive(false);
    }
    
}
