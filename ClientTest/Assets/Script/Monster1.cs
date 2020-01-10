using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1 : BaseEnemy
{
   
    void Start()
    {
    
        this.Init();

        enemytype="Monster1";
    }
    void Update()
    {
        this.basecontrl();
    }
  
  
}
