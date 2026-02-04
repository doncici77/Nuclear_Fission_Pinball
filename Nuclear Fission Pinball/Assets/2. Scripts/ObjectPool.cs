using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public struct PoolItem
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public bool shouldExpand; // 확장 가능 여부
    }

    public List<PoolItem> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // 빠른 검색을 위해 확장 여부와 프리팹을 담아둘 딕셔너리 추가
    private Dictionary<string, PoolItem> poolInfoDict;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        poolInfoDict = new Dictionary<string, PoolItem>(); // 초기화

        foreach (PoolItem pool in pools)
        {
            // 1. 정보 저장 (나중에 Find 안 쓰고 바로 찾기 위해)
            poolInfoDict.Add(pool.tag, pool);

            // 2. 오브젝트 미리 생성
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                CreateNewObject(pool.prefab, objectPool);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // 오브젝트 생성 및 설정 함수 (중복 제거)
    private GameObject CreateNewObject(GameObject prefab, Queue<GameObject> queue)
    {
        GameObject obj = Instantiate(prefab, transform); // 'transform'을 넣어 이 스크립트(GameManager)의 자식으로 둠
        obj.SetActive(false);
        queue.Enqueue(obj);
        return obj;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        // 1. 가장 오래된(큐 맨 앞) 녀석이 사용 중인지 확인
        if (poolDictionary[tag].Peek().activeSelf)
        {
            PoolItem item = poolInfoDict[tag]; // 리스트 탐색 없이 바로 접근 (O(1))

            // 2. 확장 모드라면?
            if (item.shouldExpand)
            {
                // 새 공을 만들어서 리턴 (큐에도 넣어줌)
                GameObject newObj = CreateNewObject(item.prefab, poolDictionary[tag]);

                newObj.SetActive(true);
                newObj.transform.position = position;
                newObj.transform.rotation = rotation;
                return newObj;
            }
            // 확장이 안 되면? -> 그냥 아래로 내려가서 기존 것 뺏어옴 (Steal)
        }

        // 3. 평상시 로직 (꺼내서 재사용)
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn); // 다시 줄 서기

        return objectToSpawn;
    }
}