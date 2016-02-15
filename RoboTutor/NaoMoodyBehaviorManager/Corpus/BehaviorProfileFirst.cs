using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// Corresponding text: "First; start"
    /// </summary>
    public class BehaviorProfileFirst : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "First"; }
        }

        public BehaviorProfileFirst()
        {
            this.Parameters.Add("RaisePoseWidth", new Parameter(0, "Pose"));
            this.Parameters.Add("RaisePoseHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
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
            double rpwidth = Parameters["RaisePoseWidth"].Value;
            double rpheight = Parameters["RaisePoseHeight"].Value;
            double palmdir = Parameters["PalmDirection"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headlr = Parameters["HeadLeftRight"].Value;

            // Raise pose center width
            double rp_shldroll = -30 * rpwidth;
            double rp_elbyaw = 90;
            double rp_shldpitch = -30 * rpheight + 30;
            double rp_elbroll = 88.5;
            double rp_wristyaw = -110 * palmdir + 90;
            double rp_hand = 0.4 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // - ~  degree
            double headYaw = 0;
            if (headlr > 0.5) headYaw = -40 * headlr + 20;
            else headYaw = 0;

            RArm rarm_front = new RArm(false, rp_shldpitch, rp_shldroll, rp_elbyaw, rp_elbroll, rp_wristyaw, rp_hand);
            Head head = new Head(false, headPitch, headYaw);
            // Joints
            PoseProfile pose_front_arm_r = new PoseProfile("RaisePose", rarm_front);
            PoseProfile pose_front_head = new PoseProfile("RaisePose", head);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_front_arm_r);
            lpp.Add(pose_front_head);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double raiseposewidth = 0.05 * valence + 0.5;
            double raiseposeheight = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.0075 * valence + 0.125;
            double rep = 0;

            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["RaisePoseWidth"].Value = raiseposewidth;
            Parameters["RaisePoseHeight"].Value = raiseposeheight;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            // motion
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
