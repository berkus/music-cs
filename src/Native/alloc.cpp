#if !defined(__HEADER_ALLOC_CPP__)
#define __HEADER_ALLOC_CPP__

#include <list>
#include <cstdlib>

using namespace std;

struct MemData
{	
	double * ptr;
};

class MemoryManager
{
	list<MemData> l;

	void Dealloc()
	{
		list<MemData>::iterator it;
		
		for(it = l.begin(); it != l.end(); it++)
		{
			if((*it).ptr)
				free((*it).ptr);
		}
	}

	public:
	
	void Dealloc(void * p)
	{
		if(p)
		{
			free(p);
			p = NULL;
		}
	}

	template<class T>
	T* Allocator(int sz)
	{
		return (T*) malloc(sizeof(T) * sz);
	}
	
	~MemoryManager()
	{
		Dealloc();
	}
};

#endif

// END OF FILE
