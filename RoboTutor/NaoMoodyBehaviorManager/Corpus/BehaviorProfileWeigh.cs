using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// Compare two things
    /// modified from Motivation
    /// </summary>
    public class BehaviorProfileWeigh : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Weigh"; } // used to be ShakeHands
        }

        public BehaviorProfileWeigh()
        {
            Parameters.Add("Width", new Parameter(0, "Pose"));
            Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            Parameters.Add("Finger", new Parameter(0, "Pose"));
            Parameters.Add("PalmDir", new Parameter(0, "Pose"));
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
            double amplitude = Parameters["Amplitude"].Value;
            double palmdir   = Parameters["PalmDir"].Value;
            double finger    = Parameters["Finger"].Value;
            double headud    = Parameters["HeadUpDown"].Value;
            double headlr    = Parameters["HeadLeftRight"].Value;

            // width
            double shldroll = -20 * width;
            double elbyaw = 25 * width + 65;

            // high pose
            double shldptch_high = -15 * amplitude + 45;
            double elbroll_high = 13.5 * amplitude + 75;

            // low pose
            double shldptch_low = 5 * amplitude + 85;
            double elbroll_low = 15 * amplitude + 60;

            double hand = 0.3 * finger + 0.5;
            double wristyaw = 60 * palmdir + 30;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = double.NaN;

            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_high = new RArm(false, shldptch_high, shldroll, elbyaw, elbroll_high, wristyaw, hand);
            RArm rarm_low = new RArm(false, shldptch_low, shldroll, elbyaw, elbroll_low, wristyaw, hand);
            LArm larm_high = (LArm)rarm_high.MirrorLeftRight();
            LArm larm_low = (LArm)rarm_low.MirrorLeftRight();

            // Joints
            PoseProfile pose_head = new PoseProfile("Pose1", head);
            PoseProfile pose_larm_high = new PoseProfile("Pose1", larm_high);
            PoseProfile pose_larm_low = new PoseProfile("Pose2", larm_low);
            PoseProfile pose_rarm_high = new PoseProfile("Pose2", rarm_high);
            PoseProfile pose_rarm_low = new PoseProfile("Pose3", rarm_low);
            PoseProfile pose_larm_high1 = new PoseProfile("Pose3", larm_high);
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
            double amplitude = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            // head
            double headud = 0.1 * valence;
            double headlr = 0;

            // motion
            double factor = 1.3;
            double motspd = factor *(0.035 * valence + 0.65);
            double decspd = 0.035 * valence + 0.65;
            // TODO: need a more unified solution
            this.UseSpeedFraction = true;

            double hold = 0.01;
            double rep = 0;

            Parameters["Width"].Value = width;
            Parameters["Amplitude"].Value = amplitude;;
            Parameters["Finger"].Value = finger;
            Parameters["PalmDir"].Value = palmdir;
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
