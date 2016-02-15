using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileLegRandomMove : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "LegRandomMove"; }
        }

        List<List<PoseProfile>> LegPoses = new List<List<PoseProfile>>();

        /// <summary>
        /// TODO: Make leg movement more left-right balanced.
        /// </summary>
        public BehaviorProfileLegRandomMove()
        {
            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));

            List<PoseProfile> move1 = new List<PoseProfile>();
            move1.AddRange(base.MoveLeg1_1("LegPose1"));
            move1.AddRange(base.MoveLeg1_2("LegPose2"));
            move1.AddRange(base.Stand("LegPose3"));
            List<PoseProfile> move2 = new List<PoseProfile>();
            move2.AddRange(base.MoveLeg2_1("LegPose1"));
            move2.AddRange(base.MoveLeg2_2("LegPose2"));
            move2.AddRange(base.Stand("LegPose3"));
            List<PoseProfile> move3 = new List<PoseProfile>();
            move3.AddRange(base.MoveLeg3_1("LegPose1"));
            move3.AddRange(base.MoveLeg3_2("LegPose2"));
            move3.AddRange(base.Stand("LegPose3"));
            List<PoseProfile> move4 = new List<PoseProfile>();
            move4.AddRange(base.MoveLeg_BentKneeLeft("LegPose1"));
            move4.AddRange(base.MoveLeg_BentKneeRight("LegPose2"));
            move4.AddRange(base.Stand("LegPose3"));
            List<PoseProfile> move5 = new List<PoseProfile>();
            move5.AddRange(base.MoveLeg_Back("LegPose1"));
            move5.AddRange(base.Stand("LegPose2"));
            List<PoseProfile> move6 = new List<PoseProfile>();
            move6.AddRange(base.MoveLeg_Front("LegPose1"));
            move6.AddRange(base.Stand("LegPose2"));

            LegPoses.Add(move1);
            LegPoses.Add(move2);
            LegPoses.Add(move3);
            LegPoses.Add(move4);
            LegPoses.Add(move5);
            LegPoses.Add(move6);

            int cnt = LegPoses.Count;
            for (int i = 0; i < cnt; i++)
            {
                List<PoseProfile> legmove_mirror = new List<PoseProfile>();
                foreach (PoseProfile pp in LegPoses[i])
                {
                    // shallow copy
                    PoseProfile newpp = new PoseProfile(pp, false);
                    JointChain jc = pp.Joints.MirrorLeftRight();
                    newpp.Joints = jc;
                    legmove_mirror.Add(newpp);
                }
                LegPoses.Add(legmove_mirror);
            }
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

        Random RandomIndexGen = new Random();
        /// <summary>
        /// TODO: probabilistic model to select the movements that are less selected
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            int cnt = this.LegPoses.Count;
            int index = RandomIndexGen.Next(cnt);
            System.Diagnostics.Debug.WriteLine("LegRandomMove: select " + index);
            List<PoseProfile> lpp = this.LegPoses[index];
            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // motion
            // The speed is not restricted by bounds!
            // 0.018~0.032
            double motspd = 0.0007 * valence + 0.025;
            double decspd = 0.0007 * valence + 0.025;
            double holdtime = 0.5;
            double rep = 0;

            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = holdtime;
            Parameters["Repetition"].Value = rep;
        }
    }
}
