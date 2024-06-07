using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileCounter : MonoBehaviour
{
    public Tilemap groundTiles; // �ٴ� Ÿ�ϸ�
    public TileBase tileBlue; // Ÿ�� ���
    public TileBase tileRed; // Ÿ�� ����
    public float gameTime = 60f; // ���� �ð� (��)
    public Text resultText; // ��� �ؽ�Ʈ UI
    public Text timerText; // Ÿ�̸� �ؽ�Ʈ UI
    public Text blueTileCountText; // ��� Ÿ�� ���� �ؽ�Ʈ UI
    public Text redTileCountText; // ���� Ÿ�� ���� �ؽ�Ʈ UI

    private float timer;
    private bool gameEnded = false;

    private void Start()
    {
        timer = gameTime;
        resultText.gameObject.SetActive(false); // ��� �ؽ�Ʈ �����
    }

    private void Update()
    {
        if (!gameEnded)
        {
            timer -= Time.deltaTime;

            // Ÿ�̸� �ؽ�Ʈ ������Ʈ
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

        // Ÿ�� ���� �ؽ�Ʈ ������Ʈ
        blueTileCountText.text = blueTileCount.ToString();
        redTileCountText.text = redTileCount.ToString();

        Debug.Log($"Current Blue Tile Count: {blueTileCount}");
        Debug.Log($"Current Red Tile Count: {redTileCount}");
    }

    private void UpdateTileCount(TileBase newTile, Vector3Int position)
    {
        CountTiles(out int _, out int _); // Ÿ�� ������ ������Ʈ, ����� ����
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

        resultText.gameObject.SetActive(true); // ��� �ؽ�Ʈ ǥ��
        Debug.Log(resultText.text);
    }
}
