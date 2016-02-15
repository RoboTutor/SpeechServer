using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileConvince : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Convince"; }
        }

        public BehaviorProfileConvince()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDir", new Parameter(0, "Pose"));
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
            double width   = Parameters["Width"].Value;
            double amp     = Parameters["Amplitude"].Value;
            double palmdir = Parameters["PalmDir"].Value;
            double finger  = Parameters["Finger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;

            double shoulderRoll = -30 * width;
            double elbowYaw = 39.5 * width + 80;
            double elbowRollLow = -28 * amp + 30;
            double elbowRollHigh = 28.5 * amp + 60;
            double shoulderPitchLow = 20 * amp + 70;
            double shoulderPitchHigh = -10 * amp + 30;
            double wristYaw = 74 * palmdir + 30;
            // finger 0~1
            double hand = 0.4 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = double.NaN;

            RArm rarmposelow = new RArm(false,shoulderPitchLow,shoulderRoll,elbowYaw,elbowRollLow,wristYaw,hand);
            LArm larmposelow = (LArm)rarmposelow.MirrorLeftRight();

            RArm rarmposehigh = new RArm(false,shoulderPitchHigh,shoulderRoll,elbowYaw,elbowRollHigh,wristYaw,hand);
            LArm larmposehigh = (LArm)rarmposehigh.MirrorLeftRight();

            Head head = new Head(false, headPitch, headYaw);

            PoseProfile arm_r_pose_low = new PoseProfile("LowPose", rarmposelow, true, 0.3, 0);
            PoseProfile arm_l_pose_low = new PoseProfile("LowPose", larmposelow, true, 0.3, 0);

            PoseProfile head_pose = new PoseProfile("LowPose", head);

            PoseProfile arm_r_pose_high = new PoseProfile("HighPose", rarmposehigh, true, 1, 0);
            PoseProfile arm_l_pose_high = new PoseProfile("HighPose", larmposehigh, true, 1, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(arm_r_pose_low);
            lpp.Add(arm_l_pose_low);
            lpp.Add(head_pose);
            lpp.Add(arm_r_pose_high);
            lpp.Add(arm_l_pose_high);

            return lpp;
        }
        
        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width         = 0.05 * valence + 0.5;
            double amp           = 0.05 * valence + 0.5;
            double palmdir       = 0.05 * valence + 0.5;
            double finger        = 0.05 * valence + 0.5;
            // motion
            double motspd        = 0.035 * valence + 0.65;
            double decspd        = 0.035 * valence + 0.65;
            double holdtime      = -0.01 * valence + 0.1;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0;

            Parameters["Width"].Value         = width;
            Parameters["Amplitude"].Value     = amp;
            Parameters["PalmDir"].Value       = palmdir;
            Parameters["Finger"].Value        = finger;
            Parameters["HeadUpDown"].Value    = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value   = motspd;
            Parameters["DecaySpeed"].Value    = decspd;
            Parameters["HoldTime"].Value      = holdtime;
            Parameters["Repetition"].Value    = rep;
        }
    }
}
