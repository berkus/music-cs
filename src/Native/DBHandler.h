#include <fstream>
#include <iostream>
#include <sstream>

#include <string.h>

//#define DEBUG

#if defined(DEBUG)

#define LOG(msg) cout << tabulate(t) <<  __FUNCTION__ << " at " << __LINE__ << " - " << msg << endl
#else
#define LOG(m) //m
#endif

#if defined(TRACE)
	#define LOG_IN() t++;cout << tabulate(t) <<  __FUNCTION__ << endl
	#define LOG_OUT() cout << tabulate(t) <<  __FUNCTION__ << endl;t--
#else
	#define LOG_IN() //
	#define LOG_OUT() //
#endif

namespace MusiC
{
	namespace Native
	{
		class DBHandler
		{
        private:

			std::fstream db;

			char * _sectionLabel;
			char * _dataLabel;
			int _sectionLabelSz, _dataLabelSz, _dataSectionSz;

		public:

            //::::::::::::::::::::::::::::::::::::::://

			DBHandler();

			~DBHandler();
			
			//::::::::::::::::::::::::::::::::::::::://

			void OpenDB(const char * dbName);

			void CloseDB();

			bool HasData(char * sectionLabel, char * dtLabel);
			
			int ReadData(double * dt);

			void AddData(const char * sectionLabel, const char * dataLabel, double * data, int dataSectionSz);
			
			//::::::::::::::::::::::::::::::::::::::://
			
        private:

			bool IsDBOpen();
			bool IsEndOfFile();
			bool LoadNext();
			void PrintHeader();
			void JumpDataSection();
			void ReadHeader();
			void WriteHeader(const char * sectionLabel, const char * dataLabel, int dataSectionSz);
			void WriteData(double * data, int dataSectionSz);
			char * GetSection();
			char * GetDataLabel();
			void Reset();
			void Set();
		};
	}
}