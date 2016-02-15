using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileApplaud : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Applaud"; }
        }

        public BehaviorProfileApplaud()
        {
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
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
            double amp     = Parameters["Amplitude"].Value;
            double height  = Parameters["Height"].Value;
            double finger  = Parameters["Finger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;

            double shoulderPitch = -25 * height + 70;
            double elbowRoll     = 88.5;
            double wristYaw = -20;

            double shoulderRoll_open = -20 * amp -10;
            double elbowYaw_open = 29.5 * amp + 90;
            double hand_open = 1;

            double shoulderRoll_close = -13 * amp + 18;
            double elbowYaw_close = -10 * amp + 70;
            double hand_close = 0.2 * finger + 0.7;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = Double.NaN;

            RArm rarmposeopen = new RArm(false, 
                shoulderPitch, shoulderRoll_open,
                elbowYaw_open, elbowRoll,
                wristYaw, hand_open);
            LArm larmposeopen = (LArm)rarmposeopen.MirrorLeftRight();

            RArm rarmposeclose = new RArm(false,
                shoulderPitch, shoulderRoll_close,
                elbowYaw_close, elbowRoll,
                wristYaw, hand_close);
            LArm larmposeclose = (LArm)rarmposeclose.MirrorLeftRight();

            Head head = new Head(false, headPitch, headYaw);

            PoseProfile arm_r_pose_open = new PoseProfile("OpenPose", rarmposeopen, true, 0.3, 0);
            PoseProfile arm_l_pose_open = new PoseProfile("OpenPose", larmposeopen, true, 0.3, 0);
            PoseProfile head_pose       = new PoseProfile("OpenPose", head);
            PoseProfile arm_r_pose_close = new PoseProfile("ClosePose", rarmposeclose, true, 1, 0);
            PoseProfile arm_l_pose_close = new PoseProfile("ClosePose", larmposeclose, true, 1, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(arm_r_pose_open);
            lpp.Add(arm_l_pose_open);
            lpp.Add(head_pose);
            lpp.Add(arm_r_pose_close);
            lpp.Add(arm_l_pose_close);

            return lpp;
        }
        
        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double amp    = 0.05 * valence + 0.5;
            double height   = 0.05 * valence + 0.5;
            double finger   = 0.05 * valence + 0.5;
            // head
            double headud   = 0.1 * valence;
            double headlr   = 0;

            // motion
            double motspd   = 0.035 * valence + 0.65;
            double decspd   = motspd;
            double holdtime = 0.025 * valence + 0.25;
            double rep;
            if (valence >= 8) rep = 1;
            else rep              = 0;

            Parameters["Amplitude"].Value     = amp;
            Parameters["Height"].Value        = height;
            Parameters["Finger"].Value        = finger;
            Parameters["HeadUpDown"].Value    = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value   = motspd;
            Parameters["DecaySpeed"].Value    = decspd;
            Parameters["HoldTime"].ValueList.Add(0); // open pose
            Parameters["HoldTime"].ValueList.Add(holdtime); // close pose
            Parameters["Repetition"].Value    = rep;
        }
    }
}
