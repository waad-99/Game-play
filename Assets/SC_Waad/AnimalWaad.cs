using UnityEngine;

public class Animal : MonoBehaviour
{
    public static event System.Action<Animal> OnAnimalCaught;

    [Header("Animal Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float timeToSpeed = 2f;
    [SerializeField] private float timeToChangeDirection = 3f;
    
    private bool isMoving = true;
    private Vector3 targetDirection;
    private float timeElapsed;
    private float directionChangeTime;

    private void Start()
    {
        SetNewDirection();
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveAnimal();
        }
    }

    // لتحريك الحيوان في اتجاه معين
    private void MoveAnimal()
    {
        timeElapsed += Time.deltaTime;
        transform.Translate(targetDirection * movementSpeed * Time.deltaTime);

        // إذا انتهى الوقت المخصص لتغيير الاتجاه
        if (timeElapsed >= directionChangeTime)
        {
            SetNewDirection();
        }
    }

    // تحديد اتجاه جديد للحيوان
    private void SetNewDirection()
    {
        timeElapsed = 0f;
        directionChangeTime = Random.Range(timeToChangeDirection - 1f, timeToChangeDirection + 1f);
        targetDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    // تفعيل الحركة والسرعة عند الاصطفاء
    public void Initialize(float speed, float speedChangeTime)
    {
        movementSpeed = speed;
        timeToSpeed = speedChangeTime;
    }

    // إذا تم اصطياد الحيوان، يتم استدعاء هذا الميثود
    public void Catch()
    {
        if (OnAnimalCaught != null)
        {
            OnAnimalCaught.Invoke(this); // يتم تسجيل الحيوان الذي تم اصطياده
        }
        Destroy(gameObject); // تدمير الحيوان بعد اصطياده
    }

    // إذا تم لمس الحيوان بواسطة Trigger أو Raycast
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Catch();
        }
    }
}
