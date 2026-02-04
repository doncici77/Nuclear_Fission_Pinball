using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [Header("설정")]
    public GameObject neutronPrefab; // 프리팹 직접 연결
    public int initialSize = 1000;   // 초기 개수

    // 딕셔너리 삭제 -> 그냥 큐 하나만 사용
    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewNeutron();
        }
    }

    // 새 공을 만들어서 큐에 넣는 함수
    private GameObject CreateNewNeutron()
    {
        GameObject obj = Instantiate(neutronPrefab, transform);
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
        return obj;
    }

    // 이름(tag) 검색 없이 바로 가져오는 함수
    public GameObject GetNeutron(Vector3 position, Quaternion rotation)
    {
        // 1. 큐의 맨 앞 녀석이 이미 켜져 있다? -> 풀이 꽉 찼다는 뜻
        if (poolQueue.Count > 0 && poolQueue.Peek().activeSelf)
        {
            // 무조건 확장 (무한 모드니까)
            GameObject newObj = CreateNewNeutron();

            newObj.SetActive(true);
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;

            // 큐 뒤로 보냄 (순환)
            poolQueue.Enqueue(newObj);
            return newObj;
        }

        // 2. 평상시 (꺼져있는 녀석 재사용)
        GameObject obj = poolQueue.Dequeue();

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        poolQueue.Enqueue(obj); // 다시 줄 서기

        return obj;
    }
}