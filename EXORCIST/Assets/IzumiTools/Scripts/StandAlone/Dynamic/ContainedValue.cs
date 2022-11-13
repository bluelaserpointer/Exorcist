using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzumiTools
{
    public class ContainedValue : MonoBehaviour
    {
        private float _value, _max;
        public float Value
        {
            get => _value;
            set => _value = Mathf.Clamp(value, 0, _max);
        }
        public float Max
        {
            get => _max;
            set
            {
                _max = Mathf.Max(0, value);
                if (_value > _max)
                    _value = _max;
            }
        }
        public float Ratio
        {
            get => _value / _max;
            set => _value = _max * Mathf.Clamp01(value);
        }
        public float AddAndReturnOverflow(float addition)
        {
            float newValue = _value + addition;
            float overflow = 0;
            if (addition > 0)
            {
                if (newValue > _max)
                {
                    _value = _max;
                    overflow = newValue - _max;
                }
                else
                    _value = newValue;
            }
            else
            {
                if (newValue < 0)
                {
                    _value = 0;
                    overflow = newValue;
                }
                else
                    _value = newValue;
            }
            return overflow;
        }
    }

}