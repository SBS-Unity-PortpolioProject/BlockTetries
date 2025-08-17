using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpritePerParent : MonoBehaviour
{
    [Header("랜덤 이미지 적용 대상 부모들")]
    [SerializeField] private List<Transform> parentTransforms = new List<Transform>();

    [Header("Resources 폴더 내 스프라이트 이름 (확장자 제외)")]
    [SerializeField]
    // 배열에 담길 오브젝트들
    private string[] spriteNames = new string[]
    {
        "blockBlueDimond", "blockCreamDimond", "blockCrownDimond",
        "blockGreenDimond", "blockLightBlueDimond", "blockOrangeDimond",
        "blockPinkDimond", "blockPurepleDimond", "blockRedDimond",
        "blockYellowDimond"
    };

    private List<Sprite> spriteList = new List<Sprite>();

    void Awake()
    {
        // 모든 스프라이트 로드
        // spriteNames에 저장된 이미지이름들은 name이라는 var값에 저장후 순회한다
        foreach (string name in spriteNames)
        {
            // Resources에 담긴 Sprite이미지들을 불러오는 코드
            Sprite sp = Resources.Load<Sprite>(name);
            // 불러올 이미지가 있으면 그 이미지를 List에 추가함
            if (sp != null)
                spriteList.Add(sp);
            // 그게 아니라면 (sprite 랜덤이미지 이름) 스프라이트를 폴더에서 찾을수 없다고 오류 가 날시 글자 나오기
            else
                Debug.LogWarning($"'{name}' 스프라이트를 Resources 폴더에서 찾을 수 없습니다.");
        }
        //만약 spriteList 배열에 저장된 것이 아무것도 없다면 로딩된 스프라이트가 없다고 오류 출력하기
        if (spriteList.Count == 0)
            Debug.LogError("로딩된 스프라이트가 없습니다. Resources 폴더를 확인하세요.");
    }

    void Start()
    {
        //시작했을때 함수 시작하기
        ApplyRandomSpriteForEachParent();
    }

    
    public void ApplyRandomSpriteForEachParent()
    {
        // 만일 배열에 저장된것이 아무것도 없다면 밑에 함수를 실행하지 않기
        if (spriteList.Count == 0) return;

        // 랜덤이미지를 적용할 부모계층들을 parent라는 Transform값에 저장한다 그후
       // 부모마다 단 하나의 랜덤 스프라이트 선택하기
            Sprite chosen = spriteList[Random.Range(0, spriteList.Count)];        
        foreach (Transform parent in parentTransforms)
        {
            // 부모와 그 자식들 중 Image 컴포넌트 전부 가져오기
            Image[] images = parent.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                img.sprite = chosen;
            }
            // 적용된 부모계층의 이름과 적용된 이미지가 그 자식의이미지에 적용됬다는 메세지 출력
            Debug.Log($"{parent.name}의 {images.Length}개 자식 이미지에 '{chosen.name}' 적용됨");
        }
    }
}