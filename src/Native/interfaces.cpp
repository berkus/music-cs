#include "alloc.cpp"
#include "math.cpp"

extern "C"
{
	MemoryManager mgr;
	mcMath math;
	
	///////////// SPTK Compatibility /////////////
	float * dgetmem(long sz)
	{
		return mgr.Allocator<float>(sz);
	}
	
	//void free(void *p)
	//{
	//	mgr.Dealloc(p);
	//}
	
	///////////// FFTI(in-place) /////////////
	int fft1(float* x, float* y, int m)
	{
		return math.fft(x, y, m);
	}
	
	int fftr_mag1(float* x, int m)
	{
		return math.fftr_mag(x, m);
	}
	
	///////////// FFT(no data destruction) /////////////
	int fft2(const float* ix, float* iy,float* ox, float* oy, int m)
	{
		memcpy(ox, ix, m);
		memcpy(oy, iy, m);
		
		return math.fft(ox, oy, m);
	}
	
	int fftr2(const float* ix, float* ox, float* oy, int m)
	{
		memcpy(ox, ix, m);
		memset(oy, '\0', m);
		return math.fft(ox, oy, m);
	}
	
	__declspec(dllexport) int fftr_mag2(float* x, float* mag, int m)
	{
		memcpy(mag, x, m);
		return math.fftr_mag(mag, m);
	}
}

/*
int main()
{
	Initialize();
	float * d = mgr->Allocator<float>(512);
	memset(d,'\1', 512);
	fftr1(d, 512);
	Terminate();
}
*/
// END OF FILE
