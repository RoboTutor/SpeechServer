using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileShowMic : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "ShowMic"; }
        }

        public BehaviorProfileShowMic()
        {
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(new List<double>(), "Motion"));
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
            double headud = Parameters["HeadUpDown"].Value;
            double headpitch = -15 * headud;

            Head head = new Head(false, headpitch, 0);

            // front
            List<PoseProfile> lpp1 = RightArmPointFrontMic("FrontMic");
            PoseProfile head_pose = new PoseProfile("FrontMic", head);
            // side
            List<PoseProfile> lpp2 = RightArmPointSideMic("SideMic");
            // rear
            List<PoseProfile> lpp3 = RightArmPointRearMic("RearMic");

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.AddRange(lpp1);
            lpp.Add(head_pose);
            lpp.AddRange(lpp2);
            lpp.AddRange(lpp3);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.5;
            double rep = 0;

            double headud = 0.05 * valence + 0.5;

            Parameters["HeadUpDown"].Value = headud;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["HoldTime"].ValueList.Add(0.2);
            Parameters["HoldTime"].ValueList.Add(0.3);
            Parameters["Repetition"].Value = rep;
        }
    }
}
