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

#ifndef __classentry__
#define __classentry__

#include <iostream>
#include <list>

#include "music_object.h"
#include "music_configurator.h"
#include "music_extension.h"
#include "music_constructor_entry.h"

namespace music
{
	namespace base 
	{
		class music_class_entry : public music_object
		{
			union ptr_holder
			{
				music_configurator * cfg;
				music_extension * ext;
			};
			
		private:
			
			music_class_metadata * _metadata;
			ptr_holder ptr;
			
			std::list< music_constructor_entry * > _ctor_list;
			
		public:
			
			music_class_entry( music_class_metadata * data );
			~music_class_entry();
			
			inline music_extension::type get_type() { return _metadata->kind; }
			
			void add_constructor( music_ctor_metadata * data );
			bool can_handle( std::wstring &file );
			
			template< typename T >
			T get_extension_as(){ return dynamic_cast< T >( ptr.ext ); }
		};
	}
}

#endif
