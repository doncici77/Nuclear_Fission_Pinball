using UnityEngine;

public class StressTester : MonoBehaviour
{
    // 게임의 상태 정의 (메뉴, 실행 중, 종료)
    enum GameState { Menu, Running, Ended }

    [Header("설정")]
    public float targetFPS = 10.0f;  // 이 프레임 밑으로 떨어지면 멈춤
    public float warmUpTime = 2.0f;  // 게임 시작 후 2초간은 측정 안 함 (안정화 시간)

    private float deltaTime = 0.0f;
    private float timeElapsed = 0f;  // 실행 후 지난 시간
    private GameState currentState = GameState.Menu; // 처음엔 메뉴 상태

    void Update()
    {
        // 게임 실행 중일 때만 프레임 체크
        if (currentState == GameState.Running)
        {
            // 1. 프레임 계산
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float currentFPS = 1.0f / deltaTime;

            // 2. 안정화 시간(WarmUp) 체크
            timeElapsed += Time.unscaledDeltaTime;

            // 3. 안정화 시간이 지났는데 프레임이 낮으면 종료
            if (timeElapsed > warmUpTime && currentFPS < targetFPS)
            {
                StopGame();
            }
        }
    }

    // 게임 시작 함수
    public void StartTest()
    {
        // 상태 변경
        currentState = GameState.Running;
        timeElapsed = 0f; // 타이머 초기화
        deltaTime = 0f;   // 프레임 계산 초기화

        // 시간 흐르게 하기 (혹시 멈춰있었다면)
        Time.timeScale = 1;

        StartShooter startShooter = GetComponent<StartShooter>();
        startShooter.ShootNeutron();
    }

    void StopGame()
    {
        currentState = GameState.Ended;
        Time.timeScale = 0; // 시간 정지
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;

        // --- 1. 메뉴 화면 (시작 전) ---
        if (currentState == GameState.Menu)
        {
            // 화면 중앙에 START 버튼 만들기
            if (GUI.Button(new Rect(w / 2 - 100, h / 2 - 50, 200, 100), "START TEST"))
            {
                StartTest();
            }
        }
        // --- 2. 게임 중 (프레임 표시) ---
        else if (currentState == GameState.Running)
        {
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 5 / 100;
            style.normal.textColor = Color.green;

            float fps = 1.0f / deltaTime;
            string text = "";

            if (timeElapsed < warmUpTime)
                text = $"Warming Up... ({warmUpTime - timeElapsed:0.0})"; // 준비 시간 표시
            else
                text = $"{fps:0.} FPS (Balls: {GameManager.Instance.ActiveNeutronCount})";

            GUI.Label(new Rect(20, 20, w, h), text, style);
        }
        // --- 3. 결과 화면 (종료) ---
        else if (currentState == GameState.Ended)
        {
            style.fontSize = h * 8 / 100;
            style.normal.textColor = Color.red;

            float fps = 1.0f / deltaTime;
            string resultText = $"LIMIT REACHED!\n\nFPS: {fps:0.}\nMax Balls: {GameManager.Instance.ActiveNeutronCount}";

            GUI.Label(new Rect(0, 0, w, h), resultText, style);

            // 재시작 버튼
            if (GUI.Button(new Rect(w / 2 - 100, h * 0.8f, 200, 80), "RESTART"))
            {
                // 씬을 다시 로드하거나, 초기화 로직 수행
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
        }
    }
}