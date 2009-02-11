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

#include "Barbedo.h"

using namespace MusiC::Native;

extern "C"
{
    DataCollection * Barbedo_Filter( DataCollection * extractedData );
    void * Barbedo_Train( DataCollection * extractedData );
    int Barbedo_Classify( void * data );
}

DataCollection * Barbedo_Filter( DataCollection * extractedData )
{
    Barbedo b;
    return b.Filter( extractedData );
}

void * Barbedo_Train( DataCollection * extractedData )
{
    Barbedo b;
    return b.Train( extractedData );
}

int Barbedo_Classify( void * data )
{
    return 0;
}
