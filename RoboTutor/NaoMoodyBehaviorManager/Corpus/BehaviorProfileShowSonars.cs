using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileShowSonars : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "ShowSonars"; }
        }

        public BehaviorProfileShowSonars()
        {
            this.Parameters.Add("Gaze", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("FingerRigidness", new Parameter(0, "Pose"));

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
            double finger = Parameters["FingerRigidness"].Value;
            double hand = 0.4 * finger + 0.6;

            double headud = Parameters["HeadUpDown"].Value;
            double headpitch = 29 * headud * headud - 73.5 * headud + 29.5;

            double gaze = Parameters["Gaze"].Value;
            double head_right = -(10 * gaze + 10);
            double head_left = 10 * gaze + 10;

            Head head_r = new Head(false, headpitch, head_right);
            Head head_l = new Head(false, headpitch, head_left);

            RArm prepose = new RArm(false, 60, -10, 70, 60, 0, hand);
            PoseProfile prepose_arm = new PoseProfile("PrePose", prepose);

            List<PoseProfile> lpp1 = RightArmPointRightSonar("RightSonar");
            PoseProfile gaze_right = new PoseProfile("RightSonar", head_r);

            List<PoseProfile> lpp2 = RightArmPointLeftSonar("LeftSonar");
            PoseProfile gaze_left = new PoseProfile("LeftSonar", head_l);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(prepose_arm);
            lpp.AddRange(lpp1);
            lpp.Add(gaze_right);
            lpp.AddRange(lpp2);
            lpp.Add(gaze_left);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double finger = 0.05 * valence + 0.5;
            double gaze = 0.05 * valence + 0.5;
            double headud = 0.05 * valence + 0.5;

            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.5;
            double rep = 0;

            Parameters["Gaze"].Value        = gaze;
            Parameters["HeadUpDown"].Value  = headud;
            Parameters["FingerRigidness"].Value = finger;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value  = decspd;
            Parameters["HoldTime"].Value    = holdtime;
            Parameters["Repetition"].Value  = rep;
        }
    }
}
