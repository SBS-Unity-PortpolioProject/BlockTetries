using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LayerLab.GUIScripts;

public class RandomParentActivationAndSprites : MonoBehaviour
{
    [Header("스프라이트 적용 대상 부모들")]
    [SerializeField] private List<Transform> parentTransforms = new List<Transform>();

    public Board board; // Board 스크립트 참조
    public PanelControl panelControl; // PanelControl 스크립트 참조

    [Header("Resources 폴더 내 스프라이트 이름 (확장자 제외)")]
    [SerializeField]
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
        // Resources 폴더에서 모든 스프라이트 로드
        foreach (var name in spriteNames)
        {
            Sprite sp = Resources.Load<Sprite>(name);
            if (sp != null) spriteList.Add(sp);
            else Debug.LogWarning($"Resources에서 '{name}' 스프라이트를 찾을 수 없습니다.");
        }
        if (spriteList.Count == 0)
            Debug.LogError("스프라이트가 하나도 로드되지 않았습니다.");

        // 부모 중 무작위 3개만 활성화, 나머지는 비활성화
        int total = parentTransforms.Count;
        if (total <= 3)
        {
            // 모두 활성화
            foreach (Transform p in parentTransforms) p.gameObject.SetActive(true);
        }
        else
        {
            // 인덱스 배열 생성 및 셔플
            List<int> idxs = Enumerable.Range(0, total).OrderBy(x => Random.value).ToList();
            HashSet<int> chosen = new HashSet<int>(idxs.Take(3));

            for (int i = 0; i < total; ++i)
            {
                parentTransforms[i].gameObject.SetActive(chosen.Contains(i));
            }
        }
    }

    void Start()
    {
        ApplyRandomSpritePerActiveParent();
    }

    [ContextMenu("Apply Random Sprite Per Active Parent")]
    public void ApplyRandomSpritePerActiveParent()
    {
        if (spriteList.Count == 0) return;

        foreach (Transform parent in parentTransforms)
        {
            if (!parent.gameObject.activeSelf)
                continue;

            // 활성된 부모마다 하나의 랜덤 스프라이트 선택
            Sprite chosen = spriteList[Random.Range(0, spriteList.Count)];

            // 자식 Image 모두 동일 스프라이트로 변경
            Image[] images = parent.GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
                img.sprite = chosen;

            Debug.Log($"{parent.name} 활성화 → 자식 {images.Length}개에 '{chosen.name}' 적용");
        }
    }

    // 기존의 AllChildrenDeactivated 메서드 수정
    private void AllChildrenDeactivated()
    {
        // 모든 자식이 비활성화된 경우, 게임 오버 조건을 확인합니다.

        // 보드의 CanAnyBlockBePlaced 메서드를 호출하여 배치 가능한 블록이 있는지 확인합니다.
        if (!board.CanAnyBlockBePlaced(GetActiveBlocks()))
        {
            // 배치 가능한 블록이 없는 경우, 게임 오버 패널을 활성화합니다.
            panelControl.GameOver();
        }
        else
        {
            // 배치 가능한 블록이 있다면 새로운 블록을 활성화합니다.
            // ActivateRandomParents();
        }
    }

    // 현재 활성화된 PuzzleBlock들을 리스트로 반환하는 메서드 추가
    private List<PuzzleBlock> GetActiveBlocks()
    {
        List<PuzzleBlock> activeBlocks = new List<PuzzleBlock>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                PuzzleBlock block = child.GetComponent<PuzzleBlock>();
                if (block != null)
                {
                    activeBlocks.Add(block);
                }
            }
        }
        return activeBlocks;
    }
}
