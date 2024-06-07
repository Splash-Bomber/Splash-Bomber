using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileCounter : MonoBehaviour
{
    public Tilemap groundTiles; // 바닥 타일맵
    public TileBase tileBlue; // 타일 블루
    public TileBase tileRed; // 타일 레드
    public float gameTime = 60f; // 게임 시간 (초)
    public Text resultText; // 결과 텍스트 UI
    public Text timerText; // 타이머 텍스트 UI
    public Text blueTileCountText; // 블루 타일 개수 텍스트 UI
    public Text redTileCountText; // 레드 타일 개수 텍스트 UI

    private float timer;
    private bool gameEnded = false;

    private void Start()
    {
        timer = gameTime;
        resultText.gameObject.SetActive(false); // 결과 텍스트 숨기기
    }

    private void Update()
    {
        if (!gameEnded)
        {
            timer -= Time.deltaTime;

            // 타이머 텍스트 업데이트
            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                gameEnded = true;
                DetermineWinner();
            }
        }
    }

    private void OnEnable()
    {
        WaterBombController.OnTileChanged += UpdateTileCount;
        CountTiles(out int _, out int _);
    }

    private void OnDisable()
    {
        WaterBombController.OnTileChanged -= UpdateTileCount;
    }

    private void CountTiles(out int blueTileCount, out int redTileCount)
    {
        blueTileCount = 0;
        redTileCount = 0;

        foreach (var pos in groundTiles.cellBounds.allPositionsWithin)
        {
            TileBase tile = groundTiles.GetTile(pos);
            if (tile == tileBlue)
            {
                blueTileCount++;
            }
            else if (tile == tileRed)
            {
                redTileCount++;
            }
        }

        // 타일 개수 텍스트 업데이트
        blueTileCountText.text = blueTileCount.ToString();
        redTileCountText.text = redTileCount.ToString();

        Debug.Log($"Current Blue Tile Count: {blueTileCount}");
        Debug.Log($"Current Red Tile Count: {redTileCount}");
    }

    private void UpdateTileCount(TileBase newTile, Vector3Int position)
    {
        CountTiles(out int _, out int _); // 타일 개수만 업데이트, 결과는 무시
    }

    private void DetermineWinner()
    {
        CountTiles(out int blueTileCount, out int redTileCount);

        if (blueTileCount > redTileCount)
        {
            resultText.text = "Blue Wins!";
        }
        else if (redTileCount > blueTileCount)
        {
            resultText.text = "Red Wins!";
        }
        else
        {
            resultText.text = "It's a Tie!";
        }

        resultText.gameObject.SetActive(true); // 결과 텍스트 표시
        Debug.Log(resultText.text);
    }
}
