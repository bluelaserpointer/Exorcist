namespace IzumiTools
{
    [System.Serializable]
    public class Timestamp
    {
        protected float lastStampTime;
        public float LastStampTime => lastStampTime;
        public float PassedTime => UnityEngine.Time.timeSinceLevelLoad - LastStampTime;
        public bool isStamped;
        public virtual void Stamp()
        {
            lastStampTime = UnityEngine.Time.timeSinceLevelLoad;
            isStamped = true;
        }
    }
}