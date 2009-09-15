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

#include "DBHandler.h"
#include "LogHandler.h"

//::::::::::::::::::::::::::::::::::::::://

using namespace std;

extern MusiC::Native::LogHandler log;

//::::::::::::::::::::::::::::::::::::::://

MusiC::Native::DBHandler::DBHandler()
{
	_sectionLabel = NULL;
	_dataLabel = NULL;
}

//::::::::::::::::::::::::::::::::::::::://

MusiC::Native::DBHandler::~DBHandler()
{

	CloseDB();
	/// @todo Check/Delete Allocated Data
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::OpenDB( const char * dbName )
{
	CloseDB();

	_dataSectionSz = 0;

	// Force file creation but fix input at end of file
	db.open( dbName, ios_base::in | ios_base::app ); db.close();

	// Open created file and allow flexible input
	db.open( dbName, ios_base::in | ios_base::out | ios_base::ate | ios_base::binary );

	// Set put and get pointer to start of the file
	db.seekg(0, ios_base::beg);
	db.seekp(0, ios_base::beg);

	db.clear();
}

//::::::::::::::::::::::::::::::::::::::://

void MusiC::Native::DBHandler::CloseDB()
{
	if ( db.is_open() )
		db.close();
}

//::::::::::::::::::::::::::::::::::::::://

bool MusiC::Native::DBHandler::HasData( const char * sectionLabel, const char * dtLabel )
{
	// Move read pointer to start
	db.seekg( 0, ios_base::beg );

	while ( ReadHeader() )
	{
		// Window and Feature
		if ( !strcmp( sectionLabel, GetSection() ) && !strcmp( dtLabel, GetDataLabel() ) )
			return true;

		//Jump Data
		db.seekg( _dataSectionSz, ios_base::cur );
	}

	return false;
}

//::::::::::::::::::::::::::::::::::::::://

///@brief Returns the data stored in the .db file.
///@details trusts that hasData has already positioned get-pointer
int MusiC::Native::DBHandler::ReadData( float * dt )
{
	if( IsEndOfFile() )
		return 0;

	//cout << "Reading Data Section" << endl;
	
	db.read( ( char * )dt, _dataSectionSz );
	return db.gcount();
}

//::::::::::::::::::::::::::::::::::::::://

///@brief Add or Replace Section data.
bool MusiC::Native::DBHandler::AddData( const char * sectionLabel, const char * dataLabel, float * data, int dataSectionSz )
{
	if( HasData( sectionLabel, dataLabel ) )
	{
		//cout << "Replacing Section: " << sectionLabel << " - Label: " << dataLabel << endl;
		db.seekp( db.tellg(), ios_base::beg );
	}
	else
	{
		//cout << "Adding Section: " << sectionLabel << " - Label: " << dataLabel << endl;
		db.seekp( 0, ios_base::end );
		if( !WriteHeader( sectionLabel, dataLabel, dataSectionSz ) )
			return false;
	}

	return WriteData( data, dataSectionSz );
}

//::::::::::::::::::::::::::::::::::::::://

bool MusiC::Native::DBHandler::ReadHeader()
{
	if ( IsEndOfFile() )
		return false;

	//cout << "Reading Feature Header" << endl;

	// Reading Sizes
	db.read( ( char * )&_sectionLabelSz, sizeof( int ) );
	db.read( ( char * )&_dataLabelSz, sizeof( int ) );
	db.read( ( char * )&_dataSectionSz, sizeof( int ) );

	// Check for IO failure
	if ( !db.good() )
		return false;

	Destroy();

	_sectionLabel = new char [_sectionLabelSz + 1];
	_dataLabel = new char [_dataLabelSz + 1];
	
	// Read labels
	db.read( _sectionLabel, _sectionLabelSz + 1 );
	db.read( _dataLabel, _dataLabelSz + 1 );

	// Check for IO failure
	if ( !db.good() )
		return false;

	return true;
}

//::::::::::::::::::::::::::::::::::::::://

bool MusiC::Native::DBHandler::WriteHeader( const char * sectionLabel, const char * dataLabel, int dataSectionSz )
{
	//cout << "Writing Section Header" << endl;

	// doesnt incluse '/0' (EOL)
	int sectionLabelSz = strlen( sectionLabel );
	int dataLabelSz = strlen( dataLabel );

	db.write( ( char * ) &sectionLabelSz, sizeof( int ) );
	db.write( ( char * ) &dataLabelSz, sizeof( int ) );
	db.write( ( char * ) &dataSectionSz, sizeof( int ) );

	// Check for IO failure
	if ( !db.good() )
		return false;

	db.write( const_cast<char *>( sectionLabel ), sectionLabelSz + 1 );
	db.write( const_cast<char *>( dataLabel ), dataLabelSz + 1 );

	// Check for IO failure
	if ( !db.good() )
		return false;

	db.flush();

	// Check for IO failure
	if ( !db.good() )
		return false;

	return true;
}

//::::::::::::::::::::::::::::::::::::::://

bool MusiC::Native::DBHandler::WriteData( float * data, int dataSectionSz )
{
	//cout << "Writing Feature Data - ";
	db.write( ( char * )data, dataSectionSz );
	
	// Check for IO failure
	if ( !db.good() )
		return false;
	
	db.flush();

	// Check for IO failure
	if ( !db.good() )
		return false;

	return true;
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

void MusiC::Native::DBHandler::Destroy()
{	
	// Destroy old window label
	if ( _sectionLabel )
	{
		delete[] _sectionLabel;
	}

	// Destroy old feature label
	if ( _dataLabel )
	{
		delete[] _dataLabel;
	}
}

//::::::::::::::::::::::::::::::::::::::://

//*
int main()
{
	MusiC::Native::DBHandler db;

	float ones[] = {1.0f, 1.0f, 1.0f};
	float twos[] = {2.0f, 2.0f, 2.0f};

	db.OpenDB("test.db");
	db.AddData("Ab", "Cd", ones, sizeof(float) * 3);
	db.AddData("Ab", "Cd", twos, sizeof(float) * 3);
	db.CloseDB();

	db.OpenDB("test.db");

	float * data = new float[3];

	//GetData
	if(db.HasData("Ab", "Cd"))
	{
		db.ReadData( data );
	}

	cout << data[0] << endl;
	cout << data[1] << endl;
	cout << data[2] << endl;

	system("pause");

	delete[] data;
	db.CloseDB();
}
//*/

// END OF FILE

