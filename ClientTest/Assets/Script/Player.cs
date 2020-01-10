using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Game/Player")]
public class Player : MonoBehaviour
{
    public Transform m_transform;

    // 角色控制器组件
    CharacterController ch;

    // 动画组件
    Animator ani;

    // 角色移动速度
    float movSpeed = 3.0f;
    // 角色跑步加速系数
    float runSpeed_ratio = 1.5f;
    // 重力
    float gravity = 20.0f;

    //起跳速度
    float jumpSpeed = 9.0f;
    //当前y速度
    float nowSpeed = 0.0f;
    //弹跳标记
    bool isjump = false;
    // 摄像机Transform
    Transform camTransform;
    // 寻路组件
    UnityEngine.AI.NavMeshAgent agent;

    // 生命值
    public int max_life = 10;
    public int now_life;
    //攻击属性

    float attack1_distance=3f;
    float attack2_distance=3f;
    int attack1_damage=1;
    int attack2_damage=2;
    //是否收到伤害,死亡
    bool isgethit=false,isdie=false;
    //伤害判定
    bool isdamage=false;

    //输入变量
    bool input_a,input_d,input_s,input_w,input_q,input_e,input_jump,input_shift,input_leftbutton;
    //定义3个值控制移动
    float xm = 0, ym = 0, zm = 0;
    //水平移动基数
    float horizontal=0;
    //鼠标点击地形位置
    Vector3 end_position;
    //动画信息
    AnimatorStateInfo stateInfo;
     // 计时器
    float timer ;

    void Start()
    {
        m_transform = this.transform;
        // 获取角色控制器组件
        ch = this.GetComponent<CharacterController>();
        // 获得动画播放器
        ani = this.GetComponent<Animator>();
        // 获得寻路组件
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        camTransform =  GameObject.FindGameObjectWithTag ("MainCamera").transform;
        // camTransform =  Camera.main.transform;

        now_life=max_life;

        end_position=m_transform.position;
    }

