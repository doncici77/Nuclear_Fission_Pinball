using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 현재 화면에 살아있는 공의 개수 (읽기 전용, 쓰기는 내부에서만)
    public int ActiveNeutronCount { get; private set; } = 0;

    void Awake()
    {
        Instance = this;
    }

    // 공이 태어날 때 호출
    public void RegisterNeutron()
    {
        ActiveNeutronCount++;
    }

    // 공이 죽을 때 호출
    public void UnregisterNeutron()
    {
        ActiveNeutronCount--;
    }
}