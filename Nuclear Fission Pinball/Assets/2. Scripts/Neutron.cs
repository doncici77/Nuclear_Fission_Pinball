using UnityEngine;

public class Neutron : MonoBehaviour
{
    [Header("분열 설정")]
    public int currentGen = 0;
    public int maxGen = 4;        // 4번 분열 (1 -> 16개)
    public float splitForce = 7f; // 튕겨 나가는 힘

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        // 풀에서 꺼낼 때마다 색상 업데이트
        UpdateColor();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 벽에 닿았고 + 아직 더 분열할 수 있다면
        if (collision.gameObject.CompareTag("Wall") && currentGen < maxGen)
        {
            Split();
        }
    }

    void Split()
    {
        currentGen++; // 다음 세대로 진화
        UpdateColor(); // 내 색깔 바꾸기

        // 친구(분신) 소환
        GameObject clone = ObjectPool.Instance.SpawnFromPool("Neutron", transform.position, Quaternion.identity);

        if (clone != null)
        {
            Neutron cloneScript = clone.GetComponent<Neutron>();
            cloneScript.currentGen = this.currentGen; // 친구도 같은 세대
            cloneScript.UpdateColor(); // 친구 색깔도 맞춤

            // 서로 반대 방향 등으로 튀어나가게 힘 가하기
            Rigidbody2D myRb = GetComponent<Rigidbody2D>();
            Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();

            Vector2 randomDir = Random.insideUnitCircle.normalized;

            // 팁: 서로 약간 밀어내면 더 자연스러움
            myRb.AddForce(randomDir * splitForce, ForceMode2D.Impulse);
            cloneRb.AddForce(-randomDir * splitForce, ForceMode2D.Impulse);
        }
    }

    // 세대에 따라 색을 바꿔주는 함수
    public void UpdateColor()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        // 0세대(빨강) -> 1세대(주황) -> ... -> 끝세대(보라) 
        // Color.HSVToRGB를 쓰면 무지개색으로 변환 가능!
        float hue = (float)currentGen / maxGen; // 0.0 ~ 1.0 사이 값
        sr.color = Color.HSVToRGB(hue, 1f, 1f);
    }
}