@ECHO off

:menu
cls
echo.
echo.
echo.
echo                             MusiC
echo.
echo.
echo.
echo  Choose one of the alternatives(NOTE they are case sensitive):
echo.
echo  v: Visual Studio         g: GNU Makefile       C: CodeLite
echo  c: Code::Blocks          m: Monodevelop        s: Sharpdevelop
echo.
echo  x: Exit
echo.

set /p compiler=      Enter a letter:

if %compiler%==v goto visualstudio
if %compiler%==c goto codeblocks
if %compiler%==g goto build
if %compiler%==m goto build
if %compiler%==s goto build
if %compiler%==C goto build
if %compiler%==x goto exit
goto menu

:visualstudio
cls
echo.
echo Choose VisualStudio version:
echo.
echo 0: Go Back               2: Visual Studio 2002**        4: Visual Studio 2005
echo 1: Visual Studio 6*      3: Visual Studio 2003          5: Visual Studio 2008
echo.
echo   *(don't support managed code)     **(don't support .Net 2.0)
echo.
set /p version=      Enter a number:

if %version%==0 goto menu
if %version%==1 goto build
if %version%==2 goto build
if %version%==3 goto build
if %version%==4 goto build
if %version%==5 goto build
goto visualstudio

:codeblocks
cls
echo.
echo Which compiler you would like to use ?
echo.
echo n: Go Back             o: Open Watcom*
echo g: MinGW (GCC)
echo.
echo  *(Not Supported)
echo.
set /p version=    Enter an option:

if %version%==n goto menu
if %version%==o goto codeblocks
if %version%==g goto build
goto codeblocks

:build
cls
echo.
REM echo [Options] Compiler:%compiler%, Version:%version%, Projects:%vsprojects%
echo.

if %compiler%==g (
	premake4 gmake
	goto pause-before-menu
)

if %compiler%==v (
	if %version%==4 (
		premake4 vs2005
		goto pause-before-menu
	)
	if %version%==5 (
		premake4 vs2008
		goto pause-before-menu
	)
)

if %compiler%==c (
	if %version%==o (
		premake4 --cc=ow codeblocks
		goto pause-before-menu
	)
	if %version%==g (
		premak4e --cc=gcc codeblocks
		goto pause-before-menu
	)
)

if %compiler%==C (
	premake4 --cc=gcc codelite
	goto pause-before-menu
)

if %compiler%==m (
	premake4 vs2005
	goto pause-before-menu
)

if %compiler%==s (
	premake4 vs2005
	goto pause-before-menu
)

:pause-before-menu
echo.
echo.
pause
goto menu

:exit
