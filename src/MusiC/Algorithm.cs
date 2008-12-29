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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using MusiC.Extensions;

namespace MusiC
{
	/// <summary>
	/// 
	/// </summary>
	public class Algorithm : MusiCObject
	{
		Pipeline _pipe = new Pipeline();
		
		class Pipeline : MusiCObject
		{
			mPipeline _mPipe = new mPipeline();
			uPipeline _uPipe = new uPipeline();
			
			ExtensionManagement _status = ExtensionManagement.NotSet;
			
			class mPipeline : MusiCObject
			{
				Managed.Window _window;
				Managed.Classifier _classifier;
				LinkedList<Managed.Feature> _featureList = new LinkedList<Managed.Feature>();
		
				public void AddClassifier(Managed.Classifier classifier)
				{
					_classifier = classifier;
				}
				
				public void AddWindow(Managed.Window window)
				{
					_window = window;
				}
				
				public void AddFeature(Managed.Feature feature)
				{
					_featureList.AddLast(feature);
				}
				
				public bool Check()
				{
					if(_window == null)
						return false;
					
					if(_featureList.Count == 0)
						return false;
					
					return true;
				}
				
				public void Execute()
				{
					
				}
				
				public void Say()
				{
					Message("Window: " + 
			        ((_window!=null)?_window.GetType().FullName:"((NULL))")
			        );
				
					Message("Classifier: " + 
			        ((_classifier!=null)?_classifier.GetType().FullName:"((NULL))")
			        );
					
					Message("Feature List");ReportIndent();
					
					foreach(Managed.Feature f in _featureList)
						Message(
							(f!=null)?f.GetType().FullName : "((NULL))"
							);
					
					ReportUnindent();
				}
			}
			
			class uPipeline : MusiCObject
			{
				Unmanaged.Window _window;
				Unmanaged.Classifier _classifier;
				LinkedList<Unmanaged.Feature> _featureList = new LinkedList<Unmanaged.Feature>();
				
				public void AddClassifier(Unmanaged.Classifier classifier)
				{
					_classifier = classifier;
				}
				
				public void AddWindow(Unmanaged.Window window)
				{
					_window = window;
				}
				
				public void AddFeature(Unmanaged.Feature feature)
				{
					_featureList.AddLast(feature);
				}
				
				public bool Check()
				{
					if(_window == null)
						return false;
					
					if(_featureList.Count == 0)
						return false;
					
					return true;
				}
			
				unsafe public void Execute()
				{
					foreach(TrainLabel label in Global<ExtensionCache>.GetInstance().GetConfig().LabelList)
					{
						Message("Processing "+label.InputDir);
						foreach(String file in Directory.GetFiles(label.InputDir,"*.wav"))
						{
							Message("Opening "+ file);
							Unmanaged.Handler h = Global<ExtensionCache>.GetInstance().GetUnmanagedHandler(file);
							
							if(h == null)
							{
								//Warning(file+": No handler supports this file");
								continue;
							}
							
							h.Attach(file);
							_window.Attach(h);
							
							foreach (Unmanaged.Feature f in _featureList)
								Unmanaged.Extractor.Extract(_window, f);
							
							h.Detach();
						}
					}
				}
				
				public void Say()
				{
					Message("Window: " + 
			        ((_window!=null)?_window.GetType().FullName:"((NULL))")
			        );
				
					Message("Classifier: " + 
			        ((_classifier!=null)?_classifier.GetType().FullName:"((NULL))")
			        );
					
					Message("Feature List");ReportIndent();
					
					foreach(Unmanaged.Feature f in _featureList)
						Message(
							(f!=null)?f.GetType().FullName : "((NULL))"
							);
					
					ReportUnindent();
				}
			}
			
