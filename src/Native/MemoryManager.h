#ifndef MEMORYMANAGER_H_INCLUDED
#define MEMORYMANAGER_H_INCLUDED

#include <list>
#include <cstdlib>

struct MemData
{
	double * ptr;
};

namespace MusiC
{
	namespace Native
	{
		class MemoryManager
		{
			private:
				std::list<MemData> l;

				void Dealloc();

			public:
				void Dealloc (void * p);

				template<class T>
				T* Allocator (int sz)
				{
					return (T*) calloc (sz, sizeof (T) );
				}

				~MemoryManager();
		};
	}
}

#endif // MEMORYMANAGER_H_INCLUDED
