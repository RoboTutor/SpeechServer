using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileStandHead : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "StandHead"; }
        }

        public BehaviorProfileStandHead()
        {
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));

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
            double headud   = Parameters["HeadUpDown"].Value;

            double headpitch = base.NormalizeHeadPitch(headud);

            Head head = new Head(false, headpitch, 0);

            // Joints
            PoseProfile pose_head  = new PoseProfile("Pose", head);

            List<PoseProfile> legpp = base.Stand("Pose");
            List<PoseProfile> armpp = base.StandArmPose("Pose");

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.AddRange(legpp);
            lpp.AddRange(armpp);

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
            // head
            double headud = 0.1 * valence;

            Parameters["HeadUpDown"].Value = headud;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
