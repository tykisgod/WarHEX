using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexControl:MonoBehaviour {
    Color normalHexColor = new Color(255, 255, 255);//普通六角形的颜色
    Color victoryPointHexColor = new Color(180, 255, 0);//胜利点六角形的颜色
    Color peopleResourcesPointHexColor = new Color(0, 0, 255);//人力资源六角形的颜色
    public Transform unitOnThisHex;//此地块上的单位
    public bool hasUnit = false;//有没有单位在此地块上
    public bool hasBuilding = false;//有没有建筑在此地块上
    public enum StateOfHex {    //此地块的状态
        Normal,//无任何状态
        Frozen,//冰冻状态
        Fired,//燃烧状态
    }
    public enum TerrainOfHex {//地块地形
        Plain,//平原
        Hill,//山地
        City,//城镇
        Desert//沙漠
    }
    public enum TypeOfHex {//地块类型
        Normal,//非特殊地块
        VictoryPoint,//胜利点
        PeopleResourcesPoint,//人力资源点
    }

    public TerrainOfHex terrainOfHex = TerrainOfHex.Plain;//默认平原
    public TypeOfHex typeOfHex = TypeOfHex.Normal;//默认非特殊地块
    public StateOfHex stateOfHex = StateOfHex.Normal;//默认无状态

    //为不同的地块类型染色
    private void SetColorOfHex() {
        switch(typeOfHex) {
            case TypeOfHex.VictoryPoint:
                transform.GetComponent<SpriteRenderer>().color = victoryPointHexColor;//胜利点设置为屎黄色
                break;
            case TypeOfHex.PeopleResourcesPoint:
                transform.GetComponent<SpriteRenderer>().color = peopleResourcesPointHexColor;//人力设置为蓝色
                break;
        }
    }

    // Use this for initialization
    void Start() {
        SetColorOfHex();
    }

    // Update is called once per frame
    void Update() {
        if(unitOnThisHex) {
            hasUnit = true;
        } else {
            hasUnit = false;
        }
    }
}
