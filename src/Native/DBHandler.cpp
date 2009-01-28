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

#include <fstream>
#include <iostream>
#include <sstream>

#include <string.h>

using namespace std;

int t = 0;

string tabulate(int t)
{
	ostringstream o;

	for(int i = 0; i < t-1; i++)
		o << "\t";

	o << t << ")";

	return o.str();
}

//#define DEBUG

#if defined(DEBUG)

#define LOG(msg) cout << tabulate(t) <<  __FUNCTION__ << " at " << __LINE__ << " - " << msg << endl
#else
#define LOG(m) //m
#endif

#if defined(TRACE)
	#define LOG_IN() t++;cout << tabulate(t) <<  __FUNCTION__ << endl
	#define LOG_OUT() cout << tabulate(t) <<  __FUNCTION__ << endl;t--
#else
	#define LOG_IN() //
	#define LOG_OUT() //
#endif

namespace MusiC
{
	namespace Native
	{
		class DBHandler
		{
        private:

			fstream db;

			char * m_sectionLabel;
			char * m_dataLabel;
			int m_sectionLabelSz, m_dataLabelSz, m_dataSectionSz;

		public:

            //-------------------------------------------------------------------------\\

			DBHandler()
			{
				/// @todo Add Logging initialization
			}

			~DBHandler()
			{
				CloseDB();
				/// @todo Check/Delete Allocated Data
			}

			//-------------------------------------------------------------------------\\

