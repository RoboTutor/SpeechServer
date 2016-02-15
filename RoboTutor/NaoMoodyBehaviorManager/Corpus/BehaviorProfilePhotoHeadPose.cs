using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfilePhotoHeadPose : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "PhotoHeadPose"; }
        }

        public BehaviorProfilePhotoHeadPose()
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
            // TODO: Empirical data, need to be tested
            double headpitch = 12;

            Head head = new Head(false, headpitch, 0);

            PoseProfile pose_head = new PoseProfile("Pose", head);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            double motspd = 0.2;
            double decspd = 0.2;
            double holdtime = 0;
            // Permanently hold
            this.BehaviorRecover = false;
            double rep = 0;

            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
