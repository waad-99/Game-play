using UnityEngine;


namespace MikeNspired.XRIStarterKit
{
    public class ObjectSpawnerWaad : MonoBehaviour
    {
        public bool isActive = true;
        [SerializeField] private bool onlySpawnIfRoom = true;
        [SerializeField] private GameObject Prefab = null;
        [SerializeField] private Transform spawnReference = null;
        [SerializeField] private float spawnTimer = 5f;

        private bool hitDetect;
        private float currentTimer = 0;

        private void FixedUpdate()
        {
            if (!isActive || spawnReference == null || Prefab == null)
                return;

            if (!onlySpawnIfRoom)
            {
                TickTimerAndSpawn();
                return;
            }

            if (hitDetect)
                currentTimer = 0;
            else
                TickTimerAndSpawn();
        }

        private void TickTimerAndSpawn()
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= spawnTimer)
            {
                Spawn();
                currentTimer = 0;
            }
        }

        private void Spawn()
        {
            GameObject newObj = Instantiate(Prefab, spawnReference.position, spawnReference.rotation);
            newObj.transform.SetParent(spawnReference); // اختياري

            Rigidbody rb = newObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            // نحاول نحصل على XRGrabInteractable
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = newObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null)
            {
                grab.selectEntered.AddListener((args) =>
                {
                    if (rb != null)
                        rb.constraints = RigidbodyConstraints.None;
                });
            }
        }

        private void OnTriggerStay(Collider other) => hitDetect = true;
        private void OnTriggerExit(Collider other) => hitDetect = false;
    }
}
