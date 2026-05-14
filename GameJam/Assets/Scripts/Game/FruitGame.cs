using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class FruitGame : MonoBehaviour
{

    public GameObject[] fruitPerfabs;
    public float[] fruitSize = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };

    public GameObject currentFruit;
    public int currentFruitType;

    public float fruitStartHigh = 6.0f;
    public float gameWidth = 6.0f;
    public bool isGameOver = false;
    public Camera mainCamera;

    public float fruitTimer;

    public int score = 0;
    public TextMeshPro scoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        SpawnnewFruit();
        fruitTimer = -3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        if (fruitTimer >= 0)
        {
            fruitTimer -= Time.deltaTime;
        }

        if (fruitTimer < 0 && fruitTimer >-2)
        {
            SpawnnewFruit();
            fruitTimer = -3.0f;
        }

        if (currentFruit != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 newPosition = currentFruit.transform.position;

            newPosition.x = worldPosition.x;

            float halFruitSize = fruitSize[currentFruitType] / 2f;

            if(newPosition.x < -gameWidth / 2 - halFruitSize)
            {
                newPosition.x = -gameWidth / 2 - halFruitSize;
            }

            if (newPosition.x > gameWidth / 2 + halFruitSize)
            {
                newPosition.x = gameWidth / 2 + halFruitSize;
            }

            currentFruit.transform.position = newPosition;
        }

        if (Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)
        {
            DropFruit();
        }

    }


    void SpawnnewFruit()
    {
        if (!isGameOver)
        {

            currentFruitType = Random.Range(0, 3);

            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 spawnPosition = new Vector3(worldPosition.x, fruitStartHigh, 0);

            float halfFruitSize = fruitSize[currentFruitType] / 2f;

            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 - halfFruitSize);

            currentFruit = Instantiate(fruitPerfabs[currentFruitType], spawnPosition, Quaternion.identity);
            currentFruit.transform.localScale = new Vector3(fruitSize[currentFruitType], fruitSize[currentFruitType], 1);

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.gravityScale = 0.0f;
            }

        }

    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1.0f;
            currentFruit = null;
            fruitTimer = 1.0f;
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if (fruitType < fruitPerfabs.Length - 1)
        {
            // 합쳐질 때 점수 추가 (과일 타입에 따라 더 높은 점수를 줄 수도 있음)
            AddScore((fruitType + 1) * 10);

            GameObject newFruit = Instantiate(fruitPerfabs[fruitType + 1], position, Quaternion.identity);
            newFruit.transform.localScale = new Vector3(fruitSize[fruitType + 1], fruitSize[fruitType + 1], 1.0f);
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
        {
            scoreText.text = "Score: " + score;
        }
    }

}