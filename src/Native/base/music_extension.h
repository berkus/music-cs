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

#ifndef __music_extension__
#define __music_extension__

#include "music_object.h"

namespace music
{
	namespace base
	{
		class music_extension : public music_object
		{
		public:
			
			enum type
			{
				NOT_SET,
				
				CONFIGURATOR, // Extension
				
				CLASSIFIER, // Execution
				FEATURE,
				WINDOW,
				
				IN_CHANNEL_ADAPTER, // Input
				IN_MEDIA_HANDLER,
				
				OUT_MEDIA_HANDLER, // Output
				OUT_CHANNEL_ADAPTER,
				
				REPORTER // Report
			};
			
			music_extension();
			virtual ~music_extension();
			
			virtual type get_type() = 0;
			
			virtual void dummy() {};
		};
		
		struct music_ctor_metadata
		{
			wchar_t ** type;
		};
		
		struct music_class_metadata
		{
			music_extension::type kind;
			wchar_t * classname;
			music_extension * (*ctor)();
			void (*dtor)(music_extension *);
			music_ctor_metadata * ctorInfo;
		};
	}
}

#endif // __extension__
