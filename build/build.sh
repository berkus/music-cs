#! /bin/bash

compiler=""
version=""

function build()
{
	if ["$compiler" = "g"]; then ./premake --dotnet mono2 --target gnu; fi
	if ["$compiler" = "m"]; then ./premake --dotnet mono2 --target monodev; fi
	if ["$compiler" = "s"]; then ./premake --target sharpdev; fi
	if ["$compiler" = "C"]; then ./premake --target cl-gcc; fi
	
	if ["$compiler" = "c"]; then
		if ["$version" = "o"]; then ./premake --target cb-gcc; fi
		if ["$version" = "g"]; then ./premake --target cb-gcc; fi
	fi
	
	if ["$compiler" = "v"]; then
		case "$version" in
		"1")
			./premake --target vs6
		;;
		"2")
			./premake --target vs2002
		;;
		"3")
			./premake --target vs2003
		;;
		"4")
			./premake --target vs2005
		;;
		"5")
			./premake --target vs2008
		;;
		esac
	fi
}

function visualstudio()
{
	#clear
	echo
	echo "Choose VisualStudio version:"
	echo "0: Go Back               2: Visual Studio 2002**        4: Visual Studio 2005"
	echo "1: Visual Studio 6*      3: Visual Studio 2003          5: Visual Studio 2008"
	echo
	echo "  *(don't support managed code)     **(don't support .Net 2.0)"
	echo
	echo "   Enter a number:"
	
	read version
	echo $version
	
	if [ $version -eq 0 ]; then return 1; fi
	if [ $version -ge 1] && [$version -le 5 ]; then build; return $?; fi
	
	return 1
}

function codeblocks()
{
	#clear
	echo
	echo "Choose the Code::Blocks compiler:"
	echo " 0: Go Back           2: Open Watcom*"
	echo " 1: GCC (MinGW)"
	echo
	echo "  *(Not Supported yet)"
	echo
	echo "   Enter a number:"
	
	read version
	
	if [ $version -eq 0 ]; then return 1; fi
	if [ $version -ge 1 && $version -le 2 ]; then build; return $?; fi
	
	return 1
}

function menu()
{
	#clear
	echo
	echo
	echo
	echo "                            MusiC"
	echo
	echo
	echo
	echo " Choose one of the alternatives(NOTE they are case sensitive):"
	echo
	echo "v: Visual Studio         g: GNU Makefile       C: CodeLite"
	echo "c: Code::Blocks          m: Monodevelop        s: Sharpdevelop"
	echo
	echo "x: Exit"
	echo
	echo " Enter a letter:"
	
	read compiler
	
	if [ "$compiler" = "v" ]; then visualstudio; return $?; fi
	if [ "$compiler" = "c" ]; then codeblocks; return $?; fi
	if [ "$compiler" = "g" ]; then build; exit $?; fi
	if [ "$compiler" = "m" ]; then build; exit $?; fi
	if [ "$compiler" = "s" ]; then build; exit $?; fi
	if [ "$compiler" = "C" ]; then build; exit $?; fi
	if [ "$compiler" = "x" ]; then return 0; fi
	
	return 1;
}

function start()
{
	menu
	if [ "$?" != "0" ]; then menu; fi
}

start
