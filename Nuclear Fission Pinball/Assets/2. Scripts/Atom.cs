using UnityEngine;

public class Atom : MonoBehaviour
{
    public GameObject neutronPrefab; 
    public int splitCount = 2;      
    public float explosionForce = 5f; 

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Neutron"))
        {
            Explode();
        }
    }

    void Explode()
    {
        for (int i = 0; i < splitCount; i++)
        {
            GameObject newNeutron = ObjectPool.Instance.SpawnFromPool("Neutron", transform.position, Quaternion.identity);

            Vector2 randomDir = Random.insideUnitCircle.normalized;

            Rigidbody2D rb = newNeutron.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; 
                rb.AddForce(randomDir * explosionForce, ForceMode2D.Impulse);
            }
        }

        Destroy(this.gameObject);
    }
}
