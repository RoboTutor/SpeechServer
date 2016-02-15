using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{

    public class BehaviorProfileYouAndMe : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "YouAndMe"; }
        }

        public BehaviorProfileYouAndMe()
        {
            this.Parameters.Add("FrontPoseWidth", new Parameter(0, "Pose"));
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

            double fp_shldroll = 28 * fpwidth - 10;
            double fp_shldptch = -50 * fpheight + 60;
            double fp_elbyaw   = 5 * fpwidth + 85;
            double fp_elbroll  = -35 * fpheight + 50;
            double fp_wrstyaw  = 60 * fpheight + 30;

            double cp_shldroll = -5 * cpheight + 5;
            double cp_shldptch = -40 * cpheight + 60;
            double cp_elbyaw   = -15 * cpheight + 15;
            double cp_elbroll  = 15 * cpheight + 70;
            double cp_wrstyaw = 59.5 * cpheight + 45;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;
            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            //  ~  degree
            double headYaw;
            if (headlr > 0.5) headYaw = -20 * headlr + 10;
            else headYaw = 0;

            RArm rarm_front = new RArm(false, fp_shldptch, fp_shldroll, fp_elbyaw, fp_elbroll, fp_wrstyaw, hand);
            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_chest = new RArm(false, cp_shldptch, cp_shldroll, cp_elbyaw, cp_elbroll, cp_wrstyaw, hand);

            // Joints
            PoseProfile pose_front_arm_r = new PoseProfile("FrontPose", rarm_front);
            PoseProfile pose_front_head = new PoseProfile("FrontPose", head);
            PoseProfile pose_chest_arm_r = new PoseProfile("ChestPose", rarm_chest);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_front_arm_r);
            lpp.Add(pose_front_head);
            lpp.Add(pose_chest_arm_r);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double fpwidth       = 0.05 * valence + 0.5;
            double fpheight      = 0.05 * valence + 0.5;
            double cpheight      = 0.05 * valence + 0.5;
            double finger        = 0.05 * valence + 0.5;
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            // motion
            double motspd        = 0.035 * valence + 0.65;
            double decspd        = motspd;
            double holdtime      = 0.0075 * valence + 0.125;
            double rep = 0;

            // pose
            Parameters["FrontPoseWidth"].Value  = fpwidth;
            Parameters["FrontPoseHeight"].Value = fpheight;
            Parameters["ChestPoseHeight"].Value = cpheight;
            Parameters["Finger"].Value          = finger;
            Parameters["HeadUpDown"].Value      = headud;
            Parameters["HeadLeftRight"].Value   = headlr;
            // motion
            Parameters["MotionSpeed"].Value     = motspd;
            Parameters["DecaySpeed"].Value      = decspd;
            Parameters["HoldTime"].Value        = holdtime;
            Parameters["Repetition"].Value      = rep;
        }
    }
 
}
