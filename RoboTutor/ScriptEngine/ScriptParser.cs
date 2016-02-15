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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.IO;


namespace ScriptEngine
{
    /// <summary>
    /// The script parser
    /// 
    /// 1. When behavior is slower than speech: there should be a termination routine for behavior.
    /// 2. When speech is slower; next behavior should not start.
    /// 3. Slide command should wait for behavior and speech.
    /// </summary>
    public class ScriptParser
    {
        #region Event Action
        public event ehExecuteBehavior evExecuteBehavior;
        public delegate void ehExecuteBehavior(string behaviorname);

        public event ehExecuteSpeech evExecuteSpeech;
        public delegate void ehExecuteSpeech(string text);

        public event ehExecuteListen evExecuteListen;
        public delegate void ehExecuteListen(string cmd);

        public event ehExecuteScript evExecuteScript;
        public delegate void ehExecuteScript(string file, string speechAccept);

        public event ehSlideControl evSlideControl;
        public delegate void ehSlideControl(string cmd);

        public event ehCameraPhoto evCameraPhoto;
        public delegate void ehCameraPhoto();

        public event ehFetchQuizAnswers evFetchQuizAnswers;
        public delegate void ehFetchQuizAnswers(out bool disagree, out int useranswer, out int useranswer2);

        public event ehChangeConfig evChangeConfig;
        public delegate void ehChangeConfig(string config);

        public event ehChangeMood evChangeMood;
        public delegate void ehChangeMood(string mood);

        public event ehScriptExeFinished evScriptExeFinished;
        public delegate void ehScriptExeFinished();

        public event ehLatestScriptUnit evLatestScriptUnit;
        public delegate void ehLatestScriptUnit(string behaviorname);

        internal event ehShowCurrentLine evShowCurrentLine;
        internal delegate void ehShowCurrentLine(int linenum);

        internal event ehInterupt evInterupt;
        internal delegate void ehInterupt();
        #endregion Event Action

        /// <summary>
        /// Current manner is: parse + execute piece-by-piece
        /// </summary>
        /// <param name="script"></param>
        public void ExecuteScript(string[] script)
        {
            ScriptLineCount = script.Count();

            BackgroundWorker bgwProducer = new BackgroundWorker();
            bgwProducer.DoWork += new DoWorkEventHandler(ProcessScriptInQueue);
            EnabledParse = true;
            bgwProducer.RunWorkerAsync(script);

            BackgroundWorker bgwConsumer = new BackgroundWorker();
            bgwConsumer.DoWork += new DoWorkEventHandler(ExecuteScriptFromQueue);
            EnableExecute = true;
            bgwConsumer.RunWorkerAsync();
        }

        /// <summary>
        /// For thread-safe, use ConcurrentQueue
        /// </summary>
        Queue<ScriptUnit> ScriptQueue = new Queue<ScriptUnit>();
        const int QueueSize = 10;
        Mutex MutQueue = new Mutex();
        int ScriptLineCount;

        bool ScriptParseComplete = false;

