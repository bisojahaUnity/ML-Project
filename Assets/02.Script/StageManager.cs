using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public List<GameObject> targetList = new List<GameObject>();

    public void InitStage()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].gameObject.SetActive(true);
        }
    }
}
