using UnityEngine;

public class StartShooter : MonoBehaviour
{
    public GameObject neutronPrefab;
    public float shootForce = 10f;
    public Transform shootTransform;

    void Update()
    {
        // 시간이 멈춰있으면(메뉴 상태 or 결과 상태) 발사 금지
        if (Time.timeScale == 0) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            // 씬에 있는 모든 Neutron 태그 오브젝트를 찾아서 끔 (풀로 반환은 아님, 그냥 비활성화)
            // *주의: FindGameObjectsWithTag는 느리지만, 테스트용인 R키에는 괜찮음
            GameObject[] allBalls = GameObject.FindGameObjectsWithTag("Neutron");
            foreach (var ball in allBalls)
            {
                ball.SetActive(false);
                // 풀링 시스템 큐가 꼬이지 않게 하려면 사실 ObjectPool 쪽에 'ResetAll' 함수를 만드는 게 정석입니다.
                // 하지만 지금은 간단히 끄기만 해도, 풀링 시스템이 "어? 큐에 없네?" 하고 새로 만들거나 할 테니
                // 당장 테스트엔 문제없습니다.
            }
        }
    }

    public void ShootNeutron()
    {
        Vector2 createPos = new Vector2(shootTransform.position.x, shootTransform.position.y);

        GameObject ball = Instantiate(neutronPrefab, createPos, Quaternion.identity);

        // 마우스 위치에서 랜덤 방향으로 발사
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        ball.GetComponent<Rigidbody2D>().AddForce(randomDir * shootForce, ForceMode2D.Impulse);
    }   
}
