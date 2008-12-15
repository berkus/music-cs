using System;
using System.Collections.Generic;
using System.Text;

using MusiC.Extensions;

namespace MusiC
{
	public class Algorithm : MusiCObject
	{
		Window _window = null;
		Classifier _classifier = null;
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
			
			Extension ext;
			
			switch(info.Type)
			{
				case ExtensionType.Classifier:
					ext = info.Instantiate(args);
					
					if(typeof(Classifier).IsAssignableFrom(ext.GetType()))
						_classifier = ext as Classifier;
					else
						Warning("The classifier "+extensionClass+" doesnt inherit MusiC.Classifier.");
					break;
					
				case ExtensionType.Window:
					ext = info.Instantiate(args);
					
					if(typeof(Window).IsAssignableFrom(ext.GetType()))
						_window = ext as Window;
					else
						Warning("The window "+extensionClass+" doesnt inherit MusiC.Classifier.");
					break;
					
				case ExtensionType.Feature:
					ext = info.Instantiate(args);
					
					if(typeof(Feature).IsAssignableFrom(ext.GetType()))
						_featureList.AddLast(ext as Feature);
					else
						Warning("The feature "+extensionClass+" doesnt inherit MusiC.Classifier.");
					break;
				default:
					return false;
			}
			
			return true;
		}
	}
}