using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBlock : MonoBehaviour
{
    // ���� ����� ���¸� �����ϴ� Vector2Int ����Ʈ
    [HideInInspector] public List<Vector2Int> shape;

    // Board�� ������ ���� ������ �Ǵ� Slot ��ǥ
    [HideInInspector] public Vector2Int anchorPosition;

    // Image ������Ʈ�� ������ ������ �߰��մϴ�.
    public Image imageComponent;

    private void Awake()
    {
        InitializeShapeFromSlots();
        imageComponent = GetComponentInChildren<Image>();
    }

    private void InitializeShapeFromSlots()
    {
        shape = new List<Vector2Int>();
        // �ڽ� ������Ʈ���� Slot.cs ������Ʈ�� ��� �����ɴϴ�.
        Slot[] childSlots = GetComponentsInChildren<Slot>();

        // anchor�� ����� �߽��� �Ǵ� ������ �������� ����ϴ�.
        // ���⼭�� (0,0) ��ǥ�� ���� ������ ��Ŀ�� �����մϴ�.
        // ���� ��Ŀ ������ ���ٸ� ù ��° ������ ��Ŀ�� ����մϴ�.
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
            Debug.LogError("PuzzleBlock�� Slot�� �����ϴ�.");
            return;
        }

        // ��Ŀ ������ �������� �ٸ� ���Ե��� ��� ��ǥ�� ����Ͽ� shape�� �߰��մϴ�.
        foreach (Slot slot in childSlots)
        {
            Vector2Int relativePosition = new Vector2Int(slot.X - anchorSlot.X, slot.Y - anchorSlot.Y);
            shape.Add(relativePosition);
        }
    }
}
