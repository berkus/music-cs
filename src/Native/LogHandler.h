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

#if !defined(_MUSIC_NATIVE_LOGHANDLER_H_)
#define _MUSIC_NATIVE_LOGHANDLER_H_

#include <string>
#include <fstream>
#include <sstream>
#include <iostream>

#if defined(_DEBUG)

#define TRACE

#if defined(TRACE)

#define LOG(msg) log << msg << std::endl;
#define LOG_IN() //log.BeginSection(__FUNCTION__)
#define LOG_OUT() //log.EndSection()

#else

#define LOG(msg) log << msg << std::endl
#define LOG_IN() //
#define LOG_OUT() //

#endif

#else

#define LOG(m) //m
#define LOG_IN() //
#define LOG_OUT() //

#endif

namespace MusiC
{
	namespace Native
	{
		class LogHandler : public std::ofstream
		{
			int t;
			std::string s;

			std::string Tabulate();

		public:

			LogHandler() : t(0), s("")
			{}

			LogHandler(std::string file) : t(0), s("")
			{open(file.c_str());}

			~LogHandler()
			{close();}

			friend std::ostream& __cdecl std::endl (std::ostream& _Ostr);
			friend std::ostream& std::operator<< (std::ostream& _Ostr, char ch);

			std::ostream& operator<<(std::string &str)
			{
				std::ostringstream oss; oss << s << str;
				(*this) << oss.str();
				return (*this);
			}

			std::ostream& operator<<(const char * str)
			{
				std::ostringstream oss; oss << s << str;
				(*this) << oss.str();
				return (*this);
			}

			void BeginSection(std::string name);
			void EndSection();
		};
	}
}

#endif