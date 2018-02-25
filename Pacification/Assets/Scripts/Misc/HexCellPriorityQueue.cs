using System.Collections.Generic;

public class HexCellPriorityQueue
{
    List<HexCell> list = new List<HexCell>();
    int count = 0;
    int minimum = int.MaxValue;

    public int Count
    {
        get { return count; }
    }

    public void Enqueue(HexCell cell)
    {
        ++count;
        int priority = cell.SearchPriority;
        if(priority < minimum)
            minimum = priority;
        while(priority >= list.Count)
            list.Add(null);
        cell.NextWithSamePriority = list[priority];
        list[priority] = cell;
    }

    public HexCell Dequeue()
    {
        --count;
        while(minimum < list.Count)
        {
            HexCell cell = list[minimum];
            if(cell != null)
            {
                list[minimum] = cell.NextWithSamePriority;
                return cell;
            }

            ++minimum;
        }
        return null;
    }

    public void Change(HexCell cell, int oldPriority)
    {
        HexCell current = list[oldPriority];
        HexCell next = current.NextWithSamePriority;
        if(current == cell)
            list[oldPriority] = next;
        else
        {
            while(next != cell)
            {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;
        }

        Enqueue(cell);
        --count;
    }

    public void Clear()
    {
        list.Clear();
        count = 0;
        minimum = int.MaxValue;
    }
}