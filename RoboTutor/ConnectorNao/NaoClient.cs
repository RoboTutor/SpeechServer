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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Aldebaran.Proxies; // Aldebaran NaoQiDotNet4
using System.Collections;

namespace ConnectorNao
{
    public class ClientNao
    {
        // events
        public event ehBehaviorFinished evBehaviorFinished;
        public delegate void ehBehaviorFinished(string behaviorsDesp);

        // Proxies
        MotionProxy proxyMotion = null;
        BehaviorManagerProxy proxyBehaviorManager = null;
        LedsProxy proxyLed = null;
        SentinelProxy proxySentinel = null;
        VideoDeviceProxy proxyCamera = null;

        // Locks
        object lockMotion = new object();
        object lockLed = new object();

        public ClientNao()
        {
            InitCrgBehaviorCheckTimer();
        }

        ~ClientNao()
        {
            StopMotorMonitor();
        }

        const int Port = 9559;
        public string IP;
        public bool IsConnected = false;
        public bool connect(string ip, bool onlymotion = false)
        {
            this.IP = ip;

            bool b = false;
            try
            {
                proxyMotion = new MotionProxy(ip, Port);


                if (onlymotion == false)
                {
	                bool bc = proxyMotion.setCollisionProtectionEnabled("Arms", false);
	                if (!bc)
	                    Console.WriteLine("Failed to disable collision protection");

                    StartMotorMonitor();
	
	                proxyBehaviorManager = new BehaviorManagerProxy(ip, Port);
	
	                // Turn off the LEDs
	                proxyLed = new LedsProxy(ip, Port);
	                List<string> ledgroups = proxyLed.listGroups();
	                foreach (string ledgroup in ledgroups)
	                {
	                	proxyLed.off(ledgroup);
	                }

                    // Speech
                    proxyTTS = new TextToSpeechProxy(ip, Port);

                    if (ip != "127.0.0.1")
                    {
	                    // Sentinel
	                    proxySentinel = new SentinelProxy(ip, Port);
	                    // Turning off "Motor hot!"
	                    proxySentinel.enableHeatMonitoring(false);
                    }

                    // camera
                    proxyCamera = new VideoDeviceProxy(ip, Port);
	
	                // IdleMovement
	                InitIdleMovement();
                }

                b = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("MotionProxy.Connect Exception: " + e);
                b = false;
            }

            IsConnected = b;
            return b;
        }

        public void MotorsOnOff(bool b)
        {
            // Mutex
            lock (lockMotion)
            {
                if (b == true)
                    proxyMotion.setStiffnesses("Body", 1.0F);
                else
                    proxyMotion.setStiffnesses("Body", 0.0F);
            }
        }

        System.Timers.Timer TimerMotorMonitor; 
        private void StartMotorMonitor()
        {
            TimerMotorMonitor = new System.Timers.Timer(1500);
            TimerMotorMonitor.Elapsed += new System.Timers.ElapsedEventHandler(OnTimeCheckMotorState);
            TimerMotorMonitor.Start();
        }

        internal void StopMotorMonitor()
        {
            if (TimerMotorMonitor != null)
            {
            	TimerMotorMonitor.Stop();
            }
            evMotorState = null;
        }

        public event ehMotorState evMotorState;
        public delegate void ehMotorState(bool onoff);
        public void OnTimeCheckMotorState(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(proxyMotion != null)
            {
                bool onejointoff = true; // on

                // jointName – Name of the joints, chains, “Body”, “JointActuators”, “Joints” or “Actuators”.
                List<float> jStiffnesses = null;
                lock (lockMotion) 
                {
                    jStiffnesses = proxyMotion.getStiffnesses("Body");
                }
                if (jStiffnesses != null)
                {
	                foreach (float js in jStiffnesses)
	                {
	                    if (js < 0.1) { 
                            onejointoff = false; // off
                            break; 
                        }
	                }
                }

                if (evMotorState != null)
                {
                    evMotorState(onejointoff);
                }
            }
        }

