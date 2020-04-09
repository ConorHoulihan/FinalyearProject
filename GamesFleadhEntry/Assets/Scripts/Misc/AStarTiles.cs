using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTiles : MonoBehaviour
{
    public Transform target, enemy, closestTile;
    private Transform[] children, floors;
    public List<Transform> openList, closedList, path; 
    private float[] hValues, gValues, fValues;
    private bool[] isWall;
    private bool pathFound = false;
    private int[] parentTile;
    private int targetIndex, smallestFIndex;
    private Vector3 targetPos;

    void Start()
    {
        children = GetComponentsInChildren<Transform>();
        floors = new Transform[100];
        openList = new List<Transform>();
        closedList = new List<Transform>();
        hValues = new float[100];
        gValues = new float[100];
        fValues = new float[100];
        isWall = new bool[100];
        parentTile = new int[100];

        int i = 0;
        for (int j = 0; j < children.Length; j++)
        {
            if (children[j].tag == "Floor")//Locate all the floors
            {
                floors[i] = children[j];

                for (int l = 0; l < children.Length; l++)
                {
                    if (children[l].tag == "Walls" && children[l].position==floors[i].position)//Determines if there is a wall on the floor
                    {
                        isWall[i] = true;
                        break;
                    }
                }
                i++;
            }
        }
        targetPos = target.position;
        CalculatePath();
    }
    private void Update()
    {
        if (targetPos != target.position)
        {
            pathFound = false;
            targetPos = target.position;
            CalculatePath();
        }
        if (path.Count >= 1 && pathFound)
        {
            enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, path[path.Count - 1].position, 5 * Time.deltaTime);
            if (enemy.transform.position == path[path.Count - 1].position)
            {
                path.RemoveAt(path.Count - 1);
            }
        }
    }

    private void CalculatePath()
    {
        ClearArraysAndLists();
        
        closestTile = FindTile(enemy, new Vector3(0, 0, 0));
        targetIndex = GetIndex(FindTile(target, new Vector3(0, 0, 0)));
        int closestIndex = GetIndex(closestTile);
        //set values of start position
        hValues[closestIndex] = Vector3.Distance(closestTile.position, targetPos);
        gValues[closestIndex] = 0;
        fValues[closestIndex] = hValues[closestIndex] + gValues[closestIndex];
        openList.Add(closestTile);


        //for (int tempcount = 0; tempcount < 100; tempcount++)
        while (openList.Count > 0)
        {
            closestIndex = GetSmallestFValue();//This gets the smallest f value from the open list also sets SmallestFIndex
            closestTile = floors[smallestFIndex];
            if (GetIndex(closestTile) == targetIndex)
            {
                Debug.Log("Path found");
                PathToList();
                break;
            }
            openList.RemoveAt(closestIndex);
            closedList.Add(floors[smallestFIndex]);
            //Checking 4 neighbours to try add to open list or find better path
            Transform above = FindTile(closestTile, new Vector3(0, 1.5f, 0));//Above Tile
            AddOpenList(above, GetIndex(above));

            Transform right = FindTile(closestTile, new Vector3(1.5f, 0, 0));//Right Tile
            AddOpenList(right, GetIndex(right));

            Transform below = FindTile(closestTile, new Vector3(0, -1.5f, 0));//Below Tile
            AddOpenList(below, GetIndex(below));

            Transform left = FindTile(closestTile, new Vector3(-1.5f, 0, 0));//Left Tile
            AddOpenList(left, GetIndex(left));

        }
    }

    private Transform FindTile(Transform target, Vector3 scew)
    {
        Transform tile = null;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Floor"))//finds the target floor tile
        {
            float temp = Vector2.Distance(go.transform.position, target.transform.position + scew);
            if (tile==null)
            {
                tile = go.transform;
            }
            else if (temp < Vector2.Distance(tile.transform.position, target.transform.position + scew))
            {
                tile = go.transform;
            }
        }
        return tile;
    }

    private int GetIndex(Transform tile)
    {
        for (int k = 0; k < floors.Length; k++)//Finds floors index
        {
            if (floors[k].position == tile.position)
            {
                return k;
            }
        }
        return -1;
    }

    private int GetSmallestFValue()
    {
        float temp = 99999;
        int returnValue = -1;
        for (int k = 0; k < openList.Count; k++)
        {
            int indx = GetIndex(openList[k]);//Get the index of the list item in the list of floors
            if (fValues[indx] < temp)
            {
                temp = fValues[indx];
                smallestFIndex = indx;
                returnValue = k;
            }
        }
        return returnValue;
    }

    public bool ListContains(List<Transform> list, Transform tile)
    {
        for (int k = 0; k < list.Count; k++)
        {
            if (tile.position == list[k].position)
            {
                return true;
            }
        }
            return false;
    }

    private void PathToList()
    {
        int currentIndex = targetIndex;
        path.Add(floors[currentIndex]);
        while (currentIndex != GetIndex(FindTile(enemy, new Vector3(0, 0, 0))))
        {
            currentIndex = parentTile[currentIndex];
            path.Add(floors[currentIndex]);
        }
        pathFound = true;
    }
    private void ClearArraysAndLists()
    {
        openList.Clear();
        closedList.Clear();
        path.Clear();
        System.Array.Clear(hValues, 0, hValues.Length);
        System.Array.Clear(gValues, 0, gValues.Length);
        System.Array.Clear(fValues, 0, fValues.Length);
        System.Array.Clear(parentTile, 0, parentTile.Length);
    }
    private void AddOpenList(Transform newTile, int newTileIndex)
    {
        if (!isWall[newTileIndex] && !ListContains(openList, newTile) && !ListContains(closedList, newTile))
        {
            openList.Add(newTile);
            parentTile[newTileIndex] = smallestFIndex;

            hValues[newTileIndex] = Vector3.Distance(newTile.position, targetPos);//H is straight distance from above tiles position until the end goal
            gValues[newTileIndex] = gValues[smallestFIndex] + Vector3.Distance(newTile.position, floors[smallestFIndex].position);//G is the distance between above tile and its parent and the distance between its parent and grandparent, continuing until you reach starting tile 
            fValues[newTileIndex] = hValues[newTileIndex] + gValues[newTileIndex];//F is G and H added together.
        }
        else if (ListContains(openList, newTile))
        {//checks if new parents G value is lower than original parent
            float testGValue = gValues[smallestFIndex] + Vector3.Distance(newTile.position, floors[smallestFIndex].position);
            if (testGValue < gValues[newTileIndex])
            {
                parentTile[newTileIndex] = smallestFIndex;
                gValues[newTileIndex] = testGValue;
            }
        }
    }

}
