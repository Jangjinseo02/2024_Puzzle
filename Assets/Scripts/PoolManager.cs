using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PoolManager : MonoBehaviour
{
    public GameObject tilePrefab; // move text tile


    public GameObject CreateTile()
    {
        GameObject tile = Instantiate(tilePrefab);

        return tile;
    }

}
