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

#include "DataHandler.h"

using namespace MusiC::Native;

DataHandler::DataHandler() : _data( NULL ), _curClass( NULL )
{}

void DataHandler::Attach( DataCollection * dtCol )
{
    _data = dtCol;
    _curClass = NULL;
}

ClassData * DataHandler::GetClass( unsigned int idx )
{
    if( idx > _data->nClasses - 1 )
        return NULL;

    ClassData * c = _data->pFirstClass;

    while( idx )
    {
        c = c->pNextClass;
        idx--;
    }

    return c;
}

ClassData * DataHandler::GetNextClass()
{
    if( !_curClass )
    {
		return _curClass = _data->pFirstClass;
    }

    return _curClass = _curClass->pNextClass;
}

FileData * DataHandler::GetNextLocalFile()
{
    return NULL;
}

FrameData * DataHandler::GetNextLocalFrame()
{
    return NULL;
}

FileData * DataHandler::GetNextGlobalFile()
{
    if (!_curFile)
    {
		if( !_curClass )
			GetNextClass();
		
		return _curFile = _curClass->pFirstFile;
    }
    
    _curFile = _curFile->pNextFile;
    
    if( !_curFile )
    {
		GetNextClass();
				
		if( !_curClass )
			return NULL;
		
		_curFile = _curClass->pFirstFile;
    }
    
    return _curFile;
}

FrameData * DataHandler::GetNextGlobalFrame()
{
    return NULL;
}