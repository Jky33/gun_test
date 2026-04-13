using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject hitEffect; // 爆炸特效（之後用）
    public int scoreValue = 10;

    void OnCollisionEnter(Collision collision)
{
    Debug.Log("Hit: " + collision.gameObject.name);

    if (collision.transform.root.CompareTag("Bullet"))
    {
        Debug.Log("Hit Target!");
        Destroy(gameObject);
    }
}
}