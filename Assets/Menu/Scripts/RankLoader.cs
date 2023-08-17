using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject RankDataprefab;
    [SerializeField]
    private Transform RankDataparent;


    private List<RankScore> rankDataList;

    private void Awake()
    {
        rankDataList = new List<RankScore>();

        for(int i = 0; i < 10; i++)
        {
            GameObject clone = Instantiate(RankDataprefab, RankDataparent);
            rankDataList.Add(clone.GetComponent<RankScore>());
        }
        Getranklist();
    }
    private void Getranklist()
    {
        
        for(int i = 0; i < 10; i++)
        {
            SetRankData(rankDataList[i], i + 1, "-", 0);
        }

        //�ؿ� api���� ������ ���� ����Ʈ �������� �޾Ƽ� ������ �� ���ö� api���� ��ŷ������ �����״� ����Ʈ�� �� �޾Ƽ� ������� rankDataList[i]�� ���� �Ѱ� �ָ� �ɵ�
    }
    private void SetRankData(RankScore rankscore, int rank, string name, int score)
    {
        rankscore.Rank = rank;
        rankscore.Name = name;
        rankscore.Score = score;
    }
}
