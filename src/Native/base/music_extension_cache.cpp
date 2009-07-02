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

#include "music_extension_cache.h"

#include <QDebug>

using namespace std;

music::base::music_extension_cache::music_extension_cache()
{
}

music::base::music_extension_cache::~music_extension_cache()
{
	std::list< music_class_entry * >::const_iterator it;
	music_class_entry * e;

	for( it = _class_list.begin(); it != _class_list.end(); it++ )
	{
		e = (*it);
		delete e;
	}

	_class_list.clear();
}

void music::base::music_extension_cache::load()
{
        //wxDynamicLibrary dll;
        //wxDir dir( wxGetCwd() );
        //wxString filename;

        // Name Filter
        QStringList filter;
        filter << "*.dll";

        QString dir = QDir::current().dirName();
        QDirIterator dir_it( dir, filter, QDir::Files, QDirIterator::Subdirectories );

        qDebug() << "Searching for Extensions at " << dir << endl;

//	if( !dir.IsOpened() )
//		info( L"Error opening file" );
//
//	bool cont = dir.GetFirst( &filename, L"*.mcl", wxDIR_FILES );
//	while( cont )
//	{
//		info( filename.c_str() );
//
//		dll.Load( filename );
//
//		ClassMetaData * ( * getMetaData )() = ( ClassMetaData * (*)() ) dll.GetSymbol( L"GlobalMetaData" );
//
//		ClassMetaData * gmd = getMetaData();
//		ClassMetaData * aux = gmd;
//
//		while( aux->classname && aux->ctor && aux->dtor && aux->ctorInfo )
//		{
//			ClassEntry * entry = new ClassEntry( aux );
//			//Extension * e = aux->ctor();
//			//std::wcout << typeid( e ).name() << std::endl;
//			//aux->dtor( e );
//
//			CtorMetaData * t = aux->ctorInfo;
//			while( t->type )
//			{
//				entry->addConstructor( t );
//				t++;
//			}
//
//			_class_list.push_back( entry );
//			aux++;
//		}
//
//		cont = dir.GetNext(&filename);
//	}
}

music::base::music_configurator * music::base::music_extension_cache::get_configurator( std::wstring &file )
{
	std::list< music_class_entry * >::const_iterator it;
	music_configurator * cfg = NULL;

    for( it = _class_list.begin(); it != _class_list.end(); it++ )
    {
		if( (*it)->can_handle( file ) )
        {
			cfg = (*it)->get_extension_as< music_configurator * >();

            if( cfg != NULL )
                return cfg;

            cfg = NULL;
        }
    }

    std::wostringstream oss;
    oss << __LOCATE__ << std::endl;
    oss << L"No Configurator is available for file " << file;
    error( oss.str().c_str() );

	return NULL;
}
