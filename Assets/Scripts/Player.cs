using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player:MonoBehaviour {
    public Transform readyForPut;//暂存等待放置的单位
    public int index;//当前玩家单位编号
    public List<int> unitIndex = new List<int>();//用来存放玩家的单位编号
    public GameObject unitsList;//可放置单位列表,使用GameObject！！！别用Transform
    public string waitingForPut = "defaultUnit";//等待放置的单位名称，别动这个！这个参与检测是否放置单位
    Color setHexColor = new Color(255, 255, 0);//可放置六边形的颜色
    Color defaultHexColor = new Color(255, 255, 255);//默认六边形的颜色
    public bool hasPut = false;//是否放置完成
    public Transform mainCamera;//存放主摄像机
    public Transform startLine;//放置单位的六角形线
    public int soucesPoints;//声明人力点数
    public int victoryPoints;//声明胜利点数
    public bool myTurn;//是否是己方回合，只有myTurn为Ture时，才能进入playerstate状态的转换
    public GameObject resourcesUI;//左下方代表阵营的UI
    public enum myState {
        Idle,//待命状态
        Set,//布置状态
        Command,//命令状态
        NotMyTurn,//非己方回合
    }
    public myState playerState = myState.NotMyTurn;//初始化状态;

    // Use this for initialization
    void Awake() {
        for(int i = 0;i <= 256;i++) {
            unitIndex.Add(i);
        }
        index = unitIndex[0];
    }

    // Update is called once per frame
    void Update() {
        if(myTurn == false) {
            playerState = myState.NotMyTurn;//myTurn为False时，玩家处于NotMyTurn的状态
            SetColorOfFirstLine(defaultHexColor);
        }

        switch(playerState) {
            case myState.Idle://待命状态
                break;
            case myState.Command://命令状态
                break;
            case myState.Set://等待放置状态
                if(hasPut == true) {
                    SetColorOfFirstLine(defaultHexColor);
                    playerState = Player.myState.Idle;//如果已经放置，则恢复为Idle状态
                } else {
                    PutUnit(waitingForPut);//否则继续放置
                }
                break;
        }
    }

    //为单位补充体力，意志力（等待实现）等
    public void ChargeForUnits() {
        foreach(Transform unit in transform) {//补充所有单位体力值
            if(unit.GetComponent<InfantryUnit>()) {
                unit.GetComponent<InfantryUnit>().ChargeForSelf();
            }else if(unit.GetComponent<TankUnit>()) {
                unit.GetComponent<TankUnit>().ChargeForSelf();
            }
        }
    }

    //设置第一行的颜色
    private void SetColorOfFirstLine(Color setHexColor) {
        foreach(Transform startHex in startLine) {
            startHex.GetComponent<SpriteRenderer>().color = setHexColor;
        }//获取第一排六边形，改变其颜色
    }

    //放置步兵单位
    public void PutUnit(string unitName) {
        waitingForPut = unitName;//等待放置的单位
        playerState = Player.myState.Set;//变为Set状态，等待放置
        hasPut = false;//等待放置
        if(Input.GetMouseButtonDown(0)) {//如果鼠标左键点击
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);//检测鼠标点击位置
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.transform.GetComponent<SpriteRenderer>().color == setHexColor && !hit.transform.GetComponent<HexControl>().hasUnit) {//点击位置颜色为setHexColor,且此单元格没有单位才允许放置
                    Vector3 putPosition = hit.transform.GetComponent<Renderer>().bounds.center;//获取放置位置的中心坐标
                    readyForPut = Instantiate(unitsList.transform.Find(unitName), putPosition, unitsList.transform.Find(unitName).GetComponent<Transform>().rotation, this.transform);//生成单位并且赋值给readyForPut（获取这个新生成的单位）
                    readyForPut.name = readyForPut.name + index;//设置单位名称
                    if(unitName == "GermanTiger" || unitName == "USSherman") {
                        readyForPut.GetComponent<TankUnit>().unitName = unitName;//设置单位内单位名称，注意，此名称和上一行的名称不一样
                        readyForPut.GetComponent<TankUnit>().SetData();//设置单位数据
                    } else {
                        readyForPut.GetComponent<InfantryUnit>().unitName = unitName;//设置单位内单位名称，注意，此名称和上一行的名称不一样
                        readyForPut.GetComponent<InfantryUnit>().SetData();//设置单位数据
                    }
                    readyForPut.tag = transform.name;//当前放置单位的tag为当前玩家的名字
                    index++;//增加索引
                    if(unitName == "GermanTiger" || unitName == "USSherman") {
                        readyForPut.GetComponent<TankUnit>().nowOnHex = hit.transform;//此单位所在地块信息写入此单位
                        soucesPoints -= readyForPut.GetComponent<TankUnit>().resourcesNeeded;//扣除相应的人力值
                    } else {
                        readyForPut.GetComponent<InfantryUnit>().nowOnHex = hit.transform;//此单位所在地块信息写入此单位
                        soucesPoints -= readyForPut.GetComponent<InfantryUnit>().resourcesNeeded;//扣除相应的人力值
                    }
                    hit.transform.GetComponent<HexControl>().unitOnThisHex = readyForPut;//此单位信息写入此单位所在地块
                    resourcesUI.transform.Find("Holder/iPep").GetComponent<Text>().text = "Manpower Point:" + soucesPoints;//刷新左下角人力值
                    Debug.Log("LOOOOK!");
                    hasPut = true;//已经放置
                } else {
                    hasPut = true;//还未放置，点击其他地方取消放置
                }
            }
        }
    }

    //创造单位
    public void Create(string unit) {
        if(soucesPoints <= PlayerPrefs.GetInt(unit + "resourcesNeeded")) {
            Debug.Log("Need More Resouces!!!");
        } else if(playerState == myState.Idle) {//只有在待命下，即“myState.Idle”时才能造兵
            SetColorOfFirstLine(setHexColor);
            PutUnit(unit);//点击六角形可以放置单位
        }
    }
}