        bool EnabledParse = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessScriptInQueue(object sender, DoWorkEventArgs e)
        {
            ScriptQueue.Clear();

            ScriptParseComplete = false;

            string[] script = (string[])e.Argument;

            int i = 0;
            while (i < ScriptLineCount)
            {
                if (EnabledParse)
                {
	                if (ScriptQueue.Count < QueueSize)
	                {
		                ScriptUnit su = new ScriptUnit();
	                    su.LineNumber = i;
		
		                string res;
		                ScriptType st = ParseLine(script[i], out res);
                        
                        // A full unit contains
                        //  1. behavior + speech
                        //  2. behavior + pause
                        //  3. only behavior
		                if (st == ScriptType.BEHAVIOR) 

		                {
		                    su.Behavior = res;
		                    su.UnitType |= ScriptType.BEHAVIOR;
		
                            if( i+1<script.Count() ){
                                string res1;
		                        ScriptType st1 = ParseLine(script[i+1], out res1);
		                        if (st1 == ScriptType.SPEECH)
		                        {
		                            su.Speech = res1;
		                            su.UnitType |= ScriptType.SPEECH;
		                            i++;
		                        }
                                else if (st1 == ScriptType.PAUSE)
                                {
                                    su.PauseDur = int.Parse(res1);
                                    su.UnitType |= ScriptType.PAUSE;
                                    i++;
                                }
                            }
	
	                        // Send out for check if it is a leg movement
	                        evLatestScriptUnit(su.Behavior);
		                }
		                else if (st == ScriptType.SPEECH) // this unit does not have Behavior
		                {
		                    su.Speech = res;
		                    su.UnitType |= ScriptType.SPEECH;
		                }
                        else if (st == ScriptType.LISTEN)
                        {
                            su.Listen = res;
                            su.UnitType |= ScriptType.LISTEN;
                        }
                        else if (st == ScriptType.SCRIPT)
                        {
                            string[] temp = res.Split('|');
                            su.ScriptFile = temp[0];
                            if (temp.Length > 1)
                                su.ScriptSpeechAccept = temp[1];
                            else
                                su.ScriptSpeechAccept = "";

                            //EnabledParse = false;
                            su.UnitType |= ScriptType.SCRIPT;
                        }
                        else if (st == ScriptType.PAUSE)
                        {
                            su.PauseDur = int.Parse(res);
                            su.UnitType |= ScriptType.PAUSE;
                        }
                        else if (st == ScriptType.SLIDE)
                        {
                            su.UnitType |= ScriptType.SLIDE;
                        }
                        else if (st == ScriptType.QUIZ_ANSWER)
                        {
                            su.UnitType = ScriptType.QUIZ_ANSWER;
                            su.QuizAnswer = new List<string>();
                            string[] qa = res.Split('|');
                            su.QuizAnswer.AddRange(qa.ToList<string>());
                        }
                        else if (st == ScriptType.CAMERA)
                        {
                            su.UnitType = ScriptType.CAMERA;
                        }
                        else if (st == ScriptType.EMPTY_LINE
                            || st == ScriptType.COMMENT
                            || st == ScriptType.UNDEFINED)
                        {
                            // Do nothing
                        }
                        else if (st == ScriptType.CONFIG)
                        {
                            su.UnitType = ScriptType.CONFIG;
                            su.Config = res;
                        }
                        else if (st == ScriptType.MOOD)
                        {
                            su.UnitType = ScriptType.MOOD;
                            su.Mood = res;
                        }
                        else if (st == ScriptType.INTERUPT)
                        {
                            su.UnitType = ScriptType.INTERUPT;
                        }
	
	                    MutQueue.WaitOne();
		                // In queue
		                ScriptQueue.Enqueue(su);
	                    MutQueue.ReleaseMutex();
	
	                    i++;
	                }
	                else
	                {
	                    //Debug.WriteLine("{0}: ScriptUnit queue full - Queue Length {1}", "ScriptParser", ScriptQueue.Count);
	                }
                }

                Thread.Sleep(333);
            }

            ScriptParseComplete = true;
            Debug.WriteLine("{0}: All Script Parsed! Total lines: {1}", "ScriptParser", i);
        }

        bool EnableExecute = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecuteScriptFromQueue(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while(i < ScriptLineCount)
            {
                if (EnableExecute)
                {
	                if (ScriptQueue.Count > 0)
	                {
		                MutQueue.WaitOne();
		                ScriptUnit su = ScriptQueue.Dequeue();
	                    MutQueue.ReleaseMutex();
	
	                    evShowCurrentLine(su.LineNumber);
	
		                i++;

                        // log
                        string curtime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                        string logfile = Directory.GetCurrentDirectory() + "\\..\\..\\log\\StorytellingLog.txt";
                        StreamWriter logger = new StreamWriter(logfile, true);
                        string strstart = curtime + "\t" 
                                    + su.UnitType.ToString() + "\t"
                                    + su.Behavior + "\t"
                                    + su.Speech;
                        logger.WriteLine(strstart);
                        logger.Close();
	
	                    // Execute ScriptUnit; 
	                    // This is a blocking-call; It waits for finish of e.g., speech, behavior
		                ExecuteCmd(su);
	                }
	                else
	                {
	                    if (ScriptParseComplete == true)
	                    {
	                        break;
	                    }
	                    //Debug.WriteLine("{0}: ScriptUnit queue empty! Script Line {1}", "ScriptParser", i);
	                }
                }

                Thread.Sleep(100);
            }

            evScriptExeFinished();
        }

