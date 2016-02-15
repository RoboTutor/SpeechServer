using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// leg wobble left and right
    /// </summary>
    public class BehaviorProfileBalance : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Balance"; }
        }

        public BehaviorProfileBalance()
        {
            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
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

        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            List<PoseProfile> lpp1 = StandOnRight("StandRight");
            List<PoseProfile> lpp2 = StandOnLeft("StandLeft");
            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.AddRange(lpp1);
            lpp.AddRange(lpp2);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            double motspd = -0.002 * valence + 0.06; // 0.08
            double decspd = 0.06;
            double holdtime = 0.2;
            double rep = 0;

            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
