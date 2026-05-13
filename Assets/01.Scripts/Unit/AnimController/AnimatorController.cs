using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer sr;

    [SerializeField] protected Unit owner;
    
    public int LookingDir()
    {
        return sr.flipX ? -1 : 1;
    }
}
