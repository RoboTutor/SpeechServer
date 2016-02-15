using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileFromAToB : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "FromAToB"; }
        }

        public BehaviorProfileFromAToB()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
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
            double width  = Parameters["Width"].Value;
            double height = Parameters["Height"].Value;
            double palmdir = Parameters["PalmDirection"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headlr = Parameters["HeadLeftRight"].Value;

            double lsrA = 5 * width + 25;
            double rsrA = 8 * width + 10;
            double lerA = -30;
            double rerA = 30;
            double lspA = -20 * height + 40;
            double rspA = -15 * height + 40;
            double leyA = -105*palmdir + 15;
            double reyA = 60 * palmdir - 15;
            double lwyA = -90 * palmdir;
            double rwyA = 104 * palmdir;

            double lsrB = -8 * width - 10;
            double rsrB = -10 * width - 10;
            double lerB = -15 * width - 30;
            double rerB = 30;
            double lspB = -20 * height + 40;
            double rspB = -20 * height + 40;
            double leyB = -45 * palmdir + 15;
            double reyB = 90*palmdir;
            double lwyB = -104 * palmdir;
            double rwyB = 90 * palmdir;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;
            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = 30.0 * headlr;

            LArm larmA = new LArm(false, lspA, lsrA, leyA, lerA, lwyA, hand);
            RArm rarmA = new RArm(false, rspA, rsrA, reyA, rerA, rwyA, hand);
            LArm larmB = new LArm(false, lspB, lsrB, leyB, lerB, lwyB, hand);
            RArm rarmB = new RArm(false, rspB, rsrB, reyB, rerB, rwyB, hand);
            Head head = new Head(false, headPitch, headYaw);

            // Joints
            PoseProfile pose_A_arm_l = new PoseProfile("FromA", larmA);
            PoseProfile pose_A_arm_r = new PoseProfile("FromA", rarmA);
            PoseProfile pose_A_head = new PoseProfile("FromA", head);
            PoseProfile pose_B_arm_l = new PoseProfile("ToB", larmB);
            PoseProfile pose_B_arm_r = new PoseProfile("ToB", rarmB);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_A_arm_l);
            lpp.Add(pose_A_arm_r);
            lpp.Add(pose_A_head);
            lpp.Add(pose_B_arm_l);
            lpp.Add(pose_B_arm_r);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width  = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.0075 * valence + 0.125;
            double rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0;

            Parameters["Width"].Value         = width;
            Parameters["Height"].Value        = height;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value        = finger;
            Parameters["HeadUpDown"].Value    = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            Parameters["MotionSpeed"].Value   = motspd;
            Parameters["DecaySpeed"].Value    = decspd;
            Parameters["HoldTime"].Value      = holdtime;
            Parameters["Repetition"].Value    = rep;
        }
    }
}
