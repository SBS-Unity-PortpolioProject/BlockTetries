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

    // PuzzleBlockManager ������ �߰��Ͽ� ��� ��ġ �� �˸��� �����ϴ�.
    public PuzzleBlockManager puzzleBlockManager;

    public float SlotSize => slotSize;
    public int BoardWidth => boardWidth;
    public int BoardHeight => boardHeight;

    private void Start()
    {
        InitializeGrid();
    }

    // ���� ����Ʈ�� 2D �׸��� �迭�� �ʱ�ȭ�մϴ�.
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

    // Ư�� ��Ŀ ��ġ�� ����� ���� �� �ִ��� Ȯ���մϴ�.
    public bool CanPlace(PuzzleBlock block, Vector2Int anchorPosition)
    {
        // ��Ŀ ��ġ�� �������� ����� ��� ������ ��ȿ���� Ȯ���մϴ�.
        foreach (Vector2Int slotOffset in block.shape)
        {
            int x = anchorPosition.x + slotOffset.x;
            int y = anchorPosition.y + slotOffset.y;

            // ���� ������ ������� Ȯ��
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
            {
                return false;
            }

            // �̹� ������ �������� Ȯ��
            if (grid[x, y].isOccupied)
            {
                return false;
            }
        }
        return true;
    }

    // ����� ���忡 ��ġ�մϴ�.
    public void PlaceBlock(PuzzleBlock block, Vector2Int anchorPosition)
    {
        // **������ �κ�:**
        // ����� ��ġ�ϱ� ���� �̸����⸦ ����ϴ�.
        ClearPreview();

        // ��Ŀ ��ġ�� �������� ����� ��� ������ ���� ���·� ����ϴ�.
        foreach (Vector2Int slotOffset in block.shape)
        {
            int x = anchorPosition.x + slotOffset.x;
            int y = anchorPosition.y + slotOffset.y;
            grid[x, y].SetOccupied(true, block.imageComponent.sprite);
        }

        block.anchorPosition = anchorPosition;

        // ��ġ �� ��/���� �ϼ��Ǿ����� Ȯ���մϴ�.
        CheckForClearedLines();

        // ��� ��ġ �� ���� ��� �Ŵ������� �˸��ϴ�.
        if (puzzleBlockManager != null)
        {
            puzzleBlockManager.OnPuzzleBlockPlaced(block);
        }
    }

    // **���ο� �޼���:**
    // �巡�� ���� ����� �̸����⸦ ǥ���մϴ�.
    public void ShowPreview(PuzzleBlock block, Vector2Int anchorPosition)
    {
        // ���� �������� �̸����⸦ ����ϴ�.
        ClearPreview();

        // ��Ŀ ��ġ�� �������� ����� ��� ���Կ� �̸����⸦ ǥ���մϴ�.
        foreach (Vector2Int slotOffset in block.shape)
        {
            int x = anchorPosition.x + slotOffset.x;
            int y = anchorPosition.y + slotOffset.y;

            // ���� ���� ���� �ְ� �������� ���� ���Կ��� �̸����⸦ ǥ���մϴ�.
            if (x >= 0 && x < boardWidth && y >= 0 && y < boardHeight && !grid[x, y].isOccupied)
            {
                // �������� �������� �̸����⸦ �����մϴ�.
                grid[x, y].SetPreview(block.imageComponent.sprite, new Color(1, 1, 1, 0.5f));
            }
        }
    }

    // **���ο� �޼���:**
    // ��� �̸����⸦ ����ϴ�.
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
        // ��(Row) Ȯ��
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

        // ��(Column) Ȯ��
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

    // Ư�� ���� ���ϴ�.
    private void ClearRow(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            // ������ ����� SetOccupied(false, null)�� �״�� ����ϸ� ClearPreview()�� ȣ���ϴ� �Ͱ� �����ϴ�.
            grid[x, y].SetOccupied(false, null);
        }
    }

    // Ư�� ���� ���ϴ�.
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
        // ������ ��� ������ ��ȸ�մϴ�.
        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                // **������ �κ�: grid[x, y].isOccupied ���**
                // �� ����(isOccupied�� false)�� ã���ϴ�.
                if (!grid[x, y].isOccupied)
                {
                    // Ȱ��ȭ�� �� ���� ����� ��ȸ�մϴ�.
                    foreach (PuzzleBlock block in activeBlocks)
                    {
                        // ���� ������ �� ���� ��ġ�� �������� ����� ���� �� �ִ��� Ȯ���մϴ�.
                        if (CanPlace(block, new Vector2Int(x, y)))
                        {
                            // �ϳ��� ��ġ ������ ����� �ִٸ� true�� ��ȯ�ϰ� Ž���� �����մϴ�.
                            return true;
                        }
                    }
                }
            }
        }

        // ������ ��� ��ġ���� � ��ϵ� ��ġ�� �� ���� ��� false�� ��ȯ�մϴ�.
        return false;
    }
}
