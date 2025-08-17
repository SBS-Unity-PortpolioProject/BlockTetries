using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

// IBeginDragHandler, IDragHandler, IEndDragHandler 인터페이스를 구현하여 UI 드래그 앤 드롭 기능을 처리합니다.
public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Camera mainCamera;

    // Board 스크립트 참조를 Public으로 설정하여 Unity 에디터에서 할당할 수 있도록 합니다.
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

        // 보드 참조가 Editor에서 직접 할당되지 않았을 경우, 씬에서 찾습니다.
        if (board == null)
        {
            board = FindFirstObjectByType<Board>();
        }
    }

    // 드래그가 시작될 때 호출됩니다.
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 원래 위치와 부모 오브젝트를 저장합니다.
        originalPosition = rectTransform.position;
        originalParent = transform.parent;

        // 드래그하는 동안 Canvas를 부모로 설정하여 항상 최상단에 렌더링되도록 합니다.
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // 레이캐스트 타겟을 비활성화하여 드래그하는 동안 뒤에 있는 UI 요소와 상호작용하지 않도록 합니다.
        imageComponent.raycastTarget = false;
    }

    // 드래그 중 매 프레임마다 호출됩니다.
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

        // **새로 추가된 부분:**
        // 마우스의 위치를 기반으로 보드 위에서 블록이 놓일 수 있는 가장 가까운 앵커 위치를 계산합니다.
        Vector2Int nearestAnchor = GetNearestSlotPosition();

        // 이 앵커 위치를 기준으로 블록의 모든 슬롯 위치를 확인하여
        // 보드 위에서 블록을 놓을 수 있는지 확인합니다.
        if (board.CanPlace(puzzleBlock, nearestAnchor))
        {
            // 블록을 놓을 수 있는 경우, 보드에 미리보기를 표시하도록 요청합니다.
            board.ShowPreview(puzzleBlock, nearestAnchor);
        }
        else
        {
            // 블록을 놓을 수 없는 경우, 미리보기를 지웁니다.
            board.ClearPreview();
        }
    }

    // 드래그가 끝났을 때 호출됩니다.
    public void OnEndDrag(PointerEventData eventData)
    {
        // **수정된 부분:**
        // 드래그가 끝나면 미리보기를 반드시 지워줍니다.
        if (board != null)
        {
            board.ClearPreview();
        }

        imageComponent.raycastTarget = true;

        bool placed = false;
        Vector2Int validAnchor = Vector2Int.zero;

        if (board != null && puzzleBlock != null)
        {
           

            // 드롭된 위치에서 블록을 놓을 수 있는지 확인합니다.
            Vector2Int nearestAnchor = GetNearestSlotPosition();
            if (board.CanPlace(puzzleBlock, nearestAnchor))
            {
                // 마우스 위치에 가장 가까운 슬롯을 기준으로 탐색합니다.
                Vector2Int startPosition = GetNearestSlotPosition();

                // 블록의 모든 구성 슬롯을 기준으로 배치 가능 여부를 확인합니다.
                // 예를 들어, 2x2 블록의 경우, 4개의 앵커 포인트를 각각 확인합니다.
                foreach (Vector2Int blockSlotOffset in puzzleBlock.shape)
                {
                    // 테스트할 앵커 위치를 계산합니다.
                    Vector2Int testPosition = new Vector2Int(startPosition.x - blockSlotOffset.x, startPosition.y - blockSlotOffset.y);

                    // 해당 위치에 블록을 놓을 수 있는지 확인합니다.
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
                // 성공적으로 배치되면 Board 스크립트에 블록을 배치하도록 알립니다.
                board.PlaceBlock(puzzleBlock, validAnchor);

                // 블록을 배치한 후, 더 이상 필요 없으므로 이 오브젝트를 파괴합니다.
                // PuzzleBlockManager가 새 블록을 생성할 것입니다.
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Placement Failed. Returning to original position.");
                // 배치 실패 시 원래 부모와 위치로 되돌립니다.
                transform.SetParent(originalParent);
                rectTransform.position = originalPosition;
            }
        }
        else
        {
            Debug.LogWarning("Board 또는 PuzzleBlock 참조가 없습니다. 제자리로 돌아갑니다.");
            // 참조가 없을 때도 원래 부모와 위치로 되돌립니다.
            transform.SetParent(originalParent);
            rectTransform.position = originalPosition;
        }
    }

    // 마우스 위치에 가장 가까운 슬롯의 그리드 좌표를 계산합니다..
    private Vector2Int GetNearestSlotPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        // 마우스의 스크린 좌표를 보드의 로컬 좌표로 변환합니다.
        Vector2 localBoardPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            board.GetComponent<RectTransform>(),
            mousePosition,
            canvas.worldCamera,
            out localBoardPosition
        );

        // 마우스의 로컬 보드 좌표를 그리드 좌표로 변환합니다.
        // 보드의 중심이 (0,0)이라고 가정하고, 오프셋을 더해 0부터 시작하는 그리드 인덱스를 얻습니다.
        int x = Mathf.RoundToInt(localBoardPosition.x / board.SlotSize + (board.BoardWidth - 1) * 0.5f);
        int y = Mathf.RoundToInt(localBoardPosition.y / board.SlotSize + (board.BoardHeight - 1) * 0.5f);

        // 계산된 그리드 좌표가 보드 범위를 벗어나지 않도록 클램프(Clamp)합니다.
        x = Mathf.Clamp(x, 0, board.BoardWidth - 1);
        y = Mathf.Clamp(y, 0, board.BoardHeight - 1);

        return new Vector2Int(x, y);
    }
}
