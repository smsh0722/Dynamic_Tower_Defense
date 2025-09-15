using TMPro.Examples;
using Unity.Mathematics;

public class Grid2DArray<T>
{
    // 음수 좌표를 포함한 임의 범위의 2D 그리드를 배열로 구현
    // 예: 좌표 범위가 (-5, -3) ~ (10, 7)인 그리드도 처리 가능
    private T[,] mGrid;

    // 논리적 좌표 범위 (사용자가 사용하는 좌표계)
    private int mLX; // x 최소
    private int mRX; // x 최대
    private int mLY; // y 최소
    private int mRY; // y 최대

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