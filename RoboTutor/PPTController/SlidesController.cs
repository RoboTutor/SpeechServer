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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Messenger;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace PPTController
{
    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////
    /// For PPT control see:
    /// http://msdn.microsoft.com/en-us/library/microsoft.office.interop.powerpoint.slideshowview_members(v=office.14).aspx
    /// http://msdn.microsoft.com/en-us/library/microsoft.office.interop.powerpoint.slide_members(v=office.14).aspx
    /// /////////////////////////////////////////////////////////////////////////
    /// For ScriptEditor see:
    /// http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting
    /// </summary>
    public partial class SlidesController : UserControl, IMessenger
    {
        public TurningPointData TPD = new TurningPointData();

        public SlidesController()
        {
            InitializeComponent();
            // TODO: put list into XML, and save last use
            string slidepath = Path.GetFullPath(Environment.CurrentDirectory + "\\..\\..\\Slides\\");
            cbbFileAddress.Items.Add(slidepath + "NL_lang.pptx");
            cbbFileAddress.Items.Add(slidepath + "NaoLectureSlides3.3_Part1.pptx");
            //cbbFileAddress.Items.Add("D:\\junchaoxu\\HART2014\\Junchao_HART14.pptx"); // temp

            cbbFileAddress.SelectedIndex = 0;
        }

        ~SlidesController()
        {
            //Presentation.Close();
        }

        public string ID
        {
            get { return "PPTController"; }
        }

        public event ehSendMessage evSendMessage;
        public MessageEventArgs SendMessage(string sendto, MessageEventArgs msg)
        {
            if (evSendMessage != null)
            {
                return evSendMessage(sendto, this.ID, msg);
            }
            else
                return null;
        }

        public MessageEventArgs MessageHandler(string sendfrom, MessageEventArgs message)
        {

            if (message.MsgType == MessageEventArgs.MessageType.COMMAND)
            {
                if (message.Cmd == "NextSlide")
                {
                    NextSlide();
                }
                else if (message.Cmd == "PreviousSlide")
                {
                    PreviousSlide();
                }
                else if (message.Cmd.StartsWith("RunPPT"))
                {
                    string file = message.CmdArgs[0];
                    RunPresentation(file);
                }
                else if (message.Cmd == "ClosePPT")
                {
                    Presentation.SlideShowWindow.View.Exit();
                }
                else if (message.Cmd == "ShowPhoto")
                {
                    string picfile = message.CmdArgs[0];
                    ShowPictureNewSlide(picfile);
                }
                else if (message.Cmd == "FetchQuizAnswers")
                {
                    bool disagree = false;
                    int useranswer = -1, useranswer2 = -1;
                    try
                    {
                    	TPD.AnswerDistribution(out disagree, out useranswer, out useranswer2);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine("{0}: quiz data is not ready! {1}", this.ID, ex);
                    }
                    string[] data = new string[] { disagree.ToString(), useranswer.ToString(), useranswer2.ToString()};
                    MessageEventArgs backmsg = new MessageEventArgs(data);
                    return backmsg;
                }
            }

            return null;
        }

        /// <summary>
        /// Possible runtime error:
        ///  A first chance exception of type 'System.Runtime.InteropServices.COMException' occurred in PPTController.dll
        ///  An exception of type 'System.Runtime.InteropServices.COMException' occurred in PPTController.dll but was not handled in user code
        ///  Additional information: The message filter indicated that the application is busy. (Exception from HRESULT: 0x8001010A (RPC_E_SERVERCALL_RETRYLATER))
        /// </summary>
        private void PreviousSlide()
        {
            if (Presentation != null)
            {
                Retry.Do(delegate()
                {
                    Presentation.SlideShowWindow.View.Previous();
                }, TimeSpan.FromMilliseconds(500));
            }
        }

        private void NextSlide()
        {
            if (Presentation != null)
            {
                Retry.Do(delegate()
                {
                    Presentation.SlideShowWindow.View.Next();
                }, TimeSpan.FromMilliseconds(500));
            }
        }

        private void btnOpenPPT_Click(object sender, EventArgs e)
        {
            string presExisting = @cbbFileAddress.Text;
            RunPresentation(presExisting);
        }

        PowerPoint.Presentation Presentation = null;
        public void RunPresentation(string file)
        {
            if (Presentation == null)
            {
                PowerPoint._Application pApp = new PowerPoint.Application();

                // use existing (already opened) ppt file
                if (pApp.Presentations.Count > 0)
                {
                	Presentation = pApp.Presentations[1];
                }
                else // open the file
                {
                    try
                    {
                        Presentation = pApp.Presentations.Open(
                            file, Office.MsoTriState.msoFalse,
                            Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine("PPT open error: " + ex);
                    }
                        
                        
                    
                }
            }

            //Start playing presentation.
            Retry.Do(RunPpt, TimeSpan.FromSeconds(3));
        }

        private void RunPpt()
        {
            try
            {
                Presentation.SlideShowSettings.Run();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("PPT run error: " + ex);
                throw;
            }
        }

        private void btnNextSlide_Click(object sender, EventArgs e)
        {
            NextSlide();
        }

        private void btnPreviousSlide_Click(object sender, EventArgs e)
        {
            PreviousSlide();
        }

        private void btnClosePPT_Click(object sender, EventArgs e)
        {
            try
            {
            	Presentation.SlideShowWindow.View.Exit();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("PPT close error: " + ex);
            }
        }

        private void btnGoToSlide_Click(object sender, EventArgs e)
        {
            if (Presentation != null)
            {
	            int sldno = int.Parse(txtbSlideNum.Text);
	            Presentation.SlideShowWindow.View.GotoSlide(sldno);
            }
        }

        public void JumpToSlide(int distance, bool relative = false)
        {
            int curind = Presentation.SlideShowWindow.View.Slide.SlideIndex;
            int sldcnt = Presentation.Slides.Count;
            int targetslide = curind+distance;

            if (relative || targetslide > sldcnt)
			{
				if (distance > 0)
					NextSlide();
				else
					PreviousSlide();
			}
			else
				Presentation.SlideShowWindow.View.GotoSlide(targetslide);
        }

        public void ShowPictureNewSlide(string picturefile)
        {
            int curind = Presentation.SlideShowWindow.View.Slide.SlideIndex;
            // CustomLayouts[1]: Title page; CustomLayouts[2]: slide page
            PowerPoint.CustomLayout cl = Presentation.SlideMaster.CustomLayouts[2];
            PowerPoint.Slide sld = Presentation.Slides.AddSlide(curind+1, cl);

            try
            {
                sld.Shapes.AddPicture(picturefile, Office.MsoTriState.msoFalse, Office.MsoTriState.msoTrue, 50, 50, 640, 480);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Add picture slide error: " + ex);
            }
        }
    }
}
