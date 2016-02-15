using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileConvergeHands : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "ConvergeHands"; }
        }

        public BehaviorProfileConvergeHands()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDir", new Parameter(0, "Pose"));
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
            double width   = Parameters["Width"].Value;
            double height  = Parameters["Height"].Value;
            double palmdir = Parameters["PalmDir"].Value;
            double finger  = Parameters["Finger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;

            double shoulderPitch_open = -20*height+90;
            double shoulderRoll_open = -20 * width -10;
            double elbowYaw_open = 29.5*width + 90;
            double elbowRoll_open = 88.5;
            double wristYaw_open = 60*palmdir;
            double hand_open = 1;

            double shoulderPitch_close = -25*height+70;
            double shoulderRoll_close = -10*width+10;
            double elbowYaw_close = -10*width+65;
            double elbowRoll_close = 88.5;
            double wristYaw_close = -20;
            double hand_close = 0.2 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = double.NaN;

            RArm rarmposeopen = new RArm(false, 
                shoulderPitch_open, shoulderRoll_open,
                elbowYaw_open, elbowRoll_open,
                wristYaw_open, hand_open);
            LArm larmposeopen = (LArm)rarmposeopen.MirrorLeftRight();

            RArm rarmposeclose = new RArm(false,
                shoulderPitch_close, shoulderRoll_close,
                elbowYaw_close, elbowRoll_close,
                wristYaw_close, hand_close);
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
            double width    = 0.05 * valence + 0.5;
            double height   = 0.05 * valence + 0.5;
            double finger   = 0.05 * valence + 0.5;
            double palmdir  = 0.05 * valence + 0.5;
            // head
            double headud   = 0.1 * valence;
            double headlr   = 0;

            // motion
            double motspd   = 0.035 * valence + 0.65;
            double decspd   = motspd;
            double holdtime = 0.04 * valence + 0.6;
            double rep;
            if (valence >= 8) rep = 1;
            else rep              = 0;

            Parameters["Width"].Value         = width;
            Parameters["Height"].Value        = height;
            Parameters["Finger"].Value        = finger;
            Parameters["PalmDir"].Value       = palmdir;
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
