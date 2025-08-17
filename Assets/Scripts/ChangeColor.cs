using UnityEngine;

public class SetChildrenGray : MonoBehaviour
{
    void Start()
    {
        // Board의 모든 자식들을 순회
        foreach (Transform child in transform)
        {
            // SpriteRenderer가 있다면 회색으로 설정
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.gray;
            }

            // UI용 Image 컴포넌트 처리 (선택사항)
            UnityEngine.UI.Image img = child.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                img.color = Color.gray;
            }
        }
    }
}