        AutoResetEvent areBehaviorDone = new AutoResetEvent(false);
        AutoResetEvent areSpeechDone = new AutoResetEvent(false);
        private void ExecuteCmd(ScriptUnit su)
        {
            if(su.UnitType == (ScriptType.BEHAVIOR | ScriptType.SPEECH))
            {
                evExecuteBehavior(su.Behavior);
                evExecuteSpeech(su.Speech);
                areBehaviorDone.WaitOne();
                areSpeechDone.WaitOne();
            }
            else if (su.UnitType == (ScriptType.BEHAVIOR | ScriptType.PAUSE))
            {
                evExecuteBehavior(su.Behavior);
                Thread.Sleep(su.PauseDur);
                areBehaviorDone.WaitOne();
            }
            else if (su.UnitType == ScriptType.BEHAVIOR)
            {
                evExecuteBehavior(su.Behavior);
                areBehaviorDone.WaitOne();
            }
            else if (su.UnitType == ScriptType.SPEECH)
            {
                evExecuteSpeech(su.Speech);
                areSpeechDone.WaitOne();
            }
            else if (su.UnitType == ScriptType.LISTEN)
            {
                evExecuteListen(su.Listen);
            }
            else if (su.UnitType == ScriptType.SCRIPT)
            {
                evExecuteScript(su.ScriptFile, su.ScriptSpeechAccept);
                if(su.ScriptSpeechAccept.Length > 0)
                    areSpeechDone.WaitOne();
            }
            else if (su.UnitType == ScriptType.PAUSE)
            {
                Thread.Sleep(su.PauseDur);
            }
            else if (su.UnitType == ScriptType.SLIDE)
            {
                evSlideControl("NextSlide");
            }
            else if (su.UnitType == ScriptType.CAMERA)
            {
                evCameraPhoto();
            }
            else if (su.UnitType == ScriptType.QUIZ_ANSWER)
            {
                bool disagreement;
                int useranswer1, useranswer2;
                evFetchQuizAnswers(out disagreement, out useranswer1, out useranswer2);
                string speechtoanswers;

                if (useranswer1 == -1) // this means the answer data is not ready!
                {
                    speechtoanswers = "It seems no one knows the answer!";
                }
                else
                {
                    if (disagreement == true) { speechtoanswers = su.QuizAnswer.Last<string>(); }
                    else { speechtoanswers = su.QuizAnswer[useranswer1]; }
                }

                evExecuteSpeech(speechtoanswers);
                areSpeechDone.WaitOne();
            }
            else if (su.UnitType == ScriptType.CONFIG)
            {
                evChangeConfig(su.Config);
            }
            else if (su.UnitType == ScriptType.MOOD)
            {
                evChangeMood(su.Mood);
            }
            else if (su.UnitType == ScriptType.INTERUPT)
            {
                evInterupt();
            }
        }

