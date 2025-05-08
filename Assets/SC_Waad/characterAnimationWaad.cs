using UnityEngine;

public class CharacterAnimationWaad : MonoBehaviour
{
    public Animator animator;

    // تشغيل أنيميشن idle
    public void PlayIdleAnimation()
    {
        if (animator != null)
        {
            animator.Play("Idle");
        }
    }

    // تشغيل أنيميشن المشي
    public void PlayWalkAnimation()
    {
        if (animator != null)
        {
            animator.Play("Walk");
        }
    }

    // تشغيل أنيميشن الموت
    public void PlayDieAnimation()
    {
        if (animator != null)
        {
            animator.Play("Die");
        }
    }
}
