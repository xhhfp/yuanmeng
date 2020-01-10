using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2 : BaseEnemy
{

    void Start()
    {

        this.Init();
        m_gold = 200;
        enemytype = "Monster2";
    }
    void Update()
    {
        this.basecontrl();
    }


}
