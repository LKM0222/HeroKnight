using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Main Data")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpPower;
    [SerializeField] protected float atk;
    [SerializeField] protected float atkRange;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float hp;

    [Header("UI")]
    [SerializeField] protected HPBar hpBar;
    [SerializeField] protected FloatingText floatingText;

    protected virtual void Start()
    {
        hp = maxHP;
        float hpRatio = hp / maxHP;
        hpBar.SetHPBar(hpRatio);
    }
}
