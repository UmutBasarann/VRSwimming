using System;
using Avataris.VR.Scripts.Data.Core;

namespace Avataris.VR.Scripts.Data.Swim
{
    [Serializable]
    public class SwimData: IData
    {
        public float swimForce;
        public float dragForce;
        public float minForce;
        public float minTimeBetweenStrokes;
    }
}