    void Update()
    {
        UpdateValue();
       
        UpdatePosition();

        UpdateAnimation();
    }
    //更新各个值
    void UpdateValue() 
    {
        stateInfo = ani.GetCurrentAnimatorStateInfo(0);

        //输入
        input_a=false;
        input_d=false;
        input_w=false;
        input_s=false;
        input_q=false;
        input_e=false;
        input_jump=false;
        input_shift=false;
        input_leftbutton=false;

        xm = 0;
        ym = 0;
        zm = 0;

        if (Input.GetKey(KeyCode.A))
        {
            input_a = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            input_d = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            input_w= true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            input_s = true;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            input_q = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            input_e = true;
        }
        if (Input.GetButton("Jump"))
        {
            input_jump = true;
        }
        if ((Input.GetKey("left shift") || Input.GetKey("right shift")) )
        {
            input_shift = true;
        }
        if(Input.GetButton("Mouse X")){
            input_leftbutton = true;
        }
    }
    //更新角色位置,方向
    void UpdatePosition()
    {
        //前后左右(优先左上)
        if(input_a)
        {
            xm=-1;
            horizontal=1;
        }
        else if(input_d)
        {
            xm=1;
            horizontal=1;
        }
        //if(input_w)
        //{
        //    zm=1;
        //    horizontal=1;
        //}
        //else if(input_s)
        //{
        //    zm=-1;
        //    horizontal=1;
        //}
        //获取鼠标点击位置
        // if(input_leftbutton){
        //     Getendposition();
        //     agent.SetDestination(end_position);
        // }
        //依据摄像头朝向和走路方向设置人物朝向
        Vector3 camrot = camTransform.eulerAngles;
        //方法一（没有转向动作）
        // camrot.x = 0;
        // camrot.z = 0;
        // if(zm == 0 && xm == -1)
        // {
        //     camrot.y = camrot.y-90;
        // }
        // else if(zm == 0&& xm == 1)
        // {
        //     camrot.y = camrot.y+90;
        // }
        // else if(zm == 0&& xm == 0)
        // {
        //     camrot.y = camrot.y;
        // }
        // else if(zm == -1&& xm == 0)
        // {
        //     camrot.y = camrot.y+180;
        // }
        // else
        // {
        //     camrot.y = camrot.y+Mathf.Acos(xm/zm);
        // }
        // if(xm != 0 || zm != 0){
        //     transform.eulerAngles = camrot;
        // }
        //方法二（有转向动作）
        Vector3 oriV=new Vector3(xm,0,zm);
        if(xm != 0 || zm != 0){
            // 计算出新方向
            //Quaternion.AngleAxis(camrot.y,Vector3.up)*oriV是oriV绕着y轴旋转相机的y轴偏角
             Vector3 newDir = Vector3.RotateTowards(m_transform.forward, Quaternion.AngleAxis(camrot.y,Vector3.up)*oriV, 20f * Time.deltaTime, 0.0f);
            // 旋转至新方向
            m_transform.rotation = Quaternion.LookRotation(newDir);
        }
        //重力运动
        nowSpeed -= gravity * Time.deltaTime;
        //跳跃
        if (ch.isGrounded&&nowSpeed<=0)
        {
            if (isjump == true)
            {
                nowSpeed = 0.0f;
                isjump = false;
            }
            if (input_jump)
            {
                nowSpeed = jumpSpeed;
                isjump = true;
            }
        }
        if (nowSpeed!=0)//在天上
        {
            ym += nowSpeed * Time.deltaTime;
 
        }
        if(stateInfo.fullPathHash != Animator.StringToHash("Base Layer.Walk") && stateInfo.fullPathHash != Animator.StringToHash("Base Layer.Run"))
        {
           horizontal=0;
        }
        if(input_shift)
        {
            //水平方向
            ch.Move(horizontal*m_transform.forward*Time.deltaTime*movSpeed*runSpeed_ratio);
            //垂直方向
            ch.Move(transform.TransformDirection(new Vector3(0, ym, 0)));
        }
        else{
            //水平方向
            ch.Move(horizontal*m_transform.forward*Time.deltaTime*movSpeed);
            //垂直方向
            ch.Move(transform.TransformDirection(new Vector3(0, ym, 0)));
        }
        //维持x坐标为0
        m_transform.position= new Vector3(0, m_transform.position.y,m_transform.position.z);
    }
    //更新动画
    void UpdateAnimation()
    {
        // 如果处于待机
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Idle") && !ani.IsInTransition(0))
        {
            ani.SetBool("idle", false);
            //受击,攻击1,攻击2，跑步，走路
            if(isgethit)
            {
                ani.SetBool("gethit",true);
            }
            else if(input_q)
            {
                ani.SetBool("attack1", true);
                isdamage=false;
            }
            else if(input_e)
            {
                ani.SetBool("attack2", true);
                isdamage=false;
            }
            else if((input_a||input_d)&&input_shift){
                ani.SetBool("run",true);
            }
            else if((input_a||input_d)&&!input_shift){
                ani.SetBool("walk",true);
            }
        }
        // 如果处于走路
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Walk") && !ani.IsInTransition(0))
        {
            ani.SetBool("walk", false);
            //受击,攻击1,攻击2,跑步,静止
            if(isgethit)
            {
                ani.SetBool("gethit",true);
            }
            else if(input_q)
            {
                ani.SetBool("attack1", true);
                isdamage=false;
            }
            else if(input_e)
            {
                ani.SetBool("attack2", true);
                isdamage=false;
            }
            else if((input_a||input_d||input_w||input_s)&&input_shift){
                ani.SetBool("run",true);
            }
            else if(!(input_a||input_d||input_w||input_s)){
                ani.SetBool("idle",true);
            }
        }
        // 如果处于跑步
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Run") && !ani.IsInTransition(0))
        {
            ani.SetBool("run", false);
            //受击,攻击1,攻击2,跑步,走路,静止
            if(isgethit)
            {
                ani.SetBool("gethit",true);
            }
            else if(input_q)
            {
                ani.SetBool("attack1", true);
                isdamage=false;
            }
            else if(input_e)
            {
                ani.SetBool("attack2", true);
                isdamage=false;
            }
            else if((input_a||input_d||input_w||input_s)&&input_shift){
                ani.SetBool("run",true);
            }
            else if((input_a||input_d||input_w||input_s)){
                ani.SetBool("walk",true);
            }
            else if(!(input_a||input_d||input_w||input_s)){
                ani.SetBool("idle",true);
            }
        }
        // 如果处于攻击1
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Attack1") && !ani.IsInTransition(0))
        {
             ani.SetBool("attack1", false);

             //受击，完成静止
            if(isgethit)
            {
                ani.SetBool("gethit",true);
            }
            else if(stateInfo.normalizedTime >= 0.5&&isdamage==false){
                isdamage=true;
                Damage_damage(attack1_distance,attack1_damage);
            }
            else if(stateInfo.normalizedTime >= 1.0)
            {
                Debug.Log("attack1 finish");
                 ani.SetBool("idle",true);
            }
        }
        // 如果处于攻击2
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.Attack2") && !ani.IsInTransition(0))
        {
             ani.SetBool("attack2", false);
             //受击，完成静止
            if(isgethit)
            {
                ani.SetBool("gethit",true);
            }
            else if(stateInfo.normalizedTime >= 0.5&&isdamage==false){
                isdamage=true;
                Damage_damage(attack2_distance,attack2_damage);
            }
            else if(stateInfo.normalizedTime >= 1.0)
            {
                 ani.SetBool("idle",true);
            }
        }
        // 如果处于被攻击
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.GetHit") && !ani.IsInTransition(0))
        {
             ani.SetBool("gethit", false);
             isgethit=false;
            if(isdie){
                ani.SetBool("die",true);
            }
             //受击，完成静止
            else if(stateInfo.normalizedTime >= 0.5)
            {
                ani.SetBool("idle",true);
            }
        }
    }
    //受到攻击
    public void GetGit(int damage)
    {
        this.now_life-=damage;
        isgethit=true;
        if(this.now_life<=0){
            isdie=true;
            //当角色死亡传递重新开始信号
            GameManager.Instance.restartflag = true;
        }
    }
    //对扇形所有敌人造成伤害
    public void Damage_damage(float distance,int damage){
        GameObject[] enemys;         
        enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            Vector3 deltaA = enemy.transform.position - m_transform.position;   
            float   forwardDotA= Vector3.Dot(transform.forward, deltaA);
            //正向且在范围内
            if (forwardDotA > 0 &&  Vector3.Distance( enemy.transform.position, m_transform.position) <= distance)
            {
                BaseEnemy theenemy = enemy.transform.GetComponent<BaseEnemy>();
                theenemy.OnDamage(damage);
                //怪物显血模块
                GameManager.Instance.Setenemylife(theenemy.m_life, theenemy.max_life);
                GameManager.Instance.Setenemyimage(theenemy.enemytype);
            }
        }

    }

    //获取鼠标点击位置
    // void Getendposition()
    // {
    //     Vector3 cursorScreenPosition=Input.mousePosition;//鼠标在屏幕上的位置
    //     Ray ray=Camera.main.ScreenPointToRay(cursorScreenPosition);//在鼠标所在的屏幕位置发出一条射线(暂名该射线为x射线)
    //     RaycastHit hit;
    //     if(Physics.Raycast(ray,out hit)){
    //     if(hit.collider.gameObject.tag=="Terrain"){//设置地形Tag为Terrain
    //         end_position = hit.point;
    //     }
    //     }
    // }
}
