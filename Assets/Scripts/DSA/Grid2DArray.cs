using TMPro.Examples;
using Unity.Mathematics;

public class Grid2DArray<T>
{
    // ���� ��ǥ�� ������ ���� ������ 2D �׸��带 �迭�� ����
    // ��: ��ǥ ������ (-5, -3) ~ (10, 7)�� �׸��嵵 ó�� ����
    private T[,] mGrid;

    // ���� ��ǥ ���� (����ڰ� ����ϴ� ��ǥ��)
    private int mLX; // x �ּ�
    private int mRX; // x �ִ�
    private int mLY; // y �ּ�
    private int mRY; // y �ִ�

    public Grid2DArray( int lx, int rx, int ly, int ry, T defaultVal = default(T) )
    {
        mLX = lx;
        mRX = rx;
        mLY = ly;
        mRY = ry;
        this.mGrid = new T[rx-lx+1, ry-ly+1];
        for ( int x = lx ; x <= rx; x++)
        {
            for (int y = ly; y <= ry; y++)
                SetNodeAt(x, y, defaultVal);
        }
    }

    public bool IsInBounds( int x, int y )
    {
        if (x < mLX || x > mRX || y < mLY || y > mRY)
            return false;
        return true;
    }

    public T GetNodeAt( int x, int y )
    {
        if ( IsInBounds(x,y ) == false )
            return default(T);
        return mGrid[x-mLX, y - mLY];
    }

    public bool SetNodeAt( int x, int y, T val )
    {
        if ( IsInBounds (x,y) == false )
            return false;

        mGrid[x-mLX, y-mLY] = val;
        return true;
    }

}