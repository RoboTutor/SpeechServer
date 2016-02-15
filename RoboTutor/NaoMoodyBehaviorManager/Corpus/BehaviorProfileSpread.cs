using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileSpread : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Spread"; }
        }

        public BehaviorProfileSpread()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDir", new Parameter(0, "Pose"));
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
            double width = Parameters["Width"].Value;
            double height = Parameters["Height"].Value;
            double palmdir = Parameters["PalmDir"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headlr = Parameters["HeadLeftRight"].Value;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;
            double wristyaw = 60 * palmdir + 30;
            double elbyaw = 90;
            double shldroll = -20 * width - 20;
            double shldpitch = -40 * height + 60;
            double elbroll = -28 * height + 30;

            // degree
            double headpitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = double.NaN;

            Head head = new Head(false, headpitch, headYaw);
            RArm rarm = new RArm(false, shldpitch, shldroll, elbyaw, elbroll, wristyaw, hand);
         

            // Joints
            PoseProfile pose_head = new PoseProfile("Pose", head);
            PoseProfile pose_rarm = new PoseProfile("Pose", rarm);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_rarm);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
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
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["PalmDir"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
