using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace TelegramMessagingService
{
    public partial class TelegramMessenger : ServiceBase
    {
		static string token = ConfigurationManager.AppSettings["TelegramBotToken"];
		string chatId = ConfigurationManager.AppSettings["ChatId"];
		TelegramBotClient Bot = new TelegramBotClient(token);
		string filePath = appPath + @"\Documents\";
		private volatile bool stopping = false;
		int interval = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalToCallProc"]);
		static string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		static Random random = new Random();
		public TelegramMessenger()
        {
            InitializeComponent();
        }
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
		internal void StartDebug()
		{
			Logger.WriteDebugLog("Service started in DEBUG mode.");
			OnStart(null);
		}
		protected override void OnStart(string[] args)
        {
			Logger.WriteDebugLog("Service Started at: " + DateTime.Now);
            while (!stopping)
            {
                try
                {
                    Task.Run(async () => { await sendMessage(); });
                    
                }
                catch (Exception ex)
                {
                    Logger.WriteErrorLog(ex.Message);
                }
                finally
                {
                    Thread.Sleep(interval * 60 * 1000);
                }
            }
        }

        protected override void OnStop()
        {
			
			Logger.WriteDebugLog("Service stopped at: " + DateTime.Now);
        }
		private async Task sendMessage()
		{
			try
			{
				
				InputOnlineFile inputOnlineFile;
				bool isDocument = false;
				string fileToSend = string.Empty;
				int nextDocNumber = random.Next(0, 50);
				if (nextDocNumber <= 10)
				{
					fileToSend =  @"Documentation.pdf";
					isDocument = true;
				}
				else if (nextDocNumber <= 15)
				{
					fileToSend =  @"Rexroth.html";
					isDocument = true;
				}
				else if (nextDocNumber <= 20)
				{
					fileToSend =  @"Cool.jpg";
					isDocument = false;
				}
				else if (nextDocNumber <= 25)
				{
					fileToSend =  @"SampleTable_08_05_2020_18_48_50.docx";
					isDocument = true;
				}
				else if (nextDocNumber <= 30)
				{
					fileToSend =  @"blackGreen.jpg";
					isDocument = false;
				}
				else if (nextDocNumber <= 35)
				{
					fileToSend =  @"star.png";
					isDocument = false;
				}
				else if (nextDocNumber <= 40)
				{
					fileToSend =  @"blackRed.jpg";
					isDocument = false;
				}
				else
				{
					fileToSend =  @"SampleTable_08_05_2020_18_48_50.docx";
					isDocument = true;
				}
				using (FileStream fs = System.IO.File.OpenRead(filePath+fileToSend))
				{
					inputOnlineFile = new InputOnlineFile(fs,fileToSend);
					if (isDocument)
						await Bot.SendDocumentAsync(chatId: chatId, document: inputOnlineFile);
					else
						await Bot.SendPhotoAsync(chatId: chatId, photo: inputOnlineFile);
				}
				Logger.WriteDebugLog("File sent: " + fileToSend);
			}
			catch (Exception ex)
			{
				Logger.WriteErrorLog(ex.Message);
			}

		}
	}
}
