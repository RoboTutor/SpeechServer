using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// Arm: The negative the defensive; The positive the open
    /// TODO: add torso lean forward
    /// TODO: add rhythm parameter for head movement
    /// </summary>
    public class BehaviorProfileLookAround : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "LookAround"; }
        }

        public BehaviorProfileLookAround()
        {
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose")); // head shake
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));

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
            double amp = Parameters["Amplitude"].Value;
            double height = Parameters["Height"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;

            double lsr = 3*height-18;
            double rsr = 15*height;
            double lsp = 10*height+50;
            double rsp = 30*height+30;
            double ley = 5*height-5;
            double rey = 25*height+15;
            double ler = 15*height-50;
            double rer = -20*height+80;
            double lwy = -90;
            double rwy = -45;

            // finger 0~1
            double hand = 0.6 * finger;
            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // -30 ~ 30 degree
            double headYaw_L = 10.0 * amp + 20;
            double headYaw_R = -10.0 * amp - 20;

            LArm larm = new LArm(false, lsp, lsr, ley, ler, lwy, hand);
            RArm rarm = new RArm(false, rsp, rsr, rey, rer, rwy, hand);
            Head lhead = new Head(false, headPitch, headYaw_L);
            Head rhead = new Head(false, headPitch, headYaw_R);

            // Joints       
            PoseProfile pose_close_arm_l = new PoseProfile("ArmClose", larm);
            PoseProfile pose_close_arm_r = new PoseProfile("ArmClose", rarm);
            PoseProfile pose_look_left;
            PoseProfile pose_look_right;

            if (headud >= 0) // positive
            {
	            pose_look_left = new PoseProfile("LookLeft", lhead, true, 0, 0.05);
	            pose_look_right = new PoseProfile("LookRight", rhead, true, 0, 0.05);
            } 
            else // negative
            {
                pose_look_left = new PoseProfile("LookLeft", lhead, true);
                pose_look_right = new PoseProfile("LookRight", rhead, true);
            }

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_close_arm_l);
            lpp.Add(pose_close_arm_r);
            lpp.Add(pose_look_left);
            lpp.Add(pose_look_right);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double amp = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double factor = 1.0;
            if (valence >= 0) factor = 1.5;
            else factor = 0.8;
            double motspd = factor * (0.035 * valence + 0.65);
            double decspd = factor * (0.035 * valence + 0.65);
            double holdtime = 0.0075 * valence + 0.125;
            double rep;
            if (valence >= 8) rep = 2;
            else rep = 1;
            // head
            double headud = 0.1 * valence;

            Parameters["Amplitude"].Value = amp;
            Parameters["Height"].Value = height;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            // motion
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
