using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileClap : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Clap"; }
        }

        public BehaviorProfileClap()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

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
            double width  = Parameters["Width"].Value;
            double height = Parameters["Height"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headlr = Parameters["HeadLeftRight"].Value;

            double lsr = 30 * width;
            double rsr = -30 * width;
            double lsp = 10 * height + 60;
            double rsp = -55 * height + 20;
            double ley = -10 * height - 50;
            double rey = 20 * height + 60;
            double ler = -45;
            double rer = -15 * height + 75;
            double lwy = 10 * height - 100;
            double rwy = 30 * height - 30;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = double.NaN;

            LArm larm = new LArm(false, lsp, lsr, ley, ler, lwy, hand);
            RArm rarm = new RArm(false, rsp, rsr, rey, rer, rwy, hand);
            Head head = new Head(false, headPitch, headYaw);
            LArm larm_close = new LArm(false, 60, -15, -20, -55, -104.5, hand);
            RArm rarm_close = new RArm(false, 35, 15, 20, 75, -40, hand);

            // Joints
            PoseProfile pose_open_arm_l = new PoseProfile("ArmOpen", larm, true, 0.3, 0);
            PoseProfile pose_open_arm_r = new PoseProfile("ArmOpen", rarm, true, 0.3, 0);
            PoseProfile pose_open_head = new PoseProfile("ArmOpen", head);
            PoseProfile pose_close_arm_l = new PoseProfile("ArmClose", larm_close, true, 1, 0);
            PoseProfile pose_close_arm_r = new PoseProfile("ArmClose", rarm_close, true, 1, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_open_arm_l);
            lpp.Add(pose_open_arm_r);
            lpp.Add(pose_open_head);
            lpp.Add(pose_close_arm_l);
            lpp.Add(pose_close_arm_r);

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
            double decspd = 0.035 * valence + 0.65;
            double holdtime = 0.02 * valence + 0.3;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
