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
	public class Algorithm : MusiCObject
	{
		Window _window;
		Classifier _classifier;
		LinkedList<Feature> _featureList = new LinkedList<Feature>();
		ExtensionManagement _managementStatus = ExtensionManagement.NotSet;
		
		/// <summary>
		/// Add a extension to the algorithm.
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
			
			/// @todo Throw a UnrecognizedExtensionException.
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
			
			if(
				info.Kind == ExtensionKind.Classifier ||
				info.Kind == ExtensionKind.Feature ||
				info.Kind == ExtensionKind.Window
			  )
			{
				if(_managementStatus == ExtensionManagement.NotSet)
					_managementStatus = info.Manager;
				
				if(info.Manager != _managementStatus)
				{
					Error("Cant mix managed and unmanaged classifiers/Features/Windows.");
					return false;
				}
			}
			
			Extension ext = info.Instantiate(args);
			
			switch(info.Kind)
			{
				case ExtensionKind.Classifier:
					_classifier = ext as Classifier;
					
//					if(typeof(Classifier).IsAssignableFrom(ext.GetType()))
//						_classifier = ext as Classifier;
//					else
//						Error("The classifier "+extensionClass+" doesn't inherit MusiC.Classifier.");
					
					break;
					
				case ExtensionKind.Window:
					_window = ext as Window;
					
//					if(
//						typeof(Window.ManagedImplementation).IsAssignableFrom(ext.GetType()) ||
//						typeof(Window.UnmanagedImplementation).IsAssignableFrom(ext.GetType())
//					  )
//						_window = ext as Window;
//					else
//						Error("The window "+extensionClass+" doesn't inherit MusiC.Window.ManagedImpl nor MusiC.Window.UnmanagedImpl.");
					
					break;
					
				case ExtensionKind.Feature:
					_featureList.AddLast(ext as Feature);
					
//					if(
//						typeof(Feature.ManagedImplementation).IsAssignableFrom(ext.GetType()) ||
//						typeof(Feature.UnmanagedImplementation).IsAssignableFrom(ext.GetType())
//					  )
//						_featureList.AddLast(ext as Feature);
//					else
//						Error("The feature "+extensionClass+" doesn't inherit MusiC.Feature.ManagedImplementation nor MusiC.Feature.UnmanagedImplementation.");
					
					break;
					
				default:
					return false;
			}
			
			return true;
		}
		
		public void Execute()
		{
			foreach(TrainLabel label in Global<ExtensionCache>.GetInstance().GetConfig().LabelList)
			{
				Message("Processing "+label.InputDir);
				foreach(String file in Directory.GetFiles(label.InputDir,"*.wav"))
				{
					Message("Opening "+ file);
					Handler h = Global<ExtensionCache>.GetInstance().GetHandler(file);
					
					if(h == null)
						Warning(file+": No handler supports this file");
				}
			}
		}
		
		public void Say()
		{
			Message("Window: " + 
			        ((_window!=null)?_window.GetType().FullName:"((NULL))")
			        );
			
			Message("Classifier: " +
			        ((_classifier!=null)?_classifier.GetType().FullName : "((NULL))")
					);
			
			Message("Feature List");ReportIndent();
			
			foreach(Feature f in _featureList)
				Message(
					(f!=null)?f.GetType().FullName : "((NULL))"
					);
			
			ReportUnindent();
		}
	}
}