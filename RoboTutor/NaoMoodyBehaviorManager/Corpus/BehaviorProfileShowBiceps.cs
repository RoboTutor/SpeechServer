using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileShowBiceps : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "ShowBiceps"; }
        }

        public BehaviorProfileShowBiceps()
        {
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("GazeHand", new Parameter(0, "Pose"));

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
            double gazehand = Parameters["GazeHand"].Value;

            // 
            double wristyaw = 90;

            // Down pose
            double up_shldroll = -15 * amp - 45;
            double up_elbyaw = 0;
            double up_shldpitch = 15 * height - 45;
            double up_elbroll = -43 * height + 45;
            double up_wristyaw = wristyaw;
            double up_hand = 0.2 * finger + 0.8;
            double up_headYaw = -30 * gazehand + 30;

            // Down pose
            double dp_shldroll = -15 * amp - 45;
            double dp_elbyaw = 0;
            double dp_shldpitch = -30 * height - 60;
            double dp_elbroll = 88.5;
            double dp_wristyaw = wristyaw;
            double dp_hand = -0.4 * finger + 0.4;
            double dp_headYaw = -30*gazehand-60;

            // degree
            double headpitch = base.NormalizeHeadPitch(headud);

            Head head_up = new Head(false, headpitch, up_headYaw);
            Head head_down = new Head(false, headpitch, dp_headYaw);
            RArm rarm_up = new RArm(false, up_shldpitch, up_shldroll, up_elbyaw, up_elbroll, up_wristyaw, up_hand);
            RArm rarm_down = new RArm(false, dp_shldpitch, dp_shldroll, dp_elbyaw, dp_elbroll, dp_wristyaw, dp_hand);

            // Joints
            PoseProfile pose_head_up = new PoseProfile("UpPose", head_up);
            PoseProfile pose_rarm_up = new PoseProfile("UpPose", rarm_up);
            PoseProfile pose_head_down = new PoseProfile("DownPose", head_down);
            PoseProfile pose_rarm_down = new PoseProfile("DownPose", rarm_down);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head_up);
            lpp.Add(pose_rarm_up);
            lpp.Add(pose_head_down);
            lpp.Add(pose_rarm_down);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double amplitude = 0.05 * valence + 0.5;
            double gazehand = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.0075 * valence + 0.125;
            double rep = 0;

            // head
            double headud = 0.1 * valence;

            Parameters["GazeHand"].Value = gazehand;
            Parameters["Height"].Value = height;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
