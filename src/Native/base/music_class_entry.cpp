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

#include "music_class_entry.h"

music::base::music_class_entry::music_class_entry( music_class_metadata * data ) : _metadata(data)
{
	if( _metadata->kind == music_extension::CONFIGURATOR )
    {
		ptr.cfg = dynamic_cast< music_configurator * >( _metadata->ctor() );
        return;
    }
}

music::base::music_class_entry::~music_class_entry()
{
	std::list< music_constructor_entry * >::iterator it;
	music_constructor_entry * e;

	for( it = _ctor_list.begin(); it != _ctor_list.end(); it++)
	{
		e = (*it);
		delete e;
	}

	_ctor_list.clear();
}

void music::base::music_class_entry::add_constructor( music_ctor_metadata * data )
{
	music_constructor_entry * ctor = new music_constructor_entry( data );
	_ctor_list.push_back( ctor );
}

bool music::base::music_class_entry::can_handle( std::wstring &file )
{
	if( _metadata->kind == music_extension::CONFIGURATOR )
    {
		return ptr.cfg->can_handle( file );
    }

    return false;
}
