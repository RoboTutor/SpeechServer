using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileThink : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "Think"; }
        }

        public BehaviorProfileThink()
        {
            this.Parameters.Add("Width",  new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadAmplitude", new Parameter(0, "Pose"));

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
            double width = Parameters["Width"].Value;
            double height = Parameters["Height"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headamp = Parameters["HeadAmplitude"].Value;

            double shldroll = -28 * width+18;
            double shldptch = -95 * height + 35;
            double elbyaw = 35;
            double elbroll = -23.5 * height + 88.5;
            double wrstyaw = 45 * height + 30;

            // finger 0~1
            double hand = 0.8 * finger;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);

            // - ~  degree
            double headYaw_L = 18 * headamp + 18;
            double headYaw_R = -14 * headamp - 14;

            RArm rarm = new RArm(false, shldptch, shldroll, elbyaw, elbroll, wrstyaw, hand);
            Head head_l = new Head(false, headPitch, headYaw_L);
            Head head_r = new Head(false, headPitch, headYaw_R);

            // Joints
            PoseProfile pose_arm = new PoseProfile("ArmPose", rarm);
            PoseProfile pose_head_l = new PoseProfile("HeadPoseLeft", head_l, true, 1, 0);
            PoseProfile pose_head_r = new PoseProfile("HeadPoseRight", head_r, true, 1, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_arm);
            lpp.Add(pose_head_l);
            lpp.Add(pose_head_r);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.0075 * valence + 0.125;
            double rep;
            if (valence >=8) rep = 1; // valence=8,rep=1;
            else rep = 0;

            // head
            double headud = 0.1 * valence;
            double headamp = 0.05 * valence + 0.5;

            // pose
            Parameters["Height"].Value = height;
            Parameters["Width"].Value = width;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadAmplitude"].Value = headamp;
            // motion
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
