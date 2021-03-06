﻿using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour {

    private Vector3 ORIGIN_OFFSET = new Vector3(0.0f, 0.5f, 0.0f);

    public int numColumns = 16;
    public int numRows = 16;

    public Texture tileTexture;
    public Texture tileTextureAndOverlay;
    
    public bool showOverlay;

    private GameObject ground;
    private Grid grid;
    private float width, length;
    
    void Start () {
        length = numRows;
        width = numColumns;
        showOverlay = false;

        ground = transform.parent.gameObject;
        ground.transform.localScale = new Vector3(numRows / 10.0f, 1.0f, numColumns / 10.0f);
        ground.transform.position = new Vector3(length / 2, 0, width / 2);

        Renderer renderer = ground.GetComponent<Renderer>();
        renderer.material.mainTextureScale = new Vector2(length, width);

        grid = new Grid(numRows, numColumns, width, length);
        addWaypoints();
	}
    
    public Vector3 getCenterAt(Building building, int row, int col)
    {
        Vector3 average = Vector3.zero;
        for(int i = 0; i < building.length; i++)
        {
            for(int j = 0; j < building.width; j++)
            {
                average += grid.getCenterAt(row + i, col + j);
            }
        }
        float towerArea = building.width * building.length;
        return average / towerArea;
    }

    public bool placeBuildingAt(GameObject building, int row, int col)
    {
        bool placed = grid.setObjectAt(building, row, col);
        if(placed)
        {
            building.transform.parent = transform;
            AstarPath.active.UpdateGraphs(building.GetComponent<MeshRenderer>().bounds);
        }
        return placed;
    }

    public bool canBuildAt(GameObject gameObject, int row, int col)
    {
        Building building = gameObject.GetComponent<Building>();
        if (building == null) return false;

        for (int i = 0; i < building.length; i++)
        {
            for (int j = 0; j < building.width; j++)
            {
                if (!grid.canBuildAt(row + i, col + j))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void toggleOverlay()
    {
        showOverlay = !showOverlay;

        Renderer renderer = ground.GetComponent<Renderer>();
        Texture currentTexture;
        if (showOverlay)
        {
            currentTexture = tileTextureAndOverlay;
        }
        else
        {
            currentTexture = tileTexture;
        }
        renderer.material.mainTexture = currentTexture;
    }

    void addWaypoints()
    {
        List<Transform> waypoints = new List<Transform>();
        for(int i=0; i<2; i++)
        {
            for(int j=0; j<2; j++)
            {
                GameObject waypoint = Instantiate(WaypointUtility.WAYPOINT_PREFAB, Vector3.zero, Quaternion.identity) as GameObject;
                waypoint.name = "Waypoint" + i + j;
                waypoints.Add(waypoint.transform);
                grid.setObjectAt(waypoint, i * (numRows-1), j * (numColumns-1));
            }
        }
        WaypointUtility.setWaypoints(waypoints.ToArray());
    }
}
