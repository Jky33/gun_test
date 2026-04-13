using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    void Start()
{
    Destroy(gameObject, lifeTime);
}
void OnCollisionEnter(Collision collision)
{
    Debug.Log("Bullet hit: " + collision.gameObject.name);
}
}