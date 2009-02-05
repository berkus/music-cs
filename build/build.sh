#! /bin/bash

function menu_project()
{
	clear
	echo
	echo " Choose one of the alternatives(NOTE they are case sensitive):"
	echo
	echo "v: Visual Studio 2008       c: Code::Blocks      g: GNU Makefile"
	echo "V: Visual Studio 2005*      C: CodeLite"
	echo
	echo
	echo *Also Monodevelop and Sharpdevelop.
	echo
	echo
	echo "x: Exit             [Any Other]: Back"
	echo
	echo " Enter a letter:"
	
	read compiler
	
	if [ "$compiler" = "v" ]; then build vs2008; return $?; fi
	if [ "$compiler" = "V" ]; then build vs2005; return $?; fi
	if [ "$compiler" = "c" ]; then build codeblocks gcc; return $?; fi
	if [ "$compiler" = "C" ]; then build codelite gcc; return $?; fi
	if [ "$compiler" = "g" ]; then build gmake; exit $?; fi
	if [ "$compiler" = "b" ]; then return 0; fi
	if [ "$compiler" = "x" ]; then clear; exit 0; fi
	
	return 1;
}

function build()
{
  if [ "$2" = "" ]; then 
    premake4 --file=premake.lua $1;
  else
    premake4 --cc=$2 --file=premake.lua $1
  fi

  echo
  echo "Press [Enter] key to continue. . ."
  read enterkey
}

function build_doc()
{
  doxygen
  cd ../srcDoc/latex
  make pdf
  cd ../../srcToTex
  mono bin/Release/srcToTex.exe ../src
  pdflatex printSrc
  cd ../build
  cp ../srcDoc/latex/refman.pdf ../srcToTex/printSrc.pdf .
  
  echo
  echo "Press [Enter] key to continue. . ."
  read enterkey
}

while :
do
  clear
  echo 'Would you like to build :'
  echo 
  echo 'd) Documentation (Doxygen + Latex)'
  echo 'p) Project'
  echo 'x) Exit'
  echo
  echo
  echo ' Your option:'

  read opt
  
  case $opt in
  d) build_doc;;
  p) menu_project;;
  x) clear; exit 0;;
  *) clear;
     echo "$opt is an invaild option.";
     echo "Press [Enter] key to continue. . .";
     read enterKey;;
esac
done
