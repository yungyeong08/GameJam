using TMPro;
using UnityEngine;

public class FruitGame : MonoBehaviour
{
    [Header("Timer Settings")]
    public float remainingTime = 60.0f;
    public TextMeshPro timerText;
    public bool isTimerRunning = true;

    [Header("Fruit Settings")]
    public GameObject[] fruitPerfabs;
    public float[] fruitSize = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };
    public float fruitStartHigh = 6.0f;

    [Header("Game Control")]
    public float gameWidth = 6.0f; // 인스펙터에서 이 값을 10 정도로 키워보세요.
    public bool isGameOver = false;
    public Camera mainCamera;

    public GameObject currentFruit;
    public int currentFruitType;
    private float fruitTimer;

    [Header("Score Settings")]
    public int score = 0;
    public TextMeshPro scoreText;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        UpdateScoreUI();
        SpawnnewFruit();
        fruitTimer = -3.0f;
    }

    void Update()
    {
        if (isGameOver) return; // 이미 게임오버라면 아래 로직 전부 무시

        if (isTimerRunning)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                UpdateTimerUI(); // 시간이 남았을 때만 UI 갱신
            }
            else
            {
                remainingTime = 0; // 0으로 정확히 고정
                UpdateTimerUI();   // 마지막으로 00:00 표시
                isTimerRunning = false;
                GameOver();
            }
        }

        // 2. 새로운 과일 스폰 타이머
        if (fruitTimer >= 0)
        {
            fruitTimer -= Time.deltaTime;
            if (fruitTimer < 0)
            {
                SpawnnewFruit();
                fruitTimer = -3.0f;
            }
        }

        // 3. 현재 잡고 있는 과일 이동 로직
        // MissingReferenceException 방지를 위해 Equals(null) 체크 추가
        if (currentFruit != null && !currentFruit.Equals(null))
        {
            Vector3 mousePosition = Input.mousePosition;

            // 핵심 수정: 카메라와의 거리를 Z에 넣어줘야 좌표 변환이 정확함
            mousePosition.z = -mainCamera.transform.position.z;

            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            // 이동 제한 계산
            float halfSize = fruitSize[currentFruitType] / 2f;
            float leftLimit = -gameWidth / 2f + halfSize;
            float rightLimit = gameWidth / 2f - halfSize;

            // X축 위치만 마우스 따라가기 (범위 제한 적용)
            float clampedX = Mathf.Clamp(worldPosition.x, leftLimit, rightLimit);
            currentFruit.transform.position = new Vector3(clampedX, fruitStartHigh, 0);
        }

        // 4. 클릭 시 과일 떨어뜨리기
        if (Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)
        {
            DropFruit();
        }
    }

    void SpawnnewFruit()
    {
        if (isGameOver) return;

        currentFruitType = Random.Range(0, 3);

        // 초기 위치 계산
        Vector3 spawnPosition = new Vector3(0, fruitStartHigh, 0);

        currentFruit = Instantiate(fruitPerfabs[currentFruitType], spawnPosition, Quaternion.identity);
        currentFruit.transform.localScale = new Vector3(fruitSize[currentFruitType], fruitSize[currentFruitType], 1);

        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0.0f;
            rb.linearVelocity = Vector2.zero; // 이전 물리 속도 초기화
        }
    }

    void DropFruit()
    {
        if (currentFruit == null) return;

        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.0f;
            currentFruit = null; // 참조 해제
            fruitTimer = 1.0f;   // 1초 뒤에 새 과일 생성
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if (isGameOver) return;

        // 1. 다음 단계 인덱스 계산
        int nextLevel = fruitType + 1;

        // 2. 만약 다음 레벨이 프리팹 배열 범위 안에 있다면 (합치기 가능)
        if (nextLevel < fruitPerfabs.Length)
        {
            // 다음 단계 과일 생성
            GameObject nextFruit = Instantiate(fruitPerfabs[nextLevel], position, Quaternion.identity);

            // 크기 설정 (사이즈 배열 개수도 꼭 맞춰주세요!)
            if (nextLevel < fruitSize.Length)
            {
                float nextSize = fruitSize[nextLevel];
                nextFruit.transform.localScale = new Vector3(nextSize, nextSize, 1.0f);
            }

            // 점수 추가
            AddScore(nextLevel * 10);
            Debug.Log(nextLevel + "단계 과일 생성 완료!");
        }
        else
        {
            // 3. 마지막 단계 과일끼리 합쳐졌을 때 (선택 사항)
            // 더 이상 생성할 과일이 없다면 점수만 크게 주고 끝내거나, 폭발 효과를 넣습니다.
            AddScore(1000);
            Debug.Log("최종 단계 과일이 합쳐졌습니다!");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void GameOver()
    {
        isGameOver = true;
        isTimerRunning = false;

        if (timerText != null) timerText.text = "00:00";

        Debug.Log("시간 초과! 프로그램을 종료합니다.");

        // 1. 실제 빌드된 파일(.exe 등)에서 게임을 종료할 때
        Application.Quit();

        // 2. 유니티 에디터에서 플레이 모드를 강제로 끌 때 (테스트용)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}