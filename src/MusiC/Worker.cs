/*
 * The MIT License
 * Copyright (c) 2008 Marcos José Sant'Anna Magalhães
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

//namespace MCModule
//{
//	public class Worker
//	{
//		public Worker()
//		{
//		}
//		
//		public void Run(String[] args)
//		{
//			Arguments opt = new Arguments(args);
//			Config cfg;
//			
//			try
//			{
//				if (opt["file"] == null)
//				{
//					Console.WriteLine("Arquivo não definido.");
//					cfg = Config.LoadConfig("../data/config.xml");
//				}
//				else
//				{
//					cfg = Config.LoadConfig(opt["file"]);
//				}
//				
//				Execute(cfg);
//			}
//			catch(MCModule.Exception e)
//			{
//				e.WriteMessage();
//			}
//			catch(System.Exception e)
//			{
//				Console.WriteLine(" ** Message Generated ** ");
//				Console.WriteLine(e.Message);
//												
//				Console.WriteLine(" ** Stack Trace ** ");
//				Console.WriteLine(e.StackTrace);				
//			}
//		}
//		
//		///@brief Runs the tasks for each algorithm
//		unsafe static public void Execute(Config cfg)
//		{			
//			foreach (Algorithm a in cfg.GetAlgorithms())
//			{
//				ICollection<ActionNode> trainList = cfg.GetTrainingData();
//				ICollection<ActionNode> classifyList = cfg.GetClassificationData();
//				LinkedList<string> dirs = new LinkedList<string>();
//				LinkedList<string> classList = new LinkedList<string>();
//				
//				if(trainList.Count > 0)
//				{
//					foreach(ActionNode ac in trainList)
//					{
//						//List of existing classes
//						classList.AddLast(ac.Name);
//						dirs.AddLast(ac.Dir);
//					}
//					
//					Console.WriteLine("Extracting ...");
//					MCModule.UnmanagedInterface.MCDataCollection * dtCol = Extractor.BatchExtract(a.FeatList, a.STFTWindow, dirs);
//					
//					a.AlgClassifier.Filter(dtCol);
//					
//					Console.WriteLine("Training ...");
//					a.AlgClassifier.Train(dtCol);
//				}
//				else
//				{
//					a.AlgClassifier.TryLoadingParameters();
//				}
//				
//				dirs.Clear();
//				
//				if(classifyList.Count > 0)
//				{
//					foreach(ActionNode ac in classifyList)
//					{
//						dirs.AddLast(ac.Dir);
//					}
//					
//					Console.WriteLine("Classifying ...");
//					a.AlgClassifier.Classify();
//				}
//			}
//		}
//	}
//}