        # region Choregraphe Behaviors
        string CurrentCrgBehavior = "";
        /// <summary>
        /// The behaviorname can be a path like "robotutor/specific/Twinkle"
        /// </summary>
        /// <param name="behaviorname"></param>
        public void RunChoregrapheBehavior(string behaviorname)
        {
            if (proxyBehaviorManager != null)
            {
	            lock (lockMotion)
	            {
	                if (proxyBehaviorManager.isBehaviorInstalled(behaviorname) == true)
	                {
	                    // for some reason the isBehaviorRunning is not updated??
	                    // so don't wait just stop it
	                    if (proxyBehaviorManager.isBehaviorRunning(behaviorname) == true)
	                    {
	                        proxyBehaviorManager.stopBehavior(behaviorname);
	                        Debug.WriteLine(behaviorname + " was running");
	                    }
	                    proxyBehaviorManager.post.runBehavior(behaviorname);
                        CurrentCrgBehavior = behaviorname;
                        StartCrgBehaviorChecker();
	                }
	                else
	                {
	                    string behaviornamelowercase = behaviorname.ToLower();
                        if (proxyBehaviorManager.isBehaviorInstalled(behaviornamelowercase) == true)
	                    {
	                        // for some reason the isBehaviorRunning is not updated??
	                        // so don't wait just stop it
                            if (proxyBehaviorManager.isBehaviorRunning(behaviornamelowercase) == true)
	                        {
                                proxyBehaviorManager.stopBehavior(behaviornamelowercase);
                                Debug.WriteLine(behaviornamelowercase + " was running");
	                        }
                            proxyBehaviorManager.post.runBehavior(behaviornamelowercase);
                            CurrentCrgBehavior = behaviorname;
                            StartCrgBehaviorChecker();
	                    }
	                    else
	                    {
                            Debug.WriteLine(behaviorname + " or " + behaviornamelowercase + " not installed");
	                    }
	                }
	            }
            }
            else
            {
                Debug.WriteLine("{0}: {1}", "NaoClient", "Behavior Manager not created!");
                evCrgBehaviorFinished(behaviorname);
            }
        }

        /// <summary>
        /// Initialize a timer for check whether Crg behaviors finished.
        /// </summary>
        private void InitCrgBehaviorCheckTimer()
        {
            // Create a timer with a interval (ms).
            TmCrgBehaviorCheck = new System.Timers.Timer(500);
            // Hook up the Elapsed event for the timer.
            TmCrgBehaviorCheck.Elapsed += new System.Timers.ElapsedEventHandler(OnTimeCheckCrgBehaviorRunning);
        }

        private void StartCrgBehaviorChecker()
        {
            TmCrgBehaviorCheck.Start();
        }

        public event ehCrgBehaviorFinished evCrgBehaviorFinished;
        public delegate void ehCrgBehaviorFinished(string crgbehaviorname);

        System.Timers.Timer TmCrgBehaviorCheck = null;
        public void OnTimeCheckCrgBehaviorRunning(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (lockMotion)
            {
                bool isrunning = proxyBehaviorManager.isBehaviorRunning(CurrentCrgBehavior);
                if (isrunning == false)
                {
                    TmCrgBehaviorCheck.Stop();
                    if (evCrgBehaviorFinished != null)
                    {
                        evCrgBehaviorFinished("Crg" + CurrentCrgBehavior);
                    }
                }
            }
        }

        public void StopChoregrapheBehavior(string behaviorname)
        {
            bool b = proxyBehaviorManager.isBehaviorRunning(behaviorname);
            if (b)
            {
                proxyBehaviorManager.stopBehavior(behaviorname);
            }
        }
        #endregion Choregraphe Behaviors

