using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    List<T> list;
    Func<T, T, bool> OrderRelation; 

    public int Count
    {
        get { return list.Count; }
    }

    public PriorityQueue(Func<T, T, bool> OrderRelation)
    {
        list = new List<T>();
        // Root starts at index 1 so we add a dummy element
        list.Add(default(T));
        this.OrderRelation = OrderRelation;
    }

    public void Enqueue(T element)
    {
        list.Add(element);
        int node = Count - 1;
        while(node > 0)
        {
            int parent = node / 2;
            if(!OrderRelation(list[parent], list[node]))
                Swap(parent, node);
            node = parent;
            parent /= 2;
        }
    }

    public T Dequeue()
    {
        if(IsEmpty())
            throw new Exception("Empty priority queue");

        T top = list[1];
        list[1] = list[Count - 1];
        list.RemoveAt(Count - 1);

        int node = 1;
        while(2 * node < Count)
        {
            int leftChild = 2 * node;
            int rightChild = 2 * node + 1;
            int minChild = leftChild;
            if(rightChild < Count && OrderRelation(list[rightChild], list[leftChild]))
                minChild = rightChild;
            if(!OrderRelation(list[node], list[minChild]))
                Swap(node, minChild);
            node = minChild;
        }

        return top;
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool IsEmpty()
    {
        return Count <= 1;
    }

    void Swap(int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
}