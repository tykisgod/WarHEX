using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatistics : MonoBehaviour {

    void SetData ( string unitName , int resourcesNeeded , int healthPoint , int strength , int movement , float hitRatio , float evasion , int volition , int atkToArm , int defToArm , int atkToPep , int defToPep , int atkDist ) {
        PlayerPrefs . SetInt ( unitName + "resourcesNeeded" , resourcesNeeded );//设置所需资源
        PlayerPrefs . SetInt ( unitName + "healthPoint" , healthPoint );//设置生命值
        PlayerPrefs . SetInt ( unitName + "strength" , strength );//设置体力值
        PlayerPrefs . SetInt ( unitName + "movement" , movement );//设置移动范围
        PlayerPrefs . SetFloat ( unitName + "hitRatio" , hitRatio );//设置命中率
        PlayerPrefs . SetFloat ( unitName + "evasion" , evasion );//设置回避率
        PlayerPrefs . SetInt ( unitName + "volition" , volition );//设置意志力
        PlayerPrefs . SetInt ( unitName + "atkToArm" , atkToArm );//设置对装甲攻击力
        PlayerPrefs . SetInt ( unitName + "defToArm" , defToArm );//设置对装甲防御力
        PlayerPrefs . SetInt ( unitName + "atkToPep" , atkToPep );//设置对人员攻击力
        PlayerPrefs . SetInt ( unitName + "defToPep" , defToPep );//设置对人员防御力
        PlayerPrefs . SetInt ( unitName + "atkDist" , atkDist );//设置射程
    }

    void SetGermanUnitData ( ) {
        SetData ( "GermanInfantry" , 50 , 100 , 40 , 4 , 1f , 0.3f , 120 , 10 , 30 , 30 , 20 , 4 );//德国步兵的数据！！！！！！！记得改回来！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        SetData ( "GermanMG" , 90 , 100 , 30 , 3 , 0.2f , 0.3f , 140 , 15 , 35 , 45 , 20 , 3 );//德国重机枪小队的数据
        SetData ( "GermanMortar" , 80 , 100 , 30 , 3 , 0.1f , 0.3f , 120 , 35 , 30 , 50 , 20 , 5 );//德国迫击炮的数据
        SetData ( "GermanPanzerchreck" , 150 , 100 , 40 , 4 , 0.4f , 0.2f , 150 , 280 , 30 , 20 , 20 , 4 );//德国反坦克小队的数据
        SetData ( "GermanSniper" , 90 , 20 , 40 , 4 , 0.9f , 0.5f , 200 , 5 , 5 , 30 , 5 , 4 );//德国狙击手的数据
        SetData ( "GermanTiger" , 550 , 550 , 50 , 5 , 0.5f , 0.1f , 120 , 400 , 100 , 60 , 100 , 4 );//德国虎式坦克的数据
    }

    void SetUSUnitData ( ) {
        SetData ( "USInfantry" , 50 , 100 , 40 , 4 , 1f , 0.3f , 120 , 10 , 30 , 30 , 20 , 4 );//美军步兵的数据！！！！！！！！！！记得改回来！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        SetData ( "USMG" , 90 , 100 , 30 , 3 , 0.2f , 0.3f , 140 , 15 , 35 , 45 , 20 , 3 );//美军重机枪小队的数据
        SetData ( "USMortar" , 80 , 100 , 30 , 3 , 0.1f , 0.3f , 120 , 35 , 30 , 50 , 20 , 5 );//美军迫击炮的数据
        SetData ( "USBazooka" , 150 , 100 , 40 , 4 , 0.4f , 0.2f , 150 , 280 , 30 , 20 , 20 , 4 );//美军反坦克小队的数据
        SetData ( "USSniper" , 90 , 20 , 40 , 4 , 0.9f , 0.5f , 200 , 5 , 5 , 30 , 5 , 4 );//美军狙击手的数据
        SetData ( "USSherman" , 550 , 550 , 50 , 5 , 0.5f , 0.1f , 120 , 400 , 100 , 60 , 100 , 4 );//美军谢尔曼坦克的数据
    }

    void Awake ( ) {
        SetGermanUnitData ( );
        SetUSUnitData ( );
    }

    private void Update ( ) {
    }
}