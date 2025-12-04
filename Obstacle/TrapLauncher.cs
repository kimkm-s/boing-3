using UnityEngine;

public class TrapLauncher : MonoBehaviour
{
    [Header("투사체 설정")]
    public GameObject projectilePrefab;

    [Header("발사 제어")]
    public bool shootOnStart = true;
    public float shootingInterval = 5.0f;

    private void Start()
    {
        if (shootOnStart)
        {
            // shootingInterval 마다 Shoot() 호출
            InvokeRepeating("Shoot", 0f, shootingInterval); // ⭐ 첫 발사는 즉시
        }
    }

    public void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab이 설정되지 않았습니다. Inspector를 확인하세요.");
            return;
        }

        Vector3 spawnPosition = transform.position;

        // 2. 투사체 생성 (생성되자마자 Stalactite.cs의 Start()가 실행되어 2초 대기를 시작함)
        GameObject newProjectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        Debug.Log("발사대가 투사체를 생성했습니다. 투사체가 자체적으로 2초 후 낙하합니다.");
    }
}