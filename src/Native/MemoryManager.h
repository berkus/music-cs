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

#if !defined(_MUSIC_NATIVE_MEMORYMANAGER_H_)
#define _MUSIC_NATIVE_MEMORYMANAGER_H_

#include <list>
#include <cstdlib>

namespace MusiC
{
	namespace Native
	{
		class MemoryManager
		{
			private:
				std::list<size_t> l;

				void Dealloc();

			public:
				void Dealloc (void * p);
				void DeallocOwned(void * p);

				template<class T>
				T* Allocator (int sz)
				{
				    T * ptr = (T*) calloc (sz, sizeof (T) );
				    l.push_back(reinterpret_cast<size_t>(ptr));

					return ptr;
				}

				~MemoryManager();
		};
	}
}

#endif // MEMORYMANAGER_H_INCLUDED
