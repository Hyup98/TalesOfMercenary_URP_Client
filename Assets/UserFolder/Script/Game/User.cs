using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User 
{
    private string name;
    private string userId;
    private int ratingPoint;

    public User(string id)
    {
        //������ ����Ͽ� �� �����͸� �޾ƿ�
    }

    string GetName()
    {
        return name;
    }

    string GetUserId()
    {
        return userId;
    }

    int GetRatingPoint()
    {
        return ratingPoint;
    }
}
