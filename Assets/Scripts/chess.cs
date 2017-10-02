///**********************************************************************
///作者信息
///开发者：雁回晴空
///时间：2017.01.11
///联系方式：http://blog.csdn.net/zzlyw/article/details/54345250
///**********************************************************************

using UnityEngine;
using System.Collections;

public class chess : MonoBehaviour {

	//四个锚点位置，用于计算棋子落点
    public GameObject LeftTop;
    public GameObject RightTop;
    public GameObject LeftBottom;
    public GameObject RightBottom;
    //主摄像机
    public Camera cam;
    //锚点在屏幕上的映射位置
    Vector3 LTPos;
    Vector3 RTPos;
    Vector3 LBPos;
    Vector3 RBPos;

    Vector3 PointPos;//当前点选的位置
    float gridWidth =1; //棋盘网格宽度
    float gridHeight=1; //棋盘网格高度
    float minGridDis;  //网格宽和高中较小的一个
    Vector2[,] chessPos; //存储棋盘上所有可以落子的位置
    int[,] chessState; //存储棋盘位置上的落子状态
    enum turn {black, white } ;
    turn chessTurn; //落子顺序
    public Texture2D white; //白棋子
    public Texture2D black; //黑棋子
    public Texture2D blackWin; //白子获胜提示图
    public Texture2D whiteWin; //黑子获胜提示图
    int winner = 0; //获胜方，1为黑子，-1为白子
    bool isPlaying = true; //是否处于对弈状态
	void Start () {
        chessPos = new Vector2[15, 15];
        chessState =new int[15,15];
        chessTurn = turn.black;

	}
	
