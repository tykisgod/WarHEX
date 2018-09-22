using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour {
    //下面的公共变量用来存储六边形模型的prefab;
    //通过把prefab拖入unity editor中变量的位置来初始化它;
    public GameObject Hex;
    //下面两个变量也能通过unity editor来初始化
    public int gridWidthInHexes = 10;
    public int gridHeightInHexes = 10;

    //六边形砖块在游戏中的宽度和高度
    private float hexWidth;
    private float hexHeight;

    //初始化六边形砖块的宽度和高度
    void setSizes ( ) {
        //通过六边形砖块prefab的renderer组件来获取现在的宽度和高度
        hexWidth = Hex . GetComponent<Renderer> ( ) . bounds . size . x;
        hexHeight = Hex . GetComponent<Renderer> ( ) . bounds . size . z;
    }

    //用来计算第一块六边形砖的方法
    //六边形网格的中心是(0,0,0)
    Vector3 calcInitPos ( ) {
        Vector3 initPos;
        //初始化的位置是左上角
        initPos = new Vector3 (
            -hexWidth * gridWidthInHexes / 2f + hexWidth / 2 ,
            0 ,
            gridHeightInHexes / 2f * hexHeight - hexHeight / 2
            );
        return initPos;
    }

    //将六边形网格的坐标转换成游戏世界坐标的方法
    public Vector3 calcWorldCoord ( Vector2 gridPos ) {
        //放置第一块六边形砖块
        Vector3 initPos = calcInitPos ( );
        //每隔一块砖的偏移量是半块砖的宽度
        float offset = 0;
        if ( gridPos . y % 2 != 0 )
            offset = hexWidth / 2;

        float x = initPos . x + offset + gridPos . x * hexWidth;
        //每个新行的偏移量是3/4个六边形高度（即六边形内径*3/4）
        float z = initPos . z - gridPos . y * hexHeight * 0.75f;
        return new Vector3 ( x , 0 , z );
    }

    //最后用来初始化和放置所有砖块的方法
    void createGrid ( ) {
        //创造一个HexGrid来做所有砖块的父亲
        GameObject hexGridGO = new GameObject ( "HexGrid" );

        for ( float y = 0 ; y < gridHeightInHexes ; y++ ) {
            for ( float x = 0 ; x < gridWidthInHexes ; x++ ) {
                //克隆Hex公共变量到GameObjet hex
                GameObject hex = ( GameObject ) Instantiate ( Hex );
                //现在在网格中的位置
                Vector2 gridPos = new Vector2 ( x , y );
                hex . transform . position = calcWorldCoord ( gridPos );
                hex . transform . parent = hexGridGO . transform;
            }
        }
    }

    //创建网格
    void Start ( ) {
        setSizes ( );
        createGrid ( );
    }
}