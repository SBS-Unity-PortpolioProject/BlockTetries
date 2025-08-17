using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;

    public int X => x;
    public int Y => y;
    // 내가 현재 점령당한 상태인지 => 블록이 배치된 상태인지
    public bool isOccupied;
    // 또는 public 프로퍼티를 추가
    public bool IsEmpty => !isOccupied;
    // Image 컴포넌트를 참조할 변수를 추가합니다.
    private Image slotImage;

    void Awake()
    {
        // Awake 시점에 Image 컴포넌트를 한 번만 가져옵니다.
        slotImage = GetComponent<Image>();
    }

    // 슬롯의 점유 상태를 설정하고, 이미지와 색상을 변경합니다.
    public void SetOccupied(bool occupied, Sprite sprite)
    {
        isOccupied = occupied;

        if (slotImage != null)
        {
            slotImage.sprite = sprite;
            // 슬롯이 점유되면 색상을 흰색으로, 점유되지 않으면 반투명하게 설정합니다.
            slotImage.color = occupied ? Color.white : new Color(1, 1, 1, 0.5f);
        }
    }

    // **새로운 메서드:**
    // 슬롯에 미리보기를 표시합니다. 점유 상태는 변경하지 않습니다.
    public void SetPreview(Sprite sprite, Color color)
    {
        if (!isOccupied && slotImage != null)
        {
            slotImage.sprite = sprite;
            slotImage.color = color;
        }
    }

    // **새로운 메서드:**
    // 슬롯의 미리보기를 지우고 원래의 비점유 상태로 되돌립니다.
    public void ClearPreview()
    {
        if (!isOccupied && slotImage != null)
        {
            slotImage.sprite = null;
            slotImage.color = new Color(1, 1, 1, 0.5f);
        }
    }
}
