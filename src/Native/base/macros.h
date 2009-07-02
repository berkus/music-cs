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

#ifndef __macros__
#define __macros__

#define INNER_TO_STRING(expr) #expr
#define WSTRING(expr) TO_WIDE(INNER_TO_STRING(expr))

#define INNER_TO_WIDE(expr) L ## expr
#define TO_WIDE(expr) INNER_TO_WIDE(expr)

#define CTOR_LIST_NAME(class) class##_ctor_list
#define CTOR_NAME(class) class##_ctor
#define DTOR_NAME(class) class##_dtor
#define CLASS_METADATA_NAME(class) class##_meta

#define BEGIN_CONSTRUCTOR_LIST(class) CtorMetaData CLASS_METADATA_NAME(class)[] = {
#define ADD_CONSTRUCTOR(a) { a },
#define END_CONSTRUCTOR_LIST() { NULL } \
};

#define NEW_CONSTRUCTOR_PROTOTYPE(name, ...) wchar_t * name[] = { __VA_ARGS__, NULL };

#define DECLARE_CTOR(class) Extension * CTOR_NAME(class)( ) { return new class(); }
#define DECLARE_DTOR(class) void DTOR_NAME(class)( Extension * c ) { delete c; }
#define DECLARE_CLASS(class) DECLARE_CTOR(class)\
\
DECLARE_DTOR(class)

#define BEGIN_CLASS_LIST() ClassMetaData gmd[] = {
#define ADD_CLASS(type, class) { type, TO_WIDE(#class), CTOR_NAME(class), DTOR_NAME(class), CLASS_METADATA_NAME(class) },
#define END_CLASS_LIST() { (ExtensionType) 0, NULL, NULL, NULL, NULL }\
};

#define EXPORT_CLASS_LIST() ClassMetaData * GlobalMetaData() { return gmd; }

// Use only in case of conflicts

#define ADV_BEGIN_CONSTRUCTOR_LIST(name) CtorMetaData name[] = {
#define ADV_ADD_CLASS(type, name, ctor, dtor, metadata) { type, name, ctor, dtor, metadata },

#define ADV_BEGIN_CLASS_LIST(name) ClassMetaData name[] = {
#define ADV_EXPORT_CLASS_LIST(name) ClassMetaData * GlobalMetaData() { return name; }


// Exception macros

#define __wFILE__ TO_WIDE(__FILE__)
#define _DUMMY_PRETTY_FUNCTION_(func) func
#define __wFUNCTION__ __PRETTY_FUNCTION__

#define __LOCATE__ L"File: " << __wFILE__ << std::endl << \
L"Function: " << __wFUNCTION__ << std::endl << \
L"Line: " << __LINE__

#define MUSIC_EXCEPTION(msg) music_exception( msg, __FUNCTION__, __FILE__, __LINE__ )

#endif
