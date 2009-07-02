#ifndef __return__
#define __return__

#include <boost/any.hpp>
#include <boost/optional.hpp>

class music_return_value
{
public:

	boost::any value;

	template< typename T > T getValue()
	{ return boost::any_cast< T >( value ); }
};

typedef boost::optional< music_return_value > music_return;

#endif // __return__
