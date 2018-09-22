using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUnit:MonoBehaviour {

    #region variables
    public enum myState {
        Idle,//待命状态
        Mov,//等待移动状态
        IsMoving,//正在移动
        Atk,//等待攻击状态
        IsAttacking,//正在攻击
        NotMyTurn,//回合结束状态
        Alert//警戒状态
    }
    public AudioClip audio_atk;//攻击音效
    public AudioClip audio_mov;//移动音效
    protected bool hasAttack = false;//已经攻击
    protected int nowMovement;//当前可移动距离
    protected int distInPerDist;//距离（以PERDIST为单位）
    protected List<Transform> tagetHexList = new List<Transform>();//攻击/移动六角形地块集合
    protected RaycastHit targetEnemy;//攻击目标
    protected RaycastHit targetHex;//移动的目标六角形
    public myState infantryState = myState.Idle;//默认为待命状态
    protected Button atkButton;//攻击按钮
    protected Button movButton;//移动按钮
    public Vector3 constHexDistance;//游戏中六角形地块的距离
    public Transform nowOnHex;//当前所属地块
    //Color defaultHexColor = new Color(255, 255, 255);//默认六边形的颜色
    protected List<Color> defaultHexColor = new List<Color>();//默认六边型颜色集合
    Color movHexColor = new Color(0, 255, 0);//可移动六角形的颜色
    Color atkHexColor = new Color(255, 0, 0);//可攻击六角形的颜色
    public GameObject mainCamera;//存放主摄像机
    public GameObject dataUI;//中间下方展示单位数据的UI模板
    protected GameObject ownDataUI;//此单位自己的数据UI面板
    protected GameObject bottomBar;//卡槽父物体
    protected GameObject anchorUI;//卡槽所在位置
    public string unitName;//单位名称
    public int resourcesNeeded;//所需资源
    public int healthPoint;//生命值
    protected int strength;//体力值
    protected int movement;//移动范围
    protected float hitRatio;//命中率
    protected float evasion;//回避率
    protected int volition;//士气值
    public int atkToArm;//对装甲攻击力
    public int defToArm;//对装甲防御力
    public int atkToPep;//对人攻击力
    public int defToPep;//对人防御力
    public int atkDist;//射程
    protected const float PERDIST = 8.8f;//距离基本单位（两个六边形中心之间的距离）这是手动算的，如果要改hex模型，这个地方要重新计算！！！！！！！！！！！！！！！！！！！！！！！！！
    protected const float MINMUMDISTANCE = 0.001f;//最小间距
    protected const int COMSUMEPERMOVEMENT = 10;//移动一格需要消耗的体力值
    protected const int COMSUMEPERATTACK = 20;//攻击一次需要消耗的体力值
    #endregion

    //获取单位数据
    public void SetData() {
        resourcesNeeded = PlayerPrefs.GetInt(unitName + "resourcesNeeded");//设置所需要的资源
        healthPoint = PlayerPrefs.GetInt(unitName + "healthPoint");//设置生命值
        strength = PlayerPrefs.GetInt(unitName + "strength");//设置体力值
        movement = PlayerPrefs.GetInt(unitName + "movement");//设置移动范围
        hitRatio = PlayerPrefs.GetFloat(unitName + "hitRatio");//设置命中率
        evasion = PlayerPrefs.GetFloat(unitName + "evasion");//设置回避率
        volition = PlayerPrefs.GetInt(unitName + "volition");//设置士气值
        atkToArm = PlayerPrefs.GetInt(unitName + "atkToArm");//设置对装甲攻击力
        defToArm = PlayerPrefs.GetInt(unitName + "defToArm");//设置对装甲防御力
        atkToPep = PlayerPrefs.GetInt(unitName + "atkToPep");//设置对人员攻击力
        defToPep = PlayerPrefs.GetInt(unitName + "defToPep");//设置对人员防御力
        atkDist = PlayerPrefs.GetInt(unitName + "atkDist");//设置射程
    }

    //使用Hierarchy中的UI为gameobject赋值
    protected void SetDefaultUI() {
        bottomBar = GameObject.Find("Bottom Bar");
        anchorUI = GameObject.Find("AnchorOfCard");
    }

    //初始化兵卡UI
    protected void InitiateInfantryUI() {
        ownDataUI = Instantiate(dataUI, anchorUI.GetComponent<RectTransform>().position, new Quaternion(0, 0, 0, 0), bottomBar.transform);//使用dataUI模板设置此单位兵卡，位置为anchorUI的位置，无旋转，父物体为bottomBar
    }

    //绘制步兵单位数据UI
    protected void DrawInfantryUI() {
        ownDataUI.transform.Find("Card Box/Info/iSrc").GetComponent<Text>().text = "所需资源:" + resourcesNeeded;//显示所需资源
        ownDataUI.transform.Find("Card Box/Info/iAct").GetComponent<Text>().text = "体力值:" + strength;//显示体力值
        ownDataUI.transform.Find("Card Box/Info/iMov").GetComponent<Text>().text = "移动力:" + movement;//显示移动力
        ownDataUI.transform.Find("Card Box/Info/iHit").GetComponent<Text>().text = "命中率:" + hitRatio.ToString("f2");//显示命中率
        ownDataUI.transform.Find("Card Box/Info/iHp").GetComponent<Text>().text = "生命值:" + healthPoint;//显示生命值
        ownDataUI.transform.Find("Card Box/Info/iAvd").GetComponent<Text>().text = "回避率:" + evasion.ToString("f2");//显示回避率
        ownDataUI.transform.Find("Card Box/Info/iStr").GetComponent<Text>().text = "战斗意志:" + volition;//显示战斗意志
        ownDataUI.transform.Find("Card Box/Info/pAtk/iArm").GetComponent<Text>().text = "对装甲:" + atkToArm;//显示对装甲攻击力
        ownDataUI.transform.Find("Card Box/Info/pAtk/iPep").GetComponent<Text>().text = "对人员:" + atkToPep;//显示对人员攻击力
        ownDataUI.transform.Find("Card Box/Info/pDef/iArm").GetComponent<Text>().text = "对装甲:" + defToArm;//显示对装甲防御力
        ownDataUI.transform.Find("Card Box/Info/pDef/iPep").GetComponent<Text>().text = "对人员:" + defToPep;//显示对人员防御力
        ownDataUI.transform.Find("Card Box/Info/iDis").GetComponent<Text>().text = "射程:" + atkDist;//显示射程
    }

    //是否隐藏已经绘制好的UI
    protected void IfShowInfantryUI(bool hideOrNot) {
        if(hideOrNot == true) {
            ownDataUI.SetActive(true);
        } else {
            ownDataUI.SetActive(false);
        }
    }

    //生命值为0时，单位死亡
    protected void CheckIfDie() {
        if(healthPoint <= 0) {
            Debug.Log(transform.name + "Die");
            Destroy(transform.GetComponent<TankUnit>().ownDataUI);//销毁单位卡
            Destroy(gameObject);//销毁单位
        }
    }

    //判断是否选中
    protected bool IsSelected() {
        if(Input.GetMouseButtonDown(0)) {//如果鼠标左键点击
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);//检测鼠标点击位置
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.transform.name == transform.name) {//如果选中此单位，则显示UI，并且返回True，否则返回False
                    Debug.Log(hit.transform.name);
                    IfShowInfantryUI(true);
                    return true;
                } else {
                    IfShowInfantryUI(false);
                    return false;
                }
            }
        }
        return false;
    }

    //设定按钮功能,已经实现：移动，攻击；有待实现：警戒
    protected void setButton() {
        movButton = ownDataUI.transform.Find("Card Box/mov").GetComponent<Button>();//注意，是改变ownDataUI的按钮，不是dataUI！！！
        atkButton = ownDataUI.transform.Find("Card Box/atk").GetComponent<Button>();//注意，是改变ownDataUI的按钮，不是dataUI！！！
        movButton.onClick.AddListener(MoveNow);//监听MoveNow函数
        atkButton.onClick.AddListener(AtkNow);//监听AtkNow函数
    }

    #region CodesOfAttack
    //点击之后单位状态变为攻击
    public void AtkNow() {
        if((infantryState == myState.Idle || infantryState == myState.Mov) && IfCanAttack()) {//Idle时,或者Mov时,且有体力值才能攻击
            infantryState = myState.Atk;//状态变为Mov
        }
    }

    //能否攻击
    private bool IfCanAttack() {
        if(strength >= COMSUMEPERATTACK) {
            return true;
        } else {
            return false;
        }
    }

    //对可攻击范围进行高亮
    protected void SetAtkHex() {
        GameObject movHex = GameObject.Find("HexGrid/BattleField");//BattleField为最大可移动地块
        foreach(Transform hex in movHex.transform) {//注意，foreach就是指在子物体里面查找了，不需要还findchild
            double distance =//计算目标六角形和当前六角形的距离
                Math.Sqrt(Math.Pow((double)(hex.GetComponent<Renderer>().bounds.center.x - nowOnHex.GetComponent<Renderer>().bounds.center.x), 2.0) +
                Math.Pow((double)(hex.GetComponent<Renderer>().bounds.center.z - nowOnHex.GetComponent<Renderer>().bounds.center.z), 2.0));
            if(distance <= atkDist * PERDIST && distance >= 0.1) {//如果此地块处于最大攻击范围内，并且不是自己的地块时
                if(!hex.GetComponent<HexControl>().hasUnit) {//当地块与单位所在地块的距离小于等于可攻击范围时，且该单元格不为自己和友军所在单元格，为地块染色
                    defaultHexColor.Add(hex.GetComponent<SpriteRenderer>().color);//默认地块颜色按顺序设置
                    hex.GetComponent<SpriteRenderer>().color = atkHexColor;//可攻击地块颜色为可攻击颜色
                    tagetHexList.Add(hex);//将选出的地块存入tagetHexList
                } else if(hex.GetComponent<HexControl>().unitOnThisHex.tag != transform.tag) {
                    defaultHexColor.Add(hex.GetComponent<SpriteRenderer>().color);//默认地块颜色按顺序设置
                    hex.GetComponent<SpriteRenderer>().color = atkHexColor;//可攻击地块颜色为可攻击颜色
                    tagetHexList.Add(hex);//将选出的地块存入tagetHexList
                }
            }
        }
    }

    //扣除对敌方的伤害值,只看自己的命中率。
    protected int DamageToEnemyInfantry(RaycastHit enemy) {
        int dmg;
        if(UnityEngine.Random.Range(0, 100) / 100f <= hitRatio) {
            Debug.Log("HIT!");
            dmg = atkToPep - enemy.transform.GetComponent<InfantryUnit>().defToPep;//己方攻击力减对方防御力
            if(dmg <= 0) {
                WarLogUpdate(enemy, "nodmg", dmg);
                Debug.Log("Cause DMG=" + dmg);
            } else {
                enemy.transform.GetComponent<InfantryUnit>().healthPoint -= dmg;//结算敌方受到的伤害
                Debug.Log("Cause DMG=" + dmg);
                WarLogUpdate(enemy, "hit", dmg);
            }
        } else {
            dmg = 0;
            Debug.Log("Miss!");
            WarLogUpdate(enemy, "miss", dmg);
        }
        return dmg;
    }

    //刷新战争日志
    private void WarLogUpdate(RaycastHit enemy, string state, int dmg) {
        string information="";
        if(state == "miss") {
            information = "未能命中\n";
        } else if(state == "hit") {
            information = transform.name + "对" + enemy.transform.name + "造成了" + dmg + "点伤害" + "\n";
        } else if(state == "nodmg") {
            information = enemy.transform.name + "防御力太高，无法造成伤害" + "\n";
        }
        GameObject.Find("LogText").GetComponent<Text>().text = String.Concat(GameObject.Find("LogText").GetComponent<Text>().text, information);
    }

    //扣除对敌方的伤害值,只看自己的命中率
    protected int DamageToEnemyTank(RaycastHit enemy) {
        int dmg;
        if(UnityEngine.Random.Range(0, 100) / 100f <= hitRatio) {
            Debug.Log("HIT!");
            dmg = atkToArm - enemy.transform.GetComponent<TankUnit>().defToArm;//己方攻击力减对方防御力
            if(dmg <= 0) {
                WarLogUpdate(enemy, "nodmg", dmg);
                Debug.Log("Cause DMG=" + dmg);
            } else {
                enemy.transform.GetComponent<TankUnit>().healthPoint -= dmg;//结算敌方受到的伤害
                Debug.Log("Cause DMG=" + dmg);
                WarLogUpdate(enemy, "hit", dmg);
            }
        } else {
            dmg = 0;
            Debug.Log("Miss!");
            WarLogUpdate(enemy, "miss", dmg);
        }
        return dmg;
    }

    ////扣除自己承受的反击伤害,只看自己的回避率
    //protected int DamageToSelf(RaycastHit enemy) {
    //    int dmg;
    //    if(UnityEngine.Random.Range(0, 100) / 100f >= evasion) {
    //        dmg = enemy.transform.GetComponent<TankUnit>().atkToPep - defToPep;//对方攻击力减己方防御力
    //        healthPoint -= dmg;//结算自己受到的伤害
    //    } else {
    //        dmg = 0;
    //        Debug.Log("Lucky!!");
    //    }
    //    return dmg;
    //}

    //计算敌方受到的意志损失
    protected int ThreatenToEnemy() {
        int dmg = 0;
        return dmg;
    }

    //计算自己受到的意志损失
    protected int ThreatenToSelf() {
        int dmg = 0;
        return dmg;
    }

    //选中步兵单位进行攻击
    protected void ReadyToAtk() {
        if(Input.GetMouseButtonDown(0)) {//如果鼠标左键点击
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);//检测鼠标点击位置
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.transform.GetComponent<TankUnit>() && hit.transform.GetComponent<TankUnit>().nowOnHex.GetComponent<SpriteRenderer>().color == atkHexColor) {//选中的是敌军目标，并且敌军目标目标处于红色地块上时
                    targetEnemy = hit;//确定攻击目标，赋值给targetEnemy
                    infantryState = myState.IsAttacking;//状态成为正在攻击
                } else if(hit.transform.GetComponent<InfantryUnit>() && hit.transform.GetComponent<InfantryUnit>().nowOnHex.GetComponent<SpriteRenderer>().color == atkHexColor) {//选中的是敌军目标，并且敌军目标目标处于红色地块上时
                    targetEnemy = hit;//确定攻击目标，赋值给targetEnemy
                    infantryState = myState.IsAttacking;//状态成为正在攻击
                } else {
                    for(int index = 0;index < tagetHexList.Count;index++) {
                        tagetHexList[index].GetComponent<SpriteRenderer>().color = defaultHexColor[index];
                    }
                    //foreach (Transform hex in tagetHexList) {
                    //    foreach (Color hexColor in defaultHexColor) {
                    //        hex.GetComponent<SpriteRenderer>().color = hexColor;//范围地块恢复成默认颜色
                    //    }
                    //}
                    tagetHexList.Clear();//清除目标地块
                    defaultHexColor.Clear();
                    infantryState = myState.Idle;//状态变回Idle
                }
            }
        }
    }

    //攻击函数，进行攻击
    protected void ImAttacking(RaycastHit enemy) {
        strength -= COMSUMEPERATTACK;//扣除移动体力
        if(enemy.transform.GetComponent<InfantryUnit>()) {//对敌方目标造成伤害
            DamageToEnemyInfantry(enemy);
            Debug.Log("hit inf");
        }else if(enemy.transform.GetComponent<TankUnit>()) {
            Debug.Log("hit tank");
            DamageToEnemyTank(enemy);
        }
        //if(enemy.transform.GetComponent<TankUnit>().infantryState == myState.Alert) {//如果敌方处于警戒状态，（（（（（并且此单位处于敌方单位的攻击范围内(←←←←←等待做！！！)））
        //    DamageToSelf(enemy);
        //}
        DrawInfantryUI();//重新绘制UI
        IfShowInfantryUI(true);
        for(int index = 0;index < tagetHexList.Count;index++) {
            tagetHexList[index].GetComponent<SpriteRenderer>().color = defaultHexColor[index];
        }
        infantryState = myState.Idle;
        tagetHexList.Clear();//清除目标地块列表
        defaultHexColor.Clear();
    }
    #endregion

    #region CodesOfMovement
    //点击之后单位状态变为移动
    public void MoveNow() {
        if((infantryState == myState.Idle || infantryState == myState.Atk) && IfCanMove()) {//只有在待命状态或者Atk时，并且体力值满足时才能移动
            infantryState = myState.Mov;
        }//状态变为Mov
    }

    //计算实际上移动的格子数
    protected void CountHexMoved(Transform hex) {
        double distance =
                Math.Sqrt(Math.Pow((double)(hex.GetComponent<Renderer>().bounds.center.x - nowOnHex.GetComponent<Renderer>().bounds.center.x), 2.0) +
                Math.Pow((double)(hex.GetComponent<Renderer>().bounds.center.z - nowOnHex.GetComponent<Renderer>().bounds.center.z), 2.0));//计算目标六角形和当前六角形的距离
        distInPerDist = (int)(distance / PERDIST) + 1;//这里计算的时候会出现x.9999……,转换成int会少1，所以要补上
    }

    //计算当前可移动的范围（即，在当前体力值时，能移动的范围,单位是格子数量)
    protected void CountNowMovement() {
        if(strength >= movement * COMSUMEPERMOVEMENT) {
            nowMovement = movement;
        } else {
            nowMovement = strength / COMSUMEPERMOVEMENT;
        }
    }

    //能否移动（这样实现比较好，注意有时间的话要修改能否移动的判定）
    private bool IfCanMove() {
        CountNowMovement();//计算当前可移动的范围（即，在当前体力值时，能移动的范围,单位是格子数量)
        if(nowMovement > 0) {
            return true;
        } else {
            return false;
        }
    }

    //对可移动范围进行高亮
    protected void SetMovHex() {
        GameObject movHex = GameObject.Find("HexGrid/BattleField");//BattleField为最大可移动地块
        foreach(Transform hex in movHex.transform) {//注意，foreach就是指在子物体里面查找了，不需要还findchild
            double distance =
                Math.Sqrt(Math.Pow((double)(hex.GetComponent<Renderer>().bounds.center.x - nowOnHex.GetComponent<Renderer>().bounds.center.x), 2.0) +
                Math.Pow((double)(hex.GetComponent<Renderer>().bounds.center.z - nowOnHex.GetComponent<Renderer>().bounds.center.z), 2.0));//计算目标六角形和当前六角形的距离
            CountNowMovement();//计算当前可移动的范围
            if(distance <= nowMovement * PERDIST && distance >= 0.1 && !hex.GetComponent<HexControl>().hasUnit) {//当地块与单位所在地块的距离小于等于可移动范围时，且该单元格不为自己所在单元格，且该单元格没有其他单位，可以移动到目标地块
                defaultHexColor.Add(hex.GetComponent<SpriteRenderer>().color);//默认地块颜色按顺序设置
                hex.GetComponent<SpriteRenderer>().color = movHexColor;//可移动地块颜色为绿色
                tagetHexList.Add(hex);//将选出的地块存入tagetHexList
            }
        }
    }

    //选中目标地块
    protected void ReadyToMove() {
        if(Input.GetMouseButtonDown(0)) {//如果鼠标左键点击
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);//检测鼠标点击位置
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.transform.GetComponent<SpriteRenderer>() && hit.transform.GetComponent<SpriteRenderer>().color == movHexColor) {//如果有SpriteRenderer，并且点击的是可移动位置（即点击的是地块而不是其他东西）
                    targetHex = hit;
                    CountHexMoved(targetHex.transform);//计算移动的格数，方便后面扣除体力值。
                    infantryState = myState.IsMoving;//状态成为正在移动
                } else {
                    for(int index = 0;index < tagetHexList.Count;index++) {
                        tagetHexList[index].GetComponent<SpriteRenderer>().color = defaultHexColor[index];
                    }
                    tagetHexList.Clear();//清除目标地块
                    defaultHexColor.Clear();
                    infantryState = myState.Idle;//状态变回Idle
                }
            }
        }
    }

    //移动函数，进行移动
    protected void ImMoving() {
        for(int index = 0;index < tagetHexList.Count;index++) {
            tagetHexList[index].GetComponent<SpriteRenderer>().color = defaultHexColor[index];
        }
        transform.position = Vector3.Lerp(transform.position, targetHex.transform.GetComponent<Transform>().position, 20 * Time.deltaTime);
        if((Vector3.Distance(transform.position, targetHex.transform.GetComponent<Transform>().position)) <= MINMUMDISTANCE) {
            infantryState = myState.Idle;//单位与目标地块的间距小于最小距离之后，单位状态变为Idle
            nowOnHex.GetComponent<HexControl>().unitOnThisHex = null;//出发地块中此单位信息被移除
            nowOnHex = targetHex.transform;//目前所在的地块变更为目标地块
            nowOnHex.GetComponent<HexControl>().unitOnThisHex = transform;//目标地块中此单位信息被添加
            strength -= distInPerDist * COMSUMEPERMOVEMENT;//扣除移动体力
            tagetHexList.Clear();//清除目标地块
            defaultHexColor.Clear();
            DrawInfantryUI();//重新绘制UI
        }
        IfShowInfantryUI(true);
    }
    #endregion//移动部分代码 移动部分代码 

    //补充单位体力值
    public void ChargeForSelf() {
        strength = PlayerPrefs.GetInt(unitName + "strength");//重置体力值
        Debug.Log(unitName + "WOWOWOWNICE");
        DrawInfantryUI();//重新绘制UI
    }

    //清除目前被此单位高亮的格子（攻击或者移动时）
    protected void clearAllHexHighlighted() {
        for(int index = 0;index < tagetHexList.Count;index++) {
            tagetHexList[index].GetComponent<SpriteRenderer>().color = defaultHexColor[index];
        }
        defaultHexColor.Clear();
        tagetHexList.Clear();//清除目标六边形list
    }

    private void Awake() {

    }

    // Use this for initialization
    void Start() {
        mainCamera = GameObject.Find("Main Camera");//设置主摄像机
        SetDefaultUI();//设置默认UI
        InitiateInfantryUI();//初始化单位数据UI
        DrawInfantryUI();//绘制步兵单位数据UI
        setButton();
    }

    // Update is called once per frame
    void Update() {
        CheckIfDie();//检查单位是否死亡
        Debug.Log(infantryState);
        if(infantryState != myState.NotMyTurn) {//己方回合，响应展示兵卡的行为
            IsSelected();
        } else {
            clearAllHexHighlighted();
            IfShowInfantryUI(false);//非己方回合，不展示自己的兵卡
        }
        switch(infantryState) {
            case myState.Idle://闲置状态
                break;
            case myState.Mov://移动状态
                clearAllHexHighlighted();
                SetMovHex();
                ReadyToMove();
                break;
            case myState.IsMoving:
                transform.GetComponent<AudioSource>().clip = audio_mov;
                transform.GetComponent<AudioSource>().Play();
                ImMoving();
                break;
            case myState.Atk:
                clearAllHexHighlighted();
                SetAtkHex();
                ReadyToAtk();
                break;
            case myState.IsAttacking:
                transform.GetComponent<AudioSource>().clip = audio_atk;
                transform.GetComponent<AudioSource>().Play();
                ImAttacking(targetEnemy);
                break;
            case myState.NotMyTurn:
                break;
            case myState.Alert:
                break;
        }
    }

}
