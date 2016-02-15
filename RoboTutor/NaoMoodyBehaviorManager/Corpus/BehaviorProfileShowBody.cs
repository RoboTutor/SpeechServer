using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileShowBody : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "ShowBody"; }
        }

        public BehaviorProfileShowBody()
        {
            this.Parameters.Add("BodyLeanForward", new Parameter(0, "Pose"));
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
        }

        public override ConnectorNao.MotionTimeline LoadBehavior(double valence, double arousal = 0, string args = null)
        {
            return base.LoadBehavior(valence,arousal,"BOTH");
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
            double bodyleanforward = Parameters["BodyLeanForward"].Value;
            double width           = Parameters["Width"].Value;
            double height          = Parameters["Height"].Value;
            double finger          = Parameters["Finger"].Value;
            double headud          = Parameters["HeadUpDown"].Value;
            double headlr          = Parameters["HeadLeftRight"].Value;

            // 
            double wristyaw = 30;

            // Down pose
            double up_shldroll = -20 * width - 10;
            double up_elbyaw = 30*width+60;
            double up_shldpitch = -30 * height - 30;
            double up_elbroll = -58*height+60;
            double up_wristyaw = wristyaw;
            double up_hand = 0.2*finger+0.8;

            // Down pose
            double dp_shldroll = -15 * width;
            double dp_elbyaw = 20*width+45;
            double dp_shldpitch = 45 * height + 45;
            double dp_elbroll = 18.5 * height + 70;
            double dp_wristyaw = wristyaw;
            double dp_hand = -0.4*finger+0.4;

            // degree
            double headpitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = 0;

            Head head = new Head(false, headpitch, headYaw);
            RArm rarm_up = new RArm(false, up_shldpitch, up_shldroll, up_elbyaw, up_elbroll, up_wristyaw, up_hand);
            RArm rarm_down = new RArm(false, dp_shldpitch, dp_shldroll, dp_elbyaw, dp_elbroll, dp_wristyaw, dp_hand);

            // Joints
            PoseProfile pose_head = new PoseProfile("UpPose", head);
            PoseProfile pose_rarm_up = new PoseProfile("UpPose", rarm_up);
            PoseProfile pose_rarm_down = new PoseProfile("DownPose", rarm_down);

            List<PoseProfile> legpp = base.BodyLeanForward("DownPose", bodyleanforward);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_rarm_up);
            lpp.Add(pose_rarm_down);
            lpp.AddRange(legpp);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double bodyleanforward = 0.05 * valence + 0.5;
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double holdtime = 0.0075 * valence + 0.125;
            double rep = 0;

            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            //
            Parameters["BodyLeanForward"].Value = bodyleanforward;
            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
