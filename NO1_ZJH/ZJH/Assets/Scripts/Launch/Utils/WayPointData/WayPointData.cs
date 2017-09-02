using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public struct WayPoint
{
   
    public ushort x;
    public ushort z;

    public WayPoint(ushort x, ushort z)
    {
        this.x = x;
        this.z = z;
    }
}

public struct AStarWayPoint
{
    public WayPoint point;
    public uint G;
    public uint H;
    public uint F;
}

public class WayPointData : ScriptableObject
{
    public const ushort FIND_POINT_DIS = 30;

    [Serializable]
    public class LinkWayPoint
    {
        public List<int> LinkWayPointList = new List<int>();
    }

    public List<WayPoint> WayPointList = new List<WayPoint>();
    public List<LinkWayPoint> AllWayPointDataList = new List<LinkWayPoint>();

    public int GetWaypointIndex(ushort x, ushort z)
    {
        return WayPointList.IndexOf(new WayPoint(x, z));
    }

    public List<int> GetPointLinkWayPoint(int index)
    {
        if (index < 0 || index >= AllWayPointDataList.Count)
        {
            LogSystem.LogWarning("WayPointData.GetPointLinkWayPoint 没找到这个路点");
            return null;
        }
        return AllWayPointDataList[index].LinkWayPointList;
    }


    private const uint G_OBLIQUE = 14;
    private const uint G_STEP = 10;
    private const uint H_OBLIQUE = 7;
    private const uint H_STEP = 5;
    private ushort xDis = 0;
    private ushort zDis = 0;
    private AStarWayPoint minShortDisPos = new AStarWayPoint();
    private ushort xMinDis = 0;
    private ushort zMinDis = 0;
    public void FindPointNearWayPoint(ushort startX, ushort startZ, ushort endX, ushort endZ, ref List<AStarWayPoint> nearWayPoint)
    {
        nearWayPoint.Clear();
        int count = WayPointList.Count;
        if (count < 1)
        {
            return;
        }
        minShortDisPos.point = WayPointList[0];
        xMinDis = (ushort)Mathf.Abs(WayPointList[0].x - startX);
        zMinDis = (ushort)Mathf.Abs(WayPointList[0].z - startZ);
        for (int i = 0; i < count; ++i)
        {
            xDis = (ushort)Mathf.Abs(WayPointList[i].x - startX);
            zDis = (ushort)Mathf.Abs(WayPointList[i].z - startZ);
            if (xDis > FIND_POINT_DIS || zDis > FIND_POINT_DIS)
            {
                if(xMinDis + zMinDis > xDis + zDis)
                {
                    minShortDisPos.point = WayPointList[i];
                    xMinDis = xDis;
                    zMinDis = zDis;
                }
                continue;
            }

            AStarWayPoint nearPoint = new AStarWayPoint();
            nearPoint.point = WayPointList[i];
            nearPoint.G = xDis > zDis ? zDis * G_OBLIQUE + xDis * G_STEP : xDis * G_OBLIQUE + zDis * G_STEP;

            xDis = (ushort)Mathf.Abs(WayPointList[i].x - endX);
            zDis = (ushort)Mathf.Abs(WayPointList[i].z - endZ);
            nearPoint.H = xDis > zDis ? zDis * H_OBLIQUE + xDis * H_STEP : xDis * H_OBLIQUE + zDis * H_STEP;
            nearPoint.F = nearPoint.G + nearPoint.H;
            nearWayPoint.Add(nearPoint);
        }
        if(nearWayPoint.Count < 1)
        {
            nearWayPoint.Add(minShortDisPos);
        }
        nearWayPoint = nearWayPoint.OrderBy(p => p.F).ToList();
    }

    public void Clear()
    {
        WayPointList.Clear();
        int count = AllWayPointDataList.Count; 
        for(int i = 0; i < count; ++i)
        {
            AllWayPointDataList[i].LinkWayPointList.Clear();
        }
        AllWayPointDataList.Clear();
    }
}


public class AStarWayPointPool
{
    private List<AStarWayPoint> AWayPointPool = new List<AStarWayPoint>();

    public AStarWayPoint GetAStarWayPoint()
    {
        if(AWayPointPool.Count < 0)
        {
            return new AStarWayPoint();
        }
        AStarWayPoint point = AWayPointPool[AWayPointPool.Count - 1];
        AWayPointPool.RemoveAt(AWayPointPool.Count - 1);
        return point;
    }

    public void RemoveAStarWayPoint(AStarWayPoint point)
    {

    }
}

