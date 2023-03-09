using System;

public class MinHeap<T> where T : IMinHeapItem<T>
{
    private T[] items;
    private int itemCount;

    public MinHeap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T value)
    {
        value.MinHeapIndex = itemCount;
        items[itemCount] = value;
        SortUp(value);
        itemCount++;
    }

    private void SortUp(T item)
    {
        int parentIndex = GetParentIndex(item);
        while (itemCount > 0 && item.CompareTo(items[parentIndex]) > 0)
        {
            Swap(item, items[parentIndex]);
            parentIndex = GetParentIndex(item);
        }
    }

    public T Peek()
    {
        if (itemCount == 0) throw new InvalidOperationException("Heap is empty");
        T min = items[0];
        return min;
    }

    public T RemoveMin()
    {
        if (itemCount == 0) throw new InvalidOperationException("Heap is empty");
        T min = items[0];
        itemCount--;
        items[0] = items[itemCount];
        items[0].MinHeapIndex = 0;
        T curr = items[0];
        while (true)
        {
            int leftChild = GetLeftChildIndex(curr);
            int rightChild = GetRightChildIndex(curr);

            if (leftChild >= itemCount)
            {
                break;
            }

            int swapIdx = leftChild;

            if (rightChild < itemCount && items[leftChild].CompareTo(items[rightChild]) < 0)
            {
                swapIdx = rightChild;
            }

            if (curr.CompareTo(items[swapIdx]) < 0)
            {
                Swap(curr, items[swapIdx]);
            }
            else
            {
                break;
            }
        }

        return min;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get { return itemCount; }
    }

    public bool Contains(T item)
    {
        if (item.MinHeapIndex < itemCount)
        {
            return Equals(items[item.MinHeapIndex], item);
        }
        else
        {
            return false;
        }
    }

    private void Swap(T item, T item2)
    {
        if (item.MinHeapIndex == item2.MinHeapIndex) return;
        items[item.MinHeapIndex] = item2;
        items[item2.MinHeapIndex] = item;
        //Xor swap
        item.MinHeapIndex ^= item2.MinHeapIndex;
        item2.MinHeapIndex ^= item.MinHeapIndex;
        item.MinHeapIndex ^= item2.MinHeapIndex;
    }

    private int GetParentIndex(T item)
    {
        return (item.MinHeapIndex - 1) / 2;
    }

    private int GetLeftChildIndex(T item)
    {
        return 2 * item.MinHeapIndex + 1;
    }

    private int GetRightChildIndex(T item)
    {
        return 2 * item.MinHeapIndex + 2;
    }
}

public interface IMinHeapItem<T> : IComparable<T>
{
    int MinHeapIndex { get; set; }
}