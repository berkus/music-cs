// Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#if !defined(_MUSIC_NATIVE_DBHANDLER_H_)
#define _MUSIC_NATIVE_DBHANDLER_H_

#include <fstream>
#include <iostream>
#include <sstream>

#include "LogHandler.h"

namespace MusiC
{
	namespace Native
	{
		class DBHandler
		{

        private:

			std::fstream db;
			LogHandler log;

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

#endif