using UnityEngine;

public class MonsterAttack : MonoBehaviour
{

    public enum AttackType
    {
        Projectile,     // 1) 플레이어에게 직선 투사체
        Parabola,       // 2) 포물선 투사체
        Reflect         // 3) 반사형 투사체
    }

    [Header("공격 방식")]
    public AttackType attackType = AttackType.Projectile;

    [Header("필요한 오브젝트")]
    public Transform player;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float attackCooldown = 2f;

    private float timer;

    private Collider2D[] monsterColliders;

    private Collider2D monsterCollider;
    private CircleCollider2D circleCollider;

    void Start()
    {
        // Start()에서 콜라이더를 가져와 저장
        monsterColliders = GetComponents<Collider2D>();

        // 🔥 플레이어 오브젝트를 씬에서 찾아서 'player' 변수에 할당
        // 플레이어 오브젝트에 Playercontroller1 스크립트가 붙어있다고 가정
        Playercontroller1 activePlayer = FindObjectOfType<Playercontroller1>();

        if (activePlayer != null)
        {
            // 새로 찾은 플레이어의 Transform을 할당
            player = activePlayer.transform;
        }
        else
        {
            Debug.LogError("씬에서 플레이어 오브젝트(Playercontroller1)를 찾을 수 없습니다! 플레이어가 생성되었는지 확인하세요.");
        }
    }

    void Update()
    {
        // 🔥🔥🔥 추가: firePoint의 위치를 몬스터의 현재 위치로 동기화
        if (firePoint != null)
        {
            firePoint.position = transform.position;
        }

        timer += Time.deltaTime;

        if (timer >= attackCooldown)
        {
            timer = 0f;
            Attack();
            //Debug.Log("발사");
        }
    }

    public void SetTarget(Transform newPlayer)
    {
        this.player = newPlayer;
        Debug.Log($"몬스터가 새로운 플레이어 {newPlayer.gameObject.name}를 타겟으로 설정했습니다.");
    }

    void Attack()
    {
        switch (attackType)
        {
            case AttackType.Projectile:
                ShootStraight();
                break;

            case AttackType.Parabola:
                ShootParabola();
                break;

            case AttackType.Reflect:
                ShootReflect();
                break;
        }
    }

    // MonsterAttack.cs

    void ShootStraight()
    {
        if (!player) return;

        //Debug.Log($"발사 시점 몬스터 위치: {transform.position}");
        //Debug.Log($"발사 시점 플레이어 위치: {player.position}"); // 이 값이 변하지 않는지 확인!

        // 🔥 몬스터의 현재 위치(transform.position)를 기준으로 방향 계산
        // MonsterAttack.cs의 ShootStraight()
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized; // ⬅️ 몬스터의 현재 위치 사용

        // 2. 콜라이더 겹침 방지를 위한 시작 위치 계산
        // ... (circleCollider를 사용하는 안전 거리 계산 로직은 그대로 유지) ...
        if (!circleCollider)
        {
            // 콜라이더 정보가 없으면 몬스터의 위치에서 바로 발사
            ShootProjectile(transform.position, dir);
            return;
        }

        float safeDistance = circleCollider.radius + 0.1f;
        Vector3 startPos = transform.position + (Vector3)(dir * safeDistance);

        // 3. 투사체 생성 및 초기화
        ShootProjectile(startPos, dir);
    }

    // 헬퍼 함수 추가 (코드 정리)
    private void ShootProjectile(Vector3 startPos, Vector2 dir)
    {
        // 1. 투사체 생성
        GameObject projObj = Instantiate(projectilePrefab, startPos, Quaternion.identity);

        // 2. 투사체 컴포넌트 및 콜라이더 가져오기
        Projectile projectileComponent = projObj.GetComponent<Projectile>();
        var projCollider = projObj.GetComponent<Collider2D>();

        // *** 🚨 핵심 해결책 1: Init 호출 전에 컴포넌트가 있는지 확인합니다. ***
        if (projectileComponent == null)
        {
            Debug.LogError("Projectile 프리팹에 'Projectile' 스크립트가 없습니다!");
            return;
        }

        // 3. 몬스터 콜라이더와의 충돌 무시 설정
        // Start()에서 GetComponents<Collider2D>()로 가져온 monsterColliders를 사용합니다.
        /*if (projCollider != null && monsterColliders != null)
        {
            foreach (var col in monsterColliders)
            {
                if (col != null)
                {
                    Physics2D.IgnoreCollision(projCollider, col);
                }
            }
        }*/

        projectileComponent.Init(dir, monsterColliders);

    }

        void ShootParabola()
    {
        // 포물선 공격은 추후 구현
        Debug.Log("포물선 공격!");
    }

    void ShootReflect()
    {
        // 반사형 공격은 추후 구현
        Debug.Log("반사형 공격!");
    }
}
