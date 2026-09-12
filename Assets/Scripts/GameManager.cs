using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] InputManager inputMgr;
    [SerializeField] BoardManager boardMgr;
    [SerializeField] PoolManager poolMgr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputMgr.OnDrag += boardMgr.Drag;
        boardMgr.OnCreate += poolMgr.CreateTile;

        // 초기 블록 설정
        boardMgr.RandomCreate();
        boardMgr.RandomCreate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
