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

#include "music_config.h"

#include <algorithm>
#include <cctype>
#include <sstream>

bool music::base::music_config::add_config( std::string &key, std::string &value )
{
	// Empty isn't a valid value
	if( value == "" )
		return false;

	// Make key uppercase before adding to the dictionary
	std::transform( key.begin(), key.end(), key.begin(), (int(*)(int)) toupper );

	// Make value uppercase before adding to the dictionary
	std::transform( value.begin(), value.end(), value.begin(), (int(*)(int)) toupper );

	std::pair< dictionary_type::iterator, bool> res = _dict.insert( std::make_pair( key, value ) );
	return res.second;
}

std::string music::base::music_config::get_config( std::string &key ) const
{
	std::transform( key.begin(), key.end(), key.begin(), (int(*)(int)) toupper );

	dictionary_type::const_iterator it = _dict.find( key );
	if( it != _dict.end() )
		return it->second;

	std::ostringstream oss;
	oss << "Key [" << key << "] wasn't found. ";
	throw base::MUSIC_EXCEPTION( oss.str().c_str() );
}

// END OF FILE
