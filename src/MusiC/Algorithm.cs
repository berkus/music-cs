/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos Jos� Sant'Anna Magalh�es
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
 */

using System;
using System.Collections.Generic;
using System.IO;

using MusiC.Extensions;

namespace MusiC
{
	/// <summary>
	/// This class describe which actions the library must accomplish. 
	/// 
	/// Algorithm Blocks:
	/// Windows - Only one
	/// Features - One or more
	/// Classifiers - It is not necessary.
	/// 
	/// </summary>
	/// 
	/// <see cref="MusiC.Window"/>
	/// <see cref="MusiC.Feature"/>
	/// <see cref="MusiC.Classifier"/>
	/// 	
	/// @todo Give algorithms a name.
	internal class Algorithm : MusiCObject, IAlgorithm
	{
		private Pipeline _pipe = new Pipeline();
		
		/// <summary>
		/// Add an extension to the algorithm.
		///
		/// It adds a feature, window or classifier to the algorithm. It supports only one window and one classifier(which is not required).
		/// Several features may be added.
		/// </summary>
		/// <exception cref="MusiC.Extensions.MissingExtensionException">When the exception cache can't find the class we are adding.</exception>
		/// <param name="extensionClass">The name of the class that should be added</param>
		/// <param name="args">The arguments to the constructor call. It must be in the correct order.</param>
		/// <returns>Success</returns>
		public bool Add(string extensionClass, IParamList args)
		{
			Message("Appended: " + extensionClass );
			
			ExtensionInfo info = ExtensionCache.GetInfo(extensionClass);
			
			if (info == null) throw new Exceptions.MissingExtensionException(extensionClass + " wasn't found.");
			
			if (info.Kind == ExtensionKind.Error)
			{
				Error(extensionClass + ": Can't recognize this Extension. This may happen when an extension inherits directly from MusiC.Extension");
				return false;
			}
			
			if (info.Model == MemoryModel.Error)
			{
				Error(extensionClass + ": Can't recognize if this is a Managed or Unmanaged implementation. Classifiers/Features/Windows must inherit from their subclasses.");
				return false;
			}
			
			ParamList pList = args as ParamList;
			
			if (pList == null)
				return false;
			
			Extension ext = info.Instantiate(pList);
			return _pipe.Add(ext, info);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conf">
		/// A <see cref="Config"/>
		/// </param>
		public void Execute(Config conf)
		{
			MemoryModel status = _pipe.Check();
			
			if (status == MemoryModel.NotSet || status == MemoryModel.Error)
			{
				Error("An error has ocurred. Status = " + status.ToString());
				Say();
			}
			
			_pipe.Execute(conf);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		public void Say()
		{
			_pipe.Say();
		}
	}
	
	//---------------------------------------//
	
	#region Pipeline Classes
	
	/// <summary>
	/// 
	/// </summary>
	internal class Pipeline : MusiCObject
	{
		private mPipeline _mPipe = new mPipeline();
		private uPipeline _uPipe = new uPipeline();
		
		private MemoryModel _status = MemoryModel.NotSet;
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ext">
		/// A <see cref="Extension"/>
		/// </param>
		/// <param name="info">
		/// A <see cref="ExtensionInfo"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool Add(Extension ext, ExtensionInfo info)
		{
			switch (info.Model)
			{
				case MemoryModel.Managed:
					switch (info.Kind)
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
				
				case MemoryModel.Unmanaged:
					switch (info.Kind)
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
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="ExtensionManagement"/>
		/// </returns>
		public MemoryModel Check()
		{
			bool mStatus = _mPipe.Check();
			bool uStatus = _uPipe.Check();
			
			if (uStatus && mStatus)
			{
				_status = MemoryModel.Error;
				return MemoryModel.Error;
			}
			
			if (!(uStatus || mStatus))
			{
				_status = MemoryModel.Error;
				return MemoryModel.NotSet;
			}
			
			if (uStatus)
			{
				_status = MemoryModel.Unmanaged;
				return MemoryModel.Unmanaged;
			}
			
			if (mStatus)
			{
				_status = MemoryModel.Managed;
				return MemoryModel.Managed;
			}
			
			// this shouldn't be reached.
			// conforming to compiler: not all code paths return a value (CS0161)
			return MemoryModel.NotSet;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conf">
		/// A <see cref="Config"/>
		/// </param>
		public void Execute(Config conf)
		{
			if (_status == MemoryModel.Managed)
			{
				_mPipe.Execute(conf);
				return;
			}
			
			if (_status == MemoryModel.Unmanaged)
			{
				_uPipe.Execute(conf);
				return;
			}
			
			Error("No pipeline can be executed.");
			Say();
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		public void Say()
		{
			BeginReportSection("Managed Pipeline");
			_mPipe.Say();
			EndReportSection(false);
			
			BeginReportSection("Unmanaged Pipeline");
			_uPipe.Say();
			EndReportSection(true);
		}
	}
	
	//---------------------------------------//
	
	/// <summary>
	/// 
	/// </summary>
	internal class mPipeline : MusiCObject
	{
		private Managed.Window _window;
		private Managed.Classifier _classifier;
		private LinkedList<Managed.Feature> _featureList = new LinkedList<Managed.Feature>();
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="classifier">
		/// A <see cref="Managed.Classifier"/>
		/// </param>
		public void AddClassifier(Managed.Classifier classifier)
		{
			_classifier = classifier;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		public void AddWindow(Managed.Window window)
		{
			_window = window;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="feature">
		/// A <see cref="Managed.Feature"/>
		/// </param>
		public void AddFeature(Managed.Feature feature)
		{
			_featureList.AddLast(feature);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool Check()
		{
			if (_window == null) return false;
			
			if (_featureList.Count == 0) return false;
			
			return true;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conf">
		/// A <see cref="Config"/>
		/// </param>
		public void Execute(Config conf)
		{
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		public void Say()
		{
			Message("Window: " + ((_window != null) ? _window.GetType().FullName : "((NULL))"));
			Message("Classifier: " + ((_classifier != null) ? _classifier.GetType().FullName : "((NULL))"));
			
			BeginReportSection("Feature List");
			
			foreach (Managed.Feature f in _featureList)
				Message((f != null) ? f.GetType().FullName : "((NULL))");
			
			EndReportSection(true);
		}
	}
	
	//---------------------------------------//
	
	/// <summary>
	/// 
	/// </summary>
	internal class uPipeline : MusiCObject
	{
		private Unmanaged.Window _window;
		private Unmanaged.Classifier _classifier;
		private LinkedList<Unmanaged.Feature> _featureList = new LinkedList<Unmanaged.Feature>();
		
		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Data.Unmanaged.FileData"/>
		/// </returns>
		unsafe
		private Data.Unmanaged.FileData * Extract(string file)
		{
			BeginReportSection(file);
					
			Unmanaged.Handler h = ExtensionCache.GetUnmanagedHandler(file);
			if( h == null )
			{
				Warning(file + ": No handler supports this file");
				return null;
			}
			
			h.Attach(file);
			_window.Attach(h);
			
			Data.Unmanaged.FileData * currentFile = Data.Unmanaged.DataHandler.BuildFileData( );
			Unmanaged.Extractor.Extract( _window, _featureList, currentFile );
			
			h.Detach();

			EndReportSection(true);

			return currentFile;
		}

		//::::::::::::::::::::::::::::::::::::::://

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tLabel">
		/// A <see cref="IEnumerable"/>
		/// </param>
		/// <returns>
		/// A <see cref="Data.Unmanaged.DataCollection"/>
		/// </returns>
		unsafe
		private Data.Unmanaged.DataCollection * Extract(IEnumerable<Label> tLabel)
		{
			Data.Unmanaged.DataCollection* dtCol = Data.Unmanaged.DataHandler.BuildCollection( (uint) _featureList.Count);
			
			foreach (Label label in tLabel)
			{
				Data.Unmanaged.ClassData* currentClass = Data.Unmanaged.DataHandler.BuildClassData(dtCol);	
				BeginReportSection("Processing Label: " + label.Name);
				
				//foreach (string file in Directory.GetFiles(label.InputDir, "*.wav"))
				foreach (string file in label)
				{
					Data.Unmanaged.FileData * currentFile = Extract(file);
					Data.Unmanaged.DataHandler.AddFileData(currentFile, currentClass);
				}

				EndReportSection(true);
			}

			return dtCol;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="classifier">
		/// A <see cref="Unmanaged.Classifier"/>
		/// </param>
		public void AddClassifier(Unmanaged.Classifier classifier)
		{
			_classifier = classifier;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="window">
		/// A <see cref="Unmanaged.Window"/>
		/// </param>
		public void AddWindow(Unmanaged.Window window)
		{
			_window = window;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="feature">
		/// A <see cref="Unmanaged.Feature"/>
		/// </param>
		public void AddFeature(Unmanaged.Feature feature)
		{
			_featureList.AddLast(feature);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool Check()
		{
			if (_window == null) return false;
			
			if (_featureList.Count == 0) return false;
			
			return true;
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conf">
		/// A <see cref="Config"/>
		/// </param>
		/// @todo Create a handler cache and pass it here.
		unsafe
		public void Execute(Config conf)
		{
			IEnumerable<Label> tLabel = conf.LabelList;
			
			Message("Extracting . . .");
			Data.Unmanaged.DataCollection * dtCol = Extract(tLabel);
			
            // Report what we have after extractor
			Summarize(dtCol);

            if (_classifier != null)
            {
                Message("Beginning Training");

                Message("Filtering . . .");
                Data.Unmanaged.DataCollection* filteredData = _classifier.ExtractionFilter(dtCol);

                // Training data returned by the classifier
                void * tData;

                Message("Training . . .");

                if (filteredData == null)
                    tData = _classifier.Train(dtCol);
                else
                    tData = _classifier.Train(filteredData);

                Message("Begining Classification . . .");

                foreach (string file in conf.Classify)
                {
                    Message("Classifying: " + file);

                    Data.Unmanaged.FileData * f = Extract(file);
                    Data.Unmanaged.FileData * filteredFile = _classifier.ClassificationFilter(f, dtCol->nFeatures);

                    int result;

                    if( filteredFile == null )
                        result = _classifier.Classify(f, tData);
                    else
                        result = _classifier.Classify(filteredFile, tData);

                    Message("RESULT: " + result );
                }

                Message("Freeing Extracted Data");
                Data.Unmanaged.DataHandler.DestroyCollection(dtCol);

                Message("Freeing Filtered Data");
                if (filteredData != null)
                    Data.Unmanaged.DataHandler.DestroyCollection(filteredData);
            }
            else
            {
                Message("Freeing Extracted Data");
                Data.Unmanaged.DataHandler.DestroyCollection(dtCol);
            }

			Message( "All Tasks Done" );
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtCol">
		/// A <see cref="Data.Unmanaged.DataCollection"/>
		/// </param>
		unsafe
		public void Summarize(Data.Unmanaged.DataCollection* dtCol)
		{
			BeginReportSection("Extraction Report");
			Message("Classes Found: " + dtCol->nClasses);
			Message("Features Found: " + dtCol->nFeatures);
			
			Data.Unmanaged.ClassData* c = dtCol->pFirstClass;
			
			int i = 1;
			while (c != null)
			{
				Message("Class " + i + " has " + c->nFiles + " files with " + c->nFrames + " frames.");
				c = c->pNextClass;
				i++;
			}
			
			EndReportSection(true);
		}
		
		//::::::::::::::::::::::::::::::::::::::://
		
		/// <summary>
		/// 
		/// </summary>
		public void Say()
		{
			Message("Window: " + ((_window != null) ? _window.GetType().FullName : "((NULL))"));
			Message("Classifier: " + ((_classifier != null) ? _classifier.GetType().FullName : "((NULL))"));
			
			BeginReportSection("Feature List");
			
			foreach (Unmanaged.Feature f in _featureList)
				Message((f != null) ? f.GetType().FullName : "((NULL))");
			
			EndReportSection(true);
		}
	}
	#endregion
}