        /// <summary>
        /// I found the pause in NAO TTS:
        ///   \pau=1000\
        ///   Something to say
        /// does not work, but
        ///   \pau=1000\ Something to say
        /// does work!
        /// </summary>
        /// <param name="line"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public ScriptType ParseLine(string line, out string result)
        {
            result = null;

            if (line == String.Empty)
            {
                return ScriptType.EMPTY_LINE;
            }
            else if (line.StartsWith("#"))
            {
                return ScriptType.COMMENT;
            }
            else if (line.Contains("{idle_leg}") // TODO: list in a file later
                || line.Contains("{idle_head}")
                || line.Contains("{idle_eye}")
                || line.Contains("{text_senti_enabled}")
                || line.Contains("{constant_head}")
                || line.Contains("{forwardlooking_head}")
                || line.Contains("{freelooking_head}") )
            {
                result = line.Replace("{", "").Replace("}", "");
                return ScriptType.CONFIG;
            }
            else if (line.StartsWith("{behavior|") && line.EndsWith("}"))
            {
                result = line.Replace("{behavior|", "").Replace("}", "");
                return ScriptType.BEHAVIOR;
            }
            else if (line.StartsWith("{mood|") && line.EndsWith("}"))
            {
                result = line.Replace("{mood|", "").Replace("}", "");
                return ScriptType.MOOD;
            }
            else if (line.StartsWith("{slide}"))
            {
                return ScriptType.SLIDE;
            }
            else if (line.StartsWith("{quiz|") && line.EndsWith("}"))
            {
                result = line.Replace("{quiz|", "").Replace("}", "");
                return ScriptType.QUIZ_ANSWER;
            }
            else if (line.StartsWith("{listen|") && line.EndsWith("}"))
            {
                result = line.Replace("{listen|", "").Replace("}", "");
                return ScriptType.LISTEN;
            }
            else if (line.StartsWith("{script|") && line.EndsWith("}"))
            {
                result = line.Replace("{script|", "").Replace("}", "");
                return ScriptType.SCRIPT;
            }
            else if (line.StartsWith("{camera}"))
            {
                return ScriptType.CAMERA;
            }
            else if (line.StartsWith("{pause}"))
            {
                return ScriptType.INTERUPT;
            }
            // This match a pure "\pau=?\"; see C# Regex
            else if (new Regex(@"^(\\pau=)(\w+)(\\)$").IsMatch(line))
            {
                result = line.Replace("\\pau=", String.Empty).Replace("\\", String.Empty);
                return ScriptType.PAUSE;
            }
            else if (line != String.Empty)
            {
                result = line;
                return ScriptType.SPEECH;
            }
            else
            {
                return ScriptType.UNDEFINED;
            }
        }

        public void PauseScript()
        {
            EnabledParse = false;
            EnableExecute = false;
        }

        public void ResumeScript()
        {
            EnableExecute = true;
            EnabledParse = true;
        }

        public void StopScript()
        {
            //ScriptParseComplete = false; // done by ProcessScriptInQueue()
            //IsParserOnTheWay = false; // done by ExecuteScriptFromQueue()
            ScriptLineCount = 0; // This ends the Process and Execution threads: ends the loop 
            EnabledParse = false;
            EnableExecute = false;
            ScriptQueue.Clear();
        }

        internal void BehaviorDone()
        {
            areBehaviorDone.Set();
        }

        internal void SpeechDone()
        {
            areSpeechDone.Set();
        }

    }

    /// <summary>
    /// Hex for flags use 1, 2, 4, 8
    /// 0x0512 does not mean 2^9, it is 5*256+16+2
    /// </summary>
    [Flags]
    public enum ScriptType
    {
        UNDEFINED = 0x0001,
        BEHAVIOR = 0x0002,
        SPEECH = 0x0004,
        LISTEN = 0x0008,
        QUIZ_ANSWER = 0x0010,
        SLIDE = 0x0020,
        EMPTY_LINE = 0x0040,
        COMMENT = 0x0080,
        CAMERA = 0x0100,
        SCRIPT = 0x0200,
        CONFIG = 0x0400,
        PAUSE = 0x0800,
        MOOD = 0x1000,
        INTERUPT = 0x1200,
        /*UNDEFINED   = 0x0001,
        BEHAVIOR    = 0x0002,
        SPEECH      = 0x0004,
        QUIZ_ANSWER = 0x0008,
        SLIDE       = 0x0010,
        EMPTY_LINE  = 0x0020,
        COMMENT     = 0x0040,
        CAMERA      = 0x0080,
        CONFIG      = 0x0100,
        PAUSE       = 0x0200,
        MOOD        = 0x0400,
        INTERUPT    = 0x0800,*/
    }

    /// <summary>
    /// End with a Speech Sentence
    /// Can contain only one Sentence and Behavior.
    /// Within the Unit there is no End Point Sync.
    /// </summary>
    class ScriptUnit
    {
        public ScriptType UnitType;
        public int LineNumber;

        public string Speech;
        public string Behavior;
        public string Listen;
        public string ScriptFile;
        public string ScriptSpeechAccept;
        public List<string> QuizAnswer;
        public string Config;
        public int PauseDur;

        public string Mood;
    }

    //public class ScriptBlock
    //{
    //}
}
