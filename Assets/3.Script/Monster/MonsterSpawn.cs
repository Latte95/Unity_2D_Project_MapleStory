using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    // ������Ʈ Ǯ��
    // ���� ���� ����
    public GameObject[] enemyPrefabs;
    // Ǯ���� ���͸� ���� �����ϴ� �迭
    private GameObject[] enemies;
    // Ǯ�� ��ġ
    private Vector2 poolPosition = new Vector2(25f, 0);
    // ���� ���� �� pool�� ������ ������
    public int[] count = new int[] { 3, 4, 5 };
    // ��� ���� ������
    public int totalCount = 0;

    // ���� ���� ���� ����
    // ���� �ּ�, �ִ� �ð�
    public float timeBetSpawnMin = 3f;
    public float timeBetSpawnMax = 7f;
    // ���� �ð�(����)
    public float timeBetSpawn;
    // ������ų ���� �ε���
    private int currentIndex = 0;

    [SerializeField]
    private Transform SpawnTrans;

    void Awake()
    {
        // �ݺ��� �ӵ� ����� ���� ĳ��
        int length = count.Length;

        // ��� Ǯ������ ���
        for (int i = 0; i < length; i++)
        {
            totalCount += count[i];
        }
        // Ǯ���� ���� ������ �迭 ũ�� ����
        enemies = new GameObject[totalCount];

        // ���� ���͸� ��� �����ߴ��� ������ ���� => ���� ������ ������ �ε����� �˱� ����
        int previouslyCnt = 0;
        // ���° ��������
        for (int i = 0; i < length; i++)
        {
            if (i > 0)
            {
                previouslyCnt += count[i - 1];
            }
            // �ش� ������ ���° ��������
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
        // ���������� new Ű���带 ����ؼ� �����ϴ� ��� ������ �� ������ ������ ����� �ȴ�.
        // �׷��� WaitForSeconds�� ĳ���ؼ� ����Ѵ�.
        //WaitForSeconds wfs = new WaitForSeconds(timeBetSpawn); => ���� �Ұ����ѵ��Ͽ� ĳ��x

        // ���� �ð��� �����ϰ� ����
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        while (true)
        {
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            // ������ ���͵� �����ϰ� ����
            currentIndex = Random.Range(0, totalCount);

            // �̹� �������� ���� ���͸� �����Ҷ�
            // ��� ���Ͱ� �̹� �����Ǿ� ������ ���ѷ����� �����°� �����ϱ� ����
            for (int cnt = 0; cnt < totalCount; cnt++)
            {
                // �̹� ������ ���͸�
                if (enemies[currentIndex].activeInHierarchy)
                {
                    // �ٸ� ���͸� ����
                    currentIndex = Random.Range(0, totalCount);
                }
                // ���� �������� ���� ������ ��� ������Ŵ
                else
                {
                    // Ȱ��ȭ
                    enemies[currentIndex].SetActive(true);
                    // ��ġ����
                    enemies[currentIndex].transform.position = transform.position;
                    break;
                }
            }
            yield return new WaitForSeconds(timeBetSpawn);
        }
    }
}
