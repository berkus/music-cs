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

#ifndef __factory__
#define __factory__

#include "../music_output.h"
#include "../music_input.h"
#include "../music_exec.h"

#include "../impl/music_output_impl.h"
#include "../impl/music_input_impl.h"
#include "../impl/music_exec_impl.h"

namespace music
{
	namespace impl
	{
		class factory
		{
			factory();
			
		public:
			
			inline static music_output * get_output_system() { return new music_output_impl(); }
			inline static music_input * get_input_system() { return new music_input_impl(); }
			inline static music_exec * get_exec_system() { return new music_exec_impl(); }
		};
	}
}

#endif // __factory__
