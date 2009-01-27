/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
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
	class Algorithm : MusiCObject, IAlgorithm
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
					if (_window == null) return false; 

					if (_featureList.Count == 0) return false; 

					return true;
				}

				public void Execute(ExtensionCache cache)
				{
				}

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
					if (_window == null) return false; 

					if (_featureList.Count == 0) return false; 

					return true;
				}

				/// <summary>
				/// 
				/// </summary>
				/// <param name="cache"></param>
				/// @todo Create a handler cache and pass it here.
				unsafe public void Execute(ExtensionCache cache)
				{
					IEnumerable<ILabel> tLabel = cache.GetConfigurator().LabelList;
					Data.Unmanaged.DataCollection* dtCol = Data.Unmanaged.DataHandler.BuildCollection(_featureList.Count);

					foreach (TrainLabel label in tLabel) {
						Data.Unmanaged.ClassData* currentClass = Data.Unmanaged.DataHandler.BuildClassData(dtCol);

						Message("Processing " + label.InputDir);

						foreach (string file in Directory.GetFiles(label.InputDir, "*.wav")) {
							Message("Opening " + file);

							Data.Unmanaged.FileData* currentFile = Data.Unmanaged.DataHandler.BuildFileData(currentClass);
							Unmanaged.Handler h = cache.GetUnmanagedHandler(file);

							if (h == null) {
								Warning(file + ": No handler supports this file");
								continue;
							}

							h.Attach(file);
							_window.Attach(h);

							Unmanaged.Extractor.Extract(_window, _featureList, currentFile);

							h.Detach();
						}
					}

					Message("Extraction Done.");

					Summarize(dtCol);

					if (_classifier != null) {
						Message("Beginning Classification");
						Data.Unmanaged.DataCollection* filteredData = _classifier.Filter(dtCol);

						if (filteredData == null) _classifier.Train(dtCol); 						else _classifier.Train(filteredData); 
					}

					Message("All Tasks Done");
				}

				unsafe public void Summarize(Data.Unmanaged.DataCollection* dtCol)
				{
					BeginReportSection("Extraction Report");
					Message("Classes Found: " + dtCol->nClasses);
					Message("Features Found: " + dtCol->nFeatures);

					Data.Unmanaged.ClassData* c = dtCol->pFirstClass;

					int i = 1;
					while (c != null) {
						Message("Class " + i + " has " + c->nFiles + " files with " + c->nFrames + " frames.");
						c = c->pNextClass;
						i++;
					}

					EndReportSection(true);
				}

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

			public bool Add(Extension ext, ExtensionInfo info)
			{
				switch (info.Manager) {
					case ExtensionManagement.Managed:
						switch (info.Kind) {
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
						switch (info.Kind) {
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

				if (uStatus && mStatus) {
					_status = ExtensionManagement.Error;
					return ExtensionManagement.Error;
				}

				if (!(uStatus || mStatus)) {
					_status = ExtensionManagement.Error;
					return ExtensionManagement.NotSet;
				}

				if (uStatus) {
					_status = ExtensionManagement.Unmanaged;
					return ExtensionManagement.Unmanaged;
				}

				if (mStatus) {
					_status = ExtensionManagement.Managed;
					return ExtensionManagement.Managed;
				}

				// this shouldn't be reached.
				// conforming to compiler: not all code paths return a value (CS0161)
				return ExtensionManagement.NotSet;
			}

			public void Execute(ExtensionCache cache)
			{
				if (_status == ExtensionManagement.Managed) {
					_mPipe.Execute(cache);
					return;
				}

				if (_status == ExtensionManagement.Unmanaged) {
					_uPipe.Execute(cache);
					return;
				}

				Error("No pipeline can be executed.");
				Say();
			}

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
		public bool Add(string extensionClass, IParamList args)
		{
			ExtensionInfo info = ExtensionCache.GetInfo(extensionClass);

			if (info == null) throw new Exceptions.MissingExtensionException(extensionClass + " wasn't found."); 

			if (info.Kind == ExtensionKind.Error) {
				Error(extensionClass + ": Can't recognize this Extension. This may happen when an extension inherits directly from MusiC.Extension");
				return false;
			}

			if (info.Manager == ExtensionManagement.Error) {
				Error(extensionClass + ": Can't recognize if this is a Managed or Unmanaged implementation. Classifiers/Features/Windows must inherit from their subclasses.");
				return false;
			}
			
			ParamList pList = args as ParamList;
			
			if(pList == null)
				return false;

			Extension ext = info.Instantiate(pList);
			return _pipe.Add(ext, info);
		}

		public void Execute(ExtensionCache cache)
		{
			ExtensionManagement status = _pipe.Check();

			if (status == ExtensionManagement.NotSet || status == ExtensionManagement.Error) {
				Error("An error has ocurred. Status = " + status.ToString());
				Say();
			}

			_pipe.Execute(cache);
		}

		public void Say()
		{
			_pipe.Say();
		}
	}
}
