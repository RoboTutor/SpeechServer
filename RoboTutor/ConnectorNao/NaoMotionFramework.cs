/*
 * This code and information is provided "as is" without warranty of any kind, 
 * either expressed or implied, including but not limited to the implied warranties 
 * of merchantability and/or fitness for a particular purpose.
 * 
 * License: 
 * 
 * Email: junchaoxu86@gmail.com; k.v.hindriks@tudelft.nl
 * 
 * Copyright © Junchao Xu, Interactive Intelligence, TUDelft, 2014.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConnectorNao
{
    // Single Joint Chain Pose
    public class PoseProfile
    { 
        /// <summary>
        /// Name of the pose;
        /// It should be the same as the PoseID of the MotionFrame containing this pose.
        /// </summary>
        public string ID;

        /// <summary>
        /// Whether the MotionFrame containing this pose is repeated;
        /// It should be the same as that in the MotionFrame.
        /// </summary>
        public bool IsRepeated = false;
        public double RepeatInterpol = 1.0; // Coefficient 1.0=no interpolation
        public double FastRepeatHoldTime = Double.NaN;

        // !!!Pose with different hold-time should be separated in different MotionFrames
        //public double HoldTime = Double.NaN;

        // Joint chain (e.g., arm, leg, head; multi-joints) contained in this pose
        // NB: One pose can only have one joint chain
        public JointChain Joints;

        // TODO
        public double SpeedFraction;
        // Number of the joints in this pose; 
        // Could be unequal to the joint number in the JointChain, if not all joints are used
        // TODO
        public int JointInUseNum;

        // Constructor
        public PoseProfile() { }
        // Deep Copy Constructor
        public PoseProfile(PoseProfile pp, bool isdeep = true)
        {
            this.ID = pp.ID;
            this.JointInUseNum = pp.JointInUseNum;
            this.IsRepeated = pp.IsRepeated;
            this.RepeatInterpol = pp.RepeatInterpol;
            this.FastRepeatHoldTime = pp.FastRepeatHoldTime;
            //this.SpeedFraction = pp.SpeedFraction;
            if (isdeep == true)
            {
                this.Joints = pp.Joints.DeepCopy();
            }
        }
        //
        public PoseProfile(string posename, JointChain jc, /*double holdtime = Double.NaN,*/ bool isrepeated = false, double interpol = 1.0, double fastrephold = Double.NaN)
        {
            this.ID = posename;
            this.Joints = jc;
            //this.HoldTime = holdtime;
            this.IsRepeated = isrepeated;
            this.RepeatInterpol = interpol;
            this.FastRepeatHoldTime = fastrephold;
        }

        public PoseProfile InterpolationPose(PoseProfile pose, double coef)
        {
            PoseProfile interpol_pose = new PoseProfile(this, false);
            interpol_pose.Joints = this.Joints.InterpolateJointChain(pose.Joints, coef);
            return interpol_pose;
        }
    }

    /// <summary>
    /// TODO
    /// Join the successive Stroke could be troublesome,
    /// because they share one Pose.
    /// </summary>
    public class Stroke
    {
        public JointChain PoseInit;
        public JointChain PoseEnd;

        public Stroke(JointChain pose_init, JointChain pose_end)
        {
            this.PoseInit = pose_init;
            this.PoseEnd = pose_end;
        }
    }

    // Motion Frame of one time stamp
    // What is motion frame? REF: MotionTimeLine.vsd
    public class MotionFrame
    {
        public string BehaviorName;

        public string PoseID;
        // Poses
        public List<PoseProfile> PoseList = new List<PoseProfile>();
        // General HoldTime for all poses; can be overrided by Pose.HoldTime
        public double HoldTime;
        // The time stamp of this motion frame in the whole motion time line
        public int TimeStamp = 0;

        // Constructor
        public MotionFrame() { }

        // Deep copy constructor
        public MotionFrame(MotionFrame mf, bool isdeep = true)
        {
            this.BehaviorName = mf.BehaviorName;
            this.PoseID = mf.PoseID;
            this.IsAbsolute = mf.IsAbsolute;
            this.HoldTime = mf.HoldTime;
            this.TimeStamp = mf.TimeStamp;
            this.UsingSpeedFraction = mf.UsingSpeedFraction;
            this.SpeedFraction = mf.SpeedFraction;
            this.SpeedTime = mf.SpeedTime;
            
            if (isdeep==true)
            {
                if (mf.PoseList!=null)
	            {
		            this.PoseList = new List<PoseProfile>();
		            foreach (var pp in mf.PoseList)
		            {
		                // Deep copy
		                var newpp = new PoseProfile(pp);
		                this.PoseList.Add(newpp);
		            }
	            }
                if (mf.ComboJointNames!=null)
                {
	                this.ComboJointNames = new List<string>();
		            foreach (var n in mf.ComboJointNames)
		            {
		                this.ComboJointNames.Add(n);
		            }
                }
                if (mf.ComboJointValueDegree!=null)
	            {
		            this.ComboJointValueDegree = new List<float>();
		            foreach (var jvd in mf.ComboJointValueDegree)
		            {
		                this.ComboJointValueDegree.Add(jvd);
		            }
	            }
                if (mf.ComboJointValueRadian!=null)
	            {
		            this.ComboJointValueRadian = new List<float>();
		            foreach (var jvr in mf.ComboJointValueRadian)
		            {
		                this.ComboJointValueRadian.Add(jvr);
		            }
	            }
                if (mf.ComboJointSpeedT!=null)
	            {
		            this.ComboJointSpeedT = new List<float>();
		            foreach (var jst in mf.ComboJointSpeedT)
		            {
		                this.ComboJointSpeedT.Add(jst);
		            }
	            }
            }
        }

        /// <summary>
        /// Combine two poses that can coexist simultaneously;
        /// it checks if their TimeStamps are equal!
        /// Some fields take those of this MF!
        /// Joint-ified! JointsToList() is called inside!
        /// </summary>
        /// <param name="mf"></param>
        /// <returns>Return a deep copy of the merged MotionFrame.</returns>
        public static MotionFrame CombineFrame(MotionFrame mf1, MotionFrame mf2)
        {
            // check
            if (mf1.TimeStamp != mf2.TimeStamp)
            {
                Debug.WriteLine("Inconsistent TimeStamp!");
                return null;
            }

            MotionFrame comboMF;
            comboMF = new MotionFrame();
            comboMF.TimeStamp = mf1.TimeStamp;

            // Combined name
            comboMF.BehaviorName = mf1.BehaviorName + "|" + mf2.BehaviorName;
            // required by NaoQi API
            comboMF.IsAbsolute = mf1.IsAbsolute; // ignore mf.isAbsolute;
            // Mixed hold time already presented by TimeStamp!
            comboMF.HoldTime = 0;
            //
            comboMF.UsingSpeedFraction = mf1.UsingSpeedFraction; // ignore mf.UsingSpeedFraction
            comboMF.SpeedFraction = mf1.SpeedFraction; // ignore mf.SpeedFraction

            // Combine the pose list
            // the duplicate pose in "mf" will be ignored
            comboMF.PoseList = new List<PoseProfile>();
            comboMF.PoseList.AddRange(mf1.PoseList);
            foreach (var p in mf2.PoseList)
            {
                bool nonduplicate = true;
                Dictionary<string, PoseProfile> dupPoseList = new Dictionary<string, PoseProfile>();
                foreach (var out_p in mf1.PoseList) // can also use "comboMF.PoseList"
                {
                    if (p.Joints.Name == out_p.Joints.Name)
                    {
                        nonduplicate = false;
                        dupPoseList.Add(out_p.Joints.Name, out_p); // store ref for later use
                    }
                }
                if (nonduplicate) // not duplicated pose
                {
                    comboMF.PoseList.Add(p);
                }
                else
                {
                    if (dupPoseList.ContainsKey("Head")) // duplicated head pose; "BOTH" hands symtric gestures
                    {
                        dupPoseList["Head"].Joints = new Head(false, ((Head)p.Joints).JointInUseValue[0], 0);
                    }
                }
            }
            comboMF.JointsToList();

            #region Old way
            /*
            // Make the List<PoseProfile> to List<value>
            if (this.ComboJointNames == null)
                this.JointsToList();
            if (mf.ComboJointNames == null)
                mf.JointsToList();

            // Remove head from the second to avoid duplicate
            // TODO: do not add at the first place instead of removing afterwards
            if (mf.PoseList.Count > 1)
            {
                if (mf.PoseList[1].Joints.Name == "Head")
                {
                    mf.PoseList.RemoveRange(1, 1);
                    mf.JointsToList();
                }
            }

            comboMF.ComboJointNames = new List<string>();
            comboMF.ComboJointNames.AddRange(this.ComboJointNames);
            comboMF.ComboJointNames.AddRange(mf.ComboJointNames);
            comboMF.ComboJointValueRadian = new List<float>();
            comboMF.ComboJointValueRadian.AddRange(this.ComboJointValueRadian);
            comboMF.ComboJointValueRadian.AddRange(mf.ComboJointValueRadian);
            comboMF.ComboJointSpeedT = new List<float>();
            comboMF.ComboJointSpeedT.AddRange(this.ComboJointSpeedT);
            comboMF.ComboJointSpeedT.AddRange(mf.ComboJointSpeedT);
            */
            #endregion Old way

            return comboMF;
        }

        /// <summary>
        /// Transfer "PoseList" to joint names and values (in degree and radian).
        /// </summary>
        public void JointsToList()
        {
            ComboJointNames = new List<string>();
            ComboJointValueDegree = new List<float>();
            ComboJointValueRadian = new List<float>();
            ComboJointSpeedT = new List<float>();
            foreach (var p in this.PoseList)
            {
                List<string> jn = p.Joints.JointInUseNames;
                List<float> jv = p.Joints.JointInUseValue;
                List<float> js = p.Joints.JointInUseSpeed;
                ComboJointNames.AddRange(jn);
                ComboJointValueRadian.AddRange(jv);
                if (js != null)
                    ComboJointSpeedT.AddRange(js);
                else
                {
                    for (int i = 0; i < jn.Count; i++)
                    {
                        ComboJointSpeedT.Add((float)SpeedTime);
                    }
                }
            }
        }

        #region Executable
        //! To meet the NaoQi API format
        public bool IsAbsolute = true;
        // Indicate whether to use "angleInterpolationWithSpeed" or "angleInterpolation"
        public bool UsingSpeedFraction = false;
        // Speed fraction: from previous pose to this pose
        public double SpeedFraction;
        public double SpeedTime;
        // The joint names
        public List<string> ComboJointNames = null;
        // The joint values
        private List<float> ComboJointValueDegree = null;
        // Translate the whole list into radian
        public List<float> ComboJointValueRadian = null;
        // Speed in time: from previous pose to this pose
        //  Use this, if each pose needs different speed
        public List<float> ComboJointSpeedT = null;

        // Whether there are poses that need to be Repeated
        public bool IsRepeated
        {
            get
            {
                bool isrepeated = false;
                foreach(PoseProfile pp in this.PoseList)
                {
                    if (pp.IsRepeated == true) isrepeated = true;
                }
                return isrepeated;
            }
        }

        // Add joint values in degree
        public void AddDegree(string jName, double jVal)
        {
            ComboJointNames.Add(jName);
            ComboJointValueDegree.Add((float)jVal);
            double r = jVal * Math.PI / 180.0D;
            ComboJointValueRadian.Add((float)r);
        }
        // Convert joint values from degree to radian
        public void DegreeToRadian()
        {
            foreach (double jvd in ComboJointValueDegree)
            {
                double r = jvd * Math.PI / 180.0D;
                ComboJointValueRadian.Add((float)r);
            }
        }
        // Add joint values in radian
        public void AddRadian(string jName, double jVal)
        {
            ComboJointNames.Add(jName);
            ComboJointValueRadian.Add((float)jVal);
            double d = jVal * 180.0D / Math.PI;
            ComboJointValueDegree.Add((float)d);
        }
        // Convert joint values from radian to degree
        public void RadianToDegree()
        {
            foreach (double jvr in ComboJointValueRadian)
            {
                double d = jvr * 180.0D / Math.PI;
                ComboJointValueDegree.Add((float)d);
            }
        }
        #endregion // Executable
    }

    // Multi-Behavior
    public class MotionTimeline
    {
        // Description of the behavior; it can contain multiple individual behaviors
        //  FROMAT: "beh1@emot1;beh2@emot2"
        public string BehaviorsDesp;
        // Involved joint chain names
        public List<string> ContainedJointChainNames
        {
            get
            {
                List<string> ijc = new List<string>();
                foreach (var mf in OrderedMFSeq)
                {
                    foreach (var pose in mf.PoseList)
                    {
                        string jn = pose.Joints.Name;
                        if (!ijc.Contains(jn))
                        {
                            ijc.Add(jn);
                        }
                    }
                }
                return ijc;
            }
        }
        // Involved joint chain
        public List<JointChain> ContainedJointChain
        {
            get
            {
                List<JointChain> ijc = new List<JointChain>();
                foreach (var mf in OrderedMFSeq)
                {
                    foreach (var pose in mf.PoseList)
                    {
                        JointChain jn = pose.Joints;
                        if (!ijc.Contains(jn))
                        {
                            ijc.Add(jn);
                        }
                    }
                }
                return ijc;
            }
        }

        // Pose Recover
        public bool PoseRecover = false;
        // Recover speed fraction
        public double RecovSpd = 0.5;
        // Pre-delay time: ms
        public int PreMotionHold = 0;
        // Post-delay time: ms
        public int PostMotionHold = 0;

        // ordered
        public List<MotionFrame> OrderedMFSeq = new List<MotionFrame>();
        // unordered
        public List<MotionFrame> RawMFSeq = new List<MotionFrame>();

        /// <summary>
        /// Internal use!
        /// Merge motion frames from different behavior profiles into one frame 
        /// according to the time stamp
        /// </summary>
        private void MergeMotionFrames()
        {
            //List<MotionFrame> ordered = rawMotionSequence.OrderBy(x => x.TimeStamp).ToList();
            //List<MotionFrame> grouped = ordered.GroupBy(u => u.TimeStamp).ToList();
            var v =
                from mf in RawMFSeq
                group mf by mf.TimeStamp into g
                orderby g.Key
                select g;

            foreach (var mfGroup in v)
            {
                Console.WriteLine("TimeStamp: " + mfGroup.Key);
                MotionFrame combinedMf = new MotionFrame();
                foreach (var mf in mfGroup)
                {
                    // This is incorrect!!!
                    combinedMf = MotionFrame.CombineFrame(combinedMf, mf);
                    Console.WriteLine("   " + mf.BehaviorName + "->" + mf.BehaviorName);
                }

                OrderedMFSeq.Add(combinedMf);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joints"></param>
        /// <returns></returns>
        public MotionTimeline MirrorLeftRight()
        {
            MotionTimeline mtl = new MotionTimeline();
            mtl.BehaviorsDesp  = this.BehaviorsDesp;
            mtl.PoseRecover    = this.PoseRecover;
            mtl.RecovSpd       = this.RecovSpd;      

            // do not use "foreach". Keep the same order.
            for (int i = 0; i < this.OrderedMFSeq.Count; i++ )
            {
                MotionFrame mf = this.OrderedMFSeq[i];
                MotionFrame newmf = new MotionFrame();
                newmf.BehaviorName = mf.BehaviorName;
                newmf.IsAbsolute = mf.IsAbsolute;
                newmf.UsingSpeedFraction = mf.UsingSpeedFraction;
                newmf.SpeedFraction = mf.SpeedFraction;
                newmf.SpeedTime = mf.SpeedTime;
                newmf.TimeStamp = mf.TimeStamp;
                newmf.HoldTime = mf.HoldTime;
                foreach (PoseProfile pp in mf.PoseList)
                {
                    // shallow copy
                    PoseProfile newpp = new PoseProfile(pp,false);

                    JointChain jc = pp.Joints.MirrorLeftRight();
                    newpp.Joints = jc;

                    newmf.PoseList.Add(newpp);
                }
                // this must be called
                newmf.JointsToList();
                // keep the same order
                mtl.OrderedMFSeq.Add(newmf);
            }

            return mtl;
        }

        public static MotionTimeline MergeLeftRight(MotionTimeline mtl_right, MotionTimeline mtl_left)
        {
            MotionTimeline mtl = new MotionTimeline();
            if (mtl_right.BehaviorsDesp == mtl_left.BehaviorsDesp)
            	mtl.BehaviorsDesp = mtl_right.BehaviorsDesp;
            else
                mtl.BehaviorsDesp = mtl_right.BehaviorsDesp + "|" + mtl_left.BehaviorsDesp;
            mtl.PoseRecover = mtl_right.PoseRecover;
            mtl.RecovSpd = mtl_right.RecovSpd;

            foreach(MotionFrame mf_r in mtl_right.OrderedMFSeq)
            {
                // looking for MotionFrame with the same TimeStamp in mtl_left
                for (int i = 0; i < mtl_left.OrderedMFSeq.Count; i++)
                {
                    MotionFrame mf_l = mtl_left.OrderedMFSeq[i];
                    if (mf_r.TimeStamp == mf_l.TimeStamp)
                    {
                        MotionFrame mf_new = MotionFrame.CombineFrame(mf_r, mf_l);
                        mtl.OrderedMFSeq.Add(mf_new);
                    }
                }
            }

            return mtl;
        }

        public void SetAllHold(int hold)
        {
            this.PreMotionHold = hold;
            this.PostMotionHold = hold;
            foreach(MotionFrame mf in OrderedMFSeq)
            {
                mf.HoldTime = hold;
            }
        }
    }
}
