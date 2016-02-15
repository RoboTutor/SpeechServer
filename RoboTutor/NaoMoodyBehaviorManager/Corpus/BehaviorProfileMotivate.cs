using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// 
    /// </summary>
    public class BehaviorProfileMotivate : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Motivate"; } 
        }

        public BehaviorProfileMotivate()
        {
            Parameters.Add("Width", new Parameter(0, "Pose"));
            Parameters.Add("Height", new Parameter(0, "Pose"));
            Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            Parameters.Add("PalmDir", new Parameter(0, "Pose"));
            Parameters.Add("Finger", new Parameter(0, "Pose"));
            //
            Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));
            //
            Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            Parameters.Add("Repetition", new Parameter(0, "Motion"));
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
            double width     = Parameters["Width"].Value;
            double height    = Parameters["Height"].Value;
            double amplitude = Parameters["Amplitude"].Value;
            double palmdir   = Parameters["PalmDir"].Value; 
            double finger    = Parameters["Finger"].Value;
            double headud    = Parameters["HeadUpDown"].Value;
            double headlr    = Parameters["HeadLeftRight"].Value;

            // width
            double shldroll = -10 * width;
            double elbyaw = -10 * width + 80;
            double shldptch = -20 * height + 80;
            double elbroll = -10 * height + 70;
            double wristyaw = 10 * palmdir;
            double hand = 0.3 * finger + 0.5;

            double delta_elbroll = 8 * amplitude + 10;
            double delta_shldptch = 8 * amplitude + 8;

            // high pose
            double shldptch_high = shldptch - delta_shldptch;
            double elbroll_high = elbroll + delta_elbroll;
            // low pose
            double shldptch_low = shldptch + delta_shldptch;
            double elbroll_low = elbroll - delta_elbroll;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = 0;

            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_high = new RArm(false, shldptch_high, shldroll, elbyaw, elbroll_high, wristyaw, hand);
            RArm rarm_low = new RArm(false, shldptch_low, shldroll, elbyaw, elbroll_low, wristyaw, hand);
            LArm larm_high = (LArm)rarm_high.MirrorLeftRight();
            LArm larm_low = (LArm)rarm_low.MirrorLeftRight();

            // Joints
            PoseProfile pose_head = new PoseProfile("Pose1", head);
            PoseProfile pose_larm_high = new PoseProfile("Pose1", larm_high);
            PoseProfile pose_larm_low = new PoseProfile("Pose2", larm_low, true);
            PoseProfile pose_rarm_high = new PoseProfile("Pose2", rarm_high, true);
            PoseProfile pose_rarm_low = new PoseProfile("Pose3", rarm_low, true);
            PoseProfile pose_larm_high1 = new PoseProfile("Pose3", larm_high, true);
            PoseProfile pose_larm_low1 = new PoseProfile("Pose4", larm_low);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_larm_high);
            lpp.Add(pose_larm_low);
            lpp.Add(pose_rarm_high);
            lpp.Add(pose_rarm_low);
            lpp.Add(pose_larm_high1);
            lpp.Add(pose_larm_low1);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double amplitude = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double factor = 1.2;
            double motspd = factor * (0.035 * valence + 0.65);
            double decspd = 0.035 * valence + 0.65;
            double hold = 0;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["PalmDir"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            //
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = hold;
            Parameters["Repetition"].Value = rep;
        }
    }
}
