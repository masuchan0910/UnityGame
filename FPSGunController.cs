using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // ���ύX����2
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
    // ���ύX����1
    [SerializeField]
    ParticleSystem muzzleFlashParticle = null;
    [SerializeField]
    GameObject bulletHitEffectPrefab = null;
    // ���ύX����2
    [SerializeField]
    float resupplyInterval = 10;
    [SerializeField]
    Image ammoGauge = null;
    [SerializeField]
    Image resupplyGauge = null;

    // ���ύX����4
    [SerializeField]
    AudioSource audioSource = null;
    [SerializeField]
    AudioClip fireSe = null;

    // ���ύX����2
    int currentAmmo = 0;
    bool resupplyTimerIsActive = false;
    bool fireTimerIsActive = false;
    RaycastHit hit;
    WaitForSeconds fireIntervalWait;

    // ���ύX����2
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
        fireIntervalWait = new WaitForSeconds(fireInterval);  // WaitForSeconds���L���b�V�����Ă����i�������j
                                                              // ���ύX����2
        CurrentAmmo = maxAmmo;
    }
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
        // ���ύX����2
        if (!resupplyTimerIsActive)
        {
            StartCoroutine(nameof(ResupplyTimer));
        }
    }
    // �e�̔��ˏ���
    void Fire()
    {
        if (fireTimerIsActive || CurrentAmmo <= 0)
        {
            return;
        }
        // ���ύX����1
        muzzleFlashParticle.Play();

        // ���ύX����4
        audioSource.PlayOneShot(fireSe);

        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit, maxRange, hitLayers, QueryTriggerInteraction.Ignore))
        {
            BulletHit();
        }
        StartCoroutine(nameof(FireTimer));

        // ���ύX����2
        CurrentAmmo--;
    }
    // �e���q�b�g�����Ƃ��̏���
    void BulletHit()
    {
        // �e�X�g�p
        Debug.Log("�e���u" + hit.collider.gameObject.name + "�v�Ƀq�b�g���܂����B");
        // ���ύX����1
        Instantiate(bulletHitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        // ���ύX����3
        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = hit.collider.gameObject.GetComponent<EnemyController>();
            enemy.Damage(damage);
        }
    }
    // �e�𔭎˂���Ԋu�𐧌䂷��^�C�}�[
    IEnumerator FireTimer()
    {
        fireTimerIsActive = true;
        yield return fireIntervalWait;
        fireTimerIsActive = false;
    }

    // ���ύX����2�B��莞�Ԍo�߂��Ƃɒe���S�񕜂���^�C�}�[
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

    // ���ύX����2�B�e��񕜃^�C�}�[���L�����Z�����鏈��
    public void StopResupplyTimer()
    {
        StopCoroutine(nameof(ResupplyTimer));
        resupplyTimerIsActive = false;
    }

}