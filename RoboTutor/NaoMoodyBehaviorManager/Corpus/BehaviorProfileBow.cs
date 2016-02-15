using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileBow : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "Bow"; }
        }

        public BehaviorProfileBow()
        {
            this.Parameters.Add("FrontPoseWidth",  new Parameter(0, "Pose"));
            this.Parameters.Add("FrontPoseHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("ChestPoseHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

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
            double fpwidth  = Parameters["FrontPoseWidth"].Value;
            double fpheight = Parameters["FrontPoseHeight"].Value;
            double cpheight = Parameters["ChestPoseHeight"].Value;
            double finger   = Parameters["Finger"].Value;
            double headud   = Parameters["HeadUpDown"].Value;
            double headlr   = Parameters["HeadLeftRight"].Value;

            double fp_shldroll = -30*fpwidth;
            double fp_shldptch = -30*fpheight+60;
            double fp_elbyaw   = 89.5*fpwidth+30;
            double fp_elbroll  = 30*fpheight+30;
            double fp_wrstyaw  = 30;

            double cp_shldroll = 5;
            double cp_shldptch = -30*cpheight+60;
            double cp_elbyaw   = -20*cpheight+30;
            double cp_elbroll  = 88.5;
            double cp_wrstyaw  = 30;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;
            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // - ~  degree
            double headYaw;
            if (headlr > 0.5) headYaw = -40 * headlr + 20;
            else headYaw = 0;

            RArm rarm_front = new RArm(false, fp_shldptch, fp_shldroll, fp_elbyaw, fp_elbroll, fp_wrstyaw, hand);
            Head head       = new Head(false, headPitch, headYaw);
            RArm rarm_chest = new RArm(false, cp_shldptch, cp_shldroll, cp_elbyaw, cp_elbroll, cp_wrstyaw, hand);

            LArm larm = new LArm(false, 84.6, 20, -68, -50, 5.7, 0.3);

            // Joints
            PoseProfile pose_front_arm_r = new PoseProfile("FrontPose", rarm_front, true);
            PoseProfile pose_front_head  = new PoseProfile("FrontPose", head);
            PoseProfile pose_front_arm_l = new PoseProfile("FrontPose", larm);
            PoseProfile pose_chest_arm_r = new PoseProfile("ChestPose", rarm_chest, true);

            List<PoseProfile> lppbow = BendTorso("FrontPose");

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_front_arm_r);
            lpp.Add(pose_front_head);
            lpp.Add(pose_front_arm_l);
            lpp.Add(pose_chest_arm_r);
            lpp.AddRange(lppbow);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double fpwidth = 0.05 * valence + 0.5;
            double fpheight = 0.05 * valence + 0.5;
            double cpheight = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.005*valence+0.25;
            double decspd = motspd;
            double holdtime = 0.0045*valence+0.055;
            double rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["FrontPoseWidth"].Value = fpwidth;
            Parameters["FrontPoseHeight"].Value = fpheight;
            Parameters["ChestPoseHeight"].Value = cpheight;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
