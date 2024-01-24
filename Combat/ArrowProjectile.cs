using Unity.VisualScripting;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
  [SerializeField] private Collider Collider;
  public float launchForce = 300f;
  private Rigidbody rb;
  private bool hasCollided = false;
  private float destroyAfterCollisionDuration = 1.0f;
  private float destroyAfterXSeconds = 20.0f;
  private void Start()
  {
    rb = GetComponent<Rigidbody>();

    if (gameObject.name == "ArrowProjectile(Clone)")
    {
      rb.AddForce(Camera.main.transform.forward.normalized * launchForce);
      return;
    }

    Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
    Vector3 launchDirection = (playerPos + Vector3.up + GetAimVariance(playerPos) - transform.position).normalized;
    transform.rotation = Quaternion.LookRotation(launchDirection);

    rb.AddForce(launchDirection * launchForce);
  }
  private Vector3 GetAimVariance(Vector3 playerPos)
  {
    float distanceBetween = Vector3.Distance(transform.position, playerPos);
    float varianceRange = distanceBetween / 100 * 2;
    Vector3 variance = new(
      Random.Range(-varianceRange, varianceRange),
      Random.Range(-varianceRange + 0.5f, varianceRange + 1.0f),
      Random.Range(-varianceRange, varianceRange));
    return variance;
  }
  private void Update()
  {
    destroyAfterCollisionDuration -= Time.deltaTime;
    destroyAfterXSeconds -= Time.deltaTime;

    if (hasCollided && (destroyAfterCollisionDuration <= 0.0f || destroyAfterXSeconds <= 0.0f))
    {
      gameObject.SetActive(false);
      Destroy(gameObject);
    }
  }
  private void OnTriggerEnter(Collider other)
  {
    if (other.name.Contains("Projectile") ||
        other.gameObject.CompareTag("PlayerTargeter") ||
       (gameObject.name == "ArrowProjectile(Clone)" && other.CompareTag("Player")) ||
       (gameObject.name == "MagicAttackProjectile(Clone)" && other.CompareTag("Enemy")) ||
       (gameObject.name == "EnemyArrowProjectile(Clone)" && other.CompareTag("Enemy"))) { return; }

    if (gameObject.name == "MagicAttackProjectile(Clone)") { gameObject.SetActive(false); Destroy(gameObject); }

    hasCollided = true;

    if (other.TryGetComponent<Health>(out Health health))
    {
      health.DealDamage(15);
    }

    Collider.enabled = false;
    rb.isKinematic = true;
    transform.SetParent(other.transform);
  }
}
