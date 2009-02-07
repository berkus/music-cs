/*
 * The MIT License
 * Copyright (c) 2008-2009 Marcos José Sant'Anna Magalhães
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

#include "DBHandler.h"

//::::::::::::::::::::::::::::::::::::::://

using namespace std;

//::::::::::::::::::::::::::::::::::::::://

int t = 0;

//::::::::::::::::::::::::::::::::::::::://

MusiC::Native::DBHandler::DBHandler()
{
	/// @todo Add Logging initialization
	log.open( "Native_DBHandler.txt" );
}

//::::::::::::::::::::::::::::::::::::::://

MusiC::Native::DBHandler::~DBHandler()
{
	CloseDB();
	log.close();
	/// @todo Check/Delete Allocated Data
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::OpenDB( const char * dbName )
{
	CloseDB();

	_dataSectionSz = 0;

	log << "Opening: " << dbName << endl;
	db.open( dbName, ios_base::in | ios_base::out | ios_base::app | ios_base::binary );
	log << "Is Open: " << db.is_open() << endl;
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::CloseDB()
{
	if ( db.is_open() )
	{
		log << "Closing. . ." << endl;
		db.close();
	}
	else
	{
		log << "Current DB is closed." << endl;
	}
}

//::::::::::::::::::::::::::::::::::::::://

bool MusiC::Native::DBHandler::HasData( const char * sectionLabel, const char * dtLabel )
{
	Reset();

	log << "Section: " << sectionLabel << "   Label: " << dtLabel << "   - Searching. . ." << endl;

	while ( LoadNext() )
	{
		if ( !strcmp( sectionLabel, GetSection() ) )
		{
			if ( !strcmp( dtLabel, GetDataLabel() ) )
			{
				log << "FOUND" << endl;
				return true;
			}
		}
	}

	log << "NOT FOUND!" << endl;

	return false;
}

//::::::::::::::::::::::::::::::::::::::://

///@brief Returns the data stored in the .db file.
///@details trusts that hasData has already positioned get-pointer
int MusiC::Native::DBHandler::ReadData( float * dt )
{
	long offset = db.tellg();

	//Hopes ReadData was allocated outside.
	//dt = new float[_dataSectionSz/sizeof(float)];

	db.read( ( char * )dt, _dataSectionSz );

	//LOG("Bytes Read:" << db.gcount());

	int count = db.gcount();
	db.seekg( offset, ios_base::beg );

	return count;
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::AddData( const char * sectionLabel, const char * dataLabel, float * data, int dataSectionSz )
{
	Set();//Positioning Put-Pointer
	WriteHeader( sectionLabel, dataLabel, dataSectionSz );
	WriteData( data, dataSectionSz );
}

//::::::::::::::::::::::::::::::::::::::://

bool MusiC::Native::DBHandler::LoadNext()
{
	if ( !IsDBOpen() )
	{
		log << "DB Closed!" << endl;
		return false;
	}

	if ( IsEndOfFile() )
	{
		log << "No more data available!" << endl;
		return false;
	}

	// Last Header Data Section
	JumpDataSection();

	// Fileformat
	ReadHeader();

	PrintHeader();

	LOG_OUT();

	return true;
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::PrintHeader()
{
	log << "Section Label: " << _sectionLabel << " - size: " << _sectionLabelSz << endl;
	log << "Data Label: " << _dataLabel << " - size: " << _dataLabelSz << endl;
	log << "Data Size:" << _dataSectionSz << endl;
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::JumpDataSection()
{
	log << "Current:" << db.tellg() << " - Skipping:" << _dataSectionSz << endl;
	db.seekg( _dataSectionSz, ios_base::cur );
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::ReadHeader()
{
	db.read( ( char * )&_sectionLabelSz, sizeof( int ) );
	LOG( "Bytes Read:" << db.gcount() );
	log << "Section Label Size: " << _sectionLabelSz;

	db.read( ( char * )&_dataLabelSz, sizeof( int ) );
	LOG( "Bytes Read:" << db.gcount() );
	log << "   Data Label Size: " << _dataLabelSz;

	db.read( ( char * )&_dataSectionSz, sizeof( int ) );
	LOG( "Bytes Read:" << db.gcount() );
	log << "   Data Section Size: " << _dataSectionSz << endl;

	if ( _sectionLabel )
	{
		delete _sectionLabel;
	}

	_sectionLabel = new char [_sectionLabelSz];

	if ( _dataLabel )
	{
		delete _dataLabel;
	}

	_dataLabel = new char [_dataLabelSz];

	db.read( _sectionLabel, _sectionLabelSz + 1 );
	LOG( "Bytes Read:" << db.gcount() );
	db.read( _dataLabel, _dataLabelSz + 1 );
	LOG( "Bytes Read:" << db.gcount() );
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::WriteHeader( const char * sectionLabel, const char * dataLabel, int dataSectionSz )
{
	int sectionLabelSz = strlen( sectionLabel );
	LOG( "Section Label:`" << sectionLabel << "' - Size:" << sectionLabelSz );
	db.write( ( char * ) &sectionLabelSz, sizeof( int ) );
	LOG( "Good:" << !db.fail() );

	int dataLabelSz = strlen( dataLabel );
	LOG( "Data Label:`" << dataLabel << "' - Size:" << dataLabelSz );
	db.write( ( char * ) &dataLabelSz, sizeof( int ) );
	LOG( "Good:" << !db.fail() );

	LOG( "Data Section Size:" << dataSectionSz );
	db.write( ( char * ) &dataSectionSz, sizeof( int ) );
	LOG( "Good:" << !db.fail() );

	db.write( const_cast<char *>( sectionLabel ), sectionLabelSz + 1 );
	LOG( "Good:" << !db.fail() );
	db.write( const_cast<char *>( dataLabel ), dataLabelSz + 1 );
	LOG( "Good:" << !db.fail() );
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::WriteData( float * data, int dataSectionSz )
{
	db.write( ( char * )data, dataSectionSz );
	db.flush();
}

//::::::::::::::::::::::::::::::::::::::://

char * MusiC::Native::DBHandler::GetSection()
{
	return _sectionLabel;
}

//::::::::::::::::::::::::::::::::::::::://

char * MusiC::Native::DBHandler::GetDataLabel()
{
	return _dataLabel;
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::Reset()
{
	LOG_IN();
	LOG( "Current:" << db.tellp() );

	_dataSectionSz = 0;
	db.seekg( 0, ios_base::beg );

	LOG( "After Seek:" << db.tellp() );
	LOG_OUT();
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::Set()
{
	LOG_IN();
	LOG( "Current:" << db.tellp() );

	db.clear();
	db.seekp( 0, ios_base::end );

	LOG( "After Seek:" << db.tellp() );
	LOG_OUT();
}

//::::::::::::::::::::::::::::::::::::::://

/*
int main2()
{
	double d[] = {1,2,3};
	Initialize("feat.db");
	AddFeature("w1","f1", d,sizeof(double) * 3);
	AddFeature("w1","f2", d,sizeof(double) * 3);
	AddFeature("w2","f1", d,sizeof(double) * 3);
	Terminate();
	return 0;
}

int main()
{
	Initialize("feat.db");
	cout << "Feat:" << HasFeature("w2","f2") << endl;
	cout << "Feat:" << HasFeature("w1","f2") << endl;
	Terminate();
}
*/

// END OF FILE
