using UnityEngine;

namespace IzumiTools
{
    public static class ExtendedMath
    {
        public static float RotateWithClamp(float currentValue, float deltaAngle, float minAngle, float maxAngle)
        {
            currentValue += deltaAngle;
            if (currentValue < minAngle && maxAngle < currentValue)
            {
                currentValue = deltaAngle > 0 ? maxAngle : minAngle;
            }
            return currentValue;
        }
        /// <summary>
        /// Get optimal velocity stop at specific distance without overswing.<br/>
        /// In practice, it should be multiplied safe factor (around 0.9) to prevent accumulated error causing overswing.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="maxVelocityChange"></param>
        /// <returns></returns>
        public static float OptimalVelocityStopAt(float distance, float maxVelocityChange)
        {
            return (distance > 0 ? 1 : -1) * Mathf.Sqrt(2 * maxVelocityChange * Mathf.Abs(distance));
        }
        public static bool EstimateBulletHitTime(Vector3 firePosition, float bulletSpeed, Vector3 targetPosition, Vector3 targetSpeed, out float time)
        {
            Vector3 deltaPosition = targetPosition - firePosition;
            float a = bulletSpeed - targetSpeed.sqrMagnitude;
            float b = -2 * Vector3.Dot(targetSpeed, deltaPosition);
            float c = -deltaPosition.sqrMagnitude;
            float delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                time = -1;
            }
            else if (delta == 0)
            {
                time = -b / (2 * a);
            }
            else
            {
                float sqrtDelta = Mathf.Sqrt(delta);
                if ((time = (-b - sqrtDelta) / (2 * a)) < 0)
                    time = (-b + sqrtDelta) / (2 * a);
            }
            return time >= 0;
        }
    }
}