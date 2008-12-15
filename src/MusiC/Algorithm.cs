using System;
using System.Collections.Generic;
using System.Text;

using MusiC.Extensions;

namespace MusiC
{
	public class Algorithm
	{
		Window _window = null;
		Classifier _classifier = null;
		LinkedList<Feature> _featureList = new LinkedList<Feature>();
		
		public void Add(String extensionClass, ParamList args)
		{
			ExtensionInfo info = Global<ExtensionCache>.GetInstance().GetInfo(extensionClass);
			switch(info.Type)
			{
				case ExtensionType.Classifier:
					_classifier = info.Instantiate(args) as Classifier;
					break;
				case ExtensionType.Window:
					_window = info.Instantiate(args) as Window;
					break;
				case ExtensionType.Feature:
					_featureList.AddLast(info.Instantiate(args) as Feature);
					break;
				default:
					break;
			}
		}
	}
}