using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

// IBeginDragHandler, IDragHandler, IEndDragHandler �������̽��� �����Ͽ� UI �巡�� �� ��� ����� ó���մϴ�.
public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Camera mainCamera;

    // Board ��ũ��Ʈ ������ Public���� �����Ͽ� Unity �����Ϳ��� �Ҵ��� �� �ֵ��� �մϴ�.
    public Board board;
    private PuzzleBlock puzzleBlock;
    public Image imageComponent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        puzzleBlock = GetComponent<PuzzleBlock>();
        imageComponent = GetComponentInChildren<Image>();

        // ���� ������ Editor���� ���� �Ҵ���� �ʾ��� ���, ������ ã���ϴ�.
        if (board == null)
        {
            board = FindFirstObjectByType<Board>();
        }
    }

    // �巡�װ� ���۵� �� ȣ��˴ϴ�.
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ���� ��ġ�� �θ� ������Ʈ�� �����մϴ�.
        originalPosition = rectTransform.position;
        originalParent = transform.parent;

        // �巡���ϴ� ���� Canvas�� �θ�� �����Ͽ� �׻� �ֻ�ܿ� �������ǵ��� �մϴ�.
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // ����ĳ��Ʈ Ÿ���� ��Ȱ��ȭ�Ͽ� �巡���ϴ� ���� �ڿ� �ִ� UI ��ҿ� ��ȣ�ۿ����� �ʵ��� �մϴ�.
        imageComponent.raycastTarget = false;
    }

    // �巡�� �� �� �����Ӹ��� ȣ��˴ϴ�.
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint
        );
        rectTransform.localPosition = localPoint;

        // **���� �߰��� �κ�:**
        // ���콺�� ��ġ�� ������� ���� ������ ����� ���� �� �ִ� ���� ����� ��Ŀ ��ġ�� ����մϴ�.
        Vector2Int nearestAnchor = GetNearestSlotPosition();

        // �� ��Ŀ ��ġ�� �������� ����� ��� ���� ��ġ�� Ȯ���Ͽ�
        // ���� ������ ����� ���� �� �ִ��� Ȯ���մϴ�.
        if (board.CanPlace(puzzleBlock, nearestAnchor))
        {
            // ����� ���� �� �ִ� ���, ���忡 �̸����⸦ ǥ���ϵ��� ��û�մϴ�.
            board.ShowPreview(puzzleBlock, nearestAnchor);
        }
        else
        {
            // ����� ���� �� ���� ���, �̸����⸦ ����ϴ�.
            board.ClearPreview();
        }
    }

    // �巡�װ� ������ �� ȣ��˴ϴ�.
    public void OnEndDrag(PointerEventData eventData)
    {
        // **������ �κ�:**
        // �巡�װ� ������ �̸����⸦ �ݵ�� �����ݴϴ�.
        if (board != null)
        {
            board.ClearPreview();
        }

        imageComponent.raycastTarget = true;

        bool placed = false;
        Vector2Int validAnchor = Vector2Int.zero;

        if (board != null && puzzleBlock != null)
        {
           

            // ��ӵ� ��ġ���� ����� ���� �� �ִ��� Ȯ���մϴ�.
            Vector2Int nearestAnchor = GetNearestSlotPosition();
            if (board.CanPlace(puzzleBlock, nearestAnchor))
            {
                // ���콺 ��ġ�� ���� ����� ������ �������� Ž���մϴ�.
                Vector2Int startPosition = GetNearestSlotPosition();

                // ����� ��� ���� ������ �������� ��ġ ���� ���θ� Ȯ���մϴ�.
                // ���� ���, 2x2 ����� ���, 4���� ��Ŀ ����Ʈ�� ���� Ȯ���մϴ�.
                foreach (Vector2Int blockSlotOffset in puzzleBlock.shape)
                {
                    // �׽�Ʈ�� ��Ŀ ��ġ�� ����մϴ�.
                    Vector2Int testPosition = new Vector2Int(startPosition.x - blockSlotOffset.x, startPosition.y - blockSlotOffset.y);

                    // �ش� ��ġ�� ����� ���� �� �ִ��� Ȯ���մϴ�.
                    if (board.CanPlace(puzzleBlock, testPosition))
                    {
                        validAnchor = testPosition;
                        placed = true;
                        break;
                    }
                }

            }

            if (placed)
            {
                Debug.Log($"Placement Successful at Anchor: {validAnchor}");
                // ���������� ��ġ�Ǹ� Board ��ũ��Ʈ�� ����� ��ġ�ϵ��� �˸��ϴ�.
                board.PlaceBlock(puzzleBlock, validAnchor);

                // ����� ��ġ�� ��, �� �̻� �ʿ� �����Ƿ� �� ������Ʈ�� �ı��մϴ�.
                // PuzzleBlockManager�� �� ����� ������ ���Դϴ�.
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Placement Failed. Returning to original position.");
                // ��ġ ���� �� ���� �θ�� ��ġ�� �ǵ����ϴ�.
                transform.SetParent(originalParent);
                rectTransform.position = originalPosition;
            }
        }
        else
        {
            Debug.LogWarning("Board �Ǵ� PuzzleBlock ������ �����ϴ�. ���ڸ��� ���ư��ϴ�.");
            // ������ ���� ���� ���� �θ�� ��ġ�� �ǵ����ϴ�.
            transform.SetParent(originalParent);
            rectTransform.position = originalPosition;
        }
    }

    // ���콺 ��ġ�� ���� ����� ������ �׸��� ��ǥ�� ����մϴ�..
    private Vector2Int GetNearestSlotPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        // ���콺�� ��ũ�� ��ǥ�� ������ ���� ��ǥ�� ��ȯ�մϴ�.
        Vector2 localBoardPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            board.GetComponent<RectTransform>(),
            mousePosition,
            canvas.worldCamera,
            out localBoardPosition
        );

        // ���콺�� ���� ���� ��ǥ�� �׸��� ��ǥ�� ��ȯ�մϴ�.
        // ������ �߽��� (0,0)�̶�� �����ϰ�, �������� ���� 0���� �����ϴ� �׸��� �ε����� ����ϴ�.
        int x = Mathf.RoundToInt(localBoardPosition.x / board.SlotSize + (board.BoardWidth - 1) * 0.5f);
        int y = Mathf.RoundToInt(localBoardPosition.y / board.SlotSize + (board.BoardHeight - 1) * 0.5f);

        // ���� �׸��� ��ǥ�� ���� ������ ����� �ʵ��� Ŭ����(Clamp)�մϴ�.
        x = Mathf.Clamp(x, 0, board.BoardWidth - 1);
        y = Mathf.Clamp(y, 0, board.BoardHeight - 1);

        return new Vector2Int(x, y);
    }
}
