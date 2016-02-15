using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaoManager
{
    /// <summary>
    /// LED behavior
    /// </summary>
    public class BehaviorProfileTwinkle : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { throw new NotImplementedException(); }
        }

        public override ConnectorNao.MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();
        }

        public override ConnectorNao.MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        }

        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            throw new NotImplementedException();
        }

        protected override ConnectorNao.MotionTimeline CoreParameterToMotion(bool right = true)
        {
            throw new NotImplementedException();
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            throw new NotImplementedException();
        }

        protected override List<ConnectorNao.PoseProfile> CoreParameterToPose(bool right = true)
        {
            throw new NotImplementedException();
        }
    }
}
