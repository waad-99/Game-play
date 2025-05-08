using UnityEngine;
using TMPro;

public class CapsuleHealth : MonoBehaviour
{
    public float health = 100f;
    public float headHealthMultiplier = 1.5f;
    public float bodyHealthMultiplier = 1f;
    public Transform headPosition;
    public Transform bodyPosition;
    public TextMeshProUGUI damageText;
    public CapsuleSpawner spawner;
    public GameManagerWaad2 gameManager; 
    public bool isSpecial = false;

    private bool hasDied = false;

    // رابط إلى السكربت الذي يحتوي على الأنيميشن
    public CharacterAnimationWaad characterAnimationWaad; // تأكد من إضافة هذا المتغير

    private void Start()
    {
        // إذا لم يتم تعيين CharacterAnimationWaad في الـ Inspector، نحاول الحصول عليه من المكون نفسه
        if (characterAnimationWaad == null)
        {
            characterAnimationWaad = GetComponent<CharacterAnimationWaad>(); // تأكد من أنك أضفت السكربت إلى الكائن في الـ Inspector
        }

        // تشغيل الأنيميشن idle عند بداية اللعبة
        characterAnimationWaad.PlayIdleAnimation(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasDied) return;

        float damage = 0f;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.contacts[0].thisCollider.transform == headPosition)
            {
                damage = 10f * headHealthMultiplier;
                Debug.Log("Headshot!");
            }
            else if (collision.contacts[0].thisCollider.transform == bodyPosition)
            {
                damage = 10f * bodyHealthMultiplier;
                Debug.Log("Body shot!");
            }
            else
            {
                damage = 10f * bodyHealthMultiplier;
            }

            health -= damage;

            if (damageText != null)
            {
                damageText.text = "-" + damage.ToString("F1");
                CancelInvoke(nameof(ClearText));
                Invoke(nameof(ClearText), 1.5f);
            }

            if (health <= 0f)
            {
                Die();
            }

            Destroy(collision.gameObject);
        }
    }

    private void ClearText()
    {
        damageText.text = "";
    }

    private void Die()
    {
        if (hasDied) return;
        hasDied = true;

        Debug.Log(gameObject.name + " has died.");

        // تشغيل أنيميشن الموت عندما يموت العدو
        characterAnimationWaad.PlayDieAnimation();

        if (gameManager != null)
        {
            if (isSpecial)
            {
                gameManager.AddSpecialKill();
            }
            else
            {
                gameManager.AddNormalKill();
            }
        }

        if (spawner != null)
        {
            spawner.OnEnemyKilled();
        }

        Destroy(gameObject);
    }
}
