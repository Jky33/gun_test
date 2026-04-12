using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 3f;

    void Start()
{
    Destroy(gameObject, lifeTime);
}
}