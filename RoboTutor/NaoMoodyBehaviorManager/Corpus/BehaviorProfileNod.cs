using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileNod : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "Nod"; }
        }

        public BehaviorProfileNod()
        {
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));

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
            double amplitude = Parameters["Amplitude"].Value;

            // degree
            double headPitch_b = -20 * amplitude;
            double headPitch_f = 11.5 * amplitude + 18;
            //double headPitch_b = base.NormalizeHeadPitch(amplitude);
            //double headPitch_f = base.NormalizeHeadPitch(-amplitude);
            // 
            double headYaw = 0;

            Head head_b = new Head(false, headPitch_b, headYaw);
            Head head_f = new Head(false, headPitch_f, headYaw);

            // fast hold
            double fasthold;
            if (amplitude == 0) fasthold = 0.01;
            else fasthold = 0;
            
            // Joints
            PoseProfile pose_back_head = new PoseProfile("BackPose", head_b, true, 1, fasthold);
            PoseProfile pose_front_head = new PoseProfile("FrontPose", head_f, true, 1, fasthold);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_back_head);
            lpp.Add(pose_front_head);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double amplitude = 0.05 * valence + 0.5;
            // motion
            double factor = 1.2;
            double motspd = factor * (0.035 * valence + 0.65);
            double decspd = 0.035 * valence + 0.65;
            double holdtime = 0.01 * valence + 0.1;
            double rep;
            if (valence >= 8) rep = 2;
            else rep = 1;


            Parameters["Amplitude"].Value = amplitude;
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
