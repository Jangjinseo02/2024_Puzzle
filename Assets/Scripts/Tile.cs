using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    TextMeshPro textPro;
    int value = 2;

    public bool flag = false; // 합쳐졌나요?
    public bool onDestroy = false;

    public int row = 0;
    public int col = 0;

    public bool Flag { get { return flag; } set { flag = value; } }
    public bool OnDestroy { get { return onDestroy; } set { onDestroy = value; } }
    public int Row { get { return row; } }
    public int Col { get { return col; } }

    public int Value { get { return value; } }

    private void Awake()
    {
        textPro = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        textPro.text = value.ToString();
    }

    public void TilePosition(GameObject tile)
    {
        //if (transform.parent == tile) return; // 새롭게 세팅하는 위치가 기존과 같다면 반환

        gameObject.transform.SetParent(tile.transform, true);
        string[] name = tile.name.Split("_");
        row = int.Parse(name[1]);
        col = int.Parse(name[2]);

        gameObject.SetActive(true);
    }

    public void TileMoveMent(Action<bool> CallBackCompleteMethod)
    {
        StartCoroutine(MoveRoutine(CallBackCompleteMethod));
    }

    IEnumerator MoveRoutine(Action<bool> OnComplete)
    {
        bool isMove = false;
        float distance = Vector2.Distance(Vector2.zero, transform.localPosition); // 거리
        Vector2 dirVec = (Vector2.zero - (Vector2)transform.localPosition).normalized; // 방향 

        while (distance > 0.05f) // 거리가 일정 이상인 동안
        {
            if (!isMove) isMove = true;

            transform.localPosition = (Vector2)transform.localPosition + dirVec * 10 * Time.deltaTime; // 위치 이동
            distance = Vector2.Distance(Vector2.zero, transform.localPosition); // 남은 거리 변경
            
            yield return null;
        }

        transform.localPosition = Vector2.zero;

        yield return null;

        if (!onDestroy)
            textPro.text = value.ToString(); // 파괴 되지 않는 오브젝트인 경우 값 변경
        else
            gameObject.SetActive(false); // 파괴 되는 오브젝트인 경우 파괴

        flag = false;

        OnComplete?.Invoke(isMove); // if(CallBackCompleteMethod != null) CallBackCompleteMethod
    }

    public void MulValue()
    {
        this.value *= 2;
        flag = true;
    }
}
