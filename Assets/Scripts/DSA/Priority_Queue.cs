using System;
using System.Collections.Generic;

public class Priority_Queue<T> where T: IComparable<T>
{
    private List<T> mData = new List<T>();
    public bool IsEmpty()
    {
        return mData.Count == 0;
    }

    public T Top()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Priority Queue is empty");
        }
        return mData[0];
    }


    public void Pop()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Priority Queue is empty");
        }

        int lastIdx = mData.Count - 1;
        mData[0] = mData[lastIdx];
        mData.RemoveAt(lastIdx);

        int i = 0;
        while ( i < mData.Count )
        {
            int leftIdx = i * 2 + 1;
            int rightIdx = i * 2 + 2;
            int maxIdx = i;

            if ( leftIdx < mData.Count && mData[maxIdx].CompareTo(mData[leftIdx]) < 0 )
            {
                maxIdx = leftIdx;
            }
            if (rightIdx < mData.Count && mData[maxIdx].CompareTo(mData[rightIdx]) < 0)
                maxIdx = rightIdx;

            if (maxIdx != i)
            {
                Swap(i, maxIdx);
                i = maxIdx;
            }
            else
                break;
        }
    }

    public void Push(T val)
    {
        mData.Add(val);

        int i = mData.Count-1;
        while ( i > 0)
        {
            int p = (i - 1) / 2;
            if (mData[p].CompareTo(mData[i]) < 0)
            {
                Swap(i, p);
                i = p;
            }
            else
                break;
        }
    }

    private void Swap( int i, int j )
    {
        T tmp = mData[i];
        mData[i] = mData[j];
        mData[j] = tmp;
    }
}