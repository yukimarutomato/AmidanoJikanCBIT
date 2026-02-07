using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharaStatus : MonoBehaviour
{
   
  
        public int Money { get; private set; }
        public int Health { get; private set; }

        public CharaStatus(StartJson.CharaData data)
        {
            Money = data.money;
            Health = data.health;
        }

        public void AddMoney(int value)
        {
            Money += value;
        }

        public void AddHealth(int value)
        {
            Health -= value;
            if (Health < 0) Health = 0;
        }
    
}
