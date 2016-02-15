using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// understand, emphasize
    /// ReverseCapisce (originally is left arm)
    /// </summary>
    public class BehaviorProfileState : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "State"; }
        }

        public BehaviorProfileState()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose")); // arm, head
            this.Parameters.Add("UpHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("DownHeight", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
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
            double width      = Parameters["Width"].Value;
            double upheight   = Parameters["UpHeight"].Value;
            double downheight = Parameters["DownHeight"].Value;
            double amplitude  = Parameters["Amplitude"].Value;
            double palmdir    = Parameters["PalmDirection"].Value;
            double finger     = Parameters["Finger"].Value;
            double headud     = Parameters["HeadUpDown"].Value;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;
            double wristyaw = 90 * palmdir;

            // Up pose center width
            double up_shldroll = -30 * width;
            double up_elbyaw = 90;
            // Up pose center height
            double up_shldpitch = -30 * upheight + 30;
            double up_elbroll = 88.5;
            // Up pose palm dir
            double up_wristyaw = wristyaw;
            double up_hand = hand;

            // Down pose center width
            double dp_shldroll = -30 * width;
            double dp_elbyaw = 90;
            // Down pose center height
            double dp_shldpitch = -50 * downheight + 90;
            double dp_elbroll = -20 * downheight + 60;
            // Down pose palm dir
            double dp_wristyaw = wristyaw;
            double dp_hand = 0.6;

            double delta_shldroll = 10 * amplitude + 10;
            double delta_elbyaw = 10 * amplitude + 19.5;
            double delta_elbroll = delta_shldroll / 2;
            // down pose left (to robot)
            double dpl_shldroll = dp_shldroll + delta_shldroll;
            double dpl_elbyaw = dp_elbyaw - delta_elbyaw;
            double dpl_shldpitch = dp_shldpitch;
            double dpl_elbroll = dp_elbroll + delta_elbroll;
            double dpl_wristyaw = dp_wristyaw;
            double dpl_hand = hand;
            double headYawL = 10 * amplitude + 5;
            // down pose right (to robot)
            double dpr_shldroll = dp_shldroll - delta_shldroll;
            double dpr_elbyaw = dp_elbyaw + delta_elbyaw;
            double dpr_shldpitch = dp_shldpitch;
            double dpr_elbroll = dp_elbroll - delta_elbroll;
            double dpr_wristyaw = dp_wristyaw;
            double dpr_hand = hand;
            double headYawR = -10 * amplitude - 15;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);

            RArm rarm_up = new RArm(false, up_shldpitch, up_shldroll, up_elbyaw, up_elbroll, up_wristyaw, up_hand);
            RArm rarm_down = new RArm(false, dp_shldpitch, dp_shldroll, dp_elbyaw, dp_elbroll, dp_wristyaw, dp_hand);
            RArm rarm_down_left = new RArm(false, dpl_shldpitch, dpl_shldroll, dpl_elbyaw, dpl_elbroll, dpl_wristyaw, dpl_hand);
            Head headL = new Head(false, headPitch, headYawL);
            RArm rarm_down_right = new RArm(false, dpr_shldpitch, dpr_shldroll, dpr_elbyaw, dpr_elbroll, dpr_wristyaw, dpr_hand);
            Head headR = new Head(false, headPitch, headYawR);

            // Joints
            PoseProfile pose_rarm_up = new PoseProfile("UpPose", rarm_up);
            PoseProfile pose_rarm_down = new PoseProfile("DownPose", rarm_down);
            PoseProfile pose_rarm_downL = new PoseProfile("DownPoseLeft", rarm_down_left);
            PoseProfile pose_head_l = new PoseProfile("DownPoseLeft", headL);
            PoseProfile pose_rarm_downR = new PoseProfile("DownPoseRight", rarm_down_right);
            PoseProfile pose_head_r = new PoseProfile("DownPoseRight", headR);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_rarm_up);
            lpp.Add(pose_rarm_down);
            lpp.Add(pose_rarm_downL);
            lpp.Add(pose_head_l);
            lpp.Add(pose_rarm_downR);
            lpp.Add(pose_head_r);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double upheight = 0.05 * valence + 0.5;
            double downheight = 0.05 * valence + 0.5;
            double amplitude = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = 0.035 * valence + 0.65;
            double holdtime = 0.025 * valence + 0.25;
            double rep = 0;
            // head
            double headud = 0.1 * valence;

            Parameters["Width"].Value = width;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["DownHeight"].Value = downheight;
            Parameters["UpHeight"].Value = upheight;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            // motion
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
