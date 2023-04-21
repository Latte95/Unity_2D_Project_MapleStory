using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    // 오브젝트 풀링
    // 몬스터 종류 저장
    public GameObject[] enemyPrefabs;
    // 풀링할 몬스터를 전부 저장하는 배열
    private GameObject[] enemies;
    // 풀링 위치
    private Vector2 poolPosition = new Vector2(25f, 0);
    // 몬스터 종류 별 pool로 생성할 마리수
    public int[] count = new int[] { 3, 4, 5 };
    // 모든 몬스터 마리수
    public int totalCount = 0;

    // 몬스터 스폰 관련 변수
    // 스폰 최소, 최대 시간
    public float timeBetSpawnMin = 3f;
    public float timeBetSpawnMax = 7f;
    // 스폰 시간(랜덤)
    public float timeBetSpawn;
    // 스폰시킬 몬스터 인덱스
    private int currentIndex = 0;

    [SerializeField]
    private Transform SpawnTrans;

    void Awake()
    {
        // 반복문 속도 향상을 위한 캐싱
        int length = count.Length;

        // 몇마리 풀링할지 계산
        for (int i = 0; i < length; i++)
        {
            totalCount += count[i];
        }
        // 풀링할 몬스터 저장할 배열 크기 설정
        enemies = new GameObject[totalCount];

        // 앞의 몬스터를 몇마리 생성했는지 저장할 변수 => 새로 생성할 몬스터의 인덱스를 알기 위함
        int previouslyCnt = 0;
        // 몇번째 종류인지
        for (int i = 0; i < length; i++)
        {
            if (i > 0)
            {
                previouslyCnt += count[i - 1];
            }
            // 해당 종류의 몇번째 몬스터인지
            for (int j = 0; j < count[i]; j++)
            {
                enemies[previouslyCnt + j] = Instantiate(enemyPrefabs[i], poolPosition, Quaternion.identity);
                enemies[previouslyCnt + j].transform.SetParent(SpawnTrans, false);
                enemies[previouslyCnt + j].SetActive(false);
            }
        }
        timeBetSpawn = 0;
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemy_co());
    }

    private IEnumerator SpawnEnemy_co()
    {
        //yield return 
        // 지속적으로 new 키워드를 사용해서 생성하는 경우 모조리 다 가비지 수집에 대상이 된다.
        // 그래서 WaitForSeconds를 캐싱해서 사용한다.
        //WaitForSeconds wfs = new WaitForSeconds(timeBetSpawn); => 랜덤 불가능한듯하여 캐싱x

        // 스폰 시간을 랜덤하게 설정
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        while (true)
        {
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            // 스폰할 몬스터도 랜덤하게 설정
            currentIndex = Random.Range(0, totalCount);

            // 이미 스폰되지 않은 몬스터만 스폰할때
            // 모든 몬스터가 이미 스폰되어 있으면 무한루프에 빠지는걸 방지하기 위함
            for (int cnt = 0; cnt < totalCount; cnt++)
            {
                // 이미 스폰된 몬스터면
                if (enemies[currentIndex].activeInHierarchy)
                {
                    // 다른 몬스터를 스폰
                    currentIndex = Random.Range(0, totalCount);
                }
                // 아직 스폰되지 않은 몬스터일 경우 스폰시킴
                else
                {
                    // 활성화
                    enemies[currentIndex].SetActive(true);
                    // 위치설정
                    enemies[currentIndex].transform.position = transform.position;
                    break;
                }
            }
            yield return new WaitForSeconds(timeBetSpawn);
        }
    }
}
