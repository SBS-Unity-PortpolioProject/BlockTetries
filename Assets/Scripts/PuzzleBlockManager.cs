using UnityEngine;
using System.Collections.Generic;

public class PuzzleBlockManager : MonoBehaviour
{
    public List<PuzzleBlock> puzzlePrefabs;
    public Transform position1;
    public Transform position2;
    public Transform position3;
    public Board board;
    public GameObject gameOverPanel; // 유니티에서 할당할 게임 오버 패널

    private List<PuzzleBlock> currentBlocks = new List<PuzzleBlock>();

    void Start()
    {
        GenerateInitialPuzzleBlocks();
    }

    // 게임 시작 시 3개의 블록을 생성하는 메서드
    public void GenerateInitialPuzzleBlocks()
    {
        currentBlocks.Clear();
        GenerateNewBlocks(3);
    }

    // 블록이 배치된 후 호출되는 메서드
    public void OnPuzzleBlockPlaced(PuzzleBlock placedBlock)
    {
        // 배치된 블록을 리스트에서 제거
        currentBlocks.Remove(placedBlock);

        // 만약 모든 블록이 다 배치되었으면 (리스트가 비었으면)
        // 새로운 블록을 생성하기 전에 게임 오버를 확인
        if (!board.CanAnyBlockBePlaced(GetActiveBlocks()))
        {
            // 배치 가능한 블록이 없는 경우, 게임 오버 패널 활성화
            gameOverPanel.SetActive(true);
            // 게임 일시 정지 (선택 사항)
            Time.timeScale = 0;
        }
        else
        {
            // 게임 오버가 아니면 새로운 블록을 생성
            GenerateNewBlocks(3);
        }
    }

    // 현재 활성화된 PuzzleBlock들을 리스트로 반환하는 메서드 추가
    private List<PuzzleBlock> GetActiveBlocks()
    {
        List<PuzzleBlock> activeBlocks = new List<PuzzleBlock>();
        foreach (PuzzleBlock block in currentBlocks)
        {
            if (block.gameObject.activeSelf)
            {
                activeBlocks.Add(block);
            }
        }
        return activeBlocks;
    }

    private void GenerateNewBlocks(int count)
    {
        List<Transform> targetPositions = new List<Transform> { position1, position2, position3 };

        if (puzzlePrefabs.Count < count)
        {
            Debug.LogWarning("퍼즐 프리팹이 충분하지 않습니다.");
            return;
        }

        List<PuzzleBlock> selectedPrefabs = new List<PuzzleBlock>();
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, puzzlePrefabs.Count);
            PuzzleBlock selectedPrefab = puzzlePrefabs[randomIndex];
            selectedPrefabs.Add(selectedPrefab);
        }

        foreach (var block in selectedPrefabs)
        {
            // 현재 비어있는 위치를 찾아 블록 배치
            for (int i = 0; i < 3; i++)
            {
                if (targetPositions[i].childCount == 0)
                {
                    PuzzleBlock newBlock = Instantiate(block, targetPositions[i].position, Quaternion.identity, targetPositions[i]);
                    currentBlocks.Add(newBlock);

                    DragDropHandler handler = newBlock.GetComponent<DragDropHandler>();
                    if (handler != null)
                    {
                        handler.board = board;
                    }
                    break;
                }
            }
        }
    }
}