			void OpenDB(const char * dbName)
			{
                LOG_IN();

				CloseDB();

				m_dataSectionSz = 0;
				db.open(dbName, ios_base::in | ios_base::out | ios_base::app | ios_base::binary);

				LOG("Opening:" << dbName);
				LOG("Status:" << db.is_open());
                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			void CloseDB()
			{
                LOG_IN();

				if(IsDBOpen())
				{
					db.close();
				}

                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			bool HasData(char * sectionLabel, char * dtLabel)
			{
                LOG_IN();

				Reset();

				while(LoadNext())
				{
					if(!strcmp(sectionLabel, GetSection()))
					{
						if(!strcmp(dtLabel, GetDataLabel()))
						{
                            LOG_OUT();
							return true;
						}
					}
				}

                LOG_OUT();
				return false;
			}

			//-------------------------------------------------------------------------\\

			///@brief Returns the data stored in the .db file.
			///@details trusts that hasData has already positioned get-pointer
			int ReadData(double * dt)
			{
                LOG_IN();

				long offset = db.tellg();

				dt = new double[m_dataSectionSz/sizeof(double)];
				db.read((char *)dt, m_dataSectionSz);
				//LOG("Bytes Read:" << db.gcount());
				int count = db.gcount();
				db.seekg(offset, ios_base::beg);

                LOG_OUT();
				return count;
			}

			//-------------------------------------------------------------------------\\

			void AddData(const char * sectionLabel, const char * dataLabel, double * data, int dataSectionSz)
			{
                LOG_IN();

				Set();//Positioning Put-Pointer
				WriteHeader(sectionLabel, dataLabel, dataSectionSz);
				WriteData(data, dataSectionSz);

                LOG_OUT();
			}

        private:

			bool IsDBOpen()
			{
                LOG_IN();

				bool flag = db.is_open(); LOG("OpenStatus:" << flag);

                LOG_OUT();
				return flag;
			}

			//-------------------------------------------------------------------------\\

			bool IsEndOfFile()
			{
                LOG_IN();

				bool flag = (db.peek() == EOF);	LOG("End Of File:" << flag);

                LOG_OUT();
				return flag;
			}

			//-------------------------------------------------------------------------\\

			bool LoadNext()
			{
                LOG_IN();

				if(!IsDBOpen())
				{
                    LOG_OUT();
					return false;
				}

				if(IsEndOfFile())
				{
                    LOG_OUT();
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

			//-------------------------------------------------------------------------\\

			void PrintHeader()
			{
                LOG_IN();
				LOG("Section Label: " << m_sectionLabel << " - size: " << m_sectionLabelSz);
				LOG("Data Label: " << m_dataLabel << " - size: " << m_dataLabelSz);
				LOG("Data Size:" << m_dataSectionSz);
                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			void JumpDataSection()
			{
                LOG_IN();
				LOG("Current:" << db.tellg() << " - Skipping:" << m_dataSectionSz);
				db.seekg(m_dataSectionSz, ios_base::cur);
                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			void ReadHeader()
			{
                LOG_IN();

				db.read((char *)&m_sectionLabelSz, sizeof(int));LOG("Bytes Read:" << db.gcount());
				db.read((char *)&m_dataLabelSz, sizeof(int));LOG("Bytes Read:" << db.gcount());
				db.read((char *)&m_dataSectionSz, sizeof(int));LOG("Bytes Read:" << db.gcount());

				if(m_sectionLabel)
				{
					delete m_sectionLabel;
				}
				m_sectionLabel = new char [m_sectionLabelSz];

				if(m_dataLabel)
				{
					delete m_dataLabel;
				}
				m_dataLabel = new char [m_dataLabelSz];

				db.read(m_sectionLabel, m_sectionLabelSz + 1); LOG("Bytes Read:" << db.gcount());
				db.read(m_dataLabel, m_dataLabelSz + 1); LOG("Bytes Read:" << db.gcount());
                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			void WriteHeader(const char * sectionLabel, const char * dataLabel, int dataSectionSz)
			{
                LOG_IN();

				int sectionLabelSz = strlen(sectionLabel);
				LOG("Section Label:`" << sectionLabel << "' - Size:" << sectionLabelSz);
				db.write((char *) &sectionLabelSz, sizeof(int));LOG("Good:" << !db.fail());

				int dataLabelSz = strlen(dataLabel);
				LOG("Data Label:`" << dataLabel << "' - Size:" << dataLabelSz);
				db.write((char *) &dataLabelSz, sizeof(int));LOG("Good:" << !db.fail());

				LOG("Data Section Size:" << dataSectionSz);
				db.write((char *) &dataSectionSz, sizeof(int));LOG("Good:" << !db.fail());

				db.write(const_cast<char *>(sectionLabel), sectionLabelSz + 1);LOG("Good:" << !db.fail());
				db.write(const_cast<char *>(dataLabel), dataLabelSz + 1);LOG("Good:" << !db.fail());

                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			void WriteData(double * data, int dataSectionSz)
			{
                LOG_IN();

				db.write((char *)data, dataSectionSz);
				db.flush();

				LOG("Good:" << !db.fail());
                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			char * GetSection()
			{
				return m_sectionLabel;
			}

			//-------------------------------------------------------------------------\\

			char * GetDataLabel()
			{
				return m_dataLabel;
			}

			//-------------------------------------------------------------------------\\

			void Reset()
			{
                LOG_IN();
				LOG("Current:" << db.tellp());

				m_dataSectionSz = 0;
				db.seekg(0, ios_base::beg);

				LOG("After Seek:" << db.tellp());
                LOG_OUT();
			}

			//-------------------------------------------------------------------------\\

			void Set()
			{
                LOG_IN();
				LOG("Current:" << db.tellp());

				db.clear();
				db.seekp(0, ios_base::end);

				LOG("After Seek:" << db.tellp());
                LOG_OUT();
			}
		};
	}
}

extern "C"
{
	MusiC::Native::DBHandler hnd;

	//-------------------------------------------------------------------------\\

	void Initialize(const char * db)
	{
        LOG_IN();

		hnd.OpenDB(db);

        LOG_OUT();
	}

	//-------------------------------------------------------------------------\\

	void AddFeature(const char * sectionLabel, const char * dataLabel, double * data, int dataSz)
	{
        LOG_IN();

		hnd.AddData(sectionLabel, dataLabel, data, dataSz * sizeof(double));

        LOG_OUT();
	}

	//-------------------------------------------------------------------------\\

	int GetFeature(char * wndName, char * featName, double *data)
	{
        LOG_IN();

		if(hnd.HasData(wndName, featName))
		{
			return hnd.ReadData(data);
		}

        LOG_OUT();
		return 0;
	}

	//-------------------------------------------------------------------------\\

	void Terminate()
	{
        LOG_IN();

		hnd.CloseDB();

        LOG_OUT();
	}
}

//-------------------------------------------------------------------------\\

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
