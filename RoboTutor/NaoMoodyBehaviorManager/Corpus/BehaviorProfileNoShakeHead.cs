using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileNoShakeHead : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "NoShakeHead"; }
        }

        public BehaviorProfileNoShakeHead()
        {
            this.Parameters.Add("Amplitude" , new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed" , new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime"   , new Parameter(new List<double>(), "Motion"));
            this.Parameters.Add("Repetition" , new Parameter(0, "Motion"));
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
            double amplitude = Parameters["Amplitude"].Value;
            double headud = Parameters["HeadUpDown"].Value;

            // degree
            //double headyaw = 30 * amplitude + 15;
            double headyaw = base.NormalizeHeadYaw(amplitude);
            double headyaw_L = headyaw;
            double headyaw_R = -headyaw;

            // degree
            double headpitch = base.NormalizeHeadPitch(headud);

            Head head_L = new Head(false, headpitch, headyaw_L);
            Head head_R = new Head(false, headpitch, headyaw_R);

            // Joints
            PoseProfile pose_left_head = new PoseProfile("LeftHeadPose", head_L, true, 1, 0);
            PoseProfile pose_right_head = new PoseProfile("RightHeadPose", head_R, true, 1, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_left_head);
            lpp.Add(pose_right_head);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double amplitude = 0.05 * valence + 0.5;
            double headud = 0.1 * valence;
            // motion
            double factor = 1.3;
            double motspd = factor * (0.035 * valence + 0.65);
            double decspd = 0.035 * valence + 0.65;
            double holdtime = 0.01 * valence + 0.1;
            double rep;
            if (valence >= 8) rep = 2;
            else rep = 1;

            Parameters["Amplitude"].Value = amplitude;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
