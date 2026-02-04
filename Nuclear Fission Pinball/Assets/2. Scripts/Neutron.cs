using UnityEngine;

public class Neutron : MonoBehaviour
{
    [Header("설정")]
    public float splitForce = 7f;
    public int hitsRequired = 2; // 분열하기 위해 필요한 충돌 횟수

    private int currentHitCount = 0; // 현재 충돌 횟수
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 태어날 때 초기화 (중요!)
        currentHitCount = 0;
        UpdateColor();

        if (GameManager.Instance != null) GameManager.Instance.RegisterNeutron();
    }

    void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.UnregisterNeutron();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            currentHitCount++; // 충돌 횟수 증가

            // 목표 횟수를 채웠는가?
            if (currentHitCount >= hitsRequired)
            {
                Split();
                currentHitCount = 0; // 분열 후 카운트 초기화 (다시 0부터 시작)
            }

            UpdateColor(); // 충돌할 때마다 색상 변경 (시각적 피드백)
        }
    }

    void Split()
    {
        // 최적화된 풀에서 가져오기
        GameObject clone = ObjectPool.Instance.GetNeutron(transform.position, Quaternion.identity);

        if (clone != null)
        {
            Rigidbody2D myRb = GetComponent<Rigidbody2D>();
            Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();

            Vector2 randomDir = Random.insideUnitCircle.normalized;

            // 서로 밀어내기
            if (myRb != null) myRb.AddForce(randomDir * splitForce, ForceMode2D.Impulse);
            if (cloneRb != null) cloneRb.AddForce(-randomDir * splitForce, ForceMode2D.Impulse);
        }
    }

    // 상태에 따라 색을 바꿔주는 함수 (충전량 표시)
    void UpdateColor()
    {
        if (sr == null) return;

        // 충전이 안 됐으면 하얀색, 
        // 터지기 직전(1번 남음)이면 빨간색 경고
        if (currentHitCount == 0)
        {
            sr.color = Color.white; // 평상시
        }
        else
        {
            // 터지기 일보 직전! (노란색이나 붉은 계열 추천)
            sr.color = new Color(1f, 0.5f, 0f); // 주황색
        }
    }
}