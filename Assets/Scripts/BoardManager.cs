using UnityEngine;
using System;

public class BoardManager : MonoBehaviour
{
    static int maxRow = 5, maxCol = 5;

    public GameObject[] backGroundTiles; // background tile
    private GameObject[,] tiles = new GameObject[maxRow, maxCol]; // move text tiles
    private Tile[] destroyTiles = new Tile[maxRow * maxCol];

    private int destroyCount = 0;

    bool boardUpdating = false;
    int updateCount = 0;

    public Func<GameObject> OnCreate;

    public bool IsUpdating { get { return boardUpdating || updateCount > 0; } }

    public void Request(bool isMove)
    {
        updateCount -= 1;

        if (!isMove) return;

        if (updateCount <= 0) RandomCreate();
    }

    public void CreateTileObject(Vector2Int vec)
    {
        int row = vec.x, col = vec.y;
        GameObject parent = backGroundTiles[row * maxCol + col]; // maxCol = 5 따라서, 25개의 값이 쭉 나열 될 때 5개의 값마다 다음 행으로 진행

        tiles[row, col] = OnCreate?.Invoke(); // 타일 생성 및 부모 위치로 이동
        tiles[row, col].GetComponent<Tile>().TilePosition(parent); // 타일 데이터 설정 ( 부모 오브젝트 하위로 이동, row col 값 저장 )
    }

    public void RandomCreate()
    {
        //최대 5 * 5, 25회 반복 중 한 번 생성되면 종료
        for (int i = 0; i < maxRow * maxCol; i++)
        {
            int ranRow = UnityEngine.Random.Range(0, maxRow - 1), ranCol = UnityEngine.Random.Range(0, maxCol - 1);
            if (tiles[ranRow, ranCol] != null) continue;

            CreateTileObject(new Vector2Int(ranRow, ranCol));
            break;
        }
    }

    void BoardUpdate()
    {
        boardUpdating = true;

        for (int i = 0; i < maxRow; i++)
            for (int j = 0; j < maxCol; j++)
                if (tiles[i, j] != null)
                    MoveTile(i, j);

        foreach (Tile tile in destroyTiles)
            if (tile != null) MoveTile(tile);

        System.Array.Clear(destroyTiles, 0, destroyTiles.Length);
        destroyCount = 0;
        boardUpdating = false;
    }

    // 게임 ui 상 오브젝트 이동
    void MoveTile(int row, int col)
    {
        updateCount += 1;

        GameObject parent = backGroundTiles[row * maxCol + col];
        tiles[row, col].GetComponent<Tile>().TilePosition(parent);
        tiles[row, col].GetComponent<Tile>().TileMoveMent(Request);
    }

    void MoveTile(Tile tile)
    {
        updateCount += 1;

        tile.TileMoveMent(Request);
    }

    // 오른쪽 이동
    void MoveRight()
    {
        //오른쪽 이동
        for (int i = 0; i < maxRow; i++)
        {
            for (int j = maxCol - 1; j >= 0; j--)
            {
                int col = TryMove(i, j, 0, 1);
                if (col == -1) continue;

                TryMerge(i, col, i, col + 1);
            }
        }
    }

    // 왼쪽 이동
    void MoveLeft()
    {
        //왼쪽 이동
        for (int i = 0; i < maxRow; i++)
        {
            for (int j = 0; j < maxCol; j++)
            {
                int col = TryMove(i, j, 0, -1);
                if (col == -1) continue;

                TryMerge(i, col, i, col - 1);
            }
        }
    }

    // 위쪽 이동
    void MoveUp()
    {
        //정방향( row - 1 >= 0 이어야 배열 범위를 벗어나지 않음 )
        for (int i = 0; i < maxRow; i++)
        {
            //모든 열을 검색 값이 있다면 (위로 이동)올려야함
            for (int j = 0; j < maxCol; j++)
            {
                int row = TryMove(i, j, -1, 0);
                if (row == -1) continue;

                TryMerge(row, j, row - 1, j);
            }
        }
    }