			public bool Add(Extension ext, ExtensionInfo info)
			{
				switch(info.Manager)
				{
					case ExtensionManagement.Managed:
						switch(info.Kind)
						{
							case ExtensionKind.Window:
								_mPipe.AddWindow(ext as Managed.Window);
								break;
							
							case ExtensionKind.Classifier:
								_mPipe.AddClassifier(ext as Managed.Classifier);
								break;
								
							case ExtensionKind.Feature:
								_mPipe.AddFeature(ext as Managed.Feature);
								break;
							
							default:
								return false;
					
					}
					break;
			
					case ExtensionManagement.Unmanaged:
						switch(info.Kind)
						{
							case ExtensionKind.Window:
								_uPipe.AddWindow(ext as Unmanaged.Window);
								break;
							
							case ExtensionKind.Classifier:
								_uPipe.AddClassifier(ext as Unmanaged.Classifier);
								break;
								
							case ExtensionKind.Feature:
								_uPipe.AddFeature(ext as Unmanaged.Feature);
								break;
								
							default:
								return false;
						}
						
						break;
						
					default:
						return false;
				}
				
				return true;
			}
			
			public ExtensionManagement Check()
			{
				bool mStatus = _mPipe.Check();
				bool uStatus = _uPipe.Check();
				
				if(uStatus && mStatus)
				{
					_status = ExtensionManagement.Error;
					return ExtensionManagement.Error;
				}
				
				if(!(uStatus || mStatus))
				{
					_status = ExtensionManagement.Error;
					return ExtensionManagement.NotSet;
				}
				
				if(uStatus)
				{
					_status = ExtensionManagement.Unmanaged;
					return ExtensionManagement.Unmanaged;
				}
				
				if(mStatus)
				{
					_status = ExtensionManagement.Managed;
					return ExtensionManagement.Managed;
				}
				
				// this shouldn't be reached.
				// conforming to compiler: not all code paths return a value (CS0161)
				return ExtensionManagement.NotSet;
			}
			
			public void Execute()
			{
				if(_status == ExtensionManagement.Managed)
				{
					_mPipe.Execute();
					return;
				}
				
				if(_status == ExtensionManagement.Unmanaged)
				{
					_uPipe.Execute();
					return;
				}
				
				Error("No pipeline can be executed.");
				Say();
			}
			
			public void Say()
			{
				Message("Managed Pipeline"); ReportIndent();
				_mPipe.Say();
				ReportUnindent();
				
				Message("Unmanaged Pipeline"); ReportIndent();
				_uPipe.Say();
				ReportUnindent();
			}
		}
		
		/// <summary>
		/// Add an extension to the algorithm.
		/// </summary>
		/// <description>
		/// It adds a feature, window or classifier to the algorithm. It supports only one window and one classifier(which is not required).
		/// Several features may be added.
		/// </description>
		/// <exception cref="MusiC.Extensions.MissingExtensionException">When the exception cache cant find the class we are adding.</exception>
		/// <param name="extensionClass">The name of the class that should be added</param>
		/// <param name="args">The arguments to the constructor call. It must be in the correct order.</param>
		/// <returns>Success</returns>
		public bool Add(String extensionClass, ParamList args)
		{
			ExtensionInfo info = Global<ExtensionCache>.GetInstance().GetInfo(extensionClass);
			
			if(info == null)
				throw new Exceptions.MissingExtensionException(extensionClass + " wasn't found.");
			
			if(info.Kind == ExtensionKind.Error)
			{
				Error(extensionClass+": Can't recognize this Extension. This may happen when an extension inherits directly from MusiC.Extension");
				return false;
			}
			
			if(info.Manager == ExtensionManagement.Error)
			{
				Error(extensionClass+": Can't recognize if this is a Managed or Unmanaged implementation. Classifiers/Features/Windows must inherit from their subclasses.");
				return false;
			}
			
			Extension ext = info.Instantiate(args);
			return _pipe.Add(ext, info);
		}
		
		public void Execute()
		{
			ExtensionManagement status = _pipe.Check();

			if(
				status == ExtensionManagement.NotSet ||
				status == ExtensionManagement.Error
			)
			{
				Error("An error has ocurred. Status = "+status.ToString());
				Say();
			}
			
			_pipe.Execute();
		}
		
		public void Say()
		{
			_pipe.Say();
		}
	}
}