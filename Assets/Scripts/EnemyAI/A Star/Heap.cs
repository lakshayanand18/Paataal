using System;

public class Heap<T> where T : IHeapItem<T>
{
	T[] items;
	int currentItemCount;
		//initialize the heap with a maximum size
	public Heap(int maxHeapSize)
	{
		items = new T[maxHeapSize];
	}
		//add an item to the heap
	public void Add(T item)
	{
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}
		//remove and return the first item in the heap
	public T RemoveFirst()
	{
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}
		//update the position of an item in the heap
	public void UpdateItem(T item)
	{
		SortUp(item);
	}

	public int Count
	{
		get
		{
			return currentItemCount;
		}
	}
		//check if the heap contains a specific item
	public bool Contains(T item)
	{
		return Equals(items[item.HeapIndex], item);
	}

	void SortDown(T item)
	{
		while (true)
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;
			//swap with the highest priority child
			if (childIndexLeft < currentItemCount)
			{
				swapIndex = childIndexLeft;
				//check if right child exists
				if (childIndexRight < currentItemCount)
				{
					//check which child has higher priority
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
					{
						swapIndex = childIndexRight;
					}
				}
				//check if we need to swap
				if (item.CompareTo(items[swapIndex]) < 0)
				{
					Swap(item, items[swapIndex]);
				}
				else
				{
					return;
				}

			}
			else
			{
				return;
			}

		}
	}


	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true)
		{
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0)				// item is higher priority than parent
			{
				Swap(item, parentItem);
			}
			else
			{
				break;
			}

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}
	//swap two items in the heap
	void Swap(T itemA, T itemB)
	{
		items[itemA.HeapIndex] = itemB;
		items[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

// An interface to ensure that items added to the heap can be compared and have a heap index
public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex { get; set; }
}