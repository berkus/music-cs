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
using System.Text;

using MusiC.Extensions;

namespace MusiC
{
	public class Algorithm : MusiCObject
	{
		Window _window;
		Classifier _classifier;
		LinkedList<Feature> _featureList = new LinkedList<Feature>();
		
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
			if(info.Kind == ExtensionKind.NotSet)
			{
				Warning(extensionClass+": Cant recognize this Extension. This happens when an extension inherits directly from MusiC.Extension");
			}
			
			Extension ext;
			
			switch(info.Kind)
			{
				case ExtensionKind.Classifier:
					ext = info.Instantiate(args);
					
					if(typeof(Classifier).IsAssignableFrom(ext.GetType()))
						_classifier = ext as Classifier;
					else
						Warning("The classifier "+extensionClass+" doesnt inherit MusiC.Classifier.");
					break;
					
				case ExtensionKind.Window:
					ext = info.Instantiate(args);
					
					if(
						typeof(Window.ManagedImplementation).IsAssignableFrom(ext.GetType()) ||
						typeof(Window.UnmanagedImplementation).IsAssignableFrom(ext.GetType())
					  )
						_window = ext as Window;
					else
						Warning("The window "+extensionClass+" doesnt inherit MusiC.Window.ManagedImpl or MusiC.Window.UnmanagedImpl.");
					break;
					
				case ExtensionKind.Feature:
					ext = info.Instantiate(args);
					
					if(
						typeof(Feature.ManagedImplementation).IsAssignableFrom(ext.GetType()) ||
						typeof(Feature.UnmanagedImplementation).IsAssignableFrom(ext.GetType())
					  )
						_featureList.AddLast(ext as Feature);
					else
						Warning("The feature "+extensionClass+" doesnt inherit MusiC.Feature.");
					break;
				default:
					return false;
			}
			
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