    // 아래쪽 이동
    void MoveDown()
    {
        //역방향( row + 1 < 5 이어야 배열 범위를 벗어나지 않음, 또한 마지막 위치에 값이 있는 경우는 계산할 필요가 없음 )
        for (int i = maxRow - 1; i >= 0; i--)
        {
            //모든 열을 검색 값이 있다면 (아래로 이동)내려야함
            for (int j = 0; j < maxCol; j++)
            {
                int row = TryMove(i, j, 1, 0);

                if (row == -1) continue;
                TryMerge(row, j, row + 1, j);
            }
        }
    }

    int TryMove(int curRow, int curCol, int dirRow, int dirCol)
    {
        // 타일이 없다면 반환
        if (tiles[curRow, curCol] == null) return -1;

        int targetRow = curRow, targetCol = curCol, returnValue = -1;

        if (dirRow != 0) // 입력된 값이 row 라면
        {
            // 다음 위치가 빈 경우 계속 다음 순서 위치로 넘어감
            while (targetRow + dirRow >= 0 && targetRow + dirRow < maxRow && tiles[targetRow + dirRow, curCol] == null)
                targetRow += dirRow;

            if (targetRow != curRow) // 체크한 위치가 이전 위치와 다른 경우
            {
                tiles[targetRow, curCol] = tiles[curRow, curCol];
                tiles[curRow, curCol] = null;
            }
            returnValue = targetRow;
        }
        else if (dirCol != 0) // 입력된 값이 col 라면
        {
            // 다음 위치가 빈 경우 계속 다음 순서 위치로 넘어감
            while (targetCol + dirCol >= 0 && targetCol + dirCol < maxCol && tiles[curRow, targetCol + dirCol] == null)
                targetCol += dirCol;

            if (targetCol != curCol) // 체크한 위치가 이전 위치와 다른 경우
            {
                tiles[curRow, targetCol] = tiles[curRow, curCol];
                tiles[curRow, curCol] = null;
            }
            returnValue = targetCol;
        }

        return returnValue;
    }

    //합쳐지면 현재 내 위치를 비워야함 반환 값이 null인 경우 기존 위치로 이동하면되고, 반환 값이 target 값이라면 target obj 위치까지 이동 후 종료되면됨
    void TryMerge(int curRow, int curCol, int targetRow, int targetCol)
    {
        if (!(targetRow >= 0 && targetRow < maxRow)) return; // 타겟이 범위 바깥인 경우 종료
        if (!(targetCol >= 0 && targetCol < maxCol)) return; // 타겟이 범위 바깥인 경우 종료

        if (tiles[curRow, curCol] != null && tiles[targetRow, targetCol] != null) // 현재 위치와 타겟 위치에 값이 있는 경우
        {
            Tile curTile = tiles[curRow, curCol].GetComponent<Tile>(), targetTile = tiles[targetRow, targetCol].GetComponent<Tile>();
            if (curTile.Value != targetTile.Value || targetTile.Flag) return; // 같은 값이 아닌 경우 종료, 타겟이 이미 합쳐졌다면 종료

            targetTile.MulValue(); // 현재 값 2배

            GameObject parent = backGroundTiles[targetTile.Row * maxCol + targetTile.Col];
            curTile.TilePosition(parent); // 현재 타일 포지션을 변경
            curTile.OnDestroy = true; // 파괴 오브젝트 설정
            destroyTiles[destroyCount++] = curTile; // 파괴 오브젝트 배열에 저장
            tiles[curRow, curCol] = null; // 현재 타일 위치 초기화 
        }
    }

    // 이동 > 더하기 > 이동
    public void Drag(Vector2 dir)
    {
        if (IsUpdating || dir == Vector2.zero) return;

        if (Math.Abs(dir.x) > Math.Abs(dir.y))
        {
            if (dir.x > 0)
                MoveRight(); //오른쪽 드래그
            else
                MoveLeft(); //왼쪽 드래그
        }
        else
        {
            if (dir.y > 0)
                MoveUp(); //위쪽 드래그
            else
                MoveDown(); //아래쪽 드래그
        }
        BoardUpdate();
    }
}
