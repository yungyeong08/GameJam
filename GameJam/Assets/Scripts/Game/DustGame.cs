using UnityEngine;

public class DustGame : MonoBehaviour
{
    [Header("Dust Settings")]
    public GameObject[] dustPrefabs; // fruitPerfabs -> dustPrefabs (오타 수정 포함)
    public float[] dustSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };

    [Header("Current Dust")]
    public GameObject currentDust;
    public int currentDustType;

    [Header("Game Control")]
    public float dustStartHigh = 6.0f;
    public float gameWidth = 6.0f;
    public bool isGameOver = false;
    public Camera mainCamera;
    public float dustTimer;

    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewDust();
        dustTimer = -3.0f;
    }

    void Update()
    {
        if (isGameOver) return;

        // 먼지 생성 쿨타임 계산
        if (dustTimer >= 0)
        {
            dustTimer -= Time.deltaTime;
        }

        if (dustTimer < 0 && dustTimer > -2)
        {
            SpawnNewDust();
            dustTimer = -3.0f;
        }

        // 현재 조작 중인 먼지가 있다면 마우스 위치로 이동
        if (currentDust != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 newPosition = currentDust.transform.position;
            newPosition.x = worldPosition.x;

            float halfDustSize = dustSizes[currentDustType] / 2f;

            // 화면 밖으로 나가지 않게 제한
            newPosition.x = Mathf.Clamp(newPosition.x, -gameWidth / 2f + halfDustSize, gameWidth / 2f - halfDustSize);

            currentDust.transform.position = newPosition;
        }

        // 클릭 시 먼지 떨어뜨리기
        if (Input.GetMouseButtonDown(0) && dustTimer == -3.0f)
        {
            DropDust();
        }
    }

    void SpawnNewDust()
    {
        if (!isGameOver)
        {
            // 0~2단계 먼지 중 랜덤 생성
            currentDustType = Random.Range(0, 3);

            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 spawnPosition = new Vector3(worldPosition.x, dustStartHigh, 0);
            float halfDustSize = dustSizes[currentDustType] / 2f;

            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfDustSize, gameWidth / 2 - halfDustSize);

            currentDust = Instantiate(dustPrefabs[currentDustType], spawnPosition, Quaternion.identity);
            currentDust.transform.localScale = new Vector3(dustSizes[currentDustType], dustSizes[currentDustType], 1);

            Rigidbody2D rb = currentDust.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0.0f; // 잡고 있을 때는 중력 0
            }
        }
    }

    void DropDust()
    {
        if (currentDust == null) return;

        Rigidbody2D rb = currentDust.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.0f; // 떨어뜨릴 때 중력 적용
            currentDust = null;
            dustTimer = 1.0f; // 다음 생성까지의 대기 시간
        }
    }

    // Dust.cs 스크립트에서 호출하는 병합 함수
    public void MergeDusts(int dustType, Vector3 position)
    {
        // 마지막 단계가 아닐 때만 다음 단계 먼지 생성
        if (dustType < dustPrefabs.Length - 1)
        {
            GameObject nextDust = Instantiate(dustPrefabs[dustType + 1], position, Quaternion.identity);
            float nextSize = dustSizes[dustType + 1];
            nextDust.transform.localScale = new Vector3(nextSize, nextSize, 1.0f);
        }
    }
}