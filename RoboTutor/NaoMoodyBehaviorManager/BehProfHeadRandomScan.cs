using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileHeadRandomScan : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "HeadRandomScan"; }
        }

        public BehaviorProfileHeadRandomScan()
        {
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));

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

        Random RandomGen = new Random();

        const int MinHeadAmp = 150;
        const int MaxHeadAmp = 300;
        /// <summary>
        /// TODO: probabilistic model to select the movements that are less selected
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            double LeftLook = RandomGen.Next(MinHeadAmp, MaxHeadAmp) / 10.0D;
            double RightLook = -RandomGen.Next(MinHeadAmp, MaxHeadAmp) / 10.0D;

            System.Diagnostics.Debug.WriteLine("HeadRandomScan: Left {0} right {1}", LeftLook, RightLook);

            double headud = Parameters["HeadUpDown"].Value;
            double headpitch = base.NormalizeHeadPitch(headud);

            Head head_left = new Head(false, headpitch, LeftLook);
            Head head_right = new Head(false, headpitch, RightLook);

            // Joints
            PoseProfile pose_head_left = new PoseProfile("PoseL", head_left);
            PoseProfile pose_head_right = new PoseProfile("PoseR", head_right);

            List<PoseProfile> lpp = new List<PoseProfile>();

            // sometimes first look left; sometimes right
            int lr = RandomGen.Next(0,1);
            if (lr == 0)
            {
	            lpp.Add(pose_head_left);
	            lpp.Add(pose_head_right);
            } 
            else
            {
                lpp.Add(pose_head_right);
                lpp.Add(pose_head_left);
            }

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            double motspd = 0.15;
            double decspd = 0.15;
            double holdtime_l = RandomGen.Next(10, 30) / 10.0D;
            double holdtime_r = RandomGen.Next(10, 30) / 10.0D;
            double rep = 0;

            double headud = 0.1 * valence;

            Parameters["HeadUpDown"].Value = headud;

            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(holdtime_l);
            Parameters["HoldTime"].ValueList.Add(holdtime_r);
            Parameters["Repetition"].Value = rep;
        }
    }
}
