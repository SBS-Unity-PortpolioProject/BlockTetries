using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private List<Slot> slots;
    [SerializeField] private int boardWidth = 8;
    [SerializeField] private int boardHeight = 8;
    [SerializeField] private float slotSize = 125f;
    [SerializeField] private TextMeshProUGUI scoreText;

    private Slot[,] grid;
    private int score = 0;

    // PuzzleBlockManager 참조를 추가하여 블록 배치 후 알림을 보냅니다.
    public PuzzleBlockManager puzzleBlockManager;

    public float SlotSize => slotSize;
    public int BoardWidth => boardWidth;
    public int BoardHeight => boardHeight;

    private void Start()
    {
        InitializeGrid();
    }

    // 슬롯 리스트를 2D 그리드 배열로 초기화합니다.
    private void InitializeGrid()
    {
        grid = new Slot[boardWidth, boardHeight];
        foreach (Slot slot in slots)
        {
            if (slot.X >= 0 && slot.X < boardWidth && slot.Y >= 0 && slot.Y < boardHeight)
            {
                grid[slot.X, slot.Y] = slot;
            }
        }
    }

    // 특정 앵커 위치에 블록을 놓을 수 있는지 확인합니다.
    public bool CanPlace(PuzzleBlock block, Vector2Int anchorPosition)
    {
        // 앵커 위치를 기준으로 블록의 모든 슬롯이 유효한지 확인합니다.
        foreach (Vector2Int slotOffset in block.shape)
        {
            int x = anchorPosition.x + slotOffset.x;
            int y = anchorPosition.y + slotOffset.y;

            // 보드 범위를 벗어나는지 확인
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
            {
                return false;
            }

            // 이미 점유된 슬롯인지 확인
            if (grid[x, y].isOccupied)
            {
                return false;
            }
        }
        return true;
    }

    // 블록을 보드에 배치합니다.
    public void PlaceBlock(PuzzleBlock block, Vector2Int anchorPosition)
    {
        // **수정된 부분:**
        // 블록을 배치하기 전에 미리보기를 지웁니다.
        ClearPreview();

        // 앵커 위치를 기준으로 블록의 모든 슬롯을 점유 상태로 만듭니다.
        foreach (Vector2Int slotOffset in block.shape)
        {
            int x = anchorPosition.x + slotOffset.x;
            int y = anchorPosition.y + slotOffset.y;
            grid[x, y].SetOccupied(true, block.imageComponent.sprite);
        }

        block.anchorPosition = anchorPosition;

        // 배치 후 줄/열이 완성되었는지 확인합니다.
        CheckForClearedLines();

        // 블록 배치 후 퍼즐 블록 매니저에게 알립니다.
        if (puzzleBlockManager != null)
        {
            puzzleBlockManager.OnPuzzleBlockPlaced(block);
        }
    }

    // **새로운 메서드:**
    // 드래그 중인 블록의 미리보기를 표시합니다.
    public void ShowPreview(PuzzleBlock block, Vector2Int anchorPosition)
    {
        // 이전 프레임의 미리보기를 지웁니다.
        ClearPreview();

        // 앵커 위치를 기준으로 블록의 모든 슬롯에 미리보기를 표시합니다.
        foreach (Vector2Int slotOffset in block.shape)
        {
            int x = anchorPosition.x + slotOffset.x;
            int y = anchorPosition.y + slotOffset.y;

            // 보드 범위 내에 있고 점유되지 않은 슬롯에만 미리보기를 표시합니다.
            if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight && !grid[x, y].isOccupied)
            {
                // 반투명한 색상으로 미리보기를 설정합니다.
                grid[x, y].SetPreview(block.imageComponent.sprite, new Color(1, 1, 1, 0.5f));
            }
        }
    }

    // **새로운 메서드:**
    // 모든 미리보기를 지웁니다.
    public void ClearPreview()
    {
        foreach (Slot slot in grid)
        {
            if (slot != null && !slot.isOccupied)
            {
                slot.ClearPreview();
            }
        }
    }

    private void CheckForClearedLines()
    {
        int linesCleared = 0;
        // 줄(Row) 확인
        for (int y = 0; y < boardHeight; y++)
        {
            bool isFull = true;
            for (int x = 0; x < boardWidth; x++)
            {
                if (!grid[x, y].isOccupied)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull)
            {
                ClearRow(y);
                linesCleared++;
            }
        }

        // 열(Column) 확인
        for (int x = 0; x < boardWidth; x++)
        {
            bool isFull = true;
            for (int y = 0; y < boardHeight; y++)
            {
                if (!grid[x, y].isOccupied)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull)
            {
                ClearColumn(x);
                linesCleared++;
            }
        }

        if (linesCleared > 0)
        {
            UpdateScore(linesCleared * 100);
        }
    }

    // 특정 줄을 비웁니다.
    private void ClearRow(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            // 이전에 사용한 SetOccupied(false, null)를 그대로 사용하면 ClearPreview()를 호출하는 것과 같습니다.
            grid[x, y].SetOccupied(false, null);
        }
    }

    // 특정 열을 비웁니다.
    private void ClearColumn(int x)
    {
        for (int y = 0; y < boardHeight; y++)
        {
            grid[x, y].SetOccupied(false, null);
        }
    }

    private void UpdateScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }


    public bool CanAnyBlockBePlaced(List<PuzzleBlock> activeBlocks)
    {
        // 보드의 모든 슬롯을 순회합니다.
        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                // **수정된 부분: grid[x, y].isOccupied 사용**
                // 빈 슬롯(isOccupied가 false)을 찾습니다.
                if (!grid[x, y].isOccupied)
                {
                    // 활성화된 각 퍼즐 블록을 순회합니다.
                    foreach (PuzzleBlock block in activeBlocks)
                    {
                        // 현재 보드의 빈 슬롯 위치를 기준으로 블록을 놓을 수 있는지 확인합니다.
                        if (CanPlace(block, new Vector2Int(x, y)))
                        {
                            // 하나라도 배치 가능한 블록이 있다면 true를 반환하고 탐색을 종료합니다.
                            return true;
                        }
                    }
                }
            }
        }

        // 보드의 모든 위치에서 어떤 블록도 배치할 수 없는 경우 false를 반환합니다.
        return false;
    }
}
