using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // ★変更その2
public class FpsGunControler : MonoBehaviour
{
    [SerializeField]
    Transform bulletSpawn = null;
    [SerializeField, Min(1)]
    int damage = 1;
    [SerializeField, Min(1)]
    int maxAmmo = 30;
    [SerializeField, Min(1)]
    float maxRange = 30;
    [SerializeField]
    LayerMask hitLayers = 0;
    [SerializeField, Min(0.01f)]
    float fireInterval = 0.1f;
    // ★変更その1
    [SerializeField]
    ParticleSystem muzzleFlashParticle = null;
    [SerializeField]
    GameObject bulletHitEffectPrefab = null;
    // ★変更その2
    [SerializeField]
    float resupplyInterval = 10;
    [SerializeField]
    Image ammoGauge = null;
    [SerializeField]
    Image resupplyGauge = null;

    // ★変更その4
    [SerializeField]
    AudioSource audioSource = null;
    [SerializeField]
    AudioClip fireSe = null;

    // ★変更その2
    int currentAmmo = 0;
    bool resupplyTimerIsActive = false;
    bool fireTimerIsActive = false;
    RaycastHit hit;
    WaitForSeconds fireIntervalWait;

    // ★変更その2
    public int CurrentAmmo
    {
        set
        {
            currentAmmo = Mathf.Clamp(value, 0, maxAmmo);
            float scaleX = currentAmmo / (float)maxAmmo;
            ammoGauge.rectTransform.localScale = new Vector3(scaleX, 1, 1);
        }
        get
        {
            return currentAmmo;
        }
    }

    void Start()
    {
        fireIntervalWait = new WaitForSeconds(fireInterval);  // WaitForSecondsをキャッシュしておく（高速化）
                                                              // ★変更その2
        CurrentAmmo = maxAmmo;
    }
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
        // ★変更その2
        if (!resupplyTimerIsActive)
        {
            StartCoroutine(nameof(ResupplyTimer));
        }
    }
    // 弾の発射処理
    void Fire()
    {
        if (fireTimerIsActive || CurrentAmmo <= 0)
        {
            return;
        }
        // ★変更その1
        muzzleFlashParticle.Play();

        // ★変更その4
        audioSource.PlayOneShot(fireSe);

        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit, maxRange, hitLayers, QueryTriggerInteraction.Ignore))
        {
            BulletHit();
        }
        StartCoroutine(nameof(FireTimer));

        // ★変更その2
        CurrentAmmo--;
    }
    // 弾がヒットしたときの処理
    void BulletHit()
    {
        // テスト用
        Debug.Log("弾が「" + hit.collider.gameObject.name + "」にヒットしました。");
        // ★変更その1
        Instantiate(bulletHitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        // ★変更その3
        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
            enemy.Damage(damage);
        }
    }
    // 弾を発射する間隔を制御するタイマー
    IEnumerator FireTimer()
    {
        fireTimerIsActive = true;
        yield return fireIntervalWait;
        fireTimerIsActive = false;
    }

    // ★変更その2。一定時間経過ごとに弾薬を全回復するタイマー
    IEnumerator ResupplyTimer()
    {
        resupplyTimerIsActive = true;
        float timer = 0;
        while (timer < resupplyInterval)
        {
            resupplyGauge.rectTransform.localScale = new Vector3(timer / resupplyInterval, 1, 1);
            timer += Time.deltaTime;

            yield return null;
        }
        CurrentAmmo = maxAmmo;
        resupplyTimerIsActive = false;
    }

    // ★変更その2。弾薬回復タイマーをキャンセルする処理
    public void StopResupplyTimer()
    {
        StopCoroutine(nameof(ResupplyTimer));
        resupplyTimerIsActive = false;
    }

}