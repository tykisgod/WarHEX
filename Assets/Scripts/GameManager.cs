using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager:MonoBehaviour {
    //protected GameObject[] allInfantryUnits = GameObject.FindGameObjectsWithTag("Infantry");//维护一个存放场上所有步兵单位的数组
    public GameObject player1;//玩家1
    public GameObject player2;//玩家2
    public GameObject playerWhoWin;//胜利者
    public GameObject VictoryUI;//结算面板
    public GameObject PauseUI;//暂停面板
    private bool hasPause;//是否已经暂停
    private const int INITRESOURCES = 600;
    private const int INITVP = 0;
    private const int INCREMENTRESOURCES = 20;
    private const int PLUSINCREMENTRESOURCES = 50;
    private const int INCREMENTVICTORYPOINT = 10;
    private const int WINVICTORYPOINT = 20;

    // Use this for initialization
    void Start() {
        player1.GetComponent<Player>().myTurn = true;//默认玩家1先手,控制权交给玩家1
        player1.GetComponent<Player>().playerState = Player.myState.Idle;//玩家1的状态初始化为Idle
        player1.GetComponent<Player>().myTurn = !player2.GetComponent<Player>().myTurn;
        player1.GetComponent<Player>().victoryPoints = player2.GetComponent<Player>().victoryPoints = INITVP;//初始化玩家胜利点
        player1.GetComponent<Player>().soucesPoints = player2.GetComponent<Player>().soucesPoints = INITRESOURCES;//初始化玩家资源点
        ShowResouces();//展示资源面板
    }

    // Update is called once per frame
    void Update() {
        CheckIfPause();
        //if (player2.myTurn) {
        //    Manage(player1);
        //} else {
        //    Manage(player2);
        //}
    }

    //按下ESC出现暂停面板
    protected void CheckIfPause() {
        if(Input.GetKeyUp(KeyCode.Escape) && hasPause == false) {
            PauseUI.SetActive(true);
            hasPause = true;
        } else if(Input.GetKeyUp(KeyCode.Escape) && hasPause == true) {
            ContinueGame();
        }
    }

    //继续游戏
    public void ContinueGame() {
        PauseUI.SetActive(false);
        hasPause = false;
    }

    //检查胜利者，并且把胜利者选出
    protected void CheckIfWin() {
        if(player1.GetComponent<Player>().victoryPoints >= WINVICTORYPOINT) {
            playerWhoWin = player1;
            ShowVictory();
        }
        if(player2.GetComponent<Player>().victoryPoints >= WINVICTORYPOINT) {
            playerWhoWin = player2;
            ShowVictory();
        }
    }

    //展示胜利面板
    protected void ShowVictory() {
        VictoryUI.SetActive(true);
        VictoryUI.transform.Find("Text").GetComponent<Text>().text = playerWhoWin.name + " Wins !!";
    }

    //展示数据UI
    public void ShowResouces() {
        player1.GetComponent<Player>().resourcesUI.transform.Find("Holder/iVct").GetComponent<Text>().text = "Victory Point:" + player1.GetComponent<Player>().victoryPoints;
        player2.GetComponent<Player>().resourcesUI.transform.Find("Holder/iVct").GetComponent<Text>().text = "Victory Point:" + player2.GetComponent<Player>().victoryPoints;
        player1.GetComponent<Player>().resourcesUI.transform.Find("Holder/iPep").GetComponent<Text>().text = "Manpower Point:" + player1.GetComponent<Player>().soucesPoints;
        player2.GetComponent<Player>().resourcesUI.transform.Find("Holder/iPep").GetComponent<Text>().text = "Manpower Point:" + player2.GetComponent<Player>().soucesPoints;
    }

    //冻结某一方的单位
    private void FreezeAllUnitsOf(Transform playerX) {
        foreach(Transform unit in playerX) {//遍历playerX的子物体，下同
            if(unit.GetComponent<InfantryUnit>()) {
                unit.GetComponent<InfantryUnit>().infantryState = InfantryUnit.myState.NotMyTurn;
            } else if(unit.GetComponent<TankUnit>()) {
                unit.GetComponent<TankUnit>().infantryState = TankUnit.myState.NotMyTurn;
            }
        }
    }

    //唤醒某一方的单位
    private void WakeAllUnitsOf(Transform playerX) {
        foreach(Transform unit in playerX) {
            if(unit.GetComponent<InfantryUnit>()) {
                unit.GetComponent<InfantryUnit>().infantryState = InfantryUnit.myState.Idle;
            }else if(unit.GetComponent<TankUnit>()) {
                unit.GetComponent<TankUnit>().infantryState = TankUnit.myState.Idle;
            }
        }
    }

    //补充体力值
    private void ChargeStrenghth(Transform playerX) {
        playerX.GetComponent<Player>().ChargeForUnits();//补充此玩家所有单位体力值
    }

    //检测玩家是否有单位在特殊地块上
    private void CheckIfPlayerHasUnitOnSpecialHex(Transform playerX) {
        foreach(Transform unit in playerX) {
            if(unit.GetComponent<InfantryUnit>()) {
                if(unit.GetComponent<InfantryUnit>().nowOnHex.transform.GetComponent<HexControl>().typeOfHex == HexControl.TypeOfHex.PeopleResourcesPoint) {
                    playerX.GetComponent<Player>().soucesPoints += PLUSINCREMENTRESOURCES;
                } else if(unit.GetComponent<InfantryUnit>().nowOnHex.transform.GetComponent<HexControl>().typeOfHex == HexControl.TypeOfHex.VictoryPoint) {
                    playerX.GetComponent<Player>().victoryPoints += INCREMENTVICTORYPOINT;
                }
            }else if(unit.GetComponent<TankUnit>()) {
                if(unit.GetComponent<TankUnit>().nowOnHex.transform.GetComponent<HexControl>().typeOfHex == HexControl.TypeOfHex.PeopleResourcesPoint) {
                    playerX.GetComponent<Player>().soucesPoints += PLUSINCREMENTRESOURCES;
                } else if(unit.GetComponent<TankUnit>().nowOnHex.transform.GetComponent<HexControl>().typeOfHex == HexControl.TypeOfHex.VictoryPoint) {
                    playerX.GetComponent<Player>().victoryPoints += INCREMENTVICTORYPOINT;
                }
            }
        }
    }

    //按钮使用此函数控制回合结束
    public void OnExchangePlayerControl() {
        player1.GetComponent<Player>().myTurn = !player1.GetComponent<Player>().myTurn;//玩家控制权互换
        player2.GetComponent<Player>().myTurn = !player1.GetComponent<Player>().myTurn;//玩家控制权互换
        if(player1.GetComponent<Player>().myTurn == true) {
            player1.GetComponent<Player>().playerState = Player.myState.Idle;//玩家1状态变为Idle
            WakeAllUnitsOf(player1.transform);//唤醒玩家1的单位
            FreezeAllUnitsOf(player2.transform);//冻结玩家2的单位
            ChargeStrenghth(player1.transform);//为玩家1单位充能
            CheckIfPlayerHasUnitOnSpecialHex(player1.transform);//为玩家1增加资源和胜利点
        } else {
            player2.GetComponent<Player>().playerState = Player.myState.Idle;
            WakeAllUnitsOf(player2.transform);//唤醒玩家2的单位
            FreezeAllUnitsOf(player1.transform);//冻结玩家1的单位
            ChargeStrenghth(player2.transform);//为玩家2充能
            CheckIfPlayerHasUnitOnSpecialHex(player2.transform);//为玩家2增加资源和胜利点
        }//将刚获取控制权的玩家状态变为Idle
        if(player1.GetComponent<Player>().myTurn) {//默认资源增加
            player1.GetComponent<Player>().soucesPoints += INCREMENTRESOURCES;
        } else {
            player2.GetComponent<Player>().soucesPoints += INCREMENTRESOURCES;
        }
        Vector3 temp = player1.GetComponent<Player>().resourcesUI.GetComponent<RectTransform>().position;//玩家数据面板更替
        player1.GetComponent<Player>().resourcesUI.GetComponent<RectTransform>().position = player2.GetComponent<Player>().resourcesUI.GetComponent<RectTransform>().position;//玩家数据面板更替
        player2.GetComponent<Player>().resourcesUI.GetComponent<RectTransform>().position = temp;//玩家数据面板更替
        ShowResouces();//刷新玩家数据面板
        CheckIfWin();
        Debug.Log(player1.GetComponent<Player>().myTurn);
    }

    ////仅仅通过改变，player.playerState控制玩家逻辑
    //private void Manage(Player playerX) {

    //}
}