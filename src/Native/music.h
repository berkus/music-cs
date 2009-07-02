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

#ifndef __music__
#define __music__

#include "base/macros.h"
#include "base/music_object.h"
#include "base/music_exception.h"
#include "base/music_config.h"

#include "impl/music_factory.h"

namespace music
{
	class music //: public base::music_object
	{
		music_output * _out;
		music_input * _in;
		music_exec * _exec;

		base::music_config _cfg;
		
	public:

		music()
		{
		}
		
		~music()
		{
			if(_out)
				delete _out;
			
			if(_in)
				delete _in;
			
			if(_exec)
				delete _exec;
		}
		
		music_output * get_output_system() { return (_out) ? _out : _out = impl::factory::get_output_system(); }
		music_input * get_input_system() { return (_in) ? _in : _in = impl::factory::get_input_system(); }
		music_exec * get_exec_system() { return (_exec) ? _exec : _exec = impl::factory::get_exec_system(); }

		bool initialize()
		{
			return ( get_output_system()->initialize( &_cfg ) &&
					 get_input_system()->initialize( &_cfg ) &&
					 get_exec_system()->initialize( &_cfg ) );
		}

		bool to_bool( std::wstring &value ) const
		{
			if( value == L"" )
				throw new base::MUSIC_EXCEPTION("Invalid value.");

			if( value == L"FALSE" || value == L"NO" || value == L"NULL" || value == L"0" )
					return false;

			return true;
		}

		bool set_config( std::string &key, std::string &value )
		{
			return _cfg.add_config( key, value );
		}

		std::string get_config( std::string &key )
		{
			return _cfg.get_config( key );
		}

		bool set_config2( std::string key, std::string value )
		{
			return _cfg.add_config( key, value );
		}

		std::string get_config2( std::string key )
		{
			return _cfg.get_config( key );
		}
	};
}

#endif
