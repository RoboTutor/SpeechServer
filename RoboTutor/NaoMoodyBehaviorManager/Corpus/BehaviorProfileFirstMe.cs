using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileFirstMe : BehaviorProfileBase
    {
        /// <summary>
        /// Corresponding text: "First, I ..."
        /// </summary>
        public override string BehaviorName
        {
            get { return "FirstMe"; }
        }

        public BehaviorProfileFirstMe()
        {
            this.Parameters.Add("RaisePoseWidth", new Parameter(0, "Pose"));
            this.Parameters.Add("RaisePoseHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("ChestPoseHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
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
            double rpwidth = Parameters["RaisePoseWidth"].Value;
            double rpheight = Parameters["RaisePoseHeight"].Value;
            double cpheight = Parameters["ChestPoseHeight"].Value;
            double palmdir = Parameters["PalmDirection"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headlr = Parameters["HeadLeftRight"].Value;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;

            // Raise pose center width
            double rp_shldroll = -30 * rpwidth;
            double rp_elbyaw = 90;
            double rp_shldpitch = -30 * rpheight + 30;
            double rp_elbroll = 88.5;
            double rp_wristyaw = -110 * palmdir + 90;

            double cp_shldroll = 5;
            double cp_shldptch = -30 * cpheight + 60;
            double cp_elbyaw = -20 * cpheight + 30;
            double cp_elbroll = 88.5;
            double cp_wrstyaw = 64.5 * palmdir + 40;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // - ~  degree
            double headYaw = 0;
            if (headlr > 0.5) headYaw = -40 * headlr + 20;
            else headYaw = 0;

            RArm rarm_front = new RArm(false, rp_shldpitch, rp_shldroll, rp_elbyaw, rp_elbroll, rp_wristyaw, hand);
            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_chest = new RArm(false, cp_shldptch, cp_shldroll, cp_elbyaw, cp_elbroll, cp_wrstyaw, hand);

            // Joints
            PoseProfile pose_front_arm_r = new PoseProfile("RaisePose", rarm_front);
            PoseProfile pose_front_head = new PoseProfile("RaisePose", head);
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
            double raiseposewidth = 0.05 * valence + 0.5;
            double raiseposeheight = 0.05 * valence + 0.5;
            double chestposeheight = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.015 * valence + 0.25;
            double holdraise = holdtime;
            double holdchest = holdtime * 0.8;
            double rep = 0;

            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["RaisePoseWidth"].Value = raiseposewidth;
            Parameters["RaisePoseHeight"].Value = raiseposeheight;
            Parameters["ChestPoseHeight"].Value = chestposeheight;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            // motion
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(holdraise);
            Parameters["HoldTime"].ValueList.Add(holdchest);
            Parameters["Repetition"].Value = rep;
        }
    }
}
