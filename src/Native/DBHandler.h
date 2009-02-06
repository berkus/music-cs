#include <fstream>
#include <iostream>
#include <sstream>

#include <string.h>

//#define DEBUG

#if defined(DEBUG)

#define LOG(msg) cout << __FUNCTION__ << " at " << __LINE__ << " - " << msg << endl
#else
#define LOG(m) //m
#endif

#if defined(TRACE)
	#define LOG_IN() t++;cout << __FUNCTION__ << endl
	#define LOG_OUT() cout << __FUNCTION__ << endl;t--
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
			std::ofstream log;

			char * _sectionLabel;
			char * _dataLabel;
			int _sectionLabelSz, _dataLabelSz, _dataSectionSz;

			//::::::::::::::::::::::::::::::::::::::://

		public:

			DBHandler();

			~DBHandler();

			//::::::::::::::::::::::::::::::::::::::://

        public:

            void AddData(const char * sectionLabel, const char * dataLabel, float * data, int dataSectionSz);
            void CloseDB();
            bool HasData(const char * sectionLabel, const char * dtLabel);
			void OpenDB(const char * dbName);
			int ReadData(float * dt);

			//::::::::::::::::::::::::::::::::::::::://

        private:

            char * GetSection();
			char * GetDataLabel();
			bool IsDBOpen() { return db.is_open(); }
			bool IsEndOfFile() { return (db.peek() == EOF); }
			void JumpDataSection();
			bool LoadNext();
			void PrintHeader();
			void ReadHeader();
			void Reset();
			void Set();
			void WriteHeader(const char * sectionLabel, const char * dataLabel, int dataSectionSz);
			void WriteData(float * data, int dataSectionSz);
		};
	}
}