        MotionTimeline CurMtl;
        /// <summary>
        /// This is a background worker
        /// </summary>
        /// <param name="mtl"></param>
        public void ExecuteBehaviorInQueue(MotionTimeline mtl)
        {
            if (proxyMotion != null)
            {
                /// doesn't work with 1.10.52
                //WaitAndKillPreviousBehavior(2000);
                CurMtl = mtl;
                ThreadPool.QueueUserWorkItem(new WaitCallback(queueMotionDoWork), mtl);
            }
            else
            {
                Debug.WriteLine("{0}: {1}", "NaoClient", "Proxy is NULL!");
                evBehaviorFinished(mtl.BehaviorsDesp);
            }
        }

        /// <summary>
        /// This is a background worker
        /// </summary>
        /// <param name="mtl"></param>
        public void ExecuteBehaviorAsync(MotionTimeline mtl)
        {
            if (proxyMotion != null)
            {
                /// doesn't work with 1.10.52
                //WaitAndKillPreviousBehavior(2000);
                BackgroundWorker bgwMotion = new BackgroundWorker();
                bgwMotion.DoWork += new DoWorkEventHandler(bgwMotionDoWork);
                bgwMotion.RunWorkerAsync(mtl);
            }
            else
            {
                Debug.WriteLine("{0}: {1}", "NaoClient", "Proxy is NULL!");
                evBehaviorFinished(mtl.BehaviorsDesp);
            }
        }

        private void WaitAndKillPreviousBehavior(int waittime)
        {
            int sleeptime = 0;
            while (MotionRunning == true)
            {
                Thread.Sleep(100);
                sleeptime += 100;
                if (sleeptime > waittime)
                {
                    // kill the behavior
                    proxyMotion.killAll();
                    Debug.WriteLine("ClientNao: All behaviors killed!");
                }
            }
        }

        bool MotionRunning = false;
        void bgwMotionDoWork(object sender, DoWorkEventArgs e)
        {
            MotionTimeline mtl = (MotionTimeline)e.Argument;
            MotionRunning = true;
            ExecuteMotionTimeLine(mtl);
        }

        void queueMotionDoWork(object data)
        {
            MotionTimeline mtl = (MotionTimeline)data;
            MotionRunning = true;
            ExecuteMotionTimeLine(mtl);
        }

