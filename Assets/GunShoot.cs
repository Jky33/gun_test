using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [Header("References")]
    public BLEIMUPoseSource imuSource;
    public Transform muzzle; // 槍口位置
    public GameObject bulletPrefab;

    [Header("Fire Settings")]
    public bool useKeyboard = true;
    public KeyCode fireKey = KeyCode.Mouse0;

    public enum FireMode { Semi, Auto }
    public FireMode fireMode = FireMode.Semi;

    public float fireRate = 10f; // 每秒幾發

    private float fireTimer = 0f;
    private bool lastTrigger = false;

    System.Collections.IEnumerator ApplyVelocityNextFrame(GameObject bullet)
{
    yield return null; // ⭐ 等一幀

    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.useGravity = false;
        rb.velocity = bullet.transform.forward * 30f;
    }
}

    void Update()
    {
        fireTimer += Time.deltaTime;

        bool trigger = GetTrigger();

        if (fireMode == FireMode.Semi)
        {
            // 👉 單發（按下瞬間）
            if (trigger && !lastTrigger)
            {
                Shoot();
            }
        }
        else if (fireMode == FireMode.Auto)
        {
            // 👉 連發
            if (trigger && fireTimer >= 1f / fireRate)
            {
                Shoot();
                fireTimer = 0f;
            }
        }

        lastTrigger = trigger;
    }

    bool GetTrigger()
    {
        bool imuTrigger = imuSource != null && imuSource.triggerValue == 1;
        bool keyTrigger = useKeyboard && Input.GetKey(fireKey);

        return imuTrigger || keyTrigger;
    }

    void Shoot()
{
    if (bulletPrefab == null || muzzle == null)
        return;

    Vector3 spawnPos = muzzle.position + muzzle.forward * 0.3f;

    GameObject bullet = Instantiate(
        bulletPrefab,
        spawnPos,
        muzzle.rotation
    );

    bullet.transform.forward = muzzle.forward;

    Rigidbody rb = bullet.GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.useGravity = false;

        // 先清掉舊速度
        rb.velocity = Vector3.zero;

        // 直接給速度（不要等下一幀）
        rb.velocity = muzzle.forward * 30f;

        Debug.Log("Shoot velocity: " + rb.velocity);
    }
    else
    {
        Debug.LogError("Bullet has no Rigidbody!");
    }
    Debug.Log("Is Kinematic: " + rb.isKinematic);
Debug.Log("Constraints: " + rb.constraints);
    rb.AddForce(muzzle.forward * 1000f);
}
}