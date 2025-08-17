using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBlock : MonoBehaviour
{
    // 퍼즐 블록의 형태를 정의하는 Vector2Int 리스트
    [HideInInspector] public List<Vector2Int> shape;

    // Board에 놓였을 때의 기준이 되는 Slot 좌표
    [HideInInspector] public Vector2Int anchorPosition;

    // Image 컴포넌트를 참조할 변수를 추가합니다.
    public Image imageComponent;

    private void Awake()
    {
        InitializeShapeFromSlots();
        imageComponent = GetComponentInChildren<Image>();
    }

    private void InitializeShapeFromSlots()
    {
        shape = new List<Vector2Int>();
        // 자식 오브젝트에서 Slot.cs 컴포넌트를 모두 가져옵니다.
        Slot[] childSlots = GetComponentsInChildren<Slot>();

        // anchor는 블록의 중심이 되는 슬롯을 기준으로 삼습니다.
        // 여기서는 (0,0) 좌표를 가진 슬롯을 앵커로 가정합니다.
        // 만약 앵커 슬롯이 없다면 첫 번째 슬롯을 앵커로 사용합니다.
        Slot anchorSlot = null;
        if (childSlots.Length > 0)
        {
            foreach (Slot slot in childSlots)
            {
                if (slot.X == 0 && slot.X == 0)
                {
                    anchorSlot = slot;
                    break;
                }
            }
            if (anchorSlot == null)
            {
                anchorSlot = childSlots[0];
            }
        }

        if (anchorSlot == null)
        {
            Debug.LogError("PuzzleBlock에 Slot이 없습니다.");
            return;
        }

        // 앵커 슬롯을 기준으로 다른 슬롯들의 상대 좌표를 계산하여 shape에 추가합니다.
        foreach (Slot slot in childSlots)
        {
            Vector2Int relativePosition = new Vector2Int(slot.X - anchorSlot.X, slot.Y - anchorSlot.Y);
            shape.Add(relativePosition);
        }
    }
}
