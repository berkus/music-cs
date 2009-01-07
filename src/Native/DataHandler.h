#if !defined(_MUSIC_NATIVE_DATAHANDLER_H_)
#define _MUSIC_NATIVE_DATAHANDLER_H_

#include <cstdlib>

#include "ExtractedData.h"

namespace MusiC
{
	namespace Native
	{
		class DataHandler
		{
			DataCollection * _data;
			ClassData * _curClass;

		public:
			DataHandler();

			void Attach(DataCollection *);

			int getNumClasses(){ return _data->nClasses; }
			int getNumFeatures() { return _data->nFeatures; }

			ClassData * getClass(int idx);
			ClassData * getNextClass();
		};
	}
}

#endif