	void Update () {

        //计算锚点位置
        LTPos = cam.WorldToScreenPoint(LeftTop.transform.position);
        RTPos = cam.WorldToScreenPoint(RightTop.transform.position);
        LBPos = cam.WorldToScreenPoint(LeftBottom.transform.position);
        RBPos = cam.WorldToScreenPoint(RightBottom.transform.position);
        //计算网格宽度
        gridWidth = (RTPos.x - LTPos.x) / 14;
        gridHeight = (LTPos.y - LBPos.y) / 14;
        minGridDis = gridWidth < gridHeight ? gridWidth : gridHeight;
        //计算落子点位置
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                chessPos[i, j] = new Vector2(LBPos.x + gridWidth * i, LBPos.y + gridHeight * j);
            }
        }
        //检测鼠标输入并确定落子状态
        if (isPlaying && Input.GetMouseButtonDown(0))
        {
            PointPos = Input.mousePosition;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {   
                    //找到最接近鼠标点击位置的落子点，如果空则落子
                    if (Dis(PointPos, chessPos[i, j]) < minGridDis / 2 && chessState[i,j]==0)
                    {
                        //根据下棋顺序确定落子颜色
                        chessState[i, j] = chessTurn == turn.black ? 1 : -1;
                        //落子成功，更换下棋顺序
                        chessTurn = chessTurn == turn.black ? turn.white : turn.black;                        
                    }
                }
            }
            //调用判断函数，确定是否有获胜方
            int re = result();
            if (re == 1)
            {
                Debug.Log("黑棋胜");
                winner = 1;
                isPlaying = false;
            }
            else if(re==-1)
            {
                Debug.Log("白棋胜");
                winner = -1;
                isPlaying = false;
            }            
        }
        //按下空格重新开始游戏
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    chessState[i, j] = 0;
                }
            }
            isPlaying = true;
            chessTurn = turn.black;
            winner = 0;
        }     
	}
    //计算平面距离函数
    float Dis(Vector3 mPos, Vector2 gridPos)
    {
        return Mathf.Sqrt(Mathf.Pow(mPos.x - gridPos.x, 2)+ Mathf.Pow(mPos.y - gridPos.y, 2));
    }

    void OnGUI()
    { 
        //绘制棋子
        for(int i=0;i<15;i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (chessState[i, j] == 1)
                {
                    GUI.DrawTexture(new Rect(chessPos[i,j].x-gridWidth/2, Screen.height-chessPos[i,j].y-gridHeight/2, gridWidth,gridHeight),black);
                }
                if (chessState[i, j] == -1)
                {
                    GUI.DrawTexture(new Rect(chessPos[i, j].x - gridWidth / 2, Screen.height - chessPos[i, j].y - gridHeight / 2, gridWidth, gridHeight), white);
                }               
            }
        }
        //根据获胜状态，弹出相应的胜利图片
        if (winner ==  1)
        GUI.DrawTexture(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.25f), blackWin);
        if (winner == -1)
        GUI.DrawTexture(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.25f), whiteWin);

    }
    //检测是够获胜的函数，不含黑棋禁手检测
    int result()
    {
        int flag = 0;
        //如果当前该白棋落子，标定黑棋刚刚下完一步，此时应该判断黑棋是否获胜
        if(chessTurn == turn.white)
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (j < 4)
                    {
                        //横向
                        if (chessState[i, j] == 1 && chessState[i, j + 1] == 1 && chessState[i, j + 2] == 1 && chessState[i, j + 3] == 1 && chessState[i, j + 4] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //纵向
                        if (chessState[i, j] == 1 && chessState[i + 1, j] == 1 && chessState[i + 2, j] == 1 && chessState[i + 3, j] == 1 && chessState[i + 4, j] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //右斜线
                        if (chessState[i, j] == 1 && chessState[i + 1, j + 1] == 1 && chessState[i + 2, j + 2] == 1 && chessState[i + 3, j + 3] == 1 && chessState[i + 4, j + 4] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //左斜线
                        //if (chessState[i, j] == 1 && chessState[i + 1, j - 1] == 1 && chessState[i + 2, j - 2] == 1 && chessState[i + 3, j - 3] == 1 && chessState[i + 4, j - 4] == 1)
                        //{
                        //    flag = 1;
                        //    return flag;
                        //}
                    }
                    else if (j >= 4 && j < 11)
                    {
                        //横向
                        if (chessState[i, j] == 1 && chessState[i, j + 1] == 1 && chessState[i, j + 2] == 1 && chessState[i, j + 3] == 1 && chessState[i, j + 4] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //纵向
                        if (chessState[i, j] == 1 && chessState[i + 1, j] == 1 && chessState[i + 2, j] == 1 && chessState[i + 3, j] == 1 && chessState[i + 4, j] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //右斜线
                        if (chessState[i, j] == 1 && chessState[i + 1, j + 1] == 1 && chessState[i + 2, j + 2] == 1 && chessState[i + 3, j + 3] == 1 && chessState[i + 4, j + 4] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //左斜线
                        if (chessState[i, j] == 1 && chessState[i + 1, j - 1] == 1 && chessState[i + 2, j - 2] == 1 && chessState[i + 3, j - 3] == 1 && chessState[i + 4, j - 4] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                    }
                    else
                    {
                        //横向
                        //if (chessState[i, j] == 1 && chessState[i, j + 1] == 1 && chessState[i, j + 2] == 1 && chessState[i, j + 3] == 1 && chessState[i, j + 4] == 1)
                        //{
                        //    flag = 1;
                        //    return flag;
                        //}
                        //纵向
                        if (chessState[i, j] == 1 && chessState[i + 1, j] == 1 && chessState[i + 2, j] == 1 && chessState[i + 3, j] == 1 && chessState[i + 4, j] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                        //右斜线
                        //if (chessState[i, j] == 1 && chessState[i + 1, j + 1] == 1 && chessState[i + 2, j + 2] == 1 && chessState[i + 3, j + 3] == 1 && chessState[i + 4, j + 4] == 1)
                        //{
                        //    flag = 1;
                        //    return flag;
                        //}
                        //左斜线
                        if (chessState[i, j] == 1 && chessState[i + 1, j - 1] == 1 && chessState[i + 2, j - 2] == 1 && chessState[i + 3, j - 3] == 1 && chessState[i + 4, j - 4] == 1)
                        {
                            flag = 1;
                            return flag;
                        }
                    }

                }
            }
            for (int i = 11; i < 15; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    //只需要判断横向    
                    if (chessState[i, j] == 1 && chessState[i, j + 1] == 1 && chessState[i, j + 2] == 1 && chessState[i, j + 3] == 1 && chessState[i, j + 4] == 1)
                    {
                        flag = 1;
                        return flag;
                    }
                }
            }  
        }
        //如果当前该黑棋落子，标定白棋刚刚下完一步，此时应该判断白棋是否获胜
        else if(chessTurn == turn.black)
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (j < 4)
                    {
                        //横向
                        if (chessState[i, j] == -1 && chessState[i, j + 1] == -1 && chessState[i, j + 2] == -1 && chessState[i, j + 3] == -1 && chessState[i, j + 4] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //纵向
                        if (chessState[i, j] == -1 && chessState[i + 1, j] == -1 && chessState[i + 2, j] == -1 && chessState[i + 3, j] == -1 && chessState[i + 4, j] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //右斜线
                        if (chessState[i, j] == -1 && chessState[i + 1, j + 1] == -1 && chessState[i + 2, j + 2] == -1 && chessState[i + 3, j + 3] == -1 && chessState[i + 4, j + 4] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //左斜线
                        //if (chessState[i, j] == -1 && chessState[i + 1, j - 1] == -1 && chessState[i + 2, j - 2] == -1 && chessState[i + 3, j - 3] == -1 && chessState[i + 4, j - 4] == -1)
                        //{
                        //    flag = -1;
                        //    return flag;
                        //}
                    }
                    else if (j >= 4 && j < 11)
                    {
                        //横向
                        if (chessState[i, j] == -1 && chessState[i, j + 1] == -1 && chessState[i, j + 2] == -1 && chessState[i, j + 3] == -1 && chessState[i, j + 4] ==- 1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //纵向
                        if (chessState[i, j] == -1 && chessState[i + 1, j] == -1 && chessState[i + 2, j] == -1 && chessState[i + 3, j] == -1 && chessState[i + 4, j] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //右斜线
                        if (chessState[i, j] == -1 && chessState[i + 1, j + 1] == -1 && chessState[i + 2, j + 2] == -1 && chessState[i + 3, j + 3] == -1 && chessState[i + 4, j + 4] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //左斜线
                        if (chessState[i, j] == -1 && chessState[i + 1, j - 1] == -1 && chessState[i + 2, j - 2] == -1 && chessState[i + 3, j - 3] == -1 && chessState[i + 4, j - 4] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                    }
                    else
                    {
                        //横向
                        //if (chessState[i, j] == -1 && chessState[i, j + 1] ==- 1 && chessState[i, j + 2] == -1 && chessState[i, j + 3] == -1 && chessState[i, j + 4] == -1)
                        //{
                        //    flag = -1;
                        //    return flag;
                        //}
                        //纵向
                        if (chessState[i, j] == -1 && chessState[i + 1, j] ==- 1 && chessState[i + 2, j] ==- 1 && chessState[i + 3, j] ==- 1 && chessState[i + 4, j] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                        //右斜线
                        //if (chessState[i, j] == -1 && chessState[i + 1, j + 1] == -1 && chessState[i + 2, j + 2] == -1 && chessState[i + 3, j + 3] == -1 && chessState[i + 4, j + 4] == -1)
                        //{
                        //    flag = -1;
                        //    return flag;
                        //}
                        //左斜线
                        if (chessState[i, j] == -1 && chessState[i + 1, j - 1] == -1 && chessState[i + 2, j - 2] == -1 && chessState[i + 3, j - 3] == -1 && chessState[i + 4, j - 4] == -1)
                        {
                            flag = -1;
                            return flag;
                        }
                    }
                }
            }
            for (int i = 11; i < 15; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    //只需要判断横向    
                    if (chessState[i, j] == -1 && chessState[i, j + 1] == -1 && chessState[i, j + 2] == -1 && chessState[i, j + 3] == -1 && chessState[i, j + 4] == -1)
                    {
                        flag = -1;
                        return flag;
                    }
                }
            }  
        }       
        return flag;
    }    
}
