using System;
using UnityEngine;

namespace IzumiTools
{
    /// <summary>
    /// Manual addition cooldown manager.
    /// </summary>
    [Serializable]
    public class Cooldown
    {
        //inspector
        [Min(0)]
        public float requiredValue;
        //

        public Cooldown(float requiredValue)
        {
            this.requiredValue = requiredValue;
        }
        public Cooldown()
        {
            requiredValue = 1;
        }

        [HideInInspector]
        public float value;
        [HideInInspector]
        public bool autoClamp = true;
        public float Ratio
        {
            get => value / requiredValue;
            set => this.value = requiredValue * value;
        }
        public float OneMinusRatio
        {
            get => 1 - Ratio;
            set => this.value = requiredValue * (1 - value);
        }
        public bool IsReady => value >= requiredValue;
        public void Reset()
        {
            value = 0;
        }
        public bool Eat()
        {
            if (IsReady)
            {
                Reset();
                return true;
            }
            return false;
        }
        public void Add(float value)
        {
            this.value += value;
            if (autoClamp)
                this.value = Mathf.Clamp(this.value, 0, requiredValue);
        }
        public void AddDeltaTime()
        {
            Add(Time.deltaTime);
        }
        public void AddFixedDeltaTime()
        {
            Add(Time.fixedDeltaTime);
        }
        public bool AddAndEat(float value)
        {
            Add(value);
            return Eat();
        }
        public bool AddDeltaTimeAndEat()
        {
            AddDeltaTime();
            return Eat();
        }
        public bool AddFixedDeltaTimeAndEat()
        {
            AddFixedDeltaTime();
            return Eat();
        }
    }
}