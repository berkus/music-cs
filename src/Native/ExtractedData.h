#if !defined(_MUSIC_NATIVE_EXTRACTEDDATA_H_)
#define _MUSIC_NATIVE_EXTRACTEDDATA_H_

#if defined(_MSC_VER)
	typedef __int64 Int64;
#else
	typedef long long Int64;
#endif

namespace MusiC
{
	namespace Native
	{
		struct FrameData
		{
			float * pData;

			FrameData * pNextFrame;
			FrameData * pPrevFrame;
		};

		struct ClassData;

		struct FileData
		{
			Int64 next;
			Int64 nFrames;

			FileData * pNextFile;
			FileData * pPrevFile;

			FrameData * pFirstFrame;
			FrameData * pLastFrame;

			ClassData * pClass;
		};

		struct DataCollection;

		struct ClassData
		{
			Int64 nFiles;
			Int64 nFrames;

			ClassData * pNextClass;

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