        private void ExecuteMotionTimeLine(MotionTimeline mtl)
        {
            if (mtl.PoseRecover)
            {
                //Debug.WriteLine("Initial poses are about to be stored for: " + mtl.BehaviorsDesp);
                StorePoses(mtl.BehaviorsDesp, mtl.ContainedJointChainNames);
            }

            // Hold for "PreDelay" time before execute the behavior
            Thread.Sleep(mtl.PreMotionHold);

            while (mtl.OrderedMFSeq.Count > 0 && MotionRunning == true)
            {
                MotionFrame mf = mtl.OrderedMFSeq[0];
                mtl.OrderedMFSeq.RemoveAt(0);

                // execute
                threadAngleInterpolation(mf);

                // hold time
                int t = (int)Math.Round(1000 * mf.HoldTime);
                Thread.Sleep(t);

                // for safety
                // Thread.Sleep(100);
            }

            // Hold the last pose for "PostDelay" time
            Thread.Sleep(mtl.PostMotionHold);
            
            // Recover initial pose
            if (mtl.PoseRecover)
            {
                //Debug.WriteLine("NaoClient: Pose is about to decay for: " + mtl.BehaviorsDesp);
                RecoverPoses(mtl.BehaviorsDesp, (float)mtl.RecovSpd);
            }

            // Behavior finished
            MotionRunning = false;
            CurMtl = null;
            

            //Debug.WriteLine("NaoClient: Behavior finished: " + mtl.BehaviorsDesp);
            // inform outer context
            // Event to inform the behavior is finished
            if (evBehaviorFinished != null)
            {
                evBehaviorFinished(mtl.BehaviorsDesp);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mf"></param>
        void threadAngleInterpolation(MotionFrame mf)
        {
            // Mutex
            lock (lockMotion)
            {
                HeadInhibition(mf);

                try
                {
                    if (mf.UsingSpeedFraction == false) 
                    {
                        List<float> joint_times = SmoothMoveTime(mf.ComboJointNames, mf.ComboJointValueRadian, (float)mf.SpeedFraction);
                        proxyMotion.angleInterpolation(mf.ComboJointNames, mf.ComboJointValueRadian, joint_times, mf.IsAbsolute);
                    }
                    else //mf.UsingSpeedFraction == true
                    {

                        List<float> jv = mf.ComboJointValueRadian;
                        proxyMotion.angleInterpolationWithSpeed(mf.ComboJointNames, mf.ComboJointValueRadian, (float)mf.SpeedFraction);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine("MotionProxy.angleInterpolation Exception: " + ex);
                }
            }
        }

        public bool HeadYawInhibition = false;
        public bool HeadPitchInhibition = false;
        /// <summary>
        /// Keep the head looking forward. 
        /// Disable all head manipulation.
        /// </summary>
        private void HeadInhibition(MotionFrame mf)
        {
            if (HeadYawInhibition == true)
            {
                int indhy = mf.ComboJointNames.IndexOf("HeadYaw");
                if (indhy >= 0)
                {
	                mf.ComboJointNames.RemoveAt(indhy);
	                mf.ComboJointValueRadian.RemoveAt(indhy);
                }
            }

            if (HeadPitchInhibition == true)
            {
                int indhp = mf.ComboJointNames.IndexOf("HeadPitch");
                if (indhp >= 0)
                {
	                mf.ComboJointNames.RemoveAt(indhp);
	                mf.ComboJointValueRadian.RemoveAt(indhp);
                }
            }
        }

        /// <summary>
        /// Determine the motion duration according to a defined speed value.
        ///  use the largest duration for all joints!
        /// TODO: perhaps, it is better to make a weight for each joint,
        ///              since some joints like the Wrist can move faster.
        ///              use the rad ratio: angular speed -> line speed
        ///                Wrist: width of forearm, 
        ///                Shoulder: length of upper arm
        /// </summary>
        /// <param name="jointnames"></param>
        /// <param name="jointvals"></param>
        /// <param name="speedfraction"></param>
        /// <returns></returns>
        private List<float> SmoothMoveTime(List<string> jointnames, List<float> jointvals, float speedfraction)
        {
            List<float> joint_vals_cur = proxyMotion.getAngles(jointnames, true);
            
            // empirical value: 360 degree per second
            // it is visually the same as SpeedFraction is 1.0
            float speedfactor = (float)(Math.PI * 360.0f / 180.0f); 

            // find the maximum joint angle change in radian
            float maxdelta = 0;
            for (int i = 0; i < joint_vals_cur.Count; i++)
            {
                float d = Math.Abs(jointvals[i] - joint_vals_cur[i]);
                if (d > maxdelta) maxdelta = d;
                //Debug.WriteLine("NaoClient: SmoothMove delta " + jointnames[i] + " : " + d);
            }

            // use the maximum needed time for all joints
            List<float> joint_times = new List<float>();
            float t = (float)(maxdelta / (speedfactor * speedfraction));
            if (t <= 0) t = 0.1f; // time must > 0

            //Debug.WriteLine("NaoClient: Speed Time: " + t);

            for (int j = 0; j < joint_vals_cur.Count; j++) { joint_times.Add(t); }

            return joint_times;
        }

        public void ForceMovementDecay(string behaviorname)
        {
            MotionRunning = false;
            if (CurMtl != null) // else the movement has finished
            {
	            if (CurMtl.BehaviorsDesp == "LegRandomMove")
	            {
	            	CurMtl.SetAllHold(0);
	            }
            }
        }

        // TKey: The name of the behavior that needs to be recovered
        //       Several behaviors could be executed at the same time.
        //       e.g., head behavior and arm behavior are executed at the same time.
        //       They should not have the same name.
        // TValue: joint values
        Dictionary<string, List<float>> StdJVRs = new Dictionary<string, List<float>>();
        Dictionary<string, List<string>> StdJNs = new Dictionary<string, List<string>>();
        /// <summary>
        /// Store joints
        /// </summary>
        /// <param name="bhvrName"></param>
        /// <param name="ljc"></param>
        void StorePoses(string bhvrName, List<string> ljc)
        {
            lock (lockMotion)
            {
                // Should not do this! It is safer to overwrite existing value. See below
                //StdJNs.Clear();
                //StdJVRs.Clear();

                List<float> ljvr;
                List<string> ljn = new List<string>();

                foreach (var jc in ljc)
                {
                    // radians
                    ljn.AddRange(proxyMotion.getJointNames(jc));
                    //ljn.AddRange(proxyMotion.getBodyNames(jc)); // version 1.14
                }

                // radians
                ljvr = proxyMotion.getAngles(ljn, false);
                if (StdJNs.ContainsKey(bhvrName))
                {
                    StdJNs[bhvrName] = ljn;
                    StdJVRs[bhvrName] = ljvr;
                    //Debug.WriteLine("NaoClient: StorePoses: behavior already exist: " + bhvrName + " overwritten!");
                } 
                else
                {
                    StdJNs.Add(bhvrName, ljn);
                    StdJVRs.Add(bhvrName, ljvr);
                }
            }
        }

        /// <summary>
        /// Recover to the pose before the behavior.
        /// </summary>
        /// <param name="bhvrName"></param>
        /// <param name="recovSpd"></param>
        /// <param name="recovSpdT"></param>
        void RecoverPoses(string bhvrName, float recovSpd, List<float> recovSpdT = null)
        {
            lock (lockMotion)
            {
                if (StdJNs.ContainsKey(bhvrName))
                {
	                List<string> rjns = StdJNs[bhvrName];
	                List<float> rjvrs = StdJVRs[bhvrName];
	                StdJNs.Remove(bhvrName);
	                StdJVRs.Remove(bhvrName);

                    ConstantHead(rjns, rjvrs);

                    try
                    {
	                    if (recovSpd != float.NaN)
	                    {
	                        List<float> joint_times = SmoothMoveTime(rjns, rjvrs, recovSpd);
	                        proxyMotion.angleInterpolation(rjns, rjvrs, joint_times, true);
	                        //proxyMotion.angleInterpolationWithSpeed(rjns, rjvrs, recovSpd);
	                    }
	                    else
	                    {
	                        proxyMotion.angleInterpolation(rjns, rjvrs, recovSpdT, true);
	                    }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine("RecoverPoses error: " + ex);
                    }
                }
                else
                {
                    Debug.WriteLine("RecoverPoses: not exist: " + bhvrName);
                }
            }
        }

        public bool RecoverHead = true;
        private void ConstantHead(List<string> rjns, List<float> rjvrs)
        {
            if (RecoverHead == false)
            {
                int indhp = rjns.IndexOf("HeadPitch");
                if (indhp >= 0)
                {
                    rjns.RemoveAt(indhp);
                    rjvrs.RemoveAt(indhp);
                }

                //int indhy = rjns.IndexOf("HeadYaw");
                //if (indhy >= 0)
                //{
                //rjns.RemoveAt(indhy);
                //rjvrs.RemoveAt(indhy);
                //}
            }
        }

        #region IdleMovements
        NeutralMovement IdleMovement = null;
        MovingSettings IdleMovementSettings = null;
        void InitIdleMovement()
        {
            if (IdleMovement == null)
            {
                IdleMovementSettings = MovingSettings.Load("../../ConnectorNao/MovingSettings.xml");

                // set defaults
                if (IdleMovementSettings.BlinkEyesTimes.MinTime < 100) IdleMovementSettings.BlinkEyesTimes.MinTime = 2000;
                if (IdleMovementSettings.BlinkEyesTimes.MaxTime < 100) IdleMovementSettings.BlinkEyesTimes.MaxTime = 5000;
                if (IdleMovementSettings.MoveArmsTimes.MinTime < 100) IdleMovementSettings.MoveArmsTimes.MinTime = 750;
                if (IdleMovementSettings.MoveArmsTimes.MaxTime < 100) IdleMovementSettings.MoveArmsTimes.MaxTime = 1500;
                if (IdleMovementSettings.MoveHeadTimes.MinTime < 100) IdleMovementSettings.MoveHeadTimes.MinTime = 500;
                if (IdleMovementSettings.MoveHeadTimes.MaxTime < 100) IdleMovementSettings.MoveHeadTimes.MaxTime = 1500;
                if (IdleMovementSettings.MoveHandsTimes.MinTime < 100) IdleMovementSettings.MoveHandsTimes.MinTime = 1000;
                if (IdleMovementSettings.MoveHandsTimes.MaxTime < 100) IdleMovementSettings.MoveHandsTimes.MaxTime = 2500;
                if (IdleMovementSettings.MoveHipsTimes.MinTime < 100) IdleMovementSettings.MoveHipsTimes.MinTime = 500;
                if (IdleMovementSettings.MoveHipsTimes.MaxTime < 100) IdleMovementSettings.MoveHipsTimes.MaxTime = 1000;
                if (IdleMovementSettings.RandomEyesTimes.MinTime < 100) IdleMovementSettings.RandomEyesTimes.MinTime = 500;
                if (IdleMovementSettings.RandomEyesTimes.MaxTime < 100) IdleMovementSettings.RandomEyesTimes.MaxTime = 1000;

                IdleMovement = new NeutralMovement(this.proxyMotion, this.lockMotion, IdleMovementSettings, this.proxyLed, this.lockLed);
                IdleMovement.NeutralStopped += new EventHandler(IdleMovementStopped);
                IdleMovement.NeutralPaused += new EventHandler(IdleMovementPaused);
            }
        }

        Object lockIdleBehavior = new Object();
        public void StartIdleMovement()
        {
            lock (lockIdleBehavior)
            {
	            if (IdleMovement.KeepNeutralRunning == false)
	            {
	                this.StandUp();
	                Thread.Sleep(2000);
	                IdleMovement.StartNeutralRunning();
	                IdleMovement.KeepNeutralRunning = true;
	            }
            }
        }

        public void StopIdleMovement()
        {
            lock (lockIdleBehavior)
            {
	            if (IdleMovement.KeepNeutralRunning == true)
	            {
	                IdleMovement.PauseMoving = true;
	                IdleMovement.KeepNeutralRunning = false;
	                bool result = NeutralStoppedAutoReset.WaitOne(1500);
	                Debug.WriteLine("StopIdleMovement: "  + result);
	            }
            }
        }

        public void PauseIdleMovement()
        {
            lock (lockIdleBehavior)
            {
                IdleMovement.PauseMoving = true;
                bool result = NeutralPausedAutoReset.WaitOne(1500);
                Debug.WriteLine("PauseIdleMovement: " + result);
            }
        }

        public void ResumeIdleMovement()
        {
            lock (lockIdleBehavior)
            {
                IdleMovement.PauseMoving = false;
                //Thread.Sleep(500);
            }
        }

        public AutoResetEvent NeutralPausedAutoReset = new AutoResetEvent(false);
        void IdleMovementPaused(object sender, EventArgs e)
        {
            NeutralPausedAutoReset.Set();
        }

        public AutoResetEvent NeutralStoppedAutoReset = new AutoResetEvent(false);
        void IdleMovementStopped(object sender, EventArgs e)
        {
            NeutralStoppedAutoReset.Set();
            //Debug.WriteLine("");
        }
        #endregion IdleMovements

        #region MISC behaviors
        public void StandUp()
        {
            // Mutex
            lock (lockMotion)
            {
                //proxyMotion.walkTo(0.1F,0.0F,0.0F);
                RunChoregrapheBehavior("standup");
            }
        }

        public void Squat()
        {
            // Mutex
            lock (lockMotion)
            {
                RunChoregrapheBehavior("SquatSafeMotorOff");
            }
        }

        public void Walk(float x, float y = 0, float theta = 0)
        {
            // Mutex
            lock (lockMotion)
            {
                proxyMotion.walkTo(x,y,theta);
            }
        }

        public void InitPose()
        {
            List<string> ljn = new List<string>()
            {
                "HeadYaw","HeadPitch",
                "LShoulderPitch","LShoulderRoll","LElbowYaw","LElbowRoll","LWristYaw","LHand",
                "RShoulderPitch","RShoulderRoll","RElbowYaw","RElbowRoll", "RWristYaw", "RHand", 
                "LHipYawPitch", "LHipRoll","LHipPitch","LKneePitch","LAnklePitch","LAnkleRoll",
                "RHipRoll","RHipPitch","RKneePitch","RAnklePitch","RAnkleRoll"
            };
            List<float> ljvd = new List<float>()
            {
                0,0,80,20,-80,-60,0,0,80,-20,80,60,0,0, 
                0,0,-25,40,-20,0,0,-25,40,-20,0
            };
            List<float> ljvr = ljvd.Select(x => (float)(x * Math.PI / 180)).ToList();
            // Mutex
            lock (lockMotion)
            {
                proxyMotion.angleInterpolationWithSpeed(ljn, ljvr, 0.2F);
            }
        }
        #endregion MISC behaviors

        #region Unsafe Test
        /// <summary>
        /// !!!Tested only with simulator!!!
        /// 
        /// Conclusion: 
        /// when two proxy instances in two threads operates different joints, 
        ///  they are executed in the same time.
        ///  
        /// when two proxy instances in two threads operates the same joints at the same time,
        ///  they are scheduled to be executed one by one.
        ///  
        /// SOAP error will occur if two threads operates 
        ///  the same instance of MotionProxy at the same time!
        /// </summary>
        public void TestMotionProxies()
        {
            MotionProxy mp1 = new MotionProxy(this.IP, Port);
            MotionProxy mp2 = new MotionProxy(this.IP, Port);

            List<string> jn1 = new List<string>(){"RShoulderPitch", "RShoulderRoll"};
            List<float> jv1 = new List<float>(){0.0f, -1.0f};
            List<float> jv3 = new List<float>() { -0.5f, 0.25f };
            List<float> t1 = new List<float>() { 1.0f, 1.0f };
            List<string> jn2 = new List<string>() { "RHipPitch", "RHipRoll" };
            List<float> jv2 = new List<float>() { -0.5f, -0.5f };
            List<float> t2 = new List<float>() { 1.0f, 1.0f };

            BackgroundWorker bgw1 = new BackgroundWorker();
            BackgroundWorker bgw2 = new BackgroundWorker();

            bgw1.DoWork += delegate
            {
                mp1.angleInterpolation(jn1, jv1, t1, true);
            };
            bgw2.DoWork += delegate
            {
                // No problem so far; executed simultaneously
                mp2.angleInterpolation(jn2, jv2, t2, true);

                // No problem so far; scheduled one by one; order is not definite
                //mp2.angleInterpolation(jn1, jv3, t2, true);

                // SOAP error. sometimes caused at bgw1, sometimes bgw2
                // some times no errors occurred, probably because 
                //   the two threads were not operating at the same time coincidently
                //mp1.angleInterpolation(jn2, jv2, t2, true);

                // SOAP error. sometimes caused at bgw1, sometimes bgw2
                // some times no errors occurred, probably because 
                //   the two threads were not operating at the same time coincidently
                //mp1.angleInterpolation(jn1, jv3, t2, true);
            };

            bgw1.RunWorkerAsync();
            bgw2.RunWorkerAsync();
        }

        /// <summary>
        /// !!!Tested only with simulator!!!
        /// CrgBehaviors are built using TimeLineTemplate with one key pose
        /// 
        /// when two proxy instances in two threads operates different joints at the same time, 
        ///  they are executed in the same time.
        ///  
        /// when two proxy instances in two threads operates the same joints at the same time,
        ///  one will override the other; order is not definite!
        /// 
        /// SOAP error will occur if the same proxy instance in two threads  
        ///   operates at the same time!
        /// </summary>
        public void TestBehaviorManagers()
        {
            BehaviorManagerProxy bmp1 = new BehaviorManagerProxy(this.IP, Port);
            BehaviorManagerProxy bmp2 = new BehaviorManagerProxy(this.IP, Port);

            BackgroundWorker bgw1 = new BackgroundWorker();
            BackgroundWorker bgw2 = new BackgroundWorker();

            bgw1.DoWork += delegate
            {
                bmp1.runBehavior("testarm2");
            };
            bgw2.DoWork += delegate
            {
                // One will override the other; order is not definite! 
                // No problem! I can see the two behaviors running (red square) 
                //  at the same time in Choregraphe->BehaviorManager
                //bmp2.runBehavior("testarm1");

                // Executed simultaneously
                // No problem! I can see the two behaviors running (red square) 
                //  at the same time in Choregraphe->BehaviorManager
                bmp2.runBehavior("testhead1");

                // SOAP error;
                // some times no errors occurred, probably because 
                //   the two threads were not operating at the same time coincidently
                //bmp1.runBehavior("testarm1");

                // SOAP error;
                // some times no errors occurred, probably because 
                //   the two threads were not operating at the same time coincidently
                //bmp1.runBehavior("testhead1");
            };

            bgw1.RunWorkerAsync();
            bgw2.RunWorkerAsync();
        }
        #endregion Unsafe Test



        #region TTS Speech
        TextToSpeechProxy proxyTTS = null;
        Object lockTTS = new Object();

        /// <summary>
        /// Text to speech. Queued Thread mode.
        /// </summary>
        /// <param name="text">Text to say.</param>
        public void Say(string text)
        {
            if (proxyTTS != null)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(
                    delegate(object text2say)
                    {
                        lock (lockTTS)
                        {
                            proxyTTS.say(text);

                            if (evSpeechFinished != null)
                            {
                                evSpeechFinished(text.Length.ToString());
                            }
                        }
                    }), text);
            }
            else if (evSpeechFinished != null)
            {
                evSpeechFinished("TTSNotConnected");
            }
        }

        // events
        public event ehSpeechFinished evSpeechFinished;
        public delegate void ehSpeechFinished(string msg);

        #endregion TTS Speech

        public byte[] CapturePhoto()
        {
            if (proxyCamera != null)
            {
	            // Subscribe a Vision Module to ALVideoDevice, starting the
	            // frame grabber if it was not started before.
	            string subscriberID = "RoboTutor";
	            int fps = 5;
	            // The subscriberID can be altered if other instances are already running
	            // 1: 320*240
                // 2: 640*480
	            // 13: BGR
	            subscriberID = this.proxyCamera.subscribe(subscriberID, 2, 13, fps);
	
	            // Retrieve the current image.
	            Object imageObject = this.proxyCamera.getImageRemote(subscriberID);
	            byte[] imageBytes = (byte[])((ArrayList)imageObject)[6];
	
	            // Not necessary for getImageRemote(), and it causes exceptions
	            //this.proxyCamera.releaseImage(subscriberID);
	
	            // Unsubscribe the V.M.
	            this.proxyCamera.unsubscribe(subscriberID);
	
	            return imageBytes;
            } 
            else
            {
                return null;
            }
        }
    }
}
