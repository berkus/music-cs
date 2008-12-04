using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MCModule.Exceptions;

namespace MCModule
{
	public class Worker
	{
		public Worker()
		{
		}
		
		public void Run(String[] args)
		{
			Arguments opt = new Arguments(args);
			Config cfg;
			
			try
			{
				if (opt["file"] == null)
				{
					Console.WriteLine("Arquivo não definido.");
					cfg = Config.LoadConfig("../data/config.xml");
				}
				else
				{
					cfg = Config.LoadConfig(opt["file"]);
				}
				
				Execute(cfg);
			}
			catch(MCModule.Exception e)
			{
				e.WriteMessage();
			}
			catch(System.Exception e)
			{
				Console.WriteLine(" ** Message Generated ** ");
				Console.WriteLine(e.Message);
												
				Console.WriteLine(" ** Stack Trace ** ");
				Console.WriteLine(e.StackTrace);				
			}
		}
		
		///@brief Runs the tasks for each algorithm
		unsafe static public void Execute(Config cfg)
		{			
			foreach (Algorithm a in cfg.GetAlgorithms())
			{
				ICollection<ActionNode> trainList = cfg.GetTrainingData();
				ICollection<ActionNode> classifyList = cfg.GetClassificationData();
				LinkedList<string> dirs = new LinkedList<string>();
				LinkedList<string> classList = new LinkedList<string>();
				
				if(trainList.Count > 0)
				{
					foreach(ActionNode ac in trainList)
					{
						//List of existing classes
						classList.AddLast(ac.Name);
						dirs.AddLast(ac.Dir);
					}
					
					Console.WriteLine("Extracting ...");
					MCModule.UnmanagedInterface.MCDataCollection * dtCol = Extractor.BatchExtract(a.FeatList, a.STFTWindow, dirs);
					
					a.AlgClassifier.Filter(dtCol);
					
					Console.WriteLine("Training ...");
					a.AlgClassifier.Train(dtCol);
				}
				else
				{
					a.AlgClassifier.TryLoadingParameters();
				}
				
				dirs.Clear();
				
				if(classifyList.Count > 0)
				{
					foreach(ActionNode ac in classifyList)
					{
						dirs.AddLast(ac.Dir);
					}
					
					Console.WriteLine("Classifying ...");
					a.AlgClassifier.Classify();
				}
			}
		}
	}
}
