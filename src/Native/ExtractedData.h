#if !defined(_MUSIC_NATIVE_EXTRACTEDDATA_H_)
#define _MUSIC_NATIVE_EXTRACTEDDATA_H_

typedef __int64 Int64;

namespace MusiC
{
	namespace Native
	{
		struct FrameData
		{
			float * pData;
			
			FrameData * next;
		};
		
		struct ClassData;
		
		struct FileData
		{
			Int64 next;
			Int64 nFrames;
			
			FileData * pNext;
			FrameData * pFirstFrame;
			FrameData * pLastFrame;
			ClassData * pClass;
		};
		
		struct DataCollection;
		
		struct ClassData
		{
			Int64 nFiles;
			Int64 nFrames;
			
			ClassData * pNext;
			FileData * pFirstFile;
			FileData * pLastFile;
			DataCollection * pCollection;
		};
		
		struct DataCollection
		{
			int nClasses;
			int nFeatures;
			
			ClassData * pFirstClass;
			ClassData * pLastClass;
		};
	}
}

#endif

