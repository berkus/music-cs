#include "DataHandler.h"

MusiC::Native::DataHandler::DataHandler() : _data(NULL)
{}

void MusiC::Native::DataHandler::Attach(DataCollection * dtCol)
{
	_data = dtCol;
}
