#include "DataHandler.h"

using namespace MusiC::Native;

DataHandler::DataHandler() : _data(NULL), _curClass(NULL)
{}

void DataHandler::Attach(DataCollection * dtCol)
{
	_data = dtCol;
}

ClassData * DataHandler::getClass(int idx)
{
    if(idx > _data->nClasses - 1)
        return NULL;

    ClassData * c = _data->pFirstClass;

    while(idx)
    {
        c = c->pNext;
    }

    idx--;

    return c;
}

ClassData * DataHandler::getNextClass()
{
    if(!_curClass)
        return _data->pFirstClass;

    return _curClass = _curClass->pNext;
}
