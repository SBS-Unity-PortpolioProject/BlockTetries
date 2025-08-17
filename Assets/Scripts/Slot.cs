using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private int x;
    [SerializeField] private int y;

    public int X => x;
    public int Y => y;
    // ���� ���� ���ɴ��� �������� => ����� ��ġ�� ��������
    public bool isOccupied;
    // �Ǵ� public ������Ƽ�� �߰�
    public bool IsEmpty => !isOccupied;
    // Image ������Ʈ�� ������ ������ �߰��մϴ�.
    private Image slotImage;

    void Awake()
    {
        // Awake ������ Image ������Ʈ�� �� ���� �����ɴϴ�.
        slotImage = GetComponent<Image>();
    }

    // ������ ���� ���¸� �����ϰ�, �̹����� ������ �����մϴ�.
    public void SetOccupied(bool occupied, Sprite sprite)
    {
        isOccupied = occupied;

        if (slotImage != null)
        {
            slotImage.sprite = sprite;
            // ������ �����Ǹ� ������ �������, �������� ������ �������ϰ� �����մϴ�.
            slotImage.color = occupied ? Color.white : new Color(1, 1, 1, 0.5f);
        }
    }

    // **���ο� �޼���:**
    // ���Կ� �̸����⸦ ǥ���մϴ�. ���� ���´� �������� �ʽ��ϴ�.
    public void SetPreview(Sprite sprite, Color color)
    {
        if (!isOccupied && slotImage != null)
        {
            slotImage.sprite = sprite;
            slotImage.color = color;
        }
    }

    // **���ο� �޼���:**
    // ������ �̸����⸦ ����� ������ ������ ���·� �ǵ����ϴ�.
    public void ClearPreview()
    {
        if (!isOccupied && slotImage != null)
        {
            slotImage.sprite = null;
            slotImage.color = new Color(1, 1, 1, 0.5f);
        }
    